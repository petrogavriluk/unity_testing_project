using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour
{
    public Button StartScaleButton;
    public Button StopScaleButton;

    public Button StartMoveButton;
    public Button StopMoveButton;

    public Button StartRotationButton;
    public Button StopRotationButton;

    public event EventHandler<bool> SetMoveAnimation;
    public event EventHandler<bool> SetRotationAnimation;
    public event EventHandler<bool> SetScaleAnimation;


    // Use this for initialization
    void Start()
    {
        StartScaleButton.onClick.AddListener(() => InvokeScaleAnimation(true));
        StopScaleButton.onClick.AddListener(() => InvokeScaleAnimation(false));

        StartRotationButton.onClick.AddListener(() => InvokeRotationAnimation(true));
        StopRotationButton.onClick.AddListener(() => InvokeRotationAnimation(false));

        StartMoveButton.onClick.AddListener(() => InvokeMoveAnimation(true));
        StopMoveButton.onClick.AddListener(() => InvokeMoveAnimation(false));

    }

    void InvokeRotationAnimation(bool enable)
    {
        if (SetRotationAnimation != null)
        {
            SetRotationAnimation.Invoke(this, enable);
        }
    }
    void InvokeScaleAnimation(bool enable)
    {
        if (SetScaleAnimation != null)
        {
            SetScaleAnimation.Invoke(this, enable);
        }
    }
    void InvokeMoveAnimation(bool enable)
    {
        if (SetMoveAnimation != null)
        {
            SetMoveAnimation.Invoke(this, enable);
        }
    }
}
