using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform anchorObject;
    [Header("Camera settings")]
    [SerializeField] private float zoomSpeed = 20f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float maxZoom = 600f;
    [SerializeField] private float minZoom = 20f;

    private CinemachineCamera cam;
    private float targetZoom = 0f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
    {
		PlayerInputContainer.Instance.playerInputActions.Camera.Enable();
		cam = GetComponent<CinemachineCamera>();

        gameManager.OnPlayerShipSwap += OnPlayerShipSwap;
        anchorObject = gameManager.PlayerShip.transform;
	}

	private void OnPlayerShipSwap(object sender, EventArgs e)
	{
        anchorObject = gameManager.PlayerShip.transform;
	}

	private void OnDisable()
	{
		PlayerInputContainer.Instance.playerInputActions.Camera.Disable();
		gameManager.OnPlayerShipSwap -= OnPlayerShipSwap;
	}

	// Update is called once per frame
	private void Update()
    {
        TargetToAnchor();
        HandleZoom();
    }

    private void TargetToAnchor()
    {
        if (anchorObject == null)
            return;
        
        cam.Target.TrackingTarget.transform.position = anchorObject.position;
    }
    private void HandleZoom()
    {
        float zoomInput = PlayerInputContainer.Instance.playerInputActions.Camera.Zoom.ReadValue<float>();

        targetZoom += (zoomSpeed * zoomInput);
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        cam.Lens.OrthographicSize = Mathf.Lerp(cam.Lens.OrthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
	}
}
