using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedTest : MonoBehaviour {

    public int checksPerSecond = 10;
    public int speedThreshold = 5;

    private bool isMoving = false;

	// Use this for initialization
	void Start () {
        StartCoroutine(monitorSpeed());
        StartCoroutine(printStatus());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator printStatus()
    {
        while(true)
        {
            if (isMoving)
            {
                print("Moving");
            }
            else
            {
                print("Not moving");
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    IEnumerator monitorSpeed()
    {
        Vector3 previousPosition;
        float speed;
        while(true)
        {
            previousPosition = transform.position;
            yield return new WaitForSeconds(1f / checksPerSecond);
            speed = Vector3.Distance(previousPosition, transform.position) / (1f / checksPerSecond);
            isMoving = speed >= speedThreshold ? true : false;
        }
    }
}
