using UnityEngine;

public class Candle : MonoBehaviour
{
    // Candle State
    public bool isHeld = false;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

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
            Debug.Log("???");
            isHeld = false;
            transform.SetParent(null);
            rb.simulated = true;
        }
    }
}
