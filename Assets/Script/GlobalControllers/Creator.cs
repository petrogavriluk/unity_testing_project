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
    Transform cubeParent;
    [SerializeField]
    TextMesh textPrefab;
    [SerializeField]
    uint poolInitSize;

    private readonly Queue<CubeControl> cubePool = new Queue<CubeControl>();

    private void Awake()
    {
        
    }

    void InitPool()
    {
        for (int i = 0; i < poolInitSize; i++)
        {
            var cube = InstantiateCube();
            DestroyCubeObject(cube);
        }
    }

    public IConnector CreateConnectionPoint(IAttachable owner, Side side, bool show)
    {
        var instance = Instantiate(connectionPointPrefab);
        var monoBehaviour = instance.GetComponent<ConnectionPointController>();
        if (monoBehaviour == null)
            throw new MissingComponentException("Missing script");

        monoBehaviour.Owner = owner;
        monoBehaviour.Visible = show;
        monoBehaviour.SetSide(side);
        return monoBehaviour;
    }

    public CubeControl GetCubeObject(PrimitiveType meshType = PrimitiveType.Cube)
    {
        CubeControl cube;
        if (cubePool.Count > 0)
        {
            cube = cubePool.Dequeue();
            cube.gameObject.SetActive(true);
        }
        else
        {
            cube = InstantiateCube();
        }

        cube.SetMeshDirectly(meshType);
        return cube;
    }

    public CubeControl InstantiateCube()
    {
        var instance = Instantiate(cubePrefab);
        var cube = instance.GetComponent<CubeControl>();
        if (cube == null)
            throw new MissingComponentException("Missing script");
        cube.transform.parent = cubeParent;
        return cube;
    }

    public void DestroyCubeObject(CubeControl cube)
    {
        if (cubePool.Contains(cube))
            return;

        cube.ResetDefault(cubePrefab.transform);
        cube.gameObject.SetActive(false);
        cubePool.Enqueue(cube);
    }

    public TextMesh CreateTextMesh(string initialText="")
    {
        var instance = Instantiate(textPrefab);
        instance.text = initialText;
        return instance;
    }
}
