using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace CWJ.YU.Mobility
{
    public class TopicManager : CWJ.Singleton.SingletonBehaviour<TopicManager>
    {
        [SerializeField] TextMeshProUGUI titleTxt;
        [SerializeField] TextMeshProUGUI subTitleTxt;
        [SerializeField] TextMeshProUGUI contextTxt;

        [SerializeField] Button prevBtn, nextBtn;

        string lastTitle, lastSubTitle, lastContext;
        public void SetTitleTxt(string title)
        {
            if (lastTitle == title) return;
            titleTxt.SetText(lastTitle = title);
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

        protected override void _Start()
        {
            topics = CWJ.FindUtil.FindObjectsOfType_New<Topic>(true, true).OrderBy(t => t.topicIndex).ToArray();

            prevBtn.onClick.AddListener(() => topics[curTopicIndex].Previous());
            nextBtn.onClick.AddListener(() => topics[curTopicIndex].Next());


            SetTopic(0);
        }

        public void SetTopic(int index)
        {
            ObjRotateHelper.Instance.SetTarget(null);
            foreach (Topic topic in topics)
            {
                topic.Init();
            }
            curTopicIndex = index;
            topics[index].Next();
        }
    }
}
