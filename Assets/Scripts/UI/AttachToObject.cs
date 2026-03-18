using UnityEngine;

public class AttachToObject : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] float XOffset;
    [SerializeField] float YOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = target.transform.position;
        position.x += XOffset;
        position.y += YOffset;
        this.transform.position = position;
    }
}
