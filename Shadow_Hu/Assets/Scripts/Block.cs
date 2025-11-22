using UnityEngine;

public class Block : MonoBehaviour
{
    // Block State
    public bool isHeld = false;

    // Reference
    private Rigidbody2D rb;
    private Collider2D blockCol;
    private Collider2D playerCol;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        blockCol = GetComponent<Collider2D>();
        GameObject _player = GameObject.FindWithTag("Player");

        if (_player != null)
        {
            playerCol = _player.GetComponent<Collider2D>();
        }
    }

    // Correctly update the physic for the block when lift or dropped by shadow
    public void PickOrDrop(Transform Shadow)
    {
        if (Shadow == null)
        {
            return;
        }

        if (!isHeld)
        {
            isHeld = true;
            transform.SetParent(Shadow);
            transform.localPosition = new Vector3(0, 0, 0);
            rb.simulated = false;
        }
        else
        {
            isHeld = false;
            transform.SetParent(null);
            rb.simulated = true;
        }
    }

    // When the block is about to hit the player when falling, ignore collision
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            if (rb.linearVelocityY < -0.01f)
            {
                Physics2D.IgnoreCollision(blockCol, playerCol, true);
            }
            else
            {
                Physics2D.IgnoreCollision(blockCol, playerCol, false);
            }
        }
    }
}