using UnityEngine;

public class Candle : MonoBehaviour
{
    // Candle State
    public bool isHeld = false;
    public bool isLit = false;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Debug the turnning on and off for the candle
        if (isLit)
        {
            sr.color = Color.red;
        }
        else
        {
            sr.color = Color.blue;
        }
    }

    // Set isHeld state for the candle, make the candle follow the player
    public void PickOrDrop(Transform player)
    {
        if (!isHeld)
        {
            isHeld = true;
            transform.SetParent(player);
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

    // Set isLit State for the candle
    public void LitCandle()
    {
        if (!isLit)
        {
            isLit = true;
        }
        else
        {
            isLit = false;
        }
    }


}
