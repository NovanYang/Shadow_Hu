using Switch;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Door : MonoBehaviour
{
    private SpriteResolver _spriteResolver;
    
    private bool _isDoorOpen;
    
    public bool IsDoorOpen {
        get
        {
            return _isDoorOpen;
        }
        private set
        {
            _isDoorOpen = value;
            if (_isDoorOpen)
            {
                _spriteResolver.SetCategoryAndLabel("State", "Open");
            }
            else
            {
                _spriteResolver.SetCategoryAndLabel("State", "Closed");
            }
        }
    }
    
    public SwitchGroup connectedSwitchGroup;

    private void Awake()
    {
        if (connectedSwitchGroup == null)
        {
            Debug.LogError("A Door should connect a SwitchGroup");
        }
    }

    private void Start()
    {
        connectedSwitchGroup.onSwitchGroupActiveStateChange.AddListener(OnSwitchGroupStateChange);
        _spriteResolver = GetComponent<SpriteResolver>();
    }

    private void OnSwitchGroupStateChange(bool isActive)
    {
        IsDoorOpen = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && IsDoorOpen)
        {
            Debug.Log("Switch to next scene");
            Util.SHSceneManager.Instance.SwitchNextScene();
            IsDoorOpen = false; // set to false to prevent double triggering the event
        }
    }
}
