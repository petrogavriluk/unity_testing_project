using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Interfaces;
using Helpers;


public class ConnectionPointController : MonoBehaviour, IConnector
{
    [SerializeField]
    private Color defaultColor;
    [SerializeField]
    private Color selectedColor;
    private Side side;
    bool hovered = false;
    Renderer objectRenderer;

    public IAttachable Owner { get; set; }
    public IConnector ConnectedObject { get; set; }

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
            if(hovered!=value)
            {
                hovered = value;
                if(hovered)
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
        ConnectedObject.ConnectedObject = null;
        ConnectedObject = null;
    }

    public Side GetSide() => side;

    public void SetSide(Side connectorSide)
    {
        side = connectorSide;
    }

    private void OnDestroy()
    {
        Controllers.MoveHelperInstance.UnregisterConnector(this);
    }

}
