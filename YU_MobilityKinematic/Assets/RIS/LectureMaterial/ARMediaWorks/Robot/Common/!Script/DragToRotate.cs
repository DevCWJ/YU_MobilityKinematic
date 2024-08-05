using UnityEngine;
using UnityEngine.EventSystems;

namespace CWJ.YU.Mobility
{
    /// <summary>
    /// 3D, 2D 둘다 회전가능
    /// </summary>
    [DisallowMultipleComponent]
    public class DragToRotate : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public Transform AxesPivot;
        [Range(1, 200f)]
        public float rotateSensivity = 100f;

        [FindObject, SerializeField, ErrorIfNull] Camera playerCamera;
        Transform cameraTrf;
        [GetComponentInParent, SerializeField, ErrorIfNull] Canvas rootCanvas;
        bool is3D;

        public void SetCamera(Camera camera)
        {
            if (camera == null) return;
            playerCamera = camera;
            cameraTrf = playerCamera.transform;
        }

        Quaternion rotBackup;
        private void Start()
        {
            SetCamera(playerCamera ?? Camera.main);
            is3D = GetComponentInChildren<Collider>(true) != null;
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
                ? mousePos.CanvasToWorldPos_ScreenSpaceRenderMode(playerCamera, rootCanvas)
                : mousePos.CanvasToWorldPos_WorldSpaceRenderMode(playerCamera, rootCanvas));
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
                Debug.DrawLine(cameraTrf.position, mousePos, Color.green, Time.deltaTime * 2);
#endif
                Vector3 posDelta = mousePos - prevMousePos;
                float rotSpeed = Time.deltaTime * rotateSensivity;
                AxesPivot.Rotate(Vector3.up, -Vector3.Dot(posDelta, cameraTrf.right) * Time.deltaTime * rotSpeed, Space.World);
                AxesPivot.Rotate(cameraTrf.right, Vector3.Dot(posDelta, cameraTrf.up) * Time.deltaTime * rotSpeed, Space.World);

                //prevMousePos = mousePos;
            }
        }

        bool isDragging = false;

        private void OnMouseDown()
        {
            if (!is3D || isDragging) return;
            isDragging = true;
            prevMousePos = GetMousePos_WorldSpace(Input.mousePosition);
#if UNITY_EDITOR
            Debug.DrawLine(cameraTrf.position, prevMousePos, Color.red, 2);
#endif
        }


        private void OnMouseUp()
        {
            if (!is3D) return;
            isDragging = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (is3D || isDragging) return;
            isDragging = true;
            prevMousePos = GetMousePos_WorldSpace(eventData.position);
#if UNITY_EDITOR
            Debug.DrawLine(cameraTrf.position, prevMousePos, Color.red, 2);
#endif
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (is3D) return;
            isDragging = false;
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