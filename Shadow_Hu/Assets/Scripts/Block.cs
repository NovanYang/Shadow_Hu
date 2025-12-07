using UnityEngine;

public class Block : MonoBehaviour
{
    // Block State
    public bool isHeld = false;

    // Reference
    private Rigidbody2D rb;
    private Collider2D blockCol;
    private Collider2D playerCol;
    private bool candleCollide = false;
    private Transform shadowTransform;

    private void Update()
    {
        if (isHeld) FollowShadow(shadowTransform);
    }

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
            shadowTransform = Shadow;
            isHeld = true;
            transform.localPosition = new Vector3(0, 0, 0);
            rb.simulated = false;
        }
        else
        {
            isHeld = false;
            shadowTransform = null;
            rb.simulated = true;
        }
    }

    public void FollowShadow(Transform shadow)
    {
        transform.position = shadow.position;
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
                if (!candleCollide)
                {
                    Physics2D.IgnoreCollision(blockCol, playerCol, false);
                }
            }
        }

        if (collision.CompareTag("Candle"))
        { 
            if (rb.linearVelocityY < -0.01f)
            {
                Collider2D _candleCollider = collision.gameObject.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(blockCol, _candleCollider, true);
                Physics2D.IgnoreCollision(blockCol, playerCol, true);
                candleCollide = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Candle") && rb.linearVelocityY >= -0.01f)
        {
            Collider2D _candleCollider = collision.gameObject.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(blockCol, _candleCollider, false);
            if (candleCollide)
            {
                candleCollide = false;
            }
        }
    }
}