using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using TMPro;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject clearText;
    public GameObject gameoverText;
    public float respawnDelay = 0.8f;
    public float moveSpeed = 5f;
    public float jumpPower = 8f;
    public float fallLimit = -8f;
    public float fallMultiplier = 2.2f;
    public bool isBig = false;
    public Sprite smalltype;
    public Sprite bigtype;

    private BoxCollider2D bc;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private float moveInput;
    private bool jumpRequested = false;
    private Vector3 startPosition;
    private bool isCleard = false;
    private bool isDead = false;

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (smalltype != null)
        {
            spriteRenderer.sprite = smalltype;
        }
        bc.size = new Vector2(1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
        }

        if (transform.position.y < fallLimit)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        if (isCleard || isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            jumpRequested = false;
        }

    }

    //충돌했을 때
    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGroundCollision(collision);
        CheckEnemyCollision(collision);
    }

    //충돌에서 벗어났을 때
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    //충돌 중일 때
    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckGroundCollision(collision);
    }

    //물체와 충돌했을 때
    void CheckGroundCollision(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
        {
            return;
        }
        foreach(ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    //적과 충돌했을 때
    void CheckEnemyCollision(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        foreach(ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.4f)
            {
                GoombaMovement goomba = collision.gameObject.GetComponent<GoombaMovement>();

                if (goomba != null)
                {
                    goomba.Stomp();
                }
                else
                {
                    Destroy(collision.gameObject);
                }

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower * 0.6f);

                return;
            }
        }
        if (isBig)
        {
            isBig = false;
            spriteRenderer.sprite = smalltype;
            bc.size = new Vector2(1, 1);
        }
        else
        {
            Die();
        }
    }

  
    //트리거에 들어갔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            ClearStage();
        }
    }

    //스테이지 클리어 시
    void ClearStage()
    {
        isCleard = true;
        rb.linearVelocity = Vector2.zero;
        clearText.SetActive(true);

        Debug.Log("Stage Clear!");
    }

    //죽었을 시
    void Die()
    {
        if (isDead || isCleard)
        {
            return; 
        }

        isDead = true;
        rb.linearVelocity = Vector2.zero;
        gameoverText.SetActive(true);

        StartCoroutine(RespawnAfterDelay());
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        Respawn();

        gameoverText.SetActive(false);
        isDead = false;
    }

    public void Grow()
    {
        if (isBig)
        {
            return;
        }

        isBig = true;
        spriteRenderer.sprite = bigtype;
        bc.size = new Vector2(1, 2);
    }

    //리스폰
    void Respawn()
    {
        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        isGrounded = false;
        jumpRequested = false;
    }

}
