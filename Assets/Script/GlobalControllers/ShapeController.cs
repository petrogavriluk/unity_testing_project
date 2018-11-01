using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Helpers;

public class ShapeController : MonoBehaviour
{
    public Button MakeCubeButton;
    public Button MakeCylinderButton;
    public Button MakeCapsuleButton;
    public Button MakeSphereButton;

    public Button CreateCubeButton;
    public Button CreateCylinderButton;
    public Button CreateCapsuleButton;
    public Button CreateSphereButton;

    public Toggle MoveAsGroup;

    public event EventHandler<PrimitiveType> ChangeShape;

    // Use this for initialization
    void Start()
    {
        MakeCubeButton.onClick.AddListener(() => InvokeChangeShape(PrimitiveType.Cube));
        MakeCylinderButton.onClick.AddListener(() => InvokeChangeShape(PrimitiveType.Cylinder));
        MakeCapsuleButton.onClick.AddListener(() => InvokeChangeShape(PrimitiveType.Capsule));
        MakeSphereButton.onClick.AddListener(() => InvokeChangeShape(PrimitiveType.Sphere));

        CreateCubeButton.onClick.AddListener(() => InvokeCreateShape(PrimitiveType.Cube));
        CreateCylinderButton.onClick.AddListener(() => InvokeCreateShape(PrimitiveType.Cylinder));
        CreateCapsuleButton.onClick.AddListener(() => InvokeCreateShape(PrimitiveType.Capsule));
        CreateSphereButton.onClick.AddListener(() => InvokeCreateShape(PrimitiveType.Sphere));
    }

    private void InvokeCreateShape(PrimitiveType type)
    {
        MonoBehaviour cube = Controllers.CreatorInstance.GetCubeObject(type);
        Controllers.MoveHelperInstance.StartMoving(new MonoBehaviour[] { cube });

    }

    void InvokeChangeShape(PrimitiveType type)
    {
        if (ChangeShape != null)
        {
            ChangeShape.Invoke(this, type);
        }
    }
}
