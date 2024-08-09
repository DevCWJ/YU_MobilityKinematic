using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CWJ.YU.Mobility
{

    public class LookAtCamUpdater : CWJ.Singleton.SingletonBehaviour<LookAtCamUpdater>, INeedSceneObj
    {
        [VisualizeProperty] public Camera playerCamera { get; set; }
        [VisualizeProperty] public Transform playerCamTrf { get; set; }
        [VisualizeProperty] public Canvas worldCanvas { get; set; }
        [VisualizeProperty] public RectTransform worldCanvasRectTrf { get; set; }

        [SerializeField] UnityEvent<Vector3, bool> camPosUpdateEvent = null;

        public static void AddUpdateListener(UnityAction<Vector3, bool> action)
        {
            if (Instance.camPosUpdateEvent == null)
                Instance.camPosUpdateEvent = new UnityEvent<Vector3, bool>();
            Instance.camPosUpdateEvent.AddListener_New(action);
        }

        public static void RemoveUpdateListener(UnityAction<Vector3, bool> action)
        {
            if (Instance.camPosUpdateEvent != null)
                Instance.camPosUpdateEvent.RemoveListener_New(action);
        }

        protected override void _Awake()
        {
            LastCamPos = Vector3.zero;
        }

        public static Vector3 LastCamPos;

        void Update()
        {
            if (playerCamTrf == null)
            {
                return;
            }

            Vector3 curCamPos = playerCamTrf.position;
            camPosUpdateEvent.Invoke(curCamPos, !curCamPos.Equals(LastCamPos));
            LastCamPos = curCamPos;
        }
    }

}