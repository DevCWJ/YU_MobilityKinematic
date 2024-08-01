using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
namespace CWJ
{

    public class ThreadDispatcher : CWJ.Singleton.SingletonBehaviourDontDestroy<ThreadDispatcher>, CWJ.Singleton.IDontPrecreatedInScene
    {

        static readonly object _ActQueueLock = new object();
        private static Queue<Action> _ActionQueue = new Queue<Action>();

        static readonly object _UiActQueueLock = new object();
        private static Queue<Action> _UiActionsQueue = new Queue<Action>();


        [System.Serializable]
        public struct DelayedQueueItem : IComparable<DelayedQueueItem>, IEquatable<DelayedQueueItem>
        {
            public bool hasValue;
            public float time;
            public Action action;

            public DelayedQueueItem(Action action, float delay)
            {
                this.time = delay > 0.0f ? delay /*+ Time.time*/ : 0;
                this.action = action;
                hasValue = true;
            }

            public int CompareTo(DelayedQueueItem other)
            {
                return Math.Truncate(time).CompareTo(Math.Truncate(other.time));
            }

            public bool Equals(DelayedQueueItem other)
            {
                return other.hasValue == hasValue && other.time == time && other.action == action;
            }
            public override int GetHashCode()
            {
                return HashCodeHelper.GetHashCode(hasValue, time, action);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize_AfterSceneLoad()
        {
            if (!HasInstance)
                UpdateInstance();
            ThreadDispatcher.Instance.transform.SetParent(null);
            ThreadDispatcher.Instance.gameObject.SetActive(true);
        }

        private void Update()
        {
            List<Action> tmpActList = null;

            if (_ActionQueue.Count > 0)
            {
                lock (_ActQueueLock)
                {
                    tmpActList = new List<Action>(_ActionQueue);
                    _ActionQueue.Clear();
                }
                foreach (Action act in tmpActList)
                {
                    act.Invoke();
                }
                tmpActList = null;
            }

            if (_UiActionsQueue.Count > 0)
            {
                lock (_UiActQueueLock)
                {
                    tmpActList = new List<Action>(_UiActionsQueue);
                    _UiActionsQueue.Clear();
                }
                foreach (Action uiAct in tmpActList)
                {
                    uiAct.Invoke();
                }
                tmpActList = null;
            }
        }

        protected override void _OnApplicationQuit()
        {
            Clear();
            UIClear();
        }

        public static void UIEnqueue(Action action)
        {
            if (action == null)
            {
                Debug.LogError("UIEnqueue action is null");
                return;
            }
            lock (_UiActQueueLock)
            {
                _UiActionsQueue.Enqueue(action);
            }
        }


        public static void Enqueue(System.Action action)
        {
            if (action == null)
            {
                Debug.LogError("Enqueue action is null");
                return;
            }
            lock (_ActQueueLock)
            {
                _ActionQueue.Enqueue(action);
            }
        }

        public static void Clear()
        {
            lock (_ActQueueLock)
            {
                _ActionQueue.Clear();
            }
        }

        public static void UIClear()
        {
            lock (_UiActQueueLock)
            {
                _UiActionsQueue.Clear();
            }
        }
    }

}