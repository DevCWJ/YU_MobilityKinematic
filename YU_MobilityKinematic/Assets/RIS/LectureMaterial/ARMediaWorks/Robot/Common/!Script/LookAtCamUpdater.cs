using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CWJ.YU.Mobility
{

    public class LookAtCamUpdater : CWJ.Singleton.SingletonBehaviour<LookAtCamUpdater>
    {
        [SerializeField] Transform mainCamTrf;

        [SerializeField] UnityEvent<Vector3> camPosUpdateEvent = null;

        public static void AddUpdateListener(UnityAction<Vector3> action)
        {
            if (Instance.camPosUpdateEvent == null)
                Instance.camPosUpdateEvent = new UnityEvent<Vector3>();
            Instance.camPosUpdateEvent.AddListener_New(action);
        }

        public static void RemoveUpdateListener(UnityAction<Vector3> action)
        {
            if (Instance.camPosUpdateEvent != null)
                Instance.camPosUpdateEvent.RemoveListener_New(action);
        }

        protected override void _Awake()
        {
            if (mainCamTrf == null)
                mainCamTrf = Camera.main.transform;
            LastCamPos = Vector3.zero;
        }


        public static Vector3 LastCamPos;

        void Update()
        {
            if (mainCamTrf == null)
            {
                return;
            }

            Vector3 curCamPos = mainCamTrf.position;
            if (curCamPos.Equals(LastCamPos))
            {
                return;
            }

            camPosUpdateEvent.Invoke(curCamPos);
            LastCamPos = curCamPos;
        }
    }

}