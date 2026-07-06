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

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private float moveInput;
    private bool jumpRequested = false;
    private Vector3 startPosition;
    private bool isCleard = false;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
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

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            jumpRequested = false;
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGroundCollision(collision);
        CheckEnemyCollision(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckGroundCollision(collision);
    }

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

    //적과 부딪혔을 때
    void CheckEnemyCollision(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        foreach(ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                Destroy(collision.gameObject);

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower * 0.6f);

                return;
            }
        }

        Die();
    }

  

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            ClearStage();
        }
    }

    void ClearStage()
    {
        isCleard = true;
        rb.linearVelocity = Vector2.zero;
        clearText.SetActive(true);

        Debug.Log("Stage Clear!");
    }

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
    void Respawn()
    {
        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        isGrounded = false;
        jumpRequested = false;
    }

}
