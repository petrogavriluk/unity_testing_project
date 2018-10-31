using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public event EventHandler CameraMoved;

    private readonly float rotationSpeed = 2f;
    private readonly float movementSpeed = 0.1f;
    // Use this for initialization
    void Start()
    {
        PositionChanged();
        Camera.main.transform.hasChanged = false;
    }

    private void FixedUpdate()
    {
        float horRotation = Input.GetAxis("RotateHorizontal");
        float verRotation = -Input.GetAxis("RotateVertical");

        transform.eulerAngles += new Vector3(verRotation * rotationSpeed, horRotation * rotationSpeed, 0f);

        float horMove = Input.GetAxis("Horizontal");
        float forwMove = Input.GetAxis("Vertical");

        transform.Translate(Camera.main.transform.forward.normalized * forwMove * movementSpeed, Space.World);
        var right = Vector3.Cross(Camera.main.transform.up, Camera.main.transform.forward).normalized;
        transform.Translate(right * horMove * movementSpeed, Space.World);
    }
    void LateUpdate()
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