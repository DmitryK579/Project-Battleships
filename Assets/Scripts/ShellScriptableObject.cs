using UnityEngine;

[CreateAssetMenu(fileName = "ShellScriptableObject", menuName = "Scriptable Objects/Shell")]
public class ShellScriptableObject : ScriptableObject
{
    [field: SerializeField] public GameObject Prefab { get; private set; }
	[field: SerializeField] public float Damage { get; private set; }
	[field: SerializeField] public float Speed { get; private set; }
	[field: SerializeField] public float MaxHeight { get; private set; }
	[field: SerializeField] public float ArmingTimeS { get; private set; }
	[field: SerializeField] public float FloatTimeS { get; private set; }
}
