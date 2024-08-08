using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace CWJ.YU.Mobility
{
    public interface INeedSceneObj
    {
        public void SetCamera(Camera camera);
        public void SetCanvas(Canvas canvas);
    }

    public class TopicManager : CWJ.Singleton.SingletonBehaviour<TopicManager>
    {

        public static Camera FindCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null || !mainCamera.enabled)
                mainCamera = FindObjectsOfType<Camera>().FirstOrDefault(c => c.enabled && c.CompareTag("MainCamera"));
            return mainCamera;
        }

        public const string RootPrefabName = "[CWJ.YU.Root]";
        private static bool _IsInitRootObj;
        [VisualizeField] static Transform Prefab_RootObj;

        public static void CreateRootObj()
        {
            if (!_IsInitRootObj)
            {
                _IsInitRootObj = true;

                if (Prefab_RootObj == null)
                {
                    var newCommonObj = FindUtil.GetRootGameObjects_New(true).FirstOrDefault(g => g != null && g.name.Equals(RootPrefabName));
                    if (newCommonObj == null)
                    {
                        newCommonObj = Instantiate(Resources.Load<GameObject>(RootPrefabName));
                        newCommonObj.SetActive(false);
                        newCommonObj.transform.SetParent(null, true);
                        newCommonObj.transform.position = Vector3.zero;
                        newCommonObj.transform.rotation = Quaternion.identity;
                    }

                    Prefab_RootObj = newCommonObj.transform;
                }
                UpdateInstance(isPrintLogOrPopup: false);
            }
            if (!Prefab_RootObj.gameObject.activeSelf)
                Prefab_RootObj.gameObject.SetActive(true);
        }

        protected override void _OnDestroy()
        {
            _IsInitRootObj = false;
        }

        [SerializeField] TextMeshProUGUI titleTxt;
        [SerializeField] TextMeshProUGUI titleNumberTxt;
        [SerializeField] TextMeshProUGUI subTitleTxt;
        [SerializeField] TextMeshProUGUI contextTxt;
        [SerializeField] Button prevBtn, nextBtn;
        public Transform topicParentTrf;

        string lastTitle, lastSubTitle, lastContext;
        public void SetTitleTxt(string title)
        {
            if (lastTitle == title) return;
            lastTitle = title;
            if (title.Contains(")"))
            {
                var splits = title.Split(')', 2);
                titleNumberTxt.SetText(splits[0]);
                title = splits[1];
            }
            titleTxt.SetText(title);
        }
        public void SetSubTitleTxt(string subTitle)
        {
            if (lastSubTitle == subTitle) return;
            subTitleTxt.SetText(lastSubTitle = subTitle);
        }
        public void SetContextTxt(string context)
        {
            if (lastContext == context) return;
            contextTxt.SetText(lastContext = context);
        }

        public CWJ.Serializable.DictionaryVisualized<int, Topic> topicDics = new Serializable.DictionaryVisualized<int, Topic>(10);
        public int curTopicIndex;

        protected override void _Awake()
        {
            prevBtn.onClick.AddListener(OnClickPrev);
            nextBtn.onClick.AddListener(OnClickNext);
        }

        void OnClickPrev() { topicDics[curTopicIndex].Previous(); }
        void OnClickNext() { topicDics[curTopicIndex].Next(); }

        public bool TryAddToDict(Topic topic)
        {
            if (topicDics.TryAdd(topic.topicIndex, topic))
            { //Topic 최초 추가
                if (topicParentTrf != topic.transform.parent)
                {
                    topic.canvasScaler.localScale = Vector3.one;
                    topic.transform.SetParent(topicParentTrf);
                    topic.transform.Reset();
                }
                return true;
            }
            return false;
        }

        public void SetTopic(int index)
        {
            var mainCam = FindCamera();
            var rootCanvas = titleTxt.GetComponentInParent<Canvas>();
            if (rootCanvas.worldCamera != mainCam)
                rootCanvas.worldCamera = mainCam;
            var needUpdateObjs = FindUtil.FindInterfaces<INeedSceneObj>(false);
            foreach (var obj in needUpdateObjs)
            {
                obj.SetCamera(mainCam);
                obj.SetCanvas(rootCanvas);
            }

            RotateObjByUI.Instance.SetTarget(null);
            foreach (Topic topic in topicDics.Values.ToArray())
            {
                topic.Init();
            }
            curTopicIndex = index;
            topicDics[curTopicIndex].Next();
        }
    }
}
