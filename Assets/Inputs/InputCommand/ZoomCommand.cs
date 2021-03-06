using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomCommand : Command
{
    [SerializeField]
    private float zoomSpeed = 100;
    [SerializeField]
    private float zoomSpeed1 = 10;
    [SerializeField]
    private float minZoom = 0;
    [SerializeField]
    private float maxZoom = 30;
    [SerializeField]
    private float minZoom1 = 0;
    [SerializeField]
    private float maxZoom1 = 200;

    [SerializeField]
    private CinemachineCameraOffset cameraOffset;
    bool isSystemMapOpened;
    private float zoomAmount;

    private float currentMaxZoom;
    private float currentMinZoom;
    private float currentZoomSpeed;
    private float tempZoom1 = 400f;
    private float tempZoom = 300f;

    private void OnEnable()
    {
        // SaveLoadHandlers.VirtualCamOffsetLoad.AddListener(VirtualCamOffsetLoad);
        PlayerManagerEventHandler.MapChangeEvent.AddListener(SystemMapChange);
    }
    private void OnDisable()
    {
        // SaveLoadHandlers.VirtualCamOffsetLoad.RemoveListener(VirtualCamOffsetLoad);
        PlayerManagerEventHandler.MapChangeEvent.RemoveListener(SystemMapChange);
    }
    private void Start()
    {
        zoomAmount = -cameraOffset.m_Offset.z;
        PlayerManagerEventHandler.MovementModifierEvent?.Invoke(zoomAmount);
        currentMaxZoom = maxZoom;
        currentMinZoom = minZoom;
        currentZoomSpeed = zoomSpeed;

    }
    private void SystemMapChange(bool isOpened)
    {
        if (isOpened)
        {
            tempZoom = zoomAmount;
            zoomAmount = tempZoom1;
            currentMaxZoom = maxZoom1;
            currentMinZoom = minZoom1;
            currentZoomSpeed = zoomSpeed1;
        }
        else
        {
            tempZoom1 = zoomAmount;
            zoomAmount = tempZoom;
            currentMaxZoom = maxZoom;
            currentMinZoom = minZoom;
            currentZoomSpeed = zoomSpeed;
        }
        PlayerManagerEventHandler.MovementModifierEvent?.Invoke(zoomAmount);
        cameraOffset.m_Offset = new Vector3(0, 0, -zoomAmount);

    }
    private void VirtualCamOffsetLoad(float arg0)
    {
        zoomAmount = -arg0;
        cameraOffset.m_Offset = new Vector3(0, 0, arg0);
    }

    public override void ExecuteWithFloat(float value)
    {
        if (IsMouseOverUI()) return;
        zoomAmount = Mathf.Clamp(zoomAmount - (value * currentZoomSpeed * Time.deltaTime), currentMinZoom, currentMaxZoom);
        PlayerManagerEventHandler.MovementModifierEvent?.Invoke(zoomAmount);
        cameraOffset.m_Offset = new Vector3(0, 0, -zoomAmount);
        // SaveLoadHandlers.VirtualCamOffset?.Invoke(cameraOffset.m_Offset.z);

    }
    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
