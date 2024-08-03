using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace CWJ
{
    public enum EKeyState
    {
        /// <summary>
        /// KeyDown
        /// </summary>
        Began = 0,

        /// <summary>
        /// Hold중 + 커서움직임
        /// </summary>
        Move = 1,

        /// <summary>
        /// Hold중 + 커서안움직임
        /// </summary>
        Stationary = 2,

        /// <summary>
        /// KeyUp
        /// </summary>
        Ended = 3,

        None = 4
    }

    public class KeyEventManager_PC : _KeyEventManager
    {
        public static KeyListener GetKeyListener(KeyCode keyCode, EKeyState eventState = EKeyState.None, UnityAction callback = null)
        {
            int touchListenersLength = KeyEventManager_PC.Instance.touchListeners.Length;

            var targetTl = touchListenersLength == 0 ? null 
                : KeyEventManager_PC.Instance.touchListeners.FirstOrDefault(tl => tl.detectTargetKey == keyCode);
            if (targetTl != null && targetTl.enabled == false) 
                targetTl = null;
            if (targetTl == null)
            {
                var newTlObj = touchListenersLength == 0 ? new GameObject("Key_TouchListener") 
                    : KeyEventManager_PC.Instance.touchListeners[0].gameObject;
                targetTl = newTlObj.AddComponent<KeyListener>();
                targetTl.detectTargetKey = keyCode;
            }
            if (callback != null && eventState != EKeyState.None)
                targetTl.touchEvents[eventState.ToInt()].AddListener_New(callback);
            KeyEventManager_PC.Instance.AddKeyListener(targetTl);
            return targetTl;
        }

        [SerializeField] protected KeyCode[] detectKeys = new KeyCode[0];
        protected Dictionary<KeyCode, UnityEvent[]> eventListByKeycode = new Dictionary<KeyCode, UnityEvent[]>();

        public override sealed bool AddKeyListener(KeyListener listener)
        {
            if (listener.detectTargetKey == KeyCode.None || !base.AddKeyListener(listener))
            {
                return false;
            }

            if (!detectKeys.IsExists(listener.detectTargetKey))
            {
                ArrayUtil.Add(ref detectKeys, listener.detectTargetKey);

                UnityEvent[] touchEvts = new UnityEvent[5];
                for (int i = 0; i < 5; i++) //5 = EnumUtil.GetLength<TouchPhase>()
                {
                    touchEvts[i] = new UnityEvent();
                    touchEvts[i].AddListener_New(listener.touchEvents[i].Invoke, false);
                }

                eventListByKeycode.Add(listener.detectTargetKey, touchEvts);
            }
            else
            {
                var touchEvts = eventListByKeycode[listener.detectTargetKey];
                for (int i = 0; i < 5; i++)
                {
                    touchEvts[i].AddListener_New(listener.touchEvents[i].Invoke, false);
                }
            }

            onUpdateEnded.AddListener_New(listener.onUpdateEnded.Invoke, false);
            return true;
        }

        public override sealed bool RemoveKeyListener(KeyListener listener)
        {
            if (!base.RemoveKeyListener(listener) || !detectKeys.IsExists(listener.detectTargetKey))
            {
                return false;
            }

            if (touchListeners.FirstOrDefault(t => t.detectTargetKey == listener.detectTargetKey) == null)
            {
                eventListByKeycode.Remove(listener.detectTargetKey);
                ArrayUtil.Remove(ref detectKeys, listener.detectTargetKey);
            }
            else
            {
                var touchEvts = eventListByKeycode[listener.detectTargetKey];
                for (int i = 0; i < 5; i++)
                {
                    touchEvts[i].RemoveListener_New(listener.touchEvents[i].Invoke);
                }
            }

            onUpdateEnded.RemoveListener_New(listener.onUpdateEnded.Invoke);
            return true;
        }

        public override sealed void UpdateInputSystem()
        {
            EKeyState keyState;

            bool isAnyKeyClicked = false;

            bool? _cursorMoving = null;

            foreach (KeyCode keyCode in detectKeys)
            {
                keyState = EKeyState.None;

                if (Input.GetKeyDown(keyCode))
                {
                    isAnyKeyClicked = true;
                    keyState = EKeyState.Began;
                }
                else if (Input.GetKey(keyCode))
                {
                    isAnyKeyClicked = true;
                    if (_cursorMoving == null)
                        _cursorMoving = DetectIsCursorMoving();
                    bool _moving = _cursorMoving.Value;
                    keyState = _moving ? EKeyState.Move : EKeyState.Stationary;
                }
                else if (Input.GetKeyUp(keyCode))
                {
                    keyState = EKeyState.Ended;
                }

                if (keyState != EKeyState.None)
                {
                    eventListByKeycode[keyCode][keyState.ToInt()].Invoke();
                }
            }

            isHoldDown = isAnyKeyClicked;
            isCursorMoving = _cursorMoving == null ? false : _cursorMoving.Value;

            if (!isAnyKeyClicked)
            {
                if (CO_StationaryCheck != null)
                {
                    StopCoroutine(CO_StationaryCheck);
                    CO_StationaryCheck = null;
                }
            }
            
            onUpdateEnded?.Invoke();
        }

        private Vector3 prevMousePos;

        //(Input.GetAxisRaw("Mouse X") == .0f && Input.GetAxisRaw("Mouse Y") == .0f); touch되는 windows에서 touch 커서드래그를 인식못함
        private bool GetMouseStationary()
        {
            Vector3 prevPos = prevMousePos;
            prevMousePos = Input.mousePosition;
            return prevPos == Input.mousePosition;
        }

        //GetAxisRaw 가 버그가있어서 코루틴필요
        private bool DetectIsCursorMoving()
        {
            if (CO_StationaryCheck == null)
            {
                if (GetMouseStationary())
                {
                    return false;
                }
                else
                {
                    CO_StationaryCheck = StartCoroutine(DO_StationaryCheck());
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private static Coroutine CO_StationaryCheck = null;

        private IEnumerator DO_StationaryCheck()
        {
            yield return null;

            int staionaryCnt = 0;

            while (staionaryCnt < 3)
            {
                yield return null;
                if (GetMouseStationary())
                {
                    ++staionaryCnt;
                }
                else
                {
                    staionaryCnt = 0;
                }
            }
            CO_StationaryCheck = null;
        }
    }
}