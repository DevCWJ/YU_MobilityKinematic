using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

namespace CWJ.YU.Mobility
{
    public interface INeedSceneObj
    {
        public void SetCamera(Camera camera);
        public void SetCanvas(Canvas canvas);
    }

    public class TopicManager : CWJ.Singleton.SingletonBehaviour<TopicManager>
    {
        public const string RootPrefabName = "[CWJ.YU.Root]";
        [VisualizeField] public static Transform Project_RootObj;

        public static void InitWorld()
        {
            if (Project_RootObj == null)
            {
                var rootObj = FindUtil.GetRootGameObjects_New(false).FirstOrDefault(g => g != null && g.name.Equals(RootPrefabName));
                if (rootObj == null)
                {
                    rootObj = Instantiate(Resources.Load<GameObject>(RootPrefabName));
                    rootObj.SetActive(false);
                }
                Project_RootObj = rootObj.transform;
            }
            if (Project_RootObj.parent != null)
                Project_RootObj.SetParent(null, true);
            if (Project_RootObj.position != Vector3.zero)
                Project_RootObj.position = Vector3.zero;
            if (Project_RootObj.rotation != Quaternion.identity)
                Project_RootObj.rotation = Quaternion.identity;
            if (!Project_RootObj.gameObject.activeSelf)
                Project_RootObj.gameObject.SetActive(true);
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
                    topic.transform.SetParent(topicParentTrf, true);
                    topic.transform.Reset();
                }
                return true;
            }
            return false;
        }

        static Camera FindCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null || !mainCamera.enabled)
                mainCamera = FindObjectsOfType<Camera>().FirstOrDefault(c => c.enabled && c.CompareTag("MainCamera"));
            return mainCamera;
        }

        public void SetTopic(int index)
        {
            if (!topicDics.TryGetValue(curTopicIndex, out var targetTopic))
            {
                Debug.LogError("TopicManager에 아직 등록되지 않은: index" + index);
                return;
            }
            if (targetTopic == null)
            {
                Debug.LogError("TopicManager에 등록은 됐으나 NULL: index" + index);
                return;
            }

            RotateObjByUI.Instance.SetTarget(null);
            foreach (Topic topic in topicDics.Values.ToArray())
            {
                topic.Init();
            }

            curTopicIndex = index;

            var mainCam = FindCamera();

            var rootCanvas = titleTxt.GetComponentInParent<Canvas>(true);
            if (rootCanvas.worldCamera != mainCam)
                rootCanvas.worldCamera = mainCam;

            foreach (var obj in FindUtil.FindInterfaces<INeedSceneObj>(true))
            {
                obj.SetCamera(mainCam);
                obj.SetCanvas(rootCanvas);
            }

            targetTopic.Next();
#if UNITY_EDITOR
            EditorGUIUtility.PingObject(Selection.activeObject = targetTopic.gameObject);
#endif
        }
    }
}
