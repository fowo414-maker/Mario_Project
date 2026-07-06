using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public Sprite idleRightSmallSprite;
    public Sprite idleLeftSmallSprite;
    public Sprite jumpRightSmallSprite;
    public Sprite jumpLeftSmallSprite;
    public Sprite idleRightBigSprite;
    public Sprite idleLeftBigSprite;
    public Sprite jumpRightBigSprite;
    public Sprite jumpLeftBigSprite;
    public Sprite dieSprite;
    public Sprite GoalSmallSprite;
    public Sprite GoalBigSprite;
    public Sprite GoalBottomLeftSmallSprite;
    public Sprite GoalBottomLeftBigSprite;
    public Sprite GoalBottomRightSmallSprite;
    public Sprite GoalBottomRightBigSprite;
    public Sprite[] walkRightSmallSprites;
    public Sprite[] walkLeftSmallSprites;
    public Sprite[] walkRightBigSprites;
    public Sprite[] walkLeftBigSprites;
    public Sprite[] growFlashLeftSprites;
    public Sprite[] growFlashRightSprites;
    public Transform goalBottomPoint;
    public float goalPauseTime = 0.5f;
    public float poleSlideSpeed = 2f;
    public float clearWalkSpeed = 2f;
    public float clearwalkDuration = 2f;
    public float clearwalkAnimationInterval = 0.15f;
    public float growFlashInterval = 0.1f;
    public int growFlashCount = 9;

    public float baseAnimationInterval = 0.15f;

    private int currentWalkIndex = 0;
    private float animationTimer = 0f;
    private int facingDirection = 1;
    private BoxCollider2D bc;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private float moveInput;
    private bool jumpRequested = false;
    private Vector3 startPosition;
    private bool isCleard = false;
    private bool isDead = false;
    private bool isGrowing = false;
    private int clearWalkIndex = 0;
    private float clearwalkAnimationTimer = 0f;
    private Sprite[] GrowSprites;

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
        if (isDead || isCleard || isGrowing)
        {
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");
        

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
        }

        if (transform.position.y < fallLimit)
        {
            Die();
        }

        

        if (!isDead && !isCleard)
        {
            UpdateMarioAnimation();
        }
    }

    void FixedUpdate()
    {
        if (isDead || isGrowing)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isCleard)
        {
            return;
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        if (rb.linearVelocity.x >= 0f)
        {
            facingDirection = 1;
        }
        else
        {
            facingDirection = -1;
        }
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
            if (contact.normal.y > 0.3f)
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
        if (isCleard || isDead)
        {
            return;
        }
        isCleard = true;
        rb.linearVelocity = Vector2.zero;
        Time.timeScale = 0f;
        if (isBig)
        {
            spriteRenderer.sprite = GoalBigSprite;
        }
        else
        {
            spriteRenderer.sprite = GoalSmallSprite;
        }
        StartCoroutine(GoalSequence());
        Debug.Log("Stage Clear!");
    }

    IEnumerator GoalSequence()
    {
        float plusHeight;
        if (isBig)
        {
            plusHeight = 0.5f;
        }
        else
        {
            plusHeight = 0;
        }
        rb.linearVelocity = Vector2.zero;
        Vector2 slideTarget = new Vector2(transform.position.x, goalBottomPoint.position.y + 1f + plusHeight);

        yield return new WaitForSecondsRealtime(goalPauseTime);

        if (goalBottomPoint != null)
        {
            while(Vector2.Distance(transform.position, slideTarget) > 0.05f)
            {
                transform.position = Vector2.MoveTowards(transform.position, slideTarget, poleSlideSpeed * Time.unscaledDeltaTime);

                rb.linearVelocity = Vector2.zero;

                yield return null;
            }
            transform.position = slideTarget;
            if (isBig)
            {
                spriteRenderer.sprite = GoalBottomLeftBigSprite;
            }
            else
            {
                spriteRenderer.sprite = GoalBottomLeftSmallSprite;
            }

            yield return new WaitForSecondsRealtime(goalPauseTime);
            transform.position = new Vector3(transform.position.x + 0.6f, transform.position.y, transform.position.z);
            if (isBig)
            {
                spriteRenderer.sprite = GoalBottomRightBigSprite;
            }
            else
            {
                spriteRenderer.sprite = GoalBottomRightSmallSprite;
            }
            yield return new WaitForSecondsRealtime(goalPauseTime);
        }
        

        //성으로 걸어가기
        float timer = 0f;

        transform.position = new Vector3(goalBottomPoint.position.x + 1f, goalBottomPoint.position.y + plusHeight, goalBottomPoint.position.z);
        clearWalkIndex = 0;
        clearwalkAnimationTimer = 0f;

        while (timer < clearwalkDuration)
        {
            transform.position += Vector3.right * clearWalkSpeed * Time.unscaledDeltaTime;

            UpdateClearWalkAnimation();

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        if (clearText != null)
        {
            clearText.SetActive(true);
        }
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
        spriteRenderer.sprite = dieSprite;
        gameoverText.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(RespawnAfterDelay());
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSecondsRealtime(respawnDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //버섯먹고 성장
    public void Grow()
    {
        if (isBig)
        {
            return;
        }
        
        if (facingDirection == 1)
        {
            GrowSprites = growFlashRightSprites;
        }
        else
        {
            GrowSprites = growFlashLeftSprites;
        }
        StartCoroutine(GrowSequence());
    }

    IEnumerator GrowSequence()
    {
        isGrowing = true;

        rb.linearVelocity = Vector2.zero;

        float originalTimeScale = Time.timeScale;
        float currentY = transform.position.y;
        Time.timeScale = 0f;
        for (int i = 0; i < growFlashCount; i++)
        {
            spriteRenderer.sprite = GrowSprites[i%3];
            transform.position = new Vector3(transform.position.x, currentY + (0.25f * (i%3)), transform.position.z);
            yield return new WaitForSecondsRealtime(growFlashInterval);
        }
        isBig = true;
        spriteRenderer.sprite = GrowSprites[2];
        bc.size = new Vector2(1, 2);

        Time.timeScale = originalTimeScale;

        isGrowing = false;
    }

    //마리오 애니메이션 업데이트
    void UpdateMarioAnimation()
    {
        if (!isGrounded)
        {
            if (facingDirection == 1)
            {
                if (isBig)
                {
                    spriteRenderer.sprite = jumpRightBigSprite;
                }
                else
                {
                    spriteRenderer.sprite = jumpRightSmallSprite;
                }
            }
            else
            {
                if (isBig)
                {
                    spriteRenderer.sprite = jumpLeftBigSprite;
                }
                else
                {
                    spriteRenderer.sprite = jumpLeftSmallSprite;
                }
            }
            return;
        }

        float speed = Mathf.Abs(rb.linearVelocity.x);

        if (speed < 0.1f)
        {
            if (facingDirection == 1)
            {
                if (isBig)
                {
                    spriteRenderer.sprite = idleRightBigSprite;
                }
                else
                {
                    spriteRenderer.sprite = idleRightSmallSprite;
                }
            }
            else
            {
                if (isBig)
                {
                    spriteRenderer.sprite = idleLeftBigSprite;
                }
                else
                {
                    spriteRenderer.sprite = idleLeftSmallSprite;
                }
            }
            return;
        }

        AnimateWalk(speed);
    }

    void AnimateWalk(float speed)
    {
        Sprite[] currentWalkSprites;

        if (facingDirection == 1)
        {
            if (isBig)
            {
                currentWalkSprites = walkRightBigSprites;
            }
            else
            {
                currentWalkSprites = walkRightSmallSprites;
            }
        }
        else
        {
            if (isBig)
            {
                currentWalkSprites = walkLeftBigSprites;
            }
            else
            {
                currentWalkSprites = walkLeftSmallSprites;
            }
        }

        if (currentWalkSprites == null || currentWalkSprites.Length == 0)
        {
            return;
        }

        float interval = baseAnimationInterval / Mathf.Max(speed / 4f, 1f);

        animationTimer += Time.deltaTime;

        if (animationTimer >= interval)
        {
            animationTimer = 0;
            currentWalkIndex++;

            if (currentWalkIndex >= currentWalkSprites.Length)
            {
                currentWalkIndex = 0;
            }

            spriteRenderer.sprite = currentWalkSprites[currentWalkIndex];
        }
    }

    void UpdateClearWalkAnimation()
    {
        Sprite[] currentWalkSprites;

        if (isBig)
        {
            currentWalkSprites = walkRightBigSprites;
        }
        else
        {
            currentWalkSprites = walkRightSmallSprites;
        }

        if (currentWalkSprites == null || currentWalkSprites.Length == 0)
        {
            return;
        }

        clearwalkAnimationTimer += Time.unscaledDeltaTime;

        if (clearwalkAnimationTimer >= clearwalkAnimationInterval)
        {
            clearwalkAnimationTimer = 0f;
            clearWalkIndex++;
            if(clearWalkIndex >= currentWalkSprites.Length)
            {
                clearWalkIndex = 0;
            }
            spriteRenderer.sprite = currentWalkSprites[clearWalkIndex];
        }
    }

}
//just test for github
