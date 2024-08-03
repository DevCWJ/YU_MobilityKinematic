using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CWJ.YU.Mobility
{
    public class ObjRotateHelper : MonoBehaviour
    {
        [SerializeField, ErrorIfNull] RotateAxes rotateAxes;
        Transform targetTrf;

        [SerializeField] Button leftBtn, rightBtn;

        [SerializeField] Button initRotBtn;
        [SerializeField] Toggle holdRotToggle;

        Quaternion initRot;
        bool isHoldRot;
        private void Start()
        {
            targetTrf = rotateAxes.AxesPivot;
            isHoldRot = false;
            initRot = targetTrf.rotation;

            leftBtn.AddLongPressLoopEvent(OnClickLeftLongPressLoop, 0.25f);
            leftBtn.AddShortPressUpEvent(OnClickLeftShortPressUp, 0.25f);
            rightBtn.AddLongPressLoopEvent(OnClickRightLongPressLoop, 0.25f); 
            rightBtn.AddShortPressUpEvent(OnClickRightShortPressUp, 0.25f);


            initRotBtn.onClick.AddListener_New(OnClickInitBtn);

            holdRotToggle.onValueChanged.AddListener_New(OnToggleChanged);
        }

        private void OnToggleChanged(bool isOn)
        {
            isHoldRot = isOn;
            rotateAxes.enabled = !isHoldRot;
        }

        void OnClickInitBtn()
        {
            targetTrf.rotation = initRot;
        }


        void OnClickLeftLongPressLoop()
        {
            targetTrf.Rotate(Vector3.up, Time.deltaTime * 20);
        }
        void OnClickLeftShortPressUp()
        {
            targetTrf.Rotate(Vector3.up, 15);
        }

        void OnClickRightLongPressLoop()
        {
            targetTrf.Rotate(Vector3.down, Time.deltaTime * 20);
        }

        void OnClickRightShortPressUp()
        {
            targetTrf.Rotate(Vector3.down, 15);
        }
    }
}
