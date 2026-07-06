using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GoombaMovement : MonoBehaviour
{
    public float moveSpeed = 2f;

    public Sprite[] walkSprites;
    public Sprite stompedSprite;
    public float animationInterval = 0.3f;
    public float stompedDuration = 0.2f;

    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private int direction = -1;
    private int currentSpriteIndex = 0;
    private float animationTimer = 0f;
    private bool isStomped = false;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (walkSprites.Length > 0)
        {
            spriteRenderer.sprite = walkSprites[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isStomped)
        {
            return;
        }
        AnimateGoomba();
    }

    private void FixedUpdate()
    {
        if (isStomped)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isStomped){
            return;
        }
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy"))
        {
            foreach(ContactPoint2D contact in collision.contacts)
            {
                Debug.Log("Normal : " + contact.normal);
                if (Mathf.Abs(contact.normal.x) > 0.3f)
                {
                    if (contact.normal.x > 0)
                    {
                        direction = 1;
                    }
                    else
                    {
                        direction = -1;
                    }
                }
                break;
            }
        }
    }

    void AnimateGoomba()
    {
        if (walkSprites.Length == 0)
        {
            return;
        }

        animationTimer += Time.deltaTime;

        if (animationTimer >= animationInterval)
        {
            animationTimer = 0f;

            currentSpriteIndex++;

            if (currentSpriteIndex >= walkSprites.Length)
            {
                currentSpriteIndex = 0;
            }

            spriteRenderer.sprite = walkSprites[currentSpriteIndex];
        }
    }

    public void Stomp()
    {
        if (isStomped)
        {
            return;
        }

        isStomped = true;

        rb.linearVelocity = Vector2.zero;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        if (stompedSprite != null)
        {
            spriteRenderer.sprite = stompedSprite;
        }

        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(stompedDuration);

        Destroy(gameObject);
    }
}
