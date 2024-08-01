using System;
using System.Collections.Generic;

using CWJ.Singleton.SwapSingleton;

using UnityEngine;
using UnityEngine.Events;

namespace CWJ
{
    [AddComponentMenu("Scripts/" + nameof(CWJ) + "/CWJ_" + nameof(_KeyEventManager))]
    public class _KeyEventManager : SingletonBehaviourDontDestroy_Swap<_KeyEventManager>
    {
        [DrawHeaderAndLine("Variable For Check")]
        [Tooltip("등록된 Listener가 있어야 체크가능")]
        [Readonly, SerializeField] private bool _isHoldDown;//클릭중인지

        public bool isHoldDown { get => _isHoldDown; protected set => _isHoldDown = value; }

        [Tooltip("등록된 Listener가 있어야 체크가능")]
        [Readonly, SerializeField] private bool _isCursorMoving; //클릭후 포인터를 이동중인지

        public bool isCursorMoving { get => _isCursorMoving; protected set => _isCursorMoving = value; }

#if UNITY_EDITOR

        [UnityEditor.MenuItem("GameObject/" + nameof(CWJ) + "/" + nameof(_KeyEventManager), false, 10)]
        public static void CreateTouchManager()
        {
            Transform backupParent = UnityEditor.Selection.activeTransform;

            if (IsExists)
            {
                UnityEditor.Selection.activeTransform = FindUtil.FindObjectOfType_New<_KeyEventManager>(false, false).transform;
                return;
            }

            GameObject newObj = new GameObject(nameof(_KeyEventManager), typeof(_KeyEventManager));
            newObj.transform.SetParent(backupParent);
            newObj.transform.Reset();
            UnityEditor.Selection.activeObject = newObj;
        }

#endif
        [DrawHeaderAndLine("")]
        [Space]
        /// <summary>
        /// Editor 디버깅용
        /// </summary>
        [Readonly] [SerializeField] protected bool isMobile;

        public KeyListener[] touchListeners = new KeyListener[0];

        protected UnityEvent[] touchEvents = new UnityEvent[5]
        {
            new UnityEvent(), //Began
            new UnityEvent(), //Moved
            new UnityEvent(), //Stationary
            new UnityEvent(), //Ended
            new UnityEvent() //Canceled
        };

        protected UnityEvent onUpdateEnded = new UnityEvent();



        public virtual bool AddKeyListener(KeyListener listener)
        {
            if (ArrayUtil.IsExists(touchListeners, listener)) //중복 추가 방지
            {
                return false;
            }
            ArrayUtil.Add(ref touchListeners, listener);

            return true;
        }

        public virtual bool RemoveKeyListener(KeyListener listener)
        {
            if (!ArrayUtil.IsExists(touchListeners, listener)) //제거 반복 방지
            {
                return false;
            }
            ArrayUtil.Remove(ref touchListeners, listener);
            return true;
        }

        public virtual void UpdateInputSystem()
        {
        }

        protected void Update()
        {
            if (touchListeners.Length == 0)
            {
                return;
            }

            UpdateInputSystem();
        }

        protected override Type GetSwapType()
        {
#if !UNITY_EDITOR
#if UNITY_ANDROID
            isMobile = true;
#else
            isMobile = false;
#endif
#endif
            return isMobile ? typeof(TouchManager_Mobile) : typeof(KeyEventManager_PC);
        }

        protected override void SwapSetting(_KeyEventManager newComp)
        {
            newComp.isMobile = isMobile;

            for (int i = 0; i < touchListeners.Length; i++)
            {
                newComp.AddKeyListener(touchListeners[i]);
            }
        }
    }
}