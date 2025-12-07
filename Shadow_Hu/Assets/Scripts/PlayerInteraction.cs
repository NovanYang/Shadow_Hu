using UnityEngine;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    //Attribute
    public GameObject holdingObject = null;
    public GameObject interactableObject = null;
    public Transform bottomAnchor;
    private KeyCode interactKey = KeyCode.E;
    private KeyCode lightUpKey = KeyCode.Q;

    // List of colliding objects
    private List<GameObject> overlappingObjects = new List<GameObject>();

    // Adding all interacting objects into list
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interaction"))
        {
            if (!overlappingObjects.Contains(collision.gameObject))
            { 
                overlappingObjects.Add(collision.gameObject);
            }
        }
    }

    // Removing not-collideable objects
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (overlappingObjects.Contains(collision.gameObject))
        { 
            overlappingObjects.Remove(collision.gameObject);
        }

        if (collision.gameObject == interactableObject)
        {
            interactableObject = null;
        }
    }

    private void Update()
    {
        UpdateInteractableObject();

        if (handleInteractionPriority()) ReceiveInteraction();

        ReceiveCandleSwitch();

        ReceiveCandleTune();

    }

    // Assign the nearest Interactable Object
    private void UpdateInteractableObject()
    {
        if (overlappingObjects.Count > 0)
        {
            GameObject _nearest = null;
            float _minDistance = float.MaxValue;

            foreach (GameObject collidedObject in overlappingObjects)
            {
                float distance = Vector2.Distance(transform.position, collidedObject.transform.position);
                if (distance < _minDistance && !collidedObject.CompareTag("Block"))
                {
                    _minDistance = distance;
                    _nearest = collidedObject;
                }
            }

            interactableObject = _nearest;
        }
        else
        {
            interactableObject = null;
        }
    }

    // Handle Interaction with object, if holding something, drop
    private void HandleInteraction(GameObject target)
    {
        switch (target.tag)
        {
            case "Candle":

                if (holdingObject == null)
                {
                    holdingObject = target; ;
                }
                else if (holdingObject == target)
                {
                    holdingObject = null;
                }

                target = target.transform.parent.gameObject;
                var _candle = target.GetComponent<Candle>();
                if (_candle != null)
                {
                    _candle.PickOrDrop(transform);
                }
                break;
        }
    }

    // Check for key input for interaction
    private void ReceiveInteraction()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (interactableObject != null && holdingObject == null)
            {
                HandleInteraction(interactableObject);
            }
            else if (holdingObject != null)
            {
                HandleInteraction(holdingObject);
            }
        }
    }

    // Check for key input for turning the candle
    private void ReceiveCandleSwitch()
    {
        if (Input.GetKeyDown(lightUpKey))
        {
            if (interactableObject != null && holdingObject == null)
            {
                if (interactableObject.tag == "Candle")
                {
                    var _candle = interactableObject.transform.parent;
                    _candle.GetComponent<Candle>().LitCandle();
                }
            }
            else if (holdingObject != null)
            {
                var _candle = holdingObject.transform.parent;
                _candle.GetComponent<Candle>().LitCandle();
            }
        }
    }

    // Track Signal of tuning the candle and call candle's function
    private void ReceiveCandleTune()
    {
        // check signals only when holding a candle
        if (holdingObject != null && holdingObject.tag == "Candle")
        {
            var _candle = holdingObject.transform.parent.GetComponent<Candle>();
            // check only if the candle is lit
            if (_candle.isLit)
            {
                float _scroll = Input.GetAxis("Mouse ScrollWheel");

                // return when mouse wheel is not activated
                if (Input.GetKeyDown(KeyCode.K)) 
                {
                    _candle.AdjustShadowScale(-0.5f);
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    _candle.AdjustShadowScale(0.5f);
                }

                if (_scroll == 0)
                {
                    return;
                }
                else
                {
                    if (_scroll > 0f)
                    {
                        _candle.AdjustShadowScale(0.5f);

                    }
                    else
                    {
                        _candle.AdjustShadowScale(-0.5f);
                    }
                }
            }
        }
    }

    public void EraseHoldingObject()
    { 
        holdingObject = null;
    }

    public bool handleInteractionPriority()
    {
        bool _playerCanInteract = true;

        ShadowInteraction[] shadowInteractions = FindObjectsOfType<ShadowInteraction>();

        foreach (ShadowInteraction shadow in shadowInteractions)
        {
            if (shadow.GetInteractableCount() > 0)
            {
                _playerCanInteract = false;
            }
        }

        return _playerCanInteract;
    }
}

