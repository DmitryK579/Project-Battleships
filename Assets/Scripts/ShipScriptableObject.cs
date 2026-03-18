using UnityEngine;

[CreateAssetMenu(fileName = "ShipScriptableObject", menuName = "Scriptable Objects/ShipScriptableObject")]
public class ShipScriptableObject : ScriptableObject
{
	[field: SerializeField] public float Health { get; private set; }
	[field: SerializeField] public float AccelerationFactor { get; private set; }
	[field: SerializeField] public float RotationFactor { get; private set; }
	[field: SerializeField] public float DriftFactor { get; private set; }
}
