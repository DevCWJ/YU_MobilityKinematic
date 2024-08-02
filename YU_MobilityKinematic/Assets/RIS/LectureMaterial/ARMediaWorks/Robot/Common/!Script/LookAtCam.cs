using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CWJ.YU.Mobility
{
    [DisallowMultipleComponent]
    public class LookAtCam : MonoBehaviour
    {
        private void OnEnable()
        {
            LookAtCamUpdater.AddUpdateListener(OnReceiveCamPos);
        }

        private void OnDisable()
        {
            if (MonoBehaviourEventHelper.IS_QUIT)
            {
                return;
            }
            LookAtCamUpdater.RemoveUpdateListener(OnReceiveCamPos);
        }

        void OnReceiveCamPos(Vector3 camPos)
        {
            Vector3 myPos = transform.position;
            var dir = camPos - myPos;
            transform.LookAt(myPos - dir);
        }
    }
}
