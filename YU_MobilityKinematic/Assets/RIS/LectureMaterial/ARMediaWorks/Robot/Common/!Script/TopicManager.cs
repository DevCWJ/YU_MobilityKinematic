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
    }

    public class TopicManager : CWJ.Singleton.SingletonBehaviour<TopicManager>
    {
        private static bool isInit;
        public const string CommonObjName = "[CWJ.YU.Common]";

        [Readonly] public Transform commonRootObj;
        public static Camera FindCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null || !mainCamera.enabled)
                mainCamera = FindObjectsOfType<Camera>().FirstOrDefault(c => c.enabled && c.CompareTag("MainCamera"));
            return mainCamera;
        }
        public static void InitCommonObjs()
        {
            if (isInit) return;
            isInit = true;

            if(!HasInstance || Instance.commonRootObj == null)
            {
                var newCommonObj = Resources.Load<GameObject>(CommonObjName);
                newCommonObj.SetActive(false);
                newCommonObj.transform.SetParent(null, true);
                newCommonObj.transform.position = Vector3.zero;
                newCommonObj.transform.rotation = Quaternion.identity;
                Instance.commonRootObj = newCommonObj.transform;
            }
            Instance.commonRootObj.gameObject.SetActive(true);

        }

        protected override void _OnDestroy()
        {
            isInit = false;
        }

        [SerializeField] TextMeshProUGUI titleTxt;
        [SerializeField] TextMeshProUGUI titleNumberTxt;
        [SerializeField] TextMeshProUGUI subTitleTxt;
        [SerializeField] TextMeshProUGUI contextTxt;

        [SerializeField] Button prevBtn, nextBtn;

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

        public Topic[] topics;
        public int curTopicIndex;

        protected override void OnBeforeInstanceAssigned()
        {
            Debug.LogError("OnBeforeInstanceAssigned()");
            topics = CWJ.FindUtil.FindObjectsOfType_New<Topic>(true, true).OrderBy(t => t.topicIndex).ToArray();
        }

        protected override void _Awake()
        {
            Debug.LogError("Awake");
            prevBtn.onClick.AddListener(() => topics[curTopicIndex].Previous());
            nextBtn.onClick.AddListener(() => topics[curTopicIndex].Next());
        }

        public void SetTopic(int index)
        {
            var needUpdateObjs = FindUtil.FindInterfaces<INeedSceneObj>(false);
            var mainCam = FindCamera();
            foreach (var obj in needUpdateObjs)
            {
                obj.SetCamera(mainCam);
            }

            RotateObjByUI.Instance.SetTarget(null);
            foreach (Topic topic in topics)
            {
                topic.Init();
            }
            curTopicIndex = index;
            topics[index].Next();
        }
    }
}
