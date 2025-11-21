using System;
using UnityEngine;
using UnityEngine.Events;

namespace Switch
{
    public class SwitchGroup : MonoBehaviour
    {
        // the switches direct under the switch group
        private AbstractSwitch[] _switches;

        // indicator of how many switches are currently active
        private int _numOfActiveSwitches;

        private void Start()
        {
            _switches = GetComponentsInChildren<AbstractSwitch>();
            // Debug.Log($"Found {_switches.Length} switches");
            foreach (var s in _switches)
            {
                s.onActivateStateChange.AddListener(OnSwitchStateChange);
            }
            
            if (requireAllActive)
            {
                requiredNumActive = _switches.Length;
            }
        }

        // The required number of switch to be active for the switch group to be activated
        [Min(0)] public int requiredNumActive = 1;

        // if set to true, the requiredNumActive will be ignored
        public bool requireAllActive;

        // the parameter is the active state
        public UnityEvent<bool> onSwitchGroupActiveStateChange;

        public int GetNumberOfActiveSwitches()
        {
            return _numOfActiveSwitches;
        }

        private void OnSwitchStateChange(bool active)
        {
            Debug.Log($"Switch state changed to {active}");
            if (active)
            {
                _numOfActiveSwitches += 1;
                if (_numOfActiveSwitches == requiredNumActive)
                {
                    onSwitchGroupActiveStateChange?.Invoke(true);
                    Debug.Log("Switch group activate");
                }
            }
            else
            {
                _numOfActiveSwitches -= 1;
                if (_numOfActiveSwitches == requiredNumActive - 1)
                {
                    onSwitchGroupActiveStateChange?.Invoke(false);
                    Debug.Log("Switch group deactivate");
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var s in _switches)
            {
                s.onActivateStateChange.RemoveListener(OnSwitchStateChange);
            }
        }
    }
}