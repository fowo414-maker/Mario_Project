using UnityEngine;

public class GoombaActivator : MonoBehaviour
{
    public float activatemargin = 1.5f;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private Collider2D goombaCollider;
    private Rigidbody2D rb;
    private GoombaMovement goombaMovement;

    private bool activated = false;
    void Start()
    {
        mainCamera = Camera.main;

        spriteRenderer = GetComponent<SpriteRenderer>();
        goombaCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        goombaMovement = GetComponent<GoombaMovement>();

        SetGoombaActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            return;
        }

        if (mainCamera == null)
        {
            return;
        }

        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float cameraRightEdge = mainCamera.transform.position.x + cameraHalfWidth;

        if (cameraRightEdge + activatemargin >= transform.position.x)
        {
            activated = true;
            SetGoombaActive(true);
        }
    }

    void SetGoombaActive(bool active)
    {
        spriteRenderer.enabled = active;

        goombaCollider.enabled = active;

        rb.simulated = active;
        rb.linearVelocity = Vector2.zero;

        goombaMovement.enabled = active;
    }
}
