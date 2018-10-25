using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TransformAnimator : ITransformAnimator
{
    public event EventHandler ObjectMoved;
    public event EventHandler ObjectRotated;
    public event EventHandler ObjectScaled;

    readonly MonoBehaviour animatedObject;
    Coroutine moveCoroutine = null;
    Coroutine scaleCoroutine = null;
    Coroutine rotationCoroutine = null;
    int rotationCounter = 0;
    float scaleAnimationTime = 0f;
    float moveAnimationTime = 0f;

    public AnimationCurve MovementCurve { get; set; }
    public AnimationCurve ScaleCurve { get; set; }
    public float BouncePerSec { get; set; }

    public float Amplitude { get; set; }

    public float RescalePerSec { get; set; }

    public TransformAnimator(MonoBehaviour monoBehaviour)
    {
        animatedObject = monoBehaviour;
    }

    public void SetMoveAnimationState(bool enable)
    {
        if (enable)
        {
            if (moveCoroutine == null)
                moveCoroutine = animatedObject.StartCoroutine(AnimateMovement());
        }
        else
        {
            if (moveCoroutine != null)
            {
                animatedObject.StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }
        }
    }

    public void SetRotateAnimationState(bool enable)
    {
        if (enable)
        {
            if (rotationCoroutine == null)
                rotationCoroutine = animatedObject.StartCoroutine(AnimateRotation());
        }
        else
        {
            if (rotationCoroutine != null)
            {
                animatedObject.StopCoroutine(rotationCoroutine);
                rotationCoroutine = null;
            }
        }
    }

    public void SetScaleAnimationState(bool enable)
    {
        if (enable)
        {
            if (scaleCoroutine == null)
                scaleCoroutine = animatedObject.StartCoroutine(AnimateScale());
        }
        else
        {
            if (scaleCoroutine != null)
            {
                animatedObject.StopCoroutine(scaleCoroutine);
                scaleCoroutine = null;
            }
        }
    }
    IEnumerator AnimateRotation()
    {
        while (true)
        {
            if ((rotationCounter / 50 % 2) == 1)
            {
                animatedObject.transform.Rotate(5f, 0f, 5f);
            }
            else
            {
                animatedObject.transform.Rotate(-5f, 0f, -5f);
            }
            ObjectRotated?.Invoke(this, EventArgs.Empty);
            ++rotationCounter;
            yield return null;
        }
    }
    IEnumerator AnimateScale()
    {
        Vector3 minScale = new Vector3(0.15f, 0.15f, 0.15f);
        Vector3 maxScale = new Vector3(1.50f, 1.50f, 1.50f);

        while (true)
        {
            animatedObject.transform.localScale = Vector3.Lerp(maxScale, minScale, ScaleCurve.Evaluate(scaleAnimationTime));
            scaleAnimationTime += Time.deltaTime * RescalePerSec * 2;

            ObjectScaled?.Invoke(this, EventArgs.Empty);
            yield return null;
        }
    }
    IEnumerator AnimateMovement()
    {
        while (true)
        {
            float shift = GetShiftCoefficient(moveAnimationTime + Time.deltaTime) - GetShiftCoefficient(moveAnimationTime);
            animatedObject.transform.position += new Vector3(0f, shift * Amplitude, 0f);
            moveAnimationTime += Time.deltaTime;
            ObjectMoved?.Invoke(this, EventArgs.Empty);
            yield return null;
        }
    }
    float GetShiftCoefficient(float time)
    {
        return MovementCurve.Evaluate(time * BouncePerSec * 2);
        //return -Mathf.Cos((time) * bouncePerSec * (2 * Mathf.PI));
    }
}
