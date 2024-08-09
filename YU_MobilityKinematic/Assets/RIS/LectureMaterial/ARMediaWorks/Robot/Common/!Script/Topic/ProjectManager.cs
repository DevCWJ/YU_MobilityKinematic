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
        public Camera playerCamera { get; set; }
        public Transform playerCamTrf { get; set; }

        public Canvas worldCanvas { get; set; }
        public RectTransform worldCanvasRectTrf { get; set; }
    }

    public class ProjectManager : CWJ.Singleton.SingletonBehaviour<ProjectManager>
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
            UpdateSceneObj();
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

        static Camera _PlayerCam;
        static Canvas _Target3DCanvas;

        public void UpdateSceneObj()
        {
            if (_PlayerCam == null)
            {
                _PlayerCam = Camera.main;
                if (_PlayerCam == null || !_PlayerCam.gameObject.activeSelf || !_PlayerCam.enabled)
                    _PlayerCam = FindObjectsOfType<Camera>().FirstOrDefault(c => c.enabled && c.CompareTag("MainCamera"));
            }
            if (_PlayerCam == null)
            {
                Debug.LogError("Camera 못찾음");
                return;
            }
            Transform playerCamTrf = _PlayerCam.transform;

            if (_Target3DCanvas == null || _Target3DCanvas.gameObject.scene != gameObject.scene)
                _Target3DCanvas = titleTxt.GetComponentInParent<Canvas>(true);

            if (_Target3DCanvas == null)
            {
                Debug.LogError("Canvas 못찾음");
                return;
            }

            if (_Target3DCanvas.worldCamera != _PlayerCam)
                _Target3DCanvas.worldCamera = _PlayerCam;
            var canvasRectTrf = _Target3DCanvas.GetComponent<RectTransform>();

            foreach (var obj in FindUtil.FindInterfaces<INeedSceneObj>(false))
            {
                obj.playerCamera = _PlayerCam;
                obj.playerCamTrf = playerCamTrf;
                obj.worldCanvas = _Target3DCanvas;
                obj.worldCanvasRectTrf = canvasRectTrf;
            }
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

            UpdateSceneObj();

            targetTopic.Next();
            CWJ.AccessibleEditor.AccessibleEditorUtil.PingObj(targetTopic.gameObject);
        }
    }
}
