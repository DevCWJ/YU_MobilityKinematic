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
            lastMyPos = Vector3.zero;
        }

        private void OnDisable()
        {
            if (MonoBehaviourEventHelper.IS_QUIT)
            {
                return;
            }
            LookAtCamUpdater.RemoveUpdateListener(OnReceiveCamPos);
        }

        Vector3 lastMyPos;
        void OnReceiveCamPos(Vector3 camPos, bool isChanged)
        {
            Vector3 myPos = transform.position;
            if (!isChanged && myPos.Equals(lastMyPos))
            {
                return;
            }
            var dir = camPos - myPos;
            transform.LookAt(myPos - dir);
            lastMyPos = myPos;
        }
    }
}
