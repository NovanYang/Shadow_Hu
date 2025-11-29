using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShadowInteraction : MonoBehaviour
{
    // References
    public GameObject interactableObject = null;
    public GameObject holdingObject = null;
    public bool beingLit = false;
    private KeyCode interactKey = KeyCode.E;

    // List of colliding objects
    private List<GameObject> overlappingObjects = new List<GameObject>();

    // Adding all interacting objects into list
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ShadowKiller"))
        {
            beingLit = true;
            HandleShadowKiller();
        }

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
        if (collision.gameObject.CompareTag("ShadowKiller"))
        {
            beingLit = false;
        }

        if (overlappingObjects.Contains(collision.gameObject))
        {
            overlappingObjects.Remove(collision.gameObject);
        }

        if (collision.gameObject == interactableObject)
        {
            interactableObject = null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ShadowKiller")) HandleShadowKiller();
    }

    private void Update()
    {
        UpdateInteractableObject();
        ReceiveInteraction();
    }

    // Handle Situation When lit by special light(that can eliminate shadow)
    private void HandleShadowKiller()
    {
        if (holdingObject != null)
        {
            switch (holdingObject.tag)
            {
                case "Block":
                    var _block = holdingObject.GetComponent<Block>();
                    _block.PickOrDrop(transform);
                    holdingObject = null;
                    break;
            }
        }
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
                if (distance < _minDistance)
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
            case "Block":

                if (holdingObject == null)
                {
                    holdingObject = target;
                }
                else if (holdingObject == target)
                {
                    holdingObject = null;
                }
                var _block = target.GetComponent<Block>();
                if (_block != null)
                {
                    _block.PickOrDrop(transform);
                }
                break;
        }
    }

    // Check for key input for interaction
    private void ReceiveInteraction()
    {
        if (beingLit) return;

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
}
