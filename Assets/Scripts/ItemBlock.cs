using UnityEngine;

public class ItemBlock : MonoBehaviour
{
    public GameObject mushroomPrefabs;
    public Transform spawnPoint;

    public Sprite unusedSprite;
    public Sprite usedSprite;

    public bool containsMushroom;

    private SpriteRenderer spriteRenderer;
    private bool used = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (unusedSprite != null)
        {
            spriteRenderer.sprite = unusedSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (used)
        {
            return;
        }

        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        foreach(ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y == 1f)
            {
                if (containsMushroom)
                {
                    SpawnMushroom();
                }
                
                used = true;
                spriteRenderer.sprite = usedSprite;
                break;
            }
        }
    }

    void SpawnMushroom()
    {
        Instantiate(mushroomPrefabs, spawnPoint.position, Quaternion.identity);
    }

}
