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
    // Animation Variable
    public Animator animator;
    public bool FacingLeft = false;
    public bool AnimationMoving = false;
    public bool AnimationUp = false;
    public bool AnimationFall = false;
    // Animation Reference
    public SpriteRenderer SpriteRenderer;

    // Private Variables
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
        {
            Debug.Log("ERROR: MISSING ANIMATOR");
        }
    }

    void Update()
    {
        // Get horizontal inputs and update velocity
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Update Facing
        UpdateFacing(moveInput);
        UpdateMoving(rb);
        UpdateJump(rb);

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

    private void UpdateFacing(float moveInput)
    {
        if (FacingLeft)
        {
            if (moveInput > 0)
            {
                FacingLeft = false;
                SpriteRenderer.flipX = false;
            }
            else
            { 
                return;
            }
        }
        else
        {
            if (moveInput < 0)
            {
                FacingLeft = true;
                SpriteRenderer.flipX = true;
            }
            else
            {
                return;
            }
        }
    }

    private void UpdateMoving(Rigidbody2D rb)
    {
        if (isGrounded)
        {
            if (Mathf.Abs(rb.linearVelocityX) > 0.1f)
            {
                animator.SetBool("PlayerMove", true);
                AnimationMoving = true;
            }
            else
            {
                animator.SetBool("PlayerMove", false);
                AnimationMoving = false;
            }
        }
        else
        {
            if (AnimationMoving)
            {
                animator.SetBool("PlayerMove", false);
                AnimationMoving = false;
            }
        }
    }

    private void UpdateJump(Rigidbody2D rb)
    {
        if (!isGrounded)
        {
            if (rb.linearVelocityY > 0)
            {
                AnimationUp = true;
                AnimationFall = false;

                animator.SetBool("PlayerUp", true);
                animator.SetBool("PlayerFall", false);
            }
            else if (rb.linearVelocityY < 0)
            {
                AnimationUp = false;
                AnimationFall = true;

                animator.SetBool("PlayerUp", false);
                animator.SetBool("PlayerFall", true);
            }
        }
        else
        {
            AnimationUp = false;
            AnimationFall = false;

            animator.SetBool("PlayerUp", false);
            animator.SetBool("PlayerFall", false);
        }
    }
}
