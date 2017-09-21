using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Assets.myScripts
{
    public class speedCheckScript : MonoBehaviour
    {

        public GameObject r_Controller;
        public GameObject l_Controller;
        public GameObject headset;
        public Material grey;
        public Material red;
        public int checksPerSecond = 10;

        private SteamVR_TrackedController r;
        private SteamVR_TrackedController l;
        private bool isRecording = false;
        private List<float> r_ControllerLog;
        private List<float> l_ControllerLog;
        private List<float> headsetLog;

        // Use this for initialization
        void Start()
        {
            // Have controller triggers activate an method called "Triggered"
            r = r_Controller.GetComponent<SteamVR_TrackedController>();
            l = l_Controller.GetComponent<SteamVR_TrackedController>();
            l.TriggerClicked += Triggered;
            r.TriggerClicked += Triggered;

        }

        void Triggered(object sender, ClickedEventArgs e)
        {
            // Make trigger clicks stop or start recording
            if (isRecording)
            {
                isRecording = false; // This will end the recording
                GetComponent<Renderer>().material = grey;
            }
            else
            {
                isRecording = true;
                GetComponent<Renderer>().material = red;

                // Begin Recording Movement Data
                StartCoroutine(RecordMovementData());
            }
        }

        IEnumerator RecordMovementData()
        {
            r_ControllerLog = new List<float>();
            l_ControllerLog = new List<float>();
            headsetLog = new List<float>();
            Arithmetic arithmetic = new Arithmetic();

            while (isRecording)
            {
                // Calculate and store speed of each piece of hardware
                CoroutineWithData r_cd = new CoroutineWithData(this, arithmetic.getSpeedOfObject(r_Controller, checksPerSecond));
                CoroutineWithData l_cd = new CoroutineWithData(this, arithmetic.getSpeedOfObject(l_Controller, checksPerSecond));
                CoroutineWithData h_cd = new CoroutineWithData(this, arithmetic.getSpeedOfObject(headset, checksPerSecond));
                yield return r_cd.coroutine;
                yield return l_cd.coroutine;
                yield return h_cd.coroutine;
                r_ControllerLog.Add((float)r_cd.result);
                l_ControllerLog.Add((float)l_cd.result);
                headsetLog.Add((float)h_cd.result);
            }

            // Print results
            print("Right controller average speed: " + r_ControllerLog.Average());
            print("Left controller average speed: " + l_ControllerLog.Average());
            print("Headset average speed: " + headsetLog.Average());
        }
    }
}
