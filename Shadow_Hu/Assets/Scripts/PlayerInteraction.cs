using UnityEngine;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    // Public Attribute
    public GameObject holdingObject = null;
    public GameObject interactableObject = null;

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
        #region Assign the nearest Interactable Object
        if (overlappingObjects.Count > 0)
        {
            GameObject nearest = null;
            float minDistance = float.MaxValue;

            foreach (GameObject collidedObject in overlappingObjects)
            {
                float distance = Vector2.Distance(transform.position, collidedObject.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = collidedObject;
                }
            }

            interactableObject = nearest;
        }
        else
        {
            interactableObject = null;
        }

        #endregion
    }


}
