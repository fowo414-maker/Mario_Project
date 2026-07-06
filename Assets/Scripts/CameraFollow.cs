using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float minX = 8f;
    public float maxX = 200f;

    private float fixedY;
    private float fixedZ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(Mathf.Clamp(target.position.x, minX, maxX), fixedY, fixedZ);

    }
}
