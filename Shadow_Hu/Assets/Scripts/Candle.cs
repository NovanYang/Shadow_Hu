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
    private KeyCode liftKey = KeyCode.Space;
    private bool lifting = false;

    // Shadow Settings
    public float maxShadowDistance = 20f;
    public float shadowDistanceIntensity = 2f;    
    private GameObject shadowInstance;
    private Vector3 shadowOffset = Vector3.zero;

    

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
    }

    private void Update()
    {
        HandleShadowGeneration();
        HandleShadowPosition();
        ReceiveLiftInput();
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
            sr.color = Color.red;
            if (shadowInstance == null && shadowPrefab != null)
            {
                // Get the correct generate position
                Transform _prefabAnchor = shadowPrefab.transform.Find("BottomAnchor");
                shadowOffset = -_prefabAnchor.localPosition;
                // Generate Shadow Prefab Instance
                shadowInstance = Instantiate(shadowPrefab, bottomAnchor.position + shadowOffset, Quaternion.identity);
            }
        }
        else
        {
            sr.color = Color.blue;
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

        // Get the bottom position of player and candle
        Vector3 _candleBottom = bottomAnchor.position;
        Vector3 _playerBottom = playerInteraction.bottomAnchor.position;

        // Make sure the shadow is on the same side with the player
        Vector3 _direction = (_playerBottom - _candleBottom).normalized;

        float _distance = Vector3.Distance(_candleBottom, _playerBottom);

        // calculate the distance for generation according to Intensity and clamped
        float _extendedLength = Mathf.Min(_distance * shadowDistanceIntensity, maxShadowDistance);

        // Calculate the actual position
        Vector3 _shadowPos = _candleBottom + _direction * _extendedLength;

        _shadowPos.y = _shadowPos.y + shadowOffset.y;

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

        if (Input.GetKeyDown(liftKey))
        {
            Rigidbody2D _playerRb = playerInteraction.gameObject.GetComponent<Rigidbody2D>();

            // When the shadow is not lifting player, lift player
            if (!lifting)
            {
                if (!isHeld)
                {
                    return;
                }

                lifting = true;   

                // Stop physics
                _playerRb.linearVelocityY = 0;
                _playerRb.simulated = false;

                // set player's position to the current shadow's position
                Vector3 _offset = new Vector3(0, 0.5f, 0);
                playerInteraction.gameObject.transform.position = shadowInstance.transform.position + _offset;

                // The candle need to be dropped at this point
                PickOrDrop(null);
            }
            else
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

    }
}
