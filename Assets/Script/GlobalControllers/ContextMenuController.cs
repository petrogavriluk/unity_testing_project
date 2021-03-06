﻿using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Helpers;
using Interfaces;

[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(CanvasGroup))]
public class ContextMenuController : MonoBehaviour
{
    MonoBehaviour currentObject;

    [SerializeField]
    Button deleteButton;
    [SerializeField]
    Button duplicateButton;
    [SerializeField]
    Button moveButton;
    [SerializeField]
    Slider scaleSlider;
    [SerializeField]
    Text sliderText;

    GraphicRaycaster graphicRaycaster;
    EventSystem eventSystem;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        graphicRaycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
        canvasGroup = GetComponent<CanvasGroup>();

        Controllers.CameraControllerInstance.CameraMoved += SetRotationToCamera;
        ShowMenu(false);
    }


    // Use this for initialization
    void Start()
    {
        deleteButton.onClick.AddListener(DeleteObject);
        duplicateButton.onClick.AddListener(DuplicateObject);
        moveButton.onClick.AddListener(MoveObject);
        scaleSlider.onValueChanged.AddListener(ChangeScale);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData eventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();

            graphicRaycaster.Raycast(eventData, results);

            if (results.Count == 0)
            {
                ShowMenu(false);
            }
        }
    }

    private void SetRotationToCamera(object sender = null, EventArgs e = null)
    {
        if (currentObject != null)
            gameObject.transform.rotation = Camera.main.transform.rotation;
    }

    private void OnDrawGizmos_()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 10, true);
    }

    private void ChangeScale(float scale)
    {
        sliderText.text = "Scale: " + scale;
        currentObject.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void MoveObject()
    {
        Controllers.MoveHelperInstance.StartMoving(currentObject);
        ShowMenu(false);
    }

    private void DuplicateObject()
    {
        MonoBehaviour duplicateMonoBehaviour;
        ICopyable copyable = currentObject as ICopyable;
        if (copyable != null)
        {
            duplicateMonoBehaviour = copyable.CreateCopy();
        }
        else
        {
            var duplicate = Instantiate<GameObject>(currentObject.gameObject);
            duplicateMonoBehaviour = duplicate.GetComponent<MonoBehaviour>();
        }

        currentObject = duplicateMonoBehaviour;
        MoveObject();
    }

    void DeleteObject()
    {
        var cube = currentObject as CubeControl;
        if (cube != null)
        {
            Controllers.CreatorInstance.DestroyCubeObject(cube);
        }
        else
        {
            Destroy(currentObject);
        }

        currentObject = null;
        ShowMenu(false);
    }

    public void ShowContextMenu<TObject>(TObject clickedObject, Vector3 clickPosition, ITransformAnimator animator = null) where TObject : MonoBehaviour, IObjectWithRenderer
    {
        if (clickedObject == null)
            throw new ArgumentNullException("clickedObject");

        currentObject = clickedObject;
        ShowMenu(true);
        var menuPosition = clickedObject.gameObject.transform.position;
        menuPosition.y = clickedObject.ObjectRenderer.bounds.max.y;
        gameObject.transform.position = menuPosition;
        SetRotationToCamera();

        if (animator != null)
        {
            animator.SetScaleAnimationState(false);
        }

        scaleSlider.value = clickedObject.transform.localScale.x;
    }

    void ShowMenu(bool show)
    {
        canvasGroup.alpha = show ? 1f : 0f;
        canvasGroup.blocksRaycasts = show;
        canvasGroup.interactable = show;
        if (!show)
        {
            currentObject = null;
        }
    }

}