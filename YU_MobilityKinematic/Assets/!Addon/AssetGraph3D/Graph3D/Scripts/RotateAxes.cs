using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class RotateAxes : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{   
    public Transform AxesPivot;
    public bool rotateRunEnable = true;    //Rotate on Run mode enable/disable * Поворот врежиме Run разрешить/запретить
    [Range(0.1f, 3f)]
    public float rotateSensivity = 0.1f;
   
    public float rotateHorizont = 0f;   
    public float rotateVertical = 0f;    

    private Vector3 localRotateCarr, globalRotateCarr;
    private Vector3 startDeltaPos;  //The starting position of the control is offset from the start of pressing * Стартовая позиция контрола смещенная от начала нажатия
    private Vector3 dragVector;     //Motion vector * Вектор движения

    private float limTop;
    private float limBottom;
    private float limLeft;
    private float limRight;   

    private void Start()
    {    
        limTop = transform.GetComponent<RectTransform>().rect.yMax;
        limBottom = transform.GetComponent<RectTransform>().rect.yMin;
        limLeft = transform.GetComponent<RectTransform>().rect.xMin;
        limRight = transform.GetComponent<RectTransform>().rect.xMax;       

        rotateHorizont = 0f;
        rotateVertical = 0f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (rotateRunEnable)
        {
            //Debug.Log("OnBeginDrag(PointerEventData eventData) <=");
            Camera eventCam = eventData.pressEventCamera;
            Vector3 worldPoint = eventCam.ScreenToWorldPoint(eventData.position);
            Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
            startDeltaPos = localPoint;
            //Debug.Log("OnBeginDrag(PointerEventData eventData) =>");      
        }
    }    

    public void OnDrag(PointerEventData eventData)
    {
        if (rotateRunEnable)
        {
            //Debug.Log("OnDrag(PointerEventData eventData) <=");          
            Camera eventCam = eventData.pressEventCamera;
            Vector2 worldPoint = eventCam.ScreenToWorldPoint(eventData.position);
            Vector2 localPoint = transform.InverseTransformPoint(worldPoint);
            Vector2 dragPos = localPoint;

            if (dragPos.y > limTop) return;
            if (dragPos.y < limBottom) return;
            if (dragPos.x > limRight) return;
            if (dragPos.x < limLeft) return;

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
    }
    
    //Rotate the axes in the editor inspector * Поворот осей в инспекторе редактора
    private void OnDragOnInspector()
    {
        float deltaHor =  - rotateHorizont;
        float deltaVert = - rotateVertical;
     
        //Rotate Horizontal In !Local Coordinates * Поворачиваем по горизонтали в !локальных координатах
        if (Mathf.Abs(deltaHor) > Mathf.Abs(deltaVert))
        {            
            float rotateHor =  - deltaHor * rotateSensivity;                  
            AxesPivot.Rotate(0, rotateHor, 0, Space.Self);            
        }
        //Rotate vertically in !Global coordinates * Поворачиваем по вертикали в !глобальных координатах 
        else
        {              
            float rotateVert = deltaVert * rotateSensivity;            
            AxesPivot.Rotate(rotateVert, 0, 0, Space.World);           
        }
        rotateHorizont = 0f;
        rotateVertical = 0f;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (rotateRunEnable)
        {

        }
    }

     void OnValidate ( ) 
    {
        //Debug.Log("OnValidate ( )");        
        OnDragOnInspector();
    }

}
