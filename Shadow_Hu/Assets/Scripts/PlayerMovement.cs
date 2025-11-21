using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Attributes for player movement
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    // Jump check variables
    public Transform groundCheckPoint;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask interactionLayer;

    // Private Variables
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get horizontal inputs and update velocity
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Update Ground Check
        isGrounded = CheckGround();

        // Get Jump Input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

    }

    // Update the ground check, only set isGround to True When object in checked layers and have Groundable
    // Component
    private bool CheckGround()
    {

        Collider2D[] _hits = Physics2D.OverlapCircleAll(
                groundCheckPoint.position,
                checkRadius,
                groundLayer | interactionLayer
            );

        foreach (var _hit in _hits)
        { 
            if (_hit.isTrigger) continue;

            if (_hit.GetComponent<Groundable>() != null)
            {
                return true;
            }
        }

        return false;
    }

}
