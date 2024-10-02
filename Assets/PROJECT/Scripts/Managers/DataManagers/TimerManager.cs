using UnityEngine;
using UnityEngine.Events;

namespace YagizEraslan.EclipsedEcho
{
    public class TimerManager : MonoBehaviour
    {
        public UnityAction<float> OnTimerUpdated;

        private float timer;
        private bool isTiming;
        private float lastDisplayedTime = 0f;

        public float Timer => timer;

        public static TimerManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

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

        public void ResetTimer()
        {
            isTiming = false;
            timer = 0f;
            OnTimerUpdated?.Invoke(timer);
        }

        public void SetTimer(float savedTime)
        {
            timer = savedTime;
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
