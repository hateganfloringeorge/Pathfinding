using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeCheckpoint : MonoBehaviour
{
    private GameObject objToSpawn;
    private InvDrone drone_script;

    private void Start()
    {
        drone_script = GameObject.Find("Drone").GetComponent<InvDrone>();
    }

    private void FixedUpdate()
    {
        if (!drone_script._firstPosition)
        {
            objToSpawn = new GameObject("Checkpoint");
            objToSpawn.tag = "Checkpoint";
            objToSpawn.transform.position = this.transform.position;
        }
    }
}
