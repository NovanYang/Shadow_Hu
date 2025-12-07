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
                spriteResolver.SetCategoryAndLabel("State", "On");
                audioSource.Play();
                onActivateStateChange.Invoke(true);
            }
        }
    }
}