using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public event EventHandler CameraMoved;
    // Use this for initialization
    void Start()
    {
        PositionChanged();
        Camera.main.transform.hasChanged = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main.transform.hasChanged)
        {
            PositionChanged();
            Camera.main.transform.hasChanged = false;
        }
    }

    void PositionChanged()
    {
        if(CameraMoved!=null)
        {
            CameraMoved.Invoke(this, EventArgs.Empty);
        }
    }
}