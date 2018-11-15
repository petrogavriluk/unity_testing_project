using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/AnimationData")]
public class AnimationData : ScriptableObject
{
    public float bouncePerSec;
    public float amplitude;
    public float rescalePerSec;

    public AnimationCurve MovementCurve;
    public AnimationCurve ScaleCurve;
}
