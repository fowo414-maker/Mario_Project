using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float minX = 0f;

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
        transform.position = new Vector3(Mathf.Max(target.position.x, minX), fixedY, fixedZ);

    }
}
