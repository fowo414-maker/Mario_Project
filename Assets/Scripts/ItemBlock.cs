using UnityEngine;

public class ItemBlock : MonoBehaviour
{
    public GameObject mushroomPrefabs;
    public Transform spawnPoint;

    public Color unusedColor = Color.yellow;
    public Color usedColor = Color.white;

    private SpriteRenderer spriteRenderer;
    private bool used = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = unusedColor;
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
            if (contact.normal.y > 0.5f)
            {
                SpawnMushroom();
                used = true;
                spriteRenderer.color = usedColor;
                break;
            }
        }
    }

    void SpawnMushroom()
    {
        Instantiate(mushroomPrefabs, spawnPoint.position, Quaternion.identity);
    }

}
