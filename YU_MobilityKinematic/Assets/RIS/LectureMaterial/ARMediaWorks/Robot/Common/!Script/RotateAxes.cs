using UnityEngine;
using UnityEngine.EventSystems;

public class RotateAxes : MonoBehaviour, IDragHandler/*, IBeginDragHandler, IEndDragHandler*/
{   
    public Transform AxesPivot;
    public bool rotateRunEnable = true;    //Rotate on Run mode enable/disable * Поворот врежиме Run разрешить/запретить
    [Range(0.1f, 3f)]
    public float rotateSensivity = 0.1f;

    private Vector3 localRotateCarr, globalRotateCarr;
    private Vector3 startDeltaPos;  //The starting position of the control is offset from the start of pressing * Стартовая позиция контрола смещенная от начала нажатия
    private Vector3 dragVector;     //Motion vector * Вектор движения

    Transform cameraTrf;
    Canvas canvas;
    bool is3D;
    private void Start()
    {
        if (Camera.main != null)
            cameraTrf = Camera.main.transform;
        canvas = GetComponentInParent<Canvas>();
        is3D = GetComponent<Collider>() != null;
    }

    private void OnEnable()
    {
        rotateRunEnable = true;
    }

    private void OnDisable()
    {
        rotateRunEnable = false;
    }

    Vector3 posDelta, prevMousePos = Vector3.zero;
    void OnMouseDrag()
    {
        if (!is3D || !rotateRunEnable)
        {
            return;
        }
        Vector3 mousePos = Input.mousePosition;
        posDelta = mousePos - prevMousePos;
        if(Vector3.Dot(AxesPivot.up, Vector3.up) >= 0)
        {
            AxesPivot.Rotate(transform.up, -Vector3.Dot(posDelta, cameraTrf.right), Space.World);
        }
        AxesPivot.Rotate(cameraTrf.right, Vector3.Dot(posDelta, cameraTrf.up), Space.World);

        prevMousePos = mousePos;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null || is3D || !rotateRunEnable)
        {
            return;
        }
        //Debug.Log("OnDrag(PointerEventData eventData) <=");          
        Camera eventCam = eventData.pressEventCamera;
        Vector2 worldPoint = eventCam.ScreenToWorldPoint(eventData.position);
        Vector2 localPoint = transform.InverseTransformPoint(worldPoint);
        Vector2 dragPos = localPoint;
        dragVector = dragPos;

        //Rotate Horizontal In !Local Coordinates * Поворачиваем по горизонтали в !локальных координатах
        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        {
            float rotateHor = -eventData.delta.x * rotateSensivity;
            AxesPivot.Rotate(0, rotateHor, 0, Space.Self);
        }
        //Rotate vertically in !Global coordinates * Поворачиваем по вертикали в !глобальных координатах 
        else
        {
            float rotateVert = eventData.delta.y * rotateSensivity;
            AxesPivot.Rotate(rotateVert, 0, 0, Space.World);
        }
    }


    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    if (rotateRunEnable)
    //    {

    //    }
    //}

}
