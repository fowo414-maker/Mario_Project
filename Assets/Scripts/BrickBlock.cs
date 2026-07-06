using UnityEngine;

public class BrickBlock : MonoBehaviour
{
    private BlockBump blockBump;

    private void Start()
    {
        blockBump = GetComponent<BlockBump>();
    }

    //충돌했을 때
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();

        if (player == null)
        {
            return;
        }

        foreach(ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y >= 0.9f)
            {
                if (player.isBig)
                {
                    Destroy(gameObject);
                }
                else
                {
                    blockBump.Bump();
                }
                break;
            }
        }
    }
}
