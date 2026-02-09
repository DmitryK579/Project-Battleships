using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 20f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float maxZoom = 250f;
    [SerializeField] private float minZoom = 20f;

    private PlayerInputActions playerInputActions;
    private CinemachineCamera cam;
    private float targetZoom = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Camera.Enable();

		cam = GetComponent<CinemachineCamera>();
    }

	// Update is called once per frame
	void Update()
    {
        HandleZoom();
    }

    private void HandleZoom()
    {
        float zoomInput = playerInputActions.Camera.Zoom.ReadValue<float>();

        targetZoom += zoomSpeed * zoomInput;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        cam.Lens.OrthographicSize = Mathf.Lerp(cam.Lens.OrthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
	}
}
