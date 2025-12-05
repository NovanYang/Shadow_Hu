using UnityEngine;
using UnityEngine.Events;

namespace Switch
{
    public abstract class AbstractSwitch : MonoBehaviour
    {
        public UnityEvent<bool> onActivateStateChange = new ();
        
        public bool IsActivated { get; protected set; }
        
        public abstract void Switch();

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Shadow"))
            {
                Switch();
            }
        }
    }
}