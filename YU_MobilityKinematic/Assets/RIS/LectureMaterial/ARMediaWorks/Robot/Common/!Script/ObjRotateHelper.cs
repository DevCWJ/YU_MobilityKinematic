using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CWJ.YU.Mobility
{
    public class ObjRotateHelper : CWJ.Singleton.SingletonBehaviour<ObjRotateHelper>
    {
        [SerializeField, ErrorIfNull] RotateAxes rotateAxes;
        Transform targetTrf;

        [SerializeField] Button leftBtn, rightBtn;

        [SerializeField] Button initRotBtn;
        [SerializeField] Toggle holdRotToggle;
        [SerializeField] GameObject lockedObj, unlockedObj;
        Quaternion initRot;
        bool isHoldRot;

        protected override void _Start()
        {
            leftBtn.AddLongPressLoopEvent(OnClickLeftLongPressLoop, 0.25f);
            leftBtn.AddShortPressUpEvent(OnClickLeftShortPressUp, 0.25f);
            rightBtn.AddLongPressLoopEvent(OnClickRightLongPressLoop, 0.25f);
            rightBtn.AddShortPressUpEvent(OnClickRightShortPressUp, 0.25f);


            initRotBtn.onClick.AddListener_New(OnClickInitBtn);
            holdRotToggle.onValueChanged.AddListener_New(OnToggleChanged);
        }

        public void SetTarget(Transform trf)
        {
            if (rotateAxes = trf == null ? null : trf.GetComponentInChildren<RotateAxes>())
            {
                targetTrf = rotateAxes.AxesPivot;
                initRot = targetTrf.rotation;
            }
            isHoldRot = false;
        }

        private void OnToggleChanged(bool isOn)
        {
            lockedObj.SetActive(isOn);
            unlockedObj.SetActive(!isOn);

            if (rotateAxes == null) return;
            isHoldRot = isOn;
            rotateAxes.enabled = !isHoldRot;
        }

        void OnClickInitBtn()
        {
            if (targetTrf == null) return;
            targetTrf.rotation = initRot;
        }


        void OnClickLeftLongPressLoop()
        {
            if (targetTrf == null) return;
            targetTrf.Rotate(Vector3.up, Time.deltaTime * 20);
        }
        void OnClickLeftShortPressUp()
        {
            if (targetTrf == null) return;
            targetTrf.Rotate(Vector3.up, 15);
        }

        void OnClickRightLongPressLoop()
        {
            if (targetTrf == null) return;
            targetTrf.Rotate(Vector3.down, Time.deltaTime * 20);
        }

        void OnClickRightShortPressUp()
        {
            if (targetTrf == null) return;
            targetTrf.Rotate(Vector3.down, 15);
        }
    }
}
