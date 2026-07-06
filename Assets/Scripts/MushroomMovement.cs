using UnityEngine;

public class MushroomMovement : MonoBehaviour
{
    public float moveSpeed = 2f;


    private Rigidbody2D rb;
    private int direction = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    //충돌했을 때
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Mathf.Abs(contact.normal.x) > 0.5f)
                {
                    if (contact.normal.x > 0f)
                    {
                        direction = 1;
                    }
                    else
                    {
                        direction = -1;
                    }
                    break;
                }
            }
        }
    }
}
