using UnityEngine;
using UnityEngine.EventSystems;

namespace CWJ.YU.Mobility
{
    /// <summary>
    /// 3D, 2D 둘다 회전가능
    /// <para/>Collider, Rigidbody를 갖고있거나 Canvas안에 image를 가진채 있으면됨
    /// </summary>
    [DisallowMultipleComponent]
    public class RotateObjByDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, INeedSceneObj
    {
        public Transform AxesPivot;
        [Range(1, 20f)]
        public float rotateSensivity = 7f;

        [SerializeField, Readonly] Camera playerCam;
        Transform playerCamTrf;
        [GetComponentInParent, SerializeField] Canvas rootCanvas;
        bool hasRigidbody;

        public void SetCamera(Camera camera)
        {
            if (camera == null) return;
            playerCam = camera;
            playerCamTrf = playerCam.transform;
        }

        Quaternion rotBackup;

        private void Awake()
        {
            if (rootCanvas.gameObject.scene != gameObject.scene || rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>(true);
            if (playerCam.gameObject.scene != gameObject.scene) playerCam = null;

            SetCamera(playerCam ?? Camera.main);
            hasRigidbody = GetComponentInParent<Rigidbody>(true) != null;
            rotBackup = AxesPivot.localRotation;
        }

        public void ResetRotation()
        {
            isDragging = false;
            AxesPivot.localRotation = Quaternion.identity;
            AxesPivot.localRotation = rotBackup;
        }

        private void OnDisable()
        {
            isDragging = false;
        }

        Vector3 prevMousePos = Vector3.zero;

        Vector3 GetMousePos_WorldSpace(Vector3 mousePos)
        {
            mousePos = (rootCanvas.renderMode == RenderMode.ScreenSpaceCamera
                ? mousePos.CanvasToWorldPos_ScreenSpaceRenderMode(playerCam, rootCanvas)
                : mousePos.CanvasToWorldPos_WorldSpaceRenderMode(playerCam, rootCanvas));
            return mousePos;
        }

        //float GetRotUpAngle(Vector3 posDelta, Transform camTrf)
        //{
        //    float angle = Vector3.Dot(posDelta, camTrf.right);
        //    float dot = Vector3.Dot(AxesPivot.up, Vector3.up);
        //    if (dot >= 0) //0~90도/ (90도초과~180도가 아님)
        //        angle *= -1;
        //    return angle;
        //}

        private void Update()
        {
            if (isDragging)
            {
                Vector3 mousePos = GetMousePos_WorldSpace(Input.mousePosition);
#if UNITY_EDITOR
                Debug.DrawLine(playerCamTrf.position, mousePos, Color.green, Time.deltaTime * 2);
#endif
                Vector3 posDelta = mousePos - prevMousePos;
                float rotSpeed = Time.deltaTime * rotateSensivity * 10;
                AxesPivot.Rotate(Vector3.up, -Vector3.Dot(posDelta, playerCamTrf.right) * rotSpeed, Space.World);
                AxesPivot.Rotate(playerCamTrf.right, Vector3.Dot(posDelta, playerCamTrf.up) * rotSpeed, Space.World);

                //prevMousePos = mousePos;
            }
        }

        bool isDragging = false;

        void _OnMouseDown(Vector3 mousePos)
        {
            if (isDragging) return;
            isDragging = true;
            prevMousePos = GetMousePos_WorldSpace(mousePos);
#if UNITY_EDITOR
            Debug.DrawLine(playerCamTrf.position, prevMousePos, Color.red, 2);
#endif
        }

        void _OnMouseUp()
        {
            isDragging = false;
        }

        private void OnMouseDown()
        {
            if (!hasRigidbody) return;
            _OnMouseDown(Input.mousePosition);
        }


        private void OnMouseUp()
        {
            if (!hasRigidbody) return;
            _OnMouseUp();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (hasRigidbody) return;
            _OnMouseDown(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (hasRigidbody) return;
            _OnMouseUp();
        }

        public void OnDrag(PointerEventData eventData) { }

        //public void OnDrag(PointerEventData eventData)
        //{
        //    if (is3D)
        //    {
        //        return;
        //    }
        //    if (!isDragging)
        //    {
        //        OnBeginDrag(eventData);
        //        return;
        //    }

        //    Vector2 worldPoint = eventCam.ScreenToWorldPoint(eventData.position);
        //    Vector2 localPoint = AxesPivot.InverseTransformPoint(worldPoint);
        //    Vector2 dragPos = localPoint;
        //    dragVector = dragPos;

        //    if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        //    {
        //        float rotateHor = -eventData.delta.x * rotateSensivity;
        //        AxesPivot.Rotate(0, rotateHor, 0, Space.Self);
        //    }
        //    //Rotate vertically in !Global coordinates * Поворачиваем по вертикали в !глобальных координатах 
        //    else
        //    {
        //        float rotateVert = eventData.delta.y * rotateSensivity;
        //        AxesPivot.Rotate(rotateVert, 0, 0, Space.World);
        //    }
        //}
    }

}