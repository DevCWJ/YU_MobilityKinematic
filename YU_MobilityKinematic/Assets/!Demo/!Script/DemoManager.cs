using CWJ;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static UnityEngine.Rendering.DebugUI;

public class DemoManager : CWJ.Singleton.SingletonBehaviour<DemoManager>
{
    [SerializeField] vThirdPersonCamera tpsCamera;

    KeyListener rightMouseCallback;
    protected override void _Start()
    {
        rightMouseCallback = KeyEventManager_PC.GetKeyListener(KeyCode.Mouse1);
        rightMouseCallback.onTouchBegan.AddListener(() => SetFreeCameraAndCursor(true));

        rightMouseCallback.onTouchEnded.AddListener(() => SetFreeCameraAndCursor(false));

        SetFreeCameraAndCursor(false);
    }

    void SetFreeCameraAndCursor(bool isEnabled)
    {
        if (!isEnabled)
            tpsCamera.Init();
        tpsCamera.enabled = !isEnabled;
        Cursor.visible = isEnabled;
        Cursor.lockState = isEnabled ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
