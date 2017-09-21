using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedCheckScript : MonoBehaviour {

    public GameObject rightController;
    public GameObject leftController;
    public Material greenMaterial;
    public Material redMaterial;

    private SteamVR_TrackedController r;
    private SteamVR_TrackedController l;
    private bool isRecording = false;
    private Renderer renderer;

	// Use this for initialization
	void Start () {
        renderer = GetComponent<Renderer>();

        r = rightController.GetComponent<SteamVR_TrackedController>();
        l = leftController.GetComponent<SteamVR_TrackedController>();
        l.TriggerClicked += lTriggered;
        r.TriggerClicked += rTriggered;
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void rTriggered(object sender, ClickedEventArgs e)
    {
        print("right");
        Triggered();
    }

    void lTriggered(object sender, ClickedEventArgs e)
    {
        print("left");
        Triggered();
    }

    void Triggered()
    {
        if(isRecording)
        {
            print("red");
            isRecording = false;
            renderer.material = redMaterial;
        }
        else
        {
            print("green");
            isRecording = true;
            renderer.material = greenMaterial;
        }
    }
}
