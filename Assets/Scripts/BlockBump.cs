using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BlockBump : MonoBehaviour
{
    public float bumpHeight = 0.2f;
    public float bumpSpeed = 8f;

    private Vector3 startPosition;
    private bool isBumping = false;

    private void Start()
    {
        startPosition = transform.position;
    }

    public void Bump()
    {
        if (isBumping)
        {
            return;
        }

        StartCoroutine(BumpCoroutine());
    }

    IEnumerator BumpCoroutine()
    {
        isBumping = true;

        Vector3 topPosition = startPosition + Vector3.up * bumpHeight;

        while (Vector3.Distance(transform.position, topPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, topPosition, bumpSpeed * Time.deltaTime);

            yield return null;
        }

        while (Vector3.Distance(transform.position, startPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, bumpSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = startPosition;
        isBumping = false;
    }
    
}
