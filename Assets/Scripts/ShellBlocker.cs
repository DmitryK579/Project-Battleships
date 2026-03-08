using UnityEngine;

public class ShellBlocker : MonoBehaviour, IShellBlocker
{
    [SerializeField] private float objectHeight;

    public float GetObjectHeight()
    {
        return objectHeight;
    }
}
