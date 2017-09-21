using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.myScripts
{
    public class movementReportScript : MonoBehaviour
    {
        public GameObject minMovementCube;
        public GameObject maxMovementCube;
        public GameObject songCube;
        public GameObject r_Controller;
        public GameObject l_Controller;
        public GameObject headset;
        public Material green;
        public Material red;
        public TextMesh gameText;
        public int checksPerSecond = 10;
        public float stillTolerance = 0.04f;
        public float movingTolerance;
        public float songMin = 2f;
        public float songMax = 3f;
        public float freezeMin = 3f;
        public float freezeMax = 4f;
        public int beginWaitSeconds = 3;
        public int notDancingWaitSeconds = 3;
        public float dontDanceWaitSeconds = 1f;

        private Arithmetic arithmetic;
        private const float controllerStillThreshold = 0.029f;
        private const float headsetStillThreshold = 0.016f;
        private const float controllerMovingThreshold = 0.711f;
        private const float headsetMovingThreshold = 0.201f;
        private bool nde_isRunning = false; // is notDancingEnough() coroutine running
        private bool dontMove = false;
        private bool songIsPlaying = false;
        private AudioSource song;
        private Coroutine checkForMotionCoroutine;
        private Coroutine playMusicCoroutine;
        private Coroutine notDancingEnoughCoroutine;
        private Coroutine movingDuringSilenceCoroutine;

        void Start()
        {
            arithmetic = new Arithmetic();
            song = GetComponent<AudioSource>();

            // Have controller triggers activate an method called "Triggered"
            r_Controller.GetComponent<SteamVR_TrackedController>().TriggerClicked += Triggered;
            l_Controller.GetComponent<SteamVR_TrackedController>().TriggerClicked += Triggered;

            //StartCoroutine(PlayGame());
        }

        void Triggered(object sender, ClickedEventArgs e)
        {
            // Player must pull trigger to begin game
            StartCoroutine(PlayGame());
        }

        IEnumerator PlayGame()
        {
            // Count off before game begins
            int secondsUntilGame = beginWaitSeconds;
            while(secondsUntilGame > 0)
            {
                gameText.text = "Music Starting in " + secondsUntilGame + " Seconds";
                yield return new WaitForSeconds(1);
                secondsUntilGame--;
            }
            gameText.text = "";

            // Play Music
            playMusicCoroutine = StartCoroutine(PlayMusic());

            // Start checking for motion
            checkForMotionCoroutine = StartCoroutine(checkForMotion());
        }

        IEnumerator PlayMusic()
        {
            // Play music
            songCube.GetComponent<Renderer>().material = green;
            song.Play();
            songIsPlaying = true;

            while (true)
            {
                // Wait a random amount of time
                float min = songIsPlaying ? songMin : Mathf.Abs(freezeMin - dontDanceWaitSeconds);
                float max = songIsPlaying ? songMax : Mathf.Abs(freezeMax - dontDanceWaitSeconds);
                float waitTime = Random.Range(min, max);
                //print("wait time = " + waitTime);
                yield return new WaitForSeconds(waitTime);

                // Switch song state
                if (songIsPlaying)
                {
                    song.Pause();
                    songIsPlaying = false;
                    yield return new WaitForSeconds(dontDanceWaitSeconds);
                    gameText.text = "need to be still now";
                    dontMove = true;
                    if (notDancingEnoughCoroutine != null)
                    {
                        StopCoroutine(notDancingEnoughCoroutine);
                        clearText();
                        nde_isRunning = false;
                    }
                    songCube.GetComponent<Renderer>().material = red;
                }
                else
                {
                    song.Play();
                    songIsPlaying = true;
                    dontMove = false;
                    songCube.GetComponent<Renderer>().material = green;
                }
            }
        }

        IEnumerator checkForMotion()
        {
            while (true)
            {
                // Calculate and store speed of each piece of hardware
                CoroutineWithData r_cd = new CoroutineWithData(this, arithmetic.getSpeedOfObject(r_Controller, checksPerSecond));
                CoroutineWithData l_cd = new CoroutineWithData(this, arithmetic.getSpeedOfObject(l_Controller, checksPerSecond));
                CoroutineWithData h_cd = new CoroutineWithData(this, arithmetic.getSpeedOfObject(headset,      checksPerSecond));
                yield return r_cd.coroutine;
                yield return l_cd.coroutine;
                yield return h_cd.coroutine;

                if (!songIsPlaying)
                {
                    // See if player is still enough
                    if ((float)r_cd.result > controllerStillThreshold + stillTolerance ||
                         (float)l_cd.result > controllerStillThreshold + stillTolerance ||
                         (float)h_cd.result > headsetStillThreshold + stillTolerance)
                    {
                        // Player is moving
                        if (dontMove)
                        {
                            gameText.text = "You Moved! Game Over";
                            gameOver();
                        }
                        minMovementCube.GetComponent<Renderer>().material = green;
                    }
                    else
                    {
                        // Player is stationary
                        minMovementCube.GetComponent<Renderer>().material = red;
                    }
                }
                else // if (songIsPlaying)
                {
                    // See if player is moving enough
                    if ((float)r_cd.result < controllerMovingThreshold - movingTolerance ||
                         (float)l_cd.result < controllerMovingThreshold - movingTolerance ||
                         (float)h_cd.result < headsetMovingThreshold - movingTolerance)
                    {
                        // Player is not moving enough
                        if(!nde_isRunning) notDancingEnoughCoroutine = StartCoroutine(notDancingEnough());
                        maxMovementCube.GetComponent<Renderer>().material = red;
                    }
                    else
                    {
                        // Player is moving enough
                        maxMovementCube.GetComponent<Renderer>().material = green;
                        if (notDancingEnoughCoroutine != null)
                        {
                            StopCoroutine(notDancingEnoughCoroutine);
                            clearText();
                            nde_isRunning = false;
                        }
                    }
                }
            }
        }

        IEnumerator notDancingEnough()
        {
            nde_isRunning = true;
            yield return new WaitForSeconds(1);
            int secondsUntilGameOver = notDancingWaitSeconds;
            while (secondsUntilGameOver > 0)
            {
                gameText.text = "You're not dancing enough! You have " + secondsUntilGameOver + " seconds";
                yield return new WaitForSeconds(1);
                secondsUntilGameOver--;
            }
            gameText.text = "You didn't dance enough. Game Over";
            gameOver();
            nde_isRunning = false;
        }

        void gameOver()
        {
            StopCoroutine(playMusicCoroutine);
            StopCoroutine(checkForMotionCoroutine);
            song.Stop();
        }

        void clearText()
        {
            gameText.text = "";
        }

    }
}