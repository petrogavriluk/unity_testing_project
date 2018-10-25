using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Interfaces;
using Helpers;

public class Creator : MonoBehaviour
{
    [SerializeField]
    GameObject connectionPointPrefab;
    [SerializeField]
    GameObject cubePrefab;
    [SerializeField]
    TextMesh textPrefab;
    public IConnector CreateConnectionPoint(IAttachable owner, Side side, bool show)
    {
        var instance = Instantiate(connectionPointPrefab);
        var monoBehaviour = instance.GetComponent<ConnectionPointController>();
        if (monoBehaviour == null)
            throw new MissingComponentException("Missing script");

        monoBehaviour.Owner = owner;
        monoBehaviour.Body.SetActive(show);
        monoBehaviour.SetSide(side);
        return monoBehaviour;
    }

    public CubeControl CreateMesh(PrimitiveType meshType = PrimitiveType.Cube)
    {
        var instance = Instantiate(cubePrefab);
        var monoBehaviour = instance.GetComponent<CubeControl>();
        if (monoBehaviour == null)
            throw new MissingComponentException("Missing script");

        monoBehaviour.SetMeshDirectly(meshType);

        return monoBehaviour;
    }

    public TextMesh CreateTextMesh(string initialText="")
    {
        var instance = Instantiate(textPrefab);
        instance.text = initialText;
        return instance;
    }
}
