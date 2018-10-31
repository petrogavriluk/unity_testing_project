using Helpers;
using Interfaces;
using Serialization;
using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ConnectionPointController : MonoBehaviour, IConnector
{
    [SerializeField]
    private Color defaultColor;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color connectedColor;
    private Side side;
    bool hovered = false;
    Renderer objectRenderer;
    bool isVisible = true;
    private IConnector connectedObject;

    public IAttachable Owner { get; set; }
    public IConnector ConnectedObject
    {
        get { return connectedObject; }
        set
        {
            connectedObject = value;
            UpdateColorAndVisibility();
            gameObject.layer = LayerMask.NameToLayer(
                (ConnectedObject != null) ? LayerNames.ConnectedPoint : LayerNames.ConnectionPoint);
        }
    }

    public GameObject Body
    {
        get { return gameObject; }
    }
    private Color MaterialColor
    {
        get
        {
            return objectRenderer.material.color;
        }
        set
        {
            objectRenderer.material.color = value;
        }
    }
    public bool Hovered
    {
        get
        {
            return hovered;
        }

        set
        {
            if (hovered != value)
            {
                hovered = value;
                UpdateColorAndVisibility();
                if (hovered)
                {
                    MaterialColor = selectedColor;
                }
                else
                {
                    MaterialColor = defaultColor;
                }
            }
        }
    }

    public Renderer ObjectRenderer
    {
        get
        {
            return objectRenderer;
        }
    }

    public bool Visible
    {
        get
        {
            return isVisible || ConnectedObject != null;
        }

        set
        {
            isVisible = value;
            UpdateColorAndVisibility();
        }
    }

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        Controllers.MoveHelperInstance.RegisterConnector(this);
    }

    public void Connect(IConnector connector)
    {
        ConnectedObject = connector;
        connector.ConnectedObject = this;
    }
    public void Disconnect()
    {
        if (ConnectedObject == null)
            return;

        ConnectedObject.ConnectedObject = null;
        ConnectedObject = null;
    }

    private void UpdateColorAndVisibility()
    {
        if (ConnectedObject != null)
        {
            MaterialColor = connectedColor;
        }
        else
        {
            MaterialColor = Hovered ? selectedColor : defaultColor;
        }
        if (gameObject.activeSelf != Visible)
        {
            gameObject.SetActive(Visible);
        }
    }

    public Side GetSide() => side;

    public void SetSide(Side connectorSide)
    {
        side = connectorSide;
    }

    private void OnDestroy()
    {
        Disconnect();
        Controllers.MoveHelperInstance.UnregisterConnector(this);
    }

    public ConnectorSaveData SaveState()
    {
        return new ConnectorSaveData()
        {
            Owner = Owner.UID.ToString(),
            ConnectedID = ((ConnectedObject?.Owner?.UID) ?? Guid.Empty).ToString(),
            side = (int)GetSide()
        };
    }
}
