using UnityEngine;

public class MushroomPickup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        PlayerMovement player = collision.GetComponent<PlayerMovement>();

        if (player != null)
        {
            player.Grow();
        }

        Destroy(transform.parent.gameObject);
    }
}
