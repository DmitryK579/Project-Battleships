using UnityEngine;

public class PlayerInputContainer : MonoBehaviour
{
    public static PlayerInputContainer Instance {  get; private set; }
    public PlayerInputActions playerInputActions {  get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            playerInputActions = new PlayerInputActions();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
