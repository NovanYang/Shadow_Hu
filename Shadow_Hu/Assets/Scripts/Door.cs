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
            Debug.Log($"Set Is DoorOpen to {value}");
            _isDoorOpen = value;
            if (value)
            {
                _spriteResolver.SetCategoryAndLabel("State", "Open");
                if (_isUserInDoor)
                {
                    Util.SHSceneManager.Instance.SwitchNextScene();
                }
            }
            else
            {
                _spriteResolver.SetCategoryAndLabel("State", "Closed");
            }
        }
    }

    private bool _isUserInDoor;

    private bool IsUserInDoor {
        get => _isUserInDoor;
        set
        {
            if (_isDoorOpen && value)
            {
                Util.SHSceneManager.Instance.SwitchNextScene();
            }
            _isDoorOpen = value;
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
        IsDoorOpen = isActive;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            IsUserInDoor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IsUserInDoor = false;
        }
    }
}
