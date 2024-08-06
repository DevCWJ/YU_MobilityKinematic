using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CWJ.YU.Mobility
{
    public class RotateObjByUI : CWJ.Singleton.SingletonBehaviour<RotateObjByUI>
    {
        [SerializeField, ErrorIfNull] RotateObjByDrag rotateAxes;

        [SerializeField] Button leftBtn, rightBtn;

        [SerializeField] Button initRotBtn;
        [SerializeField] Toggle holdRotToggle;
        [SerializeField] Image lockedObj, unlockedObj;

        protected override void _Awake()
        {
            leftBtn.AddLongPressLoopEvent(OnClickLeftLongPressLoop, 0.25f);
            leftBtn.AddShortPressUpEvent(OnClickLeftShortPressUp, 0.25f);
            rightBtn.AddLongPressLoopEvent(OnClickRightLongPressLoop, 0.25f);
            rightBtn.AddShortPressUpEvent(OnClickRightShortPressUp, 0.25f);


            initRotBtn.onClick.AddListener_New(OnClickInitBtn);
            holdRotToggle.onValueChanged.AddListener_New(OnToggleChanged);
            SetTarget(null);
        }

        public void SetTarget(Transform trf)
        {
            rotateAxes = trf?.GetComponentInChildren<RotateObjByDrag>(true);
            bool hasTargetTrf = rotateAxes != null && rotateAxes.AxesPivot != null;
            if (!hasTargetTrf)
                rotateAxes = null;
            
            initRotBtn.gameObject.SetActive(hasTargetTrf);
            leftBtn.gameObject.SetActive(hasTargetTrf);
            rightBtn.gameObject.SetActive(hasTargetTrf);
            holdRotToggle.gameObject.SetActive(hasTargetTrf);
            holdRotToggle.SetIsOnWithoutNotify(false);
            holdRotToggle.isOn = true;
        }

        bool isHoldRot;
        void OnToggleChanged(bool isOn)
        {
            holdRotToggle.targetGraphic = isOn ? lockedObj : unlockedObj;
            lockedObj.gameObject.SetActive(isOn);
            unlockedObj.gameObject.SetActive(!isOn);
            isHoldRot = isOn;

            if (rotateAxes == null) return;
            rotateAxes.enabled = !isHoldRot;
        }

        void OnClickInitBtn()
        {
            if (rotateAxes == null) return;
            rotateAxes.ResetRotation();
        }


        void OnClickLeftLongPressLoop()
        {
            if (rotateAxes == null) return;
            rotateAxes.AxesPivot.Rotate(Vector3.up, Time.deltaTime * 20);
        }
        void OnClickLeftShortPressUp()
        {
            if (rotateAxes== null) return;
            rotateAxes.AxesPivot.Rotate(Vector3.up, 15);
        }

        void OnClickRightLongPressLoop()
        {
            if (rotateAxes== null) return;
            rotateAxes.AxesPivot.Rotate(Vector3.down, Time.deltaTime * 20);
        }

        void OnClickRightShortPressUp()
        {
            if (rotateAxes == null) return;
            rotateAxes.AxesPivot.Rotate(Vector3.down, 15);
        }
    }
}
