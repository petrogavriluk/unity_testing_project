using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Helpers;
using Interfaces;
using UnityEngine.Assertions;

public class MoveHelper : MonoBehaviour
{
    static readonly float epsilon = 0.01f;
    private bool isMoving = false;
    private bool skipClick = false;
    IEnumerable<MonoBehaviour> movedObjects = Enumerable.Empty<MonoBehaviour>();
    Bounds groupBounds;
    bool connectionHit = false;
    private readonly Dictionary<int, IConnector> objectIdToConnectorMap = new Dictionary<int, IConnector>();
    private readonly Dictionary<Side, IConnector> sideToConnector = new Dictionary<Side, IConnector>();
    private IConnector currentConnector = null;

    public bool IsMoving
    {
        get { return isMoving; }
    }

    public IConnector CurrentConnector
    {
        get
        {
            return currentConnector;
        }

        set
        {
            if(currentConnector!=value)
            {
                if (currentConnector != null)
                    currentConnector.Owner.OnConnectorHoverEnd(currentConnector);
                if (value != null)
                    value.Owner.OnConnectorHoverStart(value);
                currentConnector = value;
            }
        }
    }

    public bool MoveAsGroup
    {
        get { return Controllers.ShapeControllerInstance.MoveAsGroup.isOn; }
    }

    private void Update()
    {
        if (!isMoving)
            return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        connectionHit = Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask(LayerNames.connectionPoint));
        if (connectionHit)
        {
            MoveToConnector(hit);
        }
        if (!connectionHit)
        {
            CurrentConnector = null;
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                MoveObjects(hit);
            }
        }

        if (Input.GetMouseButtonDown(0) && !skipClick)
        {
            StopMoving();
        }
        skipClick = false;
    }

    private void MoveToConnector(RaycastHit hit)
    {
        IConnector hoveredConnector = objectIdToConnectorMap[hit.collider.gameObject.GetInstanceID()];
        IConnector groupConnector = sideToConnector[hoveredConnector.GetSide().GetOppositeSide()];

        if (hoveredConnector.ConnectedObject != null)
        {
            connectionHit = false;
            return;
        }

        Vector3 shift = hoveredConnector.Body.transform.position - groupConnector.Body.transform.position;
        groupBounds.center += shift;
        foreach (var obj in movedObjects)
        {
            obj.transform.position += shift;
        }
        CurrentConnector = hoveredConnector;
    }

    void MoveObjects(RaycastHit hit)
    {
        Vector3 newCenter = hit.point;
        newCenter.x += GetDirection(hit.normal.x) * groupBounds.extents.x;
        newCenter.y += GetDirection(hit.normal.y) * groupBounds.extents.y;
        newCenter.z += GetDirection(hit.normal.z) * groupBounds.extents.z;
        Vector3 shift = newCenter - groupBounds.center;

        groupBounds.center = newCenter;
        foreach (var obj in movedObjects)
        {
            obj.transform.position += shift;
        }
    }

    float GetDirection(float v)
    {
        if (v > epsilon)
        {
            return 1f;
        }
        else if (v < -epsilon)
        {
            return -1f;
        }
        return 0f;
    }

    public void StartMoving(MonoBehaviour objectToMove)
    {
        IEnumerable<MonoBehaviour> group;
        if (Controllers.MoveHelperInstance.MoveAsGroup && objectToMove is IAttachable)
        {
            group = ConnectionHelper.GetAllConnected((IAttachable)objectToMove).Select(a => a as MonoBehaviour);
        }
        else
        {
            group = new MonoBehaviour[] { objectToMove };
            if (objectToMove is IAttachable)
            {
                (objectToMove as IAttachable).DetachAll();
            }
        }
        StartMoving(group);
    }

    public void StartMoving(IEnumerable<MonoBehaviour> objects)
    {
        isMoving = true;

        movedObjects = movedObjects.Union(objects);
        groupBounds = CalculateSize(objects);
        UpdateSides();

        foreach (var obj in objects)
        {
            obj.gameObject.layer = LayerMask.NameToLayer(LayerNames.ignoreRaycast);
            var movable = obj as IMovable;
            if (movable != null)
            {
                movable.IsMoving = true;
            }
        }
        ShowConnectors(true, objectIdToConnectorMap.Select(c => c.Value.Owner).Distinct());
        ShowConnectors(false, objects.Select(o => o as IAttachable).Where(o => o != null));
        skipClick = true;
    }

    void ShowConnectors(bool show, IEnumerable<IAttachable> attachables)
    {
        foreach (var attachable in attachables)
        {
            attachable.ShowConnectors = show;
        }
    }

    private void UpdateSides()
    {
        sideToConnector.Clear();
        foreach (var obj in movedObjects)
        {
            if (!(obj is IAttachable))
                continue;

            if (!sideToConnector.ContainsKey(Side.XMin) || sideToConnector[Side.XMin].Body.transform.position.x > ((IAttachable)obj).Connectors[Side.XMin].Body.transform.position.x)
                sideToConnector[Side.XMin] = ((IAttachable)obj).Connectors[Side.XMin];
            if (!sideToConnector.ContainsKey(Side.YMin) || sideToConnector[Side.YMin].Body.transform.position.y > ((IAttachable)obj).Connectors[Side.YMin].Body.transform.position.y)
                sideToConnector[Side.YMin] = ((IAttachable)obj).Connectors[Side.YMin];
            if (!sideToConnector.ContainsKey(Side.ZMin) || sideToConnector[Side.ZMin].Body.transform.position.z > ((IAttachable)obj).Connectors[Side.ZMin].Body.transform.position.z)
                sideToConnector[Side.ZMin] = ((IAttachable)obj).Connectors[Side.ZMin];

            if (!sideToConnector.ContainsKey(Side.XMax) || sideToConnector[Side.XMax].Body.transform.position.x < ((IAttachable)obj).Connectors[Side.XMax].Body.transform.position.x)
                sideToConnector[Side.XMax] = ((IAttachable)obj).Connectors[Side.XMax];
            if (!sideToConnector.ContainsKey(Side.YMax) || sideToConnector[Side.YMax].Body.transform.position.y < ((IAttachable)obj).Connectors[Side.YMax].Body.transform.position.y)
                sideToConnector[Side.YMax] = ((IAttachable)obj).Connectors[Side.YMax];
            if (!sideToConnector.ContainsKey(Side.ZMax) || sideToConnector[Side.ZMax].Body.transform.position.z < ((IAttachable)obj).Connectors[Side.ZMax].Body.transform.position.z)
                sideToConnector[Side.ZMax] = ((IAttachable)obj).Connectors[Side.ZMax];
        }
    }

    private Bounds CalculateSize(IEnumerable<MonoBehaviour> objects)
    {
        Bounds bounds = objects.First().GetBounds();
        foreach (var obj in objects)
        {
            Assert.IsTrue(obj is IObjectWithRenderer);
            bounds.Encapsulate(obj.GetBounds());
        }
        return bounds;
    }

    public void StopMoving()
    {
        if (!isMoving)
            return;

        isMoving = false;
        if(connectionHit)
        {
            IConnector groupConnector = sideToConnector[CurrentConnector.GetSide().GetOppositeSide()];
            CurrentConnector.Connect(groupConnector);
            CurrentConnector = null;
        }
        sideToConnector.Clear();
        foreach (var obj in movedObjects)
        {
            obj.gameObject.layer = LayerMask.NameToLayer(LayerNames.defaultLayer);
            var movable = obj.GetComponent(typeof(IMovable)) as IMovable;
            if (movable != null)
            {
                movable.IsMoving = false;
            }
        }
        ShowConnectors(false, objectIdToConnectorMap.Select(c => c.Value.Owner).Distinct());
        movedObjects = Enumerable.Empty<MonoBehaviour>();
        skipClick = false;
    }

    public void RegisterConnector(IConnector connector)
    {
        objectIdToConnectorMap[connector.Body.GetInstanceID()] = connector;
    }
    public void UnregisterConnector(IConnector connector)
    {
        objectIdToConnectorMap.Remove(connector.Body.GetInstanceID());
    }
}
