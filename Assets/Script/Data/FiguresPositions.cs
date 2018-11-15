using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/FiguresPositions")]
public class FiguresPositions : ScriptableObject
{
    private static FiguresPositions instance;
    public static FiguresPositions Instance { get
        {
            if (instance == null)
                instance = Resources.Load<FiguresPositions>("FiguresPositions");
            return instance;
        } }
    public Vector3 CubeShift;
    public Vector3 CylinderShift;
    public Vector3 CapsuleShift;
    public Vector3 SphereShift;

}
