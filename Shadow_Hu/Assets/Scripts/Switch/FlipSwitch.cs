using UnityEngine;

namespace Switch
{
    public class FlipSwitch: AbstractSwitch
    {
        public override void Switch()
        {
            if (!IsActivated)
            {
                IsActivated = true;
                onActivateStateChange.Invoke(true);
                spriteResolver.SetCategoryAndLabel("State", "On");
            }
        }
    }
}