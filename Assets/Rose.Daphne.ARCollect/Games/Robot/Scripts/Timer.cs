using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rose.Daphne.ARCollect.Games
{
    /// <summary>
    /// Handle Timer and Timer's display.
    /// </summary>
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text textReceiver = null;

        internal delegate void TimerEvent();
        internal TimerEvent OnTimerOver = null;

        internal float MaxTime { set; get; } = 60f;

        private float currentTime = 0f;
        private bool running = false;

        private void Awake()
        {
            textReceiver.gameObject.SetActive(false);
        }

        /// <summary>
        /// Start the timer. Resets time to zero.
        /// </summary>
        public void StartTimer()
        {
            textReceiver.gameObject.SetActive(true);
            currentTime = 0f;
            running = true;
        }

        /// <summary>
        /// Stop timer.
        /// </summary>
        public void StopTimer()
        {
            running = false;
        }

        private void Update()
        {
            if (running)
            {
                currentTime += Time.deltaTime;
                float countDown = MaxTime - currentTime;
                int seconds = Mathf.RoundToInt(countDown % 60f);
                int minutes = Mathf.RoundToInt((countDown - seconds) / 60f);
                textReceiver.text = $"{minutes}:{(seconds < 10f ? "0" : "")}{seconds}";

                if (currentTime >= MaxTime)
                {
                    OnTimerOver?.Invoke();
                    running = false;
                }
            }
        }
    }
}

