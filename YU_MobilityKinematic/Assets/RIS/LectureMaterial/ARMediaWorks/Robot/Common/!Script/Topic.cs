using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CWJ.YU.Mobility
{
    public class Topic : MonoBehaviour
    {
        public int topicIndex = 0;
        public string topicTitle;

        [System.Serializable]
        public class Scenario
        {
            public Transform activateTrf;
            public Transform rotateTargetTrf;
            public string title;
            public string message;

            public void Enable()
            {
                if (activateTrf != null)
                    activateTrf.gameObject.SetActive(true);
                if (rotateTargetTrf != null)
                    ObjRotateHelper.Instance.SetTarget(rotateTargetTrf);
            }

            public void Disable()
            {

            }
        }

        public Scenario[] scenarios;

        int curScenarioIndex = -1;
        public void Init()
        {
            curScenarioIndex = -1;
            foreach (var s in scenarios)
            {
                if (s.activateTrf != null)
                    s.activateTrf.gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
        }

        public void Previous()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            if (0 <= curScenarioIndex && curScenarioIndex < scenarios.Length)
            {
                scenarios[curScenarioIndex].activateTrf.gameObject.SetActive(false);
            }
            if (0 < curScenarioIndex)
                scenarios[--curScenarioIndex].activateTrf.gameObject.SetActive(true);
        }

        public void Next()
        {
            if (!gameObject.activeSelf)
            {

                gameObject.SetActive(true);
            }
            if (0 <= curScenarioIndex && curScenarioIndex < scenarios.Length)
            {
                scenarios[curScenarioIndex].activateTrf.gameObject.SetActive(false);
            }
            if (curScenarioIndex < scenarios.Length - 1)
            {
                var s = scenarios[++curScenarioIndex];
                s.activateTrf.gameObject.SetActive(true);
                if (s.rotateTargetTrf != null)
                    ObjRotateHelper.Instance.SetTarget(s.rotateTargetTrf);
            }
        }
    }
}
