using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Helpers;
using Interfaces;

public class CubeControl : MonoBehaviour, IMovable, ICopyable, IObjectWithRenderer, IAttachable
{
    private static readonly float doubleClickDelay = 0.4f;

    public Color defaultColor;
    public Color hoverColor;
    public Color selectedColor;
    public Color potentialConnectionColor;

    private TextMesh stateText;

    public float bouncePerSec;
    public float amplitude;
    public float rescalePerSec;

    public Vector3 CubePosition;
    public Vector3 CylinderPosition;
    public Vector3 CapsulePosition;
    public Vector3 SpherePosition;

    private bool isSelected = false;
    private bool isHovered = false;
    private bool isConnectorActive = false;

    [SerializeField]
    private Vector3 startPosition;
    [SerializeField]
    private AnimationCurve MovementCurve;
    [SerializeField]
    private AnimationCurve ScaleCurve;

    TransformAnimator animator;
    Renderer objectRenderer;
    BoxCollider boxCollider;
    MeshFilter meshFilter;
    private bool isMoving = false;
    private float lastClick = 0f;
    private bool showConnectors = false;
    private readonly Dictionary<Side, IConnector> sideConnectors = new Dictionary<Side, IConnector>();

    private Color MaterialColor
    {
        get
        {
            return ObjectRenderer.material.color;
        }
        set
        {
            ObjectRenderer.material.color = value;
        }
    }

    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
        set
        {
            isMoving = value;
            if (isMoving)
            {
                animator.SetMoveAnimationState(false);
                animator.SetRotateAnimationState(false);
            }
        }
    }

    public Renderer ObjectRenderer
    {
        get
        {
            if (objectRenderer == null)
                objectRenderer = GetComponent<Renderer>();
            return objectRenderer;
        }
    }

    public bool ShowConnectors
    {
        get
        {
            return showConnectors;
        }

        set
        {
            if (showConnectors != value)
            {
                showConnectors = value;
                foreach (var connector in Connectors.Values)
                {
                    connector.Body.SetActive(showConnectors);
                }
            }
        }
    }

    public IDictionary<Side, IConnector> Connectors
    {
        get { return sideConnectors; }
    }

    private void Awake()
    {
        stateText = Controllers.CreatorInstance.CreateTextMesh();
        stateText.transform.parent = gameObject.transform;
        boxCollider = GetComponent<BoxCollider>();
        meshFilter = GetComponent<MeshFilter>();

        transform.position = startPosition = CubePosition;
        SetMeshDirectly(PrimitiveType.Cube);

        CreateConnectors();
        InitControllers();
    }

    // Use this for initialization
    void Start()
    {
        MaterialColor = defaultColor;
        UpdateState();
    }

    void Update()
    {
        CheckButtonClicks();
    }

    private void CheckButtonClicks()
    {
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);
        if (leftClick || rightClick)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject)
            {
                if (rightClick)
                {
                    OnRightMouseDown(hit.point);
                }
                else
                {
                    if (Time.time - lastClick <= doubleClickDelay)
                    {
                        OnDoubleClick();
                    }
                    lastClick = Time.time;
                }
            }
        }
    }

    private void InitControllers()
    {
        Controllers.ShapeControllerInstance.ChangeShape += SetMesh;
        Controllers.AnimationControllerInstance.SetMoveAnimation += SetMove;
        Controllers.AnimationControllerInstance.SetRotationAnimation += SetRotation;
        Controllers.AnimationControllerInstance.SetScaleAnimation += SetScale;
        Controllers.CameraControllerInstance.CameraMoved += OnCameraMoved;

        animator = new TransformAnimator(this);
        animator.ObjectMoved += OnCameraMoved;
        animator.ObjectRotated += OnCameraMoved;
        animator.ObjectRotated += UpdateConnectors;
        animator.ObjectScaled += OnCameraMoved;
        animator.ObjectScaled += UpdateConnectors;

        animator.BouncePerSec = bouncePerSec;
        animator.Amplitude = amplitude;
        animator.RescalePerSec = rescalePerSec;
        animator.ScaleCurve = ScaleCurve;
        animator.MovementCurve = MovementCurve;
    }

    private void CreateConnectors()
    {
        CreateConnector(Side.XMin);
        CreateConnector(Side.XMax);
        CreateConnector(Side.YMin);
        CreateConnector(Side.YMax);
        CreateConnector(Side.ZMin);
        CreateConnector(Side.ZMax);
    }

    private void CreateConnector(Side side)
    {
        IConnector connector = Controllers.CreatorInstance.CreateConnectionPoint(this, side, ShowConnectors);
        connector.Body.transform.position = objectRenderer.GetEdgePoint(side);
        connector.Body.transform.parent = gameObject.transform;
        sideConnectors[side] = connector;
    }

    void OnRightMouseDown(Vector3 point)
    {
        Controllers.ContextMenuControllerInstance.ShowContextMenu(this, point, animator);
    }

    void OnDoubleClick()
    {
        if (Controllers.MoveHelperInstance.IsMoving)
            return;

        Controllers.MoveHelperInstance.StartMoving( this);
    }

    private void OnMouseDown()
    {
        isSelected = !isSelected;
        UpdateState();

    }
    private void OnMouseEnter()
    {
        isHovered = true;
        UpdateState();
    }

    private void OnMouseExit()
    {
        isHovered = false;
        UpdateState();
    }

    private void UpdateState()
    {
        if (isConnectorActive)
        {
            stateText.text = "Connect?";
            MaterialColor = potentialConnectionColor;
        }
        else if (isSelected)
        {
            stateText.text = "Selected";
            MaterialColor = selectedColor;
        }
        else if (isHovered)
        {
            stateText.text = "Hovered";
            MaterialColor = hoverColor;
        }
        else
        {
            stateText.text = "Normal";
            MaterialColor = defaultColor;
        }
    }

    void UpdateConnectors(object sender = null, EventArgs e = null)
    {
        foreach (var connector in Connectors)
        {
            connector.Value.Body.transform.position = objectRenderer.GetEdgePoint(connector.Key);
        }
    }
    private void OnCameraMoved(object sender = null, System.EventArgs e = null)
    {
        Vector3 textPosition = this.transform.position;
        textPosition.y += ObjectRenderer.bounds.size.y * 3 / 4;
        stateText.transform.position = textPosition;
        stateText.transform.rotation = (Camera.main.transform.rotation);
    }

    private void SetMesh(object sender, PrimitiveType type)
    {
        if (!isSelected)
            return;
        SetMeshDirectly(type);
    }
    public void SetMeshDirectly(PrimitiveType type)
    {
        switch (type)
        {
            case PrimitiveType.Sphere:
                meshFilter.mesh = Primitives.sphereMesh;
                transform.position += SpherePosition - startPosition;
                startPosition = SpherePosition;
                break;
            case PrimitiveType.Capsule:
                meshFilter.mesh = Primitives.capsuleMesh;
                transform.position += CapsulePosition - startPosition;
                startPosition = CapsulePosition;
                break;
            case PrimitiveType.Cylinder:
                meshFilter.mesh = Primitives.cylinderMesh;
                transform.position += CylinderPosition - startPosition;
                startPosition = CylinderPosition;
                break;
            default:
                meshFilter.mesh = Primitives.cubeMesh;
                transform.position += CubePosition - startPosition;
                startPosition = CubePosition;
                break;
        }
        boxCollider.size = ObjectRenderer.bounds.size / transform.localScale.x;
        OnCameraMoved();
        UpdateConnectors();
    }

    void SetRotation(object sender, bool enable)
    {
        if (!isSelected || isMoving)
            return;

        animator.SetRotateAnimationState(enable);
    }
    void SetMove(object sender, bool enable)
    {
        if (!isSelected || isMoving)
            return;

        animator.SetMoveAnimationState(enable);
    }
    void SetScale(object sender, bool enable)
    {
        if (!isSelected || isMoving)
            return;

        animator.SetScaleAnimationState(enable);
    }

    private void OnDestroy()
    {
        Destroy(stateText);
        DetachAll();
        Controllers.ShapeControllerInstance.ChangeShape -= SetMesh;
        Controllers.AnimationControllerInstance.SetMoveAnimation -= SetMove;
        Controllers.AnimationControllerInstance.SetRotationAnimation -= SetRotation;
        Controllers.AnimationControllerInstance.SetScaleAnimation -= SetScale;
        Controllers.CameraControllerInstance.CameraMoved -= OnCameraMoved;
        animator.ObjectMoved -= OnCameraMoved;
        animator.ObjectRotated -= OnCameraMoved;
        animator.ObjectRotated -= UpdateConnectors;
        animator.ObjectScaled -= OnCameraMoved;
        animator.ObjectScaled -= UpdateConnectors;
    }

    public void CopyFrom(object anotherObject)
    {
        var another = anotherObject as CubeControl;
        if (another == null)
            throw new ArgumentException("Wrong object to copy from");

        //stateText = Instantiate<TextMesh>(another.stateText);
    }

    public void OnConnectorHoverStart(IConnector onConnector)
    {
        isConnectorActive = true;
        UpdateState();
        onConnector.Hovered = true;
    }

    public void OnConnectorHoverEnd(IConnector onConnector)
    {
        isConnectorActive = false;
        UpdateState();
        onConnector.Hovered = false;
    }

    public void DetachAll()
    {
        foreach (var connector in Connectors.Values)
        {
            connector.Disconnect();
        }
    }
}
