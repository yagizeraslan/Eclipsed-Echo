using UnityEngine;
using UnityEngine.Events;

namespace YagizEraslan.EclipsedEcho
{
    public class TimerManager : MonoSingleton<TimerManager>
    {
        public UnityAction<float> OnTimerUpdated;

        private float timer;
        private bool isTiming;
        private float lastDisplayedTime = 0f;

        public float Timer => timer;

        public void StartTimer()
        {
            isTiming = true;
            timer = 0f;
            OnTimerUpdated?.Invoke(timer);
        }

        public void StopTimer()
        {
            isTiming = false;
        }

        public void UpdateTimer(float deltaTime)
        {
            if (isTiming)
            {
                timer += deltaTime;

                if (Mathf.FloorToInt(timer) != Mathf.FloorToInt(lastDisplayedTime))
                {
                    lastDisplayedTime = timer;
                    OnTimerUpdated?.Invoke(timer);
                }
            }
        }
    }
}
