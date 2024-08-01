using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class pPointController : MonoBehaviour  //<ver1.1>
{
    public GameObject pin;
    public GameObject point;
    //public static pPointController pointController;

    private Image imagePoint;
    private LineRenderer lineRenderPin;
	private Color colorDef;	//Цвет точек и пина по умолчанию 
      
    private void Awake()
    {
        //pointController = transform.GetComponent<pPointController>();
        imagePoint = point.transform.GetComponent<Image>();
        lineRenderPin = pin.transform.GetComponent<LineRenderer>();
        lineRenderPin.positionCount = 2;
        lineRenderPin.useWorldSpace = false;
		colorDef =  imagePoint.color;
    }

    public void Show(Graph3D.tParamXYZ param, Vector3 pos, Color? color = null, bool active = true)
    {
        if (active)
        {
            transform.gameObject.SetActive(true);
            transform.localPosition = pos;
			
			if (color != null)
			{
				imagePoint.color = (Color)color;			
				lineRenderPin.startColor = (Color)color;
				lineRenderPin.endColor = (Color)color;
			}
			else
			{
				imagePoint.color = colorDef;			
				lineRenderPin.startColor = colorDef;
				lineRenderPin.endColor = colorDef;				
			}
           
            if (param.typeForm == Graph3D.TypeForm.Points_Pins)
            {
                pin.SetActive(true);
            }
            else
            {
                pin.SetActive(false);
            }

            Vector3[] pinPoints = new Vector3[2];
                //pinPoints[0] = pos;
                pinPoints[0].Set(0, 0, 0);

                if (param.typeFunc == Graph3D.TypeFunc.Y_XZ || param.typeFunc == Graph3D.TypeFunc.Y_ZX)	 //Если функция Y
                {
                    if (pos.x < param.X.scaleAxe.LScreen || pos.x > param.X.scaleAxe.HScreen || pos.z < param.Z.scaleAxe.LScreen || pos.z > param.Z.scaleAxe.HScreen)	//Если точка вышла за пределы аргументов
                    {
                        HideObject();                        
                    }
                    else if (pos.y > param.Y.scaleAxe.HScreen)	//Если точка выше предела функции
                    {
                        HidePoint();	//Скрыть точку
                        pinPoints[0].Set(0, param.Y.scaleAxe.HScreen - transform.localPosition.y, 0);  //Начало пина на верхнем лимите функции                     
                        pinPoints[1].Set(0, param.Y.scaleAxe.LScreen - transform.localPosition.y, 0);	//Начало пина на нижнем лимите функции 
                }
                    else if (pos.y < param.Y.scaleAxe.LScreen)	//Если точка ниже предела функции
                    {
                        HidePoint();	//Скрыть точку
                        HidePin();	//Скрыть пин
                        //pinPoints[0].Set(0, param.Y.scaleAxe.LScreen - transform.localPosition.y, 0);
                    }
                    else
                    //pinPoints[1].Set(pos.x, param.Y.scaleAxe.LScreen, pos.z);
                    pinPoints[1].Set(0, param.Y.scaleAxe.LScreen - transform.localPosition.y, 0);	//Задать пин от нижнего лимита функции до точки
                }
                else if (param.typeFunc == Graph3D.TypeFunc.X_YZ || param.typeFunc == Graph3D.TypeFunc.X_ZY)	//Если функция X
                {
                    if (pos.y < param.Y.scaleAxe.LScreen || pos.y > param.Y.scaleAxe.HScreen || pos.z < param.Z.scaleAxe.LScreen || pos.z > param.Z.scaleAxe.HScreen)
                    {
                        HideObject();
                    }
                    else if (pos.x > param.X.scaleAxe.HScreen)
                    {
                        HidePoint();
                        pinPoints[0].Set(param.X.scaleAxe.HScreen - transform.localPosition.x, 0, 0);
                        pinPoints[1].Set(param.X.scaleAxe.LScreen - transform.localPosition.x, 0, 0);
                    }
                    else if (pos.x < param.X.scaleAxe.LScreen)
                    {
                        HidePoint();
                        HidePin();
                        //pinPoints[0].Set(param.X.scaleAxe.LScreen - transform.localPosition.x, 0, 0);
                    }
                    else
                    //pinPoints[1].Set(param.X.scaleAxe.LScreen, pos.y, pos.z);                
                    pinPoints[1].Set(param.X.scaleAxe.LScreen - transform.localPosition.x, 0, 0);
                }               
                else if (param.typeFunc == Graph3D.TypeFunc.Z_XY || param.typeFunc == Graph3D.TypeFunc.Z_YX)	//Если функция Z
                {
                    if (pos.y < param.Y.scaleAxe.LScreen || pos.y > param.Y.scaleAxe.HScreen || pos.x < param.X.scaleAxe.LScreen || pos.x > param.X.scaleAxe.HScreen)
                    {
                        HideObject();
                    }
                    else if (pos.z > param.Z.scaleAxe.HScreen)
                    {
                        HidePoint();
                        pinPoints[0].Set(0, 0, param.Z.scaleAxe.HScreen - transform.localPosition.z);
                        pinPoints[1].Set(0, 0, param.Z.scaleAxe.LScreen - transform.localPosition.z);
                    }
                    else if (pos.z < param.Z.scaleAxe.LScreen)
                    {
                        HidePoint();
                        HidePin();
                        //pinPoints[0].Set(0, 0, param.Z.scaleAxe.LScreen - transform.localPosition.z);
                    }
                    else
                    //pinPoints[1].Set(pos.x, pos.y, param.Z.scaleAxe.LScreen);
                    pinPoints[1].Set(0, 0, param.Z.scaleAxe.LScreen - transform.localPosition.z);
                }              
                lineRenderPin.SetPositions(pinPoints);
           
        }
        else transform.gameObject.SetActive(false);
    }

    private void HidePoint()
    {
        point.transform.gameObject.SetActive(false);
    }

    private void HidePin()
    {
        pin.transform.gameObject.SetActive(false);
    }

    private void HideObject()
    {
        transform.gameObject.SetActive(false);
    }

}   //</ver1.1>
