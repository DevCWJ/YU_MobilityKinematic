using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : CWJ.Singleton.SingletonBehaviour<DemoManager>
{
    [SerializeField] vThirdPersonCamera tpsCamera;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (tpsCamera.enabled)
                tpsCamera.enabled = false;
            return;
        }
        else
        {
            if (!tpsCamera.enabled)
                tpsCamera.enabled = true;
        }

    }
}
