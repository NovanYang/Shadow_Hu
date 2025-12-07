using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D.Animation;

namespace Switch
{
    public abstract class AbstractSwitch : MonoBehaviour
    {
        public UnityEvent<bool> onActivateStateChange = new ();
        
        public bool IsActivated { get; protected set; }
        
        public abstract void Switch();
        
        protected SpriteResolver spriteResolver;
        
        protected AudioSource audioSource;

        protected void Start()
        {
            spriteResolver = GetComponent<SpriteResolver>();
            audioSource = GetComponent<AudioSource>();
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Shadow"))
            {
                Switch();
            }
        }
    }
}