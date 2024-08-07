using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace CWJ.YU.Mobility
{
    public class Topic : MonoBehaviour
    {
        public int topicIndex = 0;
        public string topicTitle;

        bool isInstantiateInit = false;
        public Transform canvasScaler;

        private void OnEnable()
        {
            if (!isInstantiateInit)
            {
                isInstantiateInit = true;
                if (!TopicManager.HasInstance)
                    TopicManager.CreateRootObj();
                TopicManager.Instance.TryAddToDict(this);
                TopicManager.Instance.SetTopic(topicIndex); //최초 생성직후에만 SetTopic해야함 안그러면 무한루프
                if (string.IsNullOrEmpty(topicTitle) || !topicTitle.Contains(')'))
                {
                    Debug.LogError($"ERR : Topic[{topicIndex}] - {nameof(topicTitle)} null!\n{topicTitle}", gameObject);
                }
                else
                {
                    int contentIndex = int.Parse(topicTitle.Trim().Split(')', 2)[0].Replace("#", string.Empty));
                    if((contentIndex - 1) != topicIndex)
                    {
                        Debug.LogError($"ERR : Topic[{topicIndex}] - {nameof(topicIndex)}가 {nameof(topicTitle)}과 다름. 확인할것." +
                                        $"\n{topicIndex} != {topicTitle}", gameObject);
                    }
                }
            }
        }

        [System.Serializable]
        public class Scenario
        {
            public string subTitle;
            public bool isInit;
            public string context;
            public Transform activateTrf;
            public Transform rotateTargetTrf;

            public void Enable()
            {
                if (activateTrf != null)
                    activateTrf.gameObject.SetActive(true);
                RotateObjByUI.Instance.SetTarget(rotateTargetTrf);
                if (subTitle == null)
                    Debug.LogError("subTitle is null");
                TopicManager.Instance.SetSubTitleTxt(subTitle);
                if (context == null)
                    Debug.LogError("context is null");
                TopicManager.Instance.SetContextTxt(context);
            }

            public void Disable(bool isForAllInit = false)
            {
                if (rotateTargetTrf != null)
                {
                    RotateObjByUI.Instance.ResetRotation();
                    if (!isForAllInit)
                        RotateObjByUI.Instance.SetTarget(null);
                }
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
                if (scenarios[i].isInit) continue;
                scenarios[i].context = scenarioContexts[i].TrimEnd();
                scenarios[i].Disable(true);
                scenarios[i].isInit = true;
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
