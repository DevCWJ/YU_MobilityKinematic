using CWJ;
using CWJ.YU.Mobility;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class DemoManager : MonoBehaviour 
{ 
    [SerializeField] vThirdPersonCamera tpsCamera;
    [SerializeField] Topic[] topics_prefabs;

    bool escSwitch;
    private void Start()
    {
        var rightMouseCallback = KeyEventManager_PC.GetKeyListener(KeyCode.Mouse1);
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


#if UNITY_EDITOR
    private void Update()
    {
        if (!Input.anyKeyDown)
        {
            return;
        }
        int wannaIndex = -1;
        for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9; ++i)
        {
            if (Input.GetKeyDown((KeyCode)i))
            {
                int pressKey = i - ((int)KeyCode.Alpha0);
                wannaIndex = pressKey > 0 ? pressKey - 1 : 10;
                break;
            }
        }
        if (wannaIndex >=0)
        {
            Debug.LogError(wannaIndex);
            if (!TopicManager.HasInstance || !TopicManager.Instance.topicDics.ContainsKey(wannaIndex))
            {
                var topic = FindObjectsOfType<Topic>(includeInactive: true).FirstOrDefault(t => t.topicIndex == wannaIndex);
                if (topic != null && !topic.gameObject.activeSelf)
                    topic.gameObject.SetActive(true);
            }
            else
            {
                TopicManager.Instance.SetTopic(wannaIndex);
            }
        }
    }
#endif
}
