using System;

namespace PhilanthroPoints.Services
{
    public enum FlowStep
    {
        Login = 0,
        Demographics = 1,
        Cards = 2,
        Treats = 3,
        Books = 4,
        Gifts = 5,
        Cart = 6
    }

    public class FlowState
    {
        public FlowStep Current { get; private set; } = FlowStep.Login;

        public event Action? Changed;

        public void Set(FlowStep step)
        {
            if (step == Current) return;
            Current = step;
            Changed?.Invoke();
        }

        public bool CanNavigateTo(FlowStep step)
        {
            // allow navigating to current or previous, and to next if sequential
            return step <= Current + 1;
        }
    }
}
