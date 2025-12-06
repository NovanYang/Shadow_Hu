using UnityEngine;
using UnityTimer;

namespace Switch
{
    public class TimerSwitch: AbstractSwitch
    {

        [Min(0.01f)]
        public float duration = 1.0f;
            
        private Timer _timer;

        public override void Switch()
        {
            if (_timer != null)
            {
                _timer.Cancel();
            }

            if (!IsActivated)
            {
                IsActivated = true;
                spriteResolver.SetCategoryAndLabel("State", "On");
                onActivateStateChange.Invoke(true);
            }
            
            _timer = Timer.Register(duration, () =>
            {
                IsActivated = false;
                onActivateStateChange.Invoke(false);
                spriteResolver.SetCategoryAndLabel("State", "Off");
                // Debug.Log("Switch Deactivated After " + duration);
            });
        }
    }
}