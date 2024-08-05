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
            public string subTitle;
            public string context;
            public Transform activateTrf;
            public Transform rotateTargetTrf;

            public void Enable()
            {
                if (activateTrf != null)
                    activateTrf.gameObject.SetActive(true);
                if (rotateTargetTrf != null)
                    ObjRotateHelper.Instance.SetTarget(rotateTargetTrf);
                if (subTitle == null)
                    Debug.LogError("subTitle is null");
                TopicManager.Instance.SetSubTitleTxt(subTitle);
                if (context == null)
                    Debug.LogError("context is null");
                TopicManager.Instance.SetContextTxt(context);
            }

            public void Disable(bool isForAllInit = false)
            {
                if (!isForAllInit && rotateTargetTrf != null)
                    ObjRotateHelper.Instance.SetTarget(null);
                if (activateTrf != null)
                    activateTrf.gameObject.SetActive(false);
            }

            public Scenario(Scenario copySource)
            {
                this.activateTrf = copySource.activateTrf;
                this.rotateTargetTrf = copySource.rotateTargetTrf;
                this.subTitle = copySource.subTitle;
                this.context = copySource.context;
            }
        }

        public Scenario[] scenarios;
        [SerializeField, Readonly] int curScenarioIndex = -1;

        [ResizableTextArea]
        public string[] scenarioContexts;

        public void Init()
        {
            curScenarioIndex = -1;
            for (int i = 0; i < scenarios.Length; i++)
            {
                scenarios[i].context = scenarioContexts[i];
                scenarios[i].Disable(true);
            }
            gameObject.SetActive(false);
        }

        void _ChangeScenario(int toScenarioIndex)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                if (topicTitle == null)
                    Debug.LogError("TopicTitle is null", gameObject);
                TopicManager.Instance.SetTitleTxt(topicTitle);
            }
            int scenarioLength = scenarios.Length;

            if (0 <= curScenarioIndex && curScenarioIndex < scenarioLength)
            {
                scenarios[curScenarioIndex].Disable();
            }

            if (0 <= toScenarioIndex && toScenarioIndex < scenarioLength)
            {
                curScenarioIndex = toScenarioIndex;
                scenarios[curScenarioIndex].Enable();
            }
        }

        public void Previous()
        {
            _ChangeScenario(curScenarioIndex - 1);
        }

        public void Next()
        {
            _ChangeScenario(curScenarioIndex + 1);
        }
    }
}
