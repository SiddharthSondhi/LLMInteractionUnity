using System.Collections.Generic;
using UnityEngine;

namespace VRQuestionnaireToolkit {
    public class QuestionTimingManager : MonoBehaviour {
        public static QuestionTimingManager Instance;

        private float startTime;
        private bool first = true;

        public List<float> questionTimes = new List<float>();
        private PageController pageController;

        private void Awake() {
            Instance = this;
            pageController = FindFirstObjectByType<PageController>();
        }

        public void StartTimerAndRecordTime() {
            // for the first quesiton, dont record time, otherwise record elapsed time
            if (!first) {
                float elapsed = Time.time - startTime;
                questionTimes.Add(elapsed);
                //Debug.Log($"Recorded time: {elapsed:F2}s");
            }

            // start timer for this question
            first = false;
            startTime = Time.time;
            //Debug.Log("Timer started");
        }
    }
}