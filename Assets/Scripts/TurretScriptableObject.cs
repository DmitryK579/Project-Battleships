using UnityEngine;

[CreateAssetMenu(fileName = "TurretScriptableObject", menuName = "Scriptable Objects/Turret")]
public class TurretScriptableObject : ScriptableObject
{
	[field: SerializeField] public ShellScriptableObject ShellScriptableObject { get; private set; }
	[field: SerializeField] public bool IsPrimaryTurret { get; private set; }
	[field: SerializeField] public float ReloadTimeS { get; private set; }
	[field: SerializeField] public float RotationSpeed { get; private set; }
	[field: SerializeField] public float MaxRotationAngle { get; private set; }
	[field: SerializeField] public float MaxRange { get; private set; }
	[field: SerializeField] public float MinRange { get; private set; }
	[field: SerializeField] public float MaxDispersion { get; private set; }
	[field: SerializeField] public float MinDispersion { get; private set; }
}
