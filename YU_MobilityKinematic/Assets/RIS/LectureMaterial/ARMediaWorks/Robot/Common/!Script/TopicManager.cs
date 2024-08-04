using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace CWJ.YU.Mobility
{
    public class TopicManager : CWJ.Singleton.SingletonBehaviour<TopicManager>
    {
        public Topic[] topics;
        public int curTopicIndex;

        protected override void _Start()
        {
            topics = CWJ.FindUtil.FindObjectsOfType_New<Topic>(true, true);
            topics = topics.OrderBy(t => t.topicIndex).ToArray();

            SetTopic(0);
        }

        public void SetTopic(int index)
        {
            foreach (Topic topic in topics)
            {
                topic.Init();
            }
            ObjRotateHelper.Instance.SetTarget(null);
            topics[index].Next();
        }
    }
}
