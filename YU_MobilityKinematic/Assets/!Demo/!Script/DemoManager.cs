using CWJ;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : CWJ.Singleton.SingletonBehaviour<DemoManager>
{
    [SerializeField] vThirdPersonCamera tpsCamera;

    KeyListener rightMouseCallback;

    bool escSwitch;
    protected override void _Start()
    {
        rightMouseCallback = KeyEventManager_PC.GetKeyListener(KeyCode.Mouse1);
        rightMouseCallback.onTouchBegan.AddListener(() => SetFreeCameraAndCursor(true));
        rightMouseCallback.onTouchEnded.AddListener(() => SetFreeCameraAndCursor(false));

        var escCallback = KeyEventManager_PC.GetKeyListener(KeyCode.Escape);
        escCallback.onTouchBegan.AddListener(() => SetFreeCameraAndCursor(escSwitch = !escSwitch));
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
