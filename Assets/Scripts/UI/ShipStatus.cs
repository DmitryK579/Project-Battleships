using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ShipStatus : MonoBehaviour
{
	[field: SerializeField] public ShipHandler TrackedShip { get; set; }
    [SerializeField] private Transform hullIcon;
    [SerializeField] private List<Transform> turretIcons;
    private List<TurretHandler> controllableTurrets;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        controllableTurrets = TrackedShip.GetControllableTurrets();
    }

    // Update is called once per frame
    void Update()
    {
        if (TrackedShip == null)
            return;
        
        hullIcon.transform.rotation = TrackedShip.transform.rotation;
        for (int i = 0; i < turretIcons.Count; i++)
        {
            turretIcons[i].transform.rotation = controllableTurrets[i].transform.rotation;
        }
    }
}
