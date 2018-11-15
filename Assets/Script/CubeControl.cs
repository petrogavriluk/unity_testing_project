using Helpers;
using Interfaces;
using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(LineRenderer))]
public class CubeControl : MonoBehaviour, IMovable, ICopyable, IObjectWithRenderer, IAttachable
{
    private static readonly float doubleClickDelay = 0.4f;
    [SerializeField]
    AnimationData animationData;

    public Color defaultColor;
    public Color hoverColor;
    public Color selectedColor;
    public Color potentialConnectionColor;

    private TextMesh stateText;

    private Vector3 currentShift;

    private bool isSelected = false;
    private bool isHovered = false;
    private bool isConnectorActive = false;
    private TransformAnimator animator;
    private Renderer objectRenderer;
    private BoxCollider boxCollider;
    private MeshFilter meshFilter;
    private LineRenderer lineRenderer;
    private bool isMoving = false;
    private float lastClick = 0f;
    private bool showConnectors = false;
    private PrimitiveType currentMeshType = PrimitiveType.Cube;
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
            ShowLineRender(isMoving);
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
                    connector.Visible = showConnectors;
                }
            }
        }
    }

    public IDictionary<Side, IConnector> Connectors
    {
        get { return sideConnectors; }
    }

    public Guid UID
    {
        get;
        private set;
    } = Guid.NewGuid();

    private bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            UpdateState();
        }
    }

    private bool IsHovered
    {
        get { return isHovered; }
        set
        {
            isHovered = value;
            UpdateState();
        }
    }

    private bool IsConnectorActive
    {
        get { return isConnectorActive; }
        set
        {
            isConnectorActive = value;
            UpdateState();
        }
    }

    private void Awake()
    {
        stateText = Controllers.CreatorInstance.CreateTextMesh();
        stateText.transform.parent = gameObject.transform;
        boxCollider = GetComponent<BoxCollider>();
        meshFilter = GetComponent<MeshFilter>();
        lineRenderer = GetComponent<LineRenderer>();
        ShowLineRender(false);

        currentShift = FiguresPositions.Instance.CubeShift;
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

        animator.AnimationParameters = animationData;
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
        connector.Body.transform.position = objectRenderer.GetEdgePoint(side, connector.ObjectRenderer.bounds.extents.x);
        connector.Body.transform.parent = gameObject.transform;
        sideConnectors[side] = connector;
    }

    private void OnRightMouseDown(Vector3 point) => Controllers.ContextMenuControllerInstance.ShowContextMenu(this, point, animator);

    private void OnDoubleClick()
    {
        if (Controllers.MoveHelperInstance.IsMoving)
            return;

        Controllers.MoveHelperInstance.StartMoving(this);
    }

    private void OnMouseDown() => IsSelected = !IsSelected;
    private void OnMouseEnter() => IsHovered = true;

    private void OnMouseExit() => IsHovered = false;

    private void UpdateState()
    {
        if (IsConnectorActive)
        {
            stateText.text = "Connect?";
            MaterialColor = potentialConnectionColor;
        }
        else if (IsSelected)
        {
            stateText.text = "Selected";
            MaterialColor = selectedColor;
        }
        else if (IsHovered)
        {
            stateText.text = "Hovered";
            MaterialColor = hoverColor;
        }
        else
        {
            stateText.text = "Normal";
            MaterialColor = defaultColor;
        }
        // For Debug
        // stateText.text = UID.ToString().Substring(0, 5);
    }

    void UpdateConnectors(object sender = null, EventArgs e = null)
    {
        foreach (var connector in Connectors)
        {
            connector.Value.Body.transform.position = objectRenderer.GetEdgePoint(connector.Key,connector.Value.ObjectRenderer.bounds.extents.x);
        }
    }

    void ShowLineRender(bool show)
    {
        lineRenderer.enabled = show;

        if (show)
        {
            Bounds localBounds = ObjectRenderer.bounds;
            localBounds.center = Vector3.zero;
            localBounds.size /= transform.localScale.x;
            var points = localBounds.GetBoxFrameLines(0.1f);
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
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
        if (!IsSelected)
            return;
        SetMeshDirectly(type);
    }
    public void SetMeshDirectly(PrimitiveType type)
    {
        Bounds oldBounds = ObjectRenderer.bounds;
        switch (type)
        {
            case PrimitiveType.Sphere:
                meshFilter.mesh = Primitives.sphereMesh;
                transform.position += FiguresPositions.Instance.SphereShift - currentShift;
                currentShift = FiguresPositions.Instance.SphereShift;
                break;
            case PrimitiveType.Capsule:
                meshFilter.mesh = Primitives.capsuleMesh;
                transform.position += FiguresPositions.Instance.CapsuleShift - currentShift;
                currentShift = FiguresPositions.Instance.CapsuleShift;
                break;
            case PrimitiveType.Cylinder:
                meshFilter.mesh = Primitives.cylinderMesh;
                transform.position += FiguresPositions.Instance.CylinderShift - currentShift;
                currentShift = FiguresPositions.Instance.CylinderShift;
                break;
            default:
                meshFilter.mesh = Primitives.cubeMesh;
                transform.position += FiguresPositions.Instance.CubeShift - currentShift;
                currentShift = FiguresPositions.Instance.CubeShift;
                break;
        }
        currentMeshType = type;
        if (oldBounds!=ObjectRenderer.bounds)
        {
            DetachAll();
        }
        boxCollider.size = ObjectRenderer.bounds.size / transform.localScale.x;
        OnCameraMoved();
        UpdateConnectors();
    }

    void SetRotation(object sender, bool enable)
    {
        if (!IsSelected || isMoving)
            return;

        animator.SetRotateAnimationState(enable);
        if (enable)
            DetachAll();
    }
    void SetMove(object sender, bool enable)
    {
        if (!IsSelected || isMoving)
            return;

        animator.SetMoveAnimationState(enable);
        if (enable)
            DetachAll();
    }
    void SetScale(object sender, bool enable)
    {
        if (!IsSelected || isMoving)
            return;

        animator.SetScaleAnimationState(enable);
        if (enable)
            DetachAll();
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

    public void OnConnectorHoverStart(IConnector onConnector)
    {
        IsConnectorActive = true;
        UpdateState();
        onConnector.Hovered = true;
    }

    public void OnConnectorHoverEnd(IConnector onConnector)
    {
        IsConnectorActive = false;
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

    public CubeSaveData SaveState()
    {
        return new CubeSaveData
        {
            ID = UID.ToString(),
            primitiveType = (int)currentMeshType,
            showConnectors = ShowConnectors,
            isSelected = IsSelected,
            isHovered = IsHovered,
            rotation = transform.rotation,
            position = transform.position,
            scale = transform.localScale,
        };
    }

    public void RestoreFromState(CubeSaveData state)
    {
        UID = Guid.Parse(state.ID);
        SetMeshDirectly((PrimitiveType)state.primitiveType);
        ShowConnectors = state.showConnectors;
        IsSelected = state.isSelected;
        IsHovered = state.isHovered;
        transform.position = state.position;
        transform.rotation = state.rotation;
        transform.localScale = state.scale;
        OnCameraMoved();
    }

    public void ResetDefault(Transform defaultTransform)
    {
        animator.SetMoveAnimationState(false);
        animator.SetRotateAnimationState(false);
        animator.SetScaleAnimationState(false);

        transform.position = defaultTransform.position;
        transform.rotation = defaultTransform.localRotation;
        transform.localScale = defaultTransform.localScale;

        DetachAll();
        ShowConnectors = false;
        IsMoving = false;
        IsSelected = false;
        IsHovered = false;
        OnCameraMoved();
    }

    public MonoBehaviour CreateCopy()
    {
        var copied = Controllers.CreatorInstance.GetCubeObject(currentMeshType);
        CubeSaveData saveData = SaveState();
        copied.RestoreFromState(saveData);
        copied.UID = Guid.NewGuid();
        return copied;
    }
}
