using Switch;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool IsDoorOpen { get; private set; }

    
    public SwitchGroup connectedSwitchGroup;

    private void Awake()
    {
        if (connectedSwitchGroup == null)
        {
            Debug.LogError("connectedSwitchGroup is not assigned");
        }
    }

    private void Start()
    {
        connectedSwitchGroup.onSwitchGroupActiveStateChange.AddListener(OnSwitchGroupStateChange);
    }

    private void OnSwitchGroupStateChange(bool isActive)
    {
        IsDoorOpen = true;
        Debug.Log("Door open state change");
    }
}
