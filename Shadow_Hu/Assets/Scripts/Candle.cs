using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class Candle : MonoBehaviour
{
    // Candle State
    public bool isHeld = false;
    public bool isLit = false;

    // References
    public Transform bottomAnchor;
    public GameObject shadowPrefab;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private PlayerInteraction playerInteraction;
    private KeyCode liftKey = KeyCode.F;
    private KeyCode jumpKey = KeyCode.Space;
    private bool lifting = false;
    public SpriteRenderer ActivateSprite;

    // Candle Animation
    public float floatAmplitude = 0.1f;
    public float floatFrequency = 2f;
    private Vector3 activateSpriteDefaultPos;
    public Sprite LitFrame;
    public Sprite OffFrame;

    // Shadow Settings
    public float maxShadowDistance = 30f;
    public float shadowDistanceIntensity = 2f;    
    private GameObject shadowInstance;
    private Vector3 shadowOffset = Vector3.zero;
    public float fineTuneOffset = 0f;

    // Shadow Animation Settings
    public SpriteRenderer shadowSpriteRenderer;
    public Animator shadowAnimator;

    // Shadow Lift interaction
    public float pressTimeCount = 0f;
    float pressTimeMax = 15f;

    private void Start()
    {
        GameObject _player = GameObject.FindWithTag("Player");
        if (_player != null)
        {
            playerInteraction = _player.GetComponent<PlayerInteraction>();
        }
        else
        {
            Debug.Log("ERROR: Player Not Found");
        }
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (ActivateSprite == null)
        {
            Debug.Log("ERROR: NO ACTIVATE SPRITE FOR CANDLE");
        }
        else
        { 
            activateSpriteDefaultPos = ActivateSprite.transform.localPosition;
            ActivateSprite.enabled = false;
        }
    }

    private void Update()
    {
        HandleShadowGeneration();

        if (shadowInstance != null)
        {
            HandleShadowPosition();
        }
        
        ReceiveLiftInput();
        ActivateCandleFloating();

        if (shadowSpriteRenderer != null && shadowAnimator != null)
        {
            HandleAnimation();
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

            sr.enabled = false;
            ActivateSprite.enabled = true;
        }
        else
        {
            isHeld = false;
            transform.SetParent(null);
            rb.simulated = true;

            sr.enabled = true;
            ActivateSprite.enabled = false;
        }
    }

    // Set isLit State for the candle
    public void LitCandle()
    {
        if (!isLit)
        {
            isLit = true;
            if (ActivateSprite != null) ActivateSprite.sprite = LitFrame;
            if (sr != null) sr.sprite = LitFrame;
        }
        else
        {
            isLit = false;
            if (ActivateSprite != null) ActivateSprite.sprite = OffFrame;
            if (sr != null) sr.sprite = OffFrame;
        }
    }

    // Changing the scale of the shadow
    public void AdjustShadowScale(float scrollInput)
    {
        if (shadowInstance == null) return;

        RecalculateOffset();

        Vector3 currentScale = shadowInstance.transform.localScale;

        float scaleSpeed = 0.5f;

        float newScale = Mathf.Clamp(currentScale.x + scrollInput * scaleSpeed, 1f, 3f);

        shadowInstance.transform.localScale = new Vector3(newScale, newScale, 1f);
    }

    // Generate the corresponding shadow when candle lit and vice versa
    private void HandleShadowGeneration()
    {
        if (isLit)
        {
            // sr.color = Color.red;
            if (shadowInstance == null && shadowPrefab != null)
            {
                // Get the correct generate position
                Transform _prefabAnchor = shadowPrefab.transform.Find("BottomAnchor");
                shadowOffset = -_prefabAnchor.localPosition;
                
                // Generate Shadow Prefab Instance
                shadowInstance = Instantiate(shadowPrefab, bottomAnchor.position + shadowOffset, Quaternion.identity);

                // Getting Animation Reference
                shadowSpriteRenderer = shadowInstance.GetComponentInChildren<SpriteRenderer>();
                shadowAnimator = shadowInstance.GetComponentInChildren<Animator>();
            }
        }
        else
        {
            // sr.color = Color.blue;
            //Clear the shadow Instance when not Lit
            if (shadowInstance != null)
            {
                Destroy(shadowInstance);
                shadowInstance = null;
            }
        }
    }
    

    // Update the shadow's Position 
    private void HandleShadowPosition()
    {
        if (lifting) return;

        if (shadowInstance == null || bottomAnchor == null)
        {
            Debug.Log("ERROR: Missing Referenec: " +
                "shadowInstance == null || bottomAnchor == null");
            return;
        }

        RecalculateOffset();


        Vector3 _shadowPos = Vector3.zero;
        // Get the bottom position of player and candle
        Vector3 _candleBottom = bottomAnchor.position;
        Vector3 _playerBottom = playerInteraction.bottomAnchor.position;
        float _distance = Vector3.Distance(_candleBottom, _playerBottom);
        // Make sure the shadow is on the same side with the player
        Vector3 _direction = (_playerBottom - _candleBottom).normalized;

        // If CandlePinhole exist, handle flip
        if (transform.Find("CandlePinhole"))
        {
            var shadowSprite = shadowInstance.transform.Find("ShadowSprite").GetComponent<SpriteRenderer>();
            //_shadowPos.y += shadowSprite.bounds.size.y;
            shadowSprite.flipY = true;

            _direction.y = -_direction.y;
            // calculate the distance for generation according to Intensity and clamped
            float _extendedLength = Mathf.Min(_distance * shadowDistanceIntensity, maxShadowDistance);

            _shadowPos = _candleBottom + _direction * _extendedLength;
            _shadowPos.y -= shadowOffset.y + fineTuneOffset;

            // Calculate the thickness of the ground and apply.
            RaycastHit2D hit = Physics2D.Raycast(
                playerInteraction.bottomAnchor.position,
                Vector2.down,
                5f,
                LayerMask.GetMask("Ground")
            );

            if (hit.collider != null)
            { 
                GameObject ground = hit.collider.gameObject;

                BoxCollider2D col = hit.collider as BoxCollider2D;

                if (col != null)
                {
                    float thickness = col.size.y * ground.transform.localScale.y;

                    _shadowPos.y -= thickness;
                }
            }

        }
        else 
        {
            // calculate the distance for generation according to Intensity and clamped
            float _extendedLength = Mathf.Min(_distance * shadowDistanceIntensity, maxShadowDistance);

            // Calculate the actual position
            _shadowPos = _candleBottom + _direction * _extendedLength;

            _shadowPos.y += shadowOffset.y;
        }
        
        // Update
        shadowInstance.transform.position = _shadowPos;
    }

    // Get the new Offset according to WorldPosition
    private void RecalculateOffset()
    {
        Transform shadowAnchor = shadowInstance.transform.Find("BottomAnchor");
        if (shadowAnchor != null)
        {
            float bottomToOrigin = shadowAnchor.position.y - shadowInstance.transform.position.y;
            shadowOffset = new Vector3(0, -bottomToOrigin, 0);
        }
    }

    // Check for input when the shadow created by the candle can lift the player
    private void ReceiveLiftInput()
    {
        if (shadowInstance == null) return;

        Rigidbody2D _playerRb = playerInteraction.gameObject.GetComponent<Rigidbody2D>();

        if (Input.GetKey(liftKey) && isHeld && !lifting)
        {
            pressTimeCount += 0.1f;

            if (pressTimeCount > pressTimeMax)
            {
                lifting = true;
                pressTimeCount = 0;

                // Stop physics
                _playerRb.linearVelocityY = 0;
                _playerRb.simulated = false;

                // set player's position to the current shadow's position
                Vector3 _offset = new Vector3(0, 0.5f, 0);
                playerInteraction.gameObject.transform.position = shadowInstance.transform.position + _offset;

                // The candle need to be dropped at this point
                PickOrDrop(null);
                playerInteraction.EraseHoldingObject();
            }
        }

        if (Input.GetKeyDown(jumpKey) && lifting)
        {
            // Reactivate physics
            _playerRb.simulated = true;
            // Manual setup a jump for player
            PlayerMovement _playerMovement = playerInteraction.gameObject.GetComponent<PlayerMovement>();
            _playerRb.linearVelocityY = _playerMovement.jumpForce;

            // change the state
            lifting = false;
        }

    }

    private void ActivateCandleFloating()
    { 
        if (ActivateSprite == null || !ActivateSprite.enabled) return;

        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        ActivateSprite.transform.localPosition = 
            activateSpriteDefaultPos + new Vector3(0f, floatOffset, 0f);
    }

    private void HandleAnimation()
    {
        if (playerInteraction == null) return;

        PlayerMovement _playerMovement = playerInteraction.gameObject.GetComponent<PlayerMovement>();
        Animator _playerAnimator = _playerMovement.animator;

        if (_playerMovement == null) return;

        if (_playerMovement.FacingLeft)
        {
            shadowSpriteRenderer.flipX = true;
        }
        else
        {
            shadowSpriteRenderer.flipX = false;
        }

        if (_playerMovement.AnimationMoving)
        {
            shadowAnimator.SetBool("Moving", true);
        }
        else
        {
            shadowAnimator.SetBool("Moving", false);
        }

        if (_playerMovement.AnimationUp)
        {
            shadowAnimator.SetBool("Up", true);
        }
        else
        {
            shadowAnimator.SetBool("Up", false);
        }

        if (_playerMovement.AnimationFall)
        {
            shadowAnimator.SetBool("Fall", true);
        }
        else
        {
            shadowAnimator.SetBool("Fall", false); 
        }

    }

}
