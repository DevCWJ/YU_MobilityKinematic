// "(ver1.1), <ver1.1> ... </ver1.1>" - Update string or lines * Строка или строки обновления 

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

//The MerkerChart3D class is intended for displaying a merker on a graph with showing the value of the function and auxiliary lines along the axes
//Класс MerkerChart3D предназначен для отображения меркера на графике с показом значения функции и вспомогательных линий по осям
//Is an addition to the Graph3D class * Является дополнением к классу Graph3D
public class MerkerChart3D : MonoBehaviour
{    
    private GameObject pMerker; //Prefab merker
    private RectTransform merkersParentPath;    //Parent for merkers in the project hierarchy * Родитель для меркеров в иерархии проекта
    private Graph3D axesParent;                 //Parent (Graph3D panel) * Родитель (панель графика)
    private Graph3D.tParamXYZ dataFunc;         //Graph parameters * Параметры графика
    private List<Merker> listMerkers;           //List of merkers with unique names * Список меркеров с уникальными именами
    
    public MerkerChart3D(GameObject prefab, RectTransform path, Graph3D axes, Graph3D.tParamXYZ dataFunc)
    {
        pMerker = prefab;
        merkersParentPath = path;
        axesParent = axes;
        this.dataFunc = dataFunc;   
    }

    //Method for creating mutually perpendicular function curves passing through the Mercker point
    //Метод создания взаимноперпендикулярных кривых функции, проходящих через точку меркера
    private void CurvesFuncCreate(string name, bool arg1 = true, bool arg2 = true)
    {        
        if (listMerkers == null) return;
        int index = listMerkers.FindIndex(m => m.name == name);
        if (index >= 0)
        {
            Graph3D.TypeGraph typeConstArg1, typeConstArg2;
            float ConstArg1, ConstArg2;

            switch (dataFunc.typeFunc)
                {
                    case Graph3D.TypeFunc.X_YZ:
                        typeConstArg1 = Graph3D.TypeGraph.Curve_Yconst;
                        typeConstArg2 = Graph3D.TypeGraph.Curve_Zconst;
                        ConstArg1 = listMerkers[index].point.pos.y;
                        ConstArg2 = listMerkers[index].point.pos.z;
                        break;
                    case Graph3D.TypeFunc.X_ZY:
                        typeConstArg1 = Graph3D.TypeGraph.Curve_Zconst;
                        typeConstArg2 = Graph3D.TypeGraph.Curve_Yconst;
                        ConstArg1 = listMerkers[index].point.pos.z;
                        ConstArg2 = listMerkers[index].point.pos.y;
                        break;
                    case Graph3D.TypeFunc.Y_ZX:
                        typeConstArg1 = Graph3D.TypeGraph.Curve_Zconst;
                        typeConstArg2 = Graph3D.TypeGraph.Curve_Xconst;
                        ConstArg1 = listMerkers[index].point.pos.z;
                        ConstArg2 = listMerkers[index].point.pos.x;
                        break;
                    case Graph3D.TypeFunc.Y_XZ:
                        typeConstArg1 = Graph3D.TypeGraph.Curve_Xconst;
                        typeConstArg2 = Graph3D.TypeGraph.Curve_Zconst;
                        ConstArg1 = listMerkers[index].point.pos.x;
                        ConstArg2 = listMerkers[index].point.pos.z;
                        break;
                    case Graph3D.TypeFunc.Z_XY:
                        typeConstArg1 = Graph3D.TypeGraph.Curve_Xconst;
                        typeConstArg2 = Graph3D.TypeGraph.Curve_Yconst;
                        ConstArg1 = listMerkers[index].point.pos.x;
                        ConstArg2 = listMerkers[index].point.pos.y;
                        break;
                    case Graph3D.TypeFunc.Z_YX:
                        typeConstArg1 = Graph3D.TypeGraph.Curve_Yconst;
                        typeConstArg2 = Graph3D.TypeGraph.Curve_Xconst;
                        ConstArg1 = listMerkers[index].point.pos.y;
                        ConstArg2 = listMerkers[index].point.pos.x;
                        break;
                    default:
                    typeConstArg2 = Graph3D.TypeGraph.Curve_Xconst;
                    typeConstArg1 = Graph3D.TypeGraph.Curve_Zconst;
                    ConstArg1 = listMerkers[index].point.pos.x;
                    ConstArg2 = listMerkers[index].point.pos.z;
                    break;
            }

            Merker carrMerker = listMerkers[index];
            carrMerker.curveArg1 = new Graph3D.Curve(axesParent);
            carrMerker.curveArg2 = new Graph3D.Curve(axesParent);            
            listMerkers[index] = carrMerker;

            listMerkers[index].curveArg1.data = dataFunc;
            listMerkers[index].curveArg1.data.typeGraph = typeConstArg1;
            listMerkers[index].curveArg1.data.X.argConst = ConstArg1;
            listMerkers[index].curveArg1.data.Y.argConst = ConstArg1;
            listMerkers[index].curveArg1.data.Z.argConst = ConstArg1;
            listMerkers[index].curveArg1.DataSet();

            listMerkers[index].curveArg2.data = dataFunc;
            listMerkers[index].curveArg2.data.typeGraph = typeConstArg2;
            listMerkers[index].curveArg2.data.X.argConst = ConstArg2;
            listMerkers[index].curveArg2.data.Y.argConst = ConstArg2;
            listMerkers[index].curveArg2.data.Z.argConst = ConstArg2;
            listMerkers[index].curveArg2.DataSet();


            //Moving Merker Curve Objects to the Curves Parent of a Merker Object * Перемещение объектов кривых меркера в родителя Curves объекта Merker
            foreach (Graph3D.tCurve curve in listMerkers[index].curveArg1.listCurves)
            {
                curve.objLine.transform.SetParent(listMerkers[index].obj.Find("Curves"));
            }
            foreach (Graph3D.tCurve curve in listMerkers[index].curveArg2.listCurves)
            {
                curve.objLine.transform.SetParent(listMerkers[index].obj.Find("Curves"));
            }
        }
    }

    //Add a new merker with a name to the list of merkers * Добавить новый меркер с именем в список меркеров
    private void Add(string name)
    {
        if (listMerkers == null) listMerkers = new List<Merker>(); 
        if (!listMerkers.Any(n => n.name == name))
        {
            Merker merker = new Merker(Instantiate(pMerker, merkersParentPath).transform, name);
            merker.obj.gameObject.SetActive(false);
            listMerkers.Add(merker);
        }
        else
        {
            Debug.Log("Меркер с именем " + name + " уже существует");
            Debug.Log("Merker with the name " + name + " already exists");
        }
    }

    //Create a named merker for two function arguments (for TypeData = Function)s * Создать именованный меркер по двум аргументам функции (для TypeData = Function)
    public void Set(string name, float arg1, float arg2, int dec, string text = "")  //"dec" - number of simbols after comma * количество знаков после запятой
    {
        if (dataFunc.typeData == Graph3D.TypeData.Function) //(ver1.1)
        {
            if (dataFunc.func != null)
            {
                float func = dataFunc.func(arg1, arg2);
                SetBase(name, arg1, arg2, func, dec, text);
            }
        }
        else Debug.Log("This method is only for typeData == Graph3D.TypeData.Function * Этот метод только для typeData == Graph3D.TypeData.Function");  //(ver1.1)
    }

    //(ver1.1) Create a named marker by full coordinate  (TypeData = { Function or Array }) * Создать именованный меркер по полной координате (TypeData = { Function или Array })
    public void Set(string name, float arg1, float arg2, float func, int dec, string text = "")  //<ver1.1>    
    {
        SetBase(name, arg1, arg2, func, dec, text);
    }   //</ver1.1> 

    private void SetBase(string name, float arg1, float arg2, float func, int dec, string text = "")   //<ver1.1> Not changed
    {        
            if (dec > 7) dec = 7;
            float factor = Mathf.Pow(10f, dec); //Required to set the number of decimal places * Необходим для задания количества знаков после запятой
            Add(name);    //Add a merker if it does not exist * Добавит меркер если он не существует    
            int index = listMerkers.FindIndex(m => m.name == name);
            if (index >= 0)
            {
                Vector3 point = new Vector3();
                Vector3 pointScreen = new Vector3();
                float arg1_screen, arg2_screen, func_screen;

                switch (dataFunc.typeFunc)
                {
                    case Graph3D.TypeFunc.X_YZ:
                        arg1_screen = dataFunc.Y.scaleFunc.Convert(arg1);
                        arg2_screen = dataFunc.Z.scaleFunc.Convert(arg2);
                        func_screen = dataFunc.X.scaleFunc.Convert(func);
                        point.Set(func, arg1, arg2);
                        pointScreen.Set(func_screen, arg1_screen, arg2_screen);

                        if ((func_screen - dataFunc.X.scaleAxe.LScreen) * (func_screen - dataFunc.X.scaleAxe.HScreen) > 0)
                        {
                            listMerkers[index].obj_Point.gameObject.SetActive(false);
                            listMerkers[index].X.SetPosition(0, new Vector3(-func_screen, 0, 0));
                        }
                        else
                        {
                            listMerkers[index].obj_Point.gameObject.SetActive(true);
                            listMerkers[index].X.SetPosition(0, new Vector3(0, 0, 0));
                        }

                        listMerkers[index].X.transform.localPosition = new Vector3(0, 0, 0);
                        listMerkers[index].X.SetPosition(1, new Vector3(dataFunc.X.scaleAxe.HScreen - func_screen, 0, 0));
                        listMerkers[index].X_Label.color = axesParent.AxeX_label.color;
                        listMerkers[index].X_Label.SetText(((Mathf.Round((func * factor))) / factor).ToString() + " " +  text);
                        listMerkers[index].X_Label.transform.localPosition = new Vector3(dataFunc.X.scaleAxe.HScreen - func_screen, 0, 0);

                        listMerkers[index].Z.transform.localPosition = new Vector3(dataFunc.X.scaleAxe.HScreen - func_screen, 0, -arg2_screen);
                        listMerkers[index].Z.SetPosition(1, new Vector3(0, 0, dataFunc.Z.scaleAxe.HScreen));
                        listMerkers[index].Z_Label.color = axesParent.AxeY_label.color;
                        listMerkers[index].Z_Label.SetText(((Mathf.Round((arg1 * factor))) / factor).ToString());
                        listMerkers[index].Z_Label.transform.localPosition = new Vector3(0, 0, dataFunc.Z.scaleAxe.HScreen - arg1);

                        listMerkers[index].Y.transform.localPosition = new Vector3(dataFunc.X.scaleAxe.HScreen - func_screen, -arg1_screen, 0);
                        listMerkers[index].Y.SetPosition(1, new Vector3(0, dataFunc.Y.scaleAxe.HScreen, 0));
                        listMerkers[index].Y_Label.color = axesParent.AxeZ_label.color;
                        listMerkers[index].Y_Label.SetText(((Mathf.Round((arg2 * factor))) / factor).ToString());
                        listMerkers[index].Y_Label.transform.localPosition = new Vector3(0, dataFunc.Y.scaleAxe.HScreen - arg2, 0);


                        break;
                    case Graph3D.TypeFunc.X_ZY:
                        arg1_screen = dataFunc.Z.scaleFunc.Convert(arg1);
                        arg2_screen = dataFunc.Y.scaleFunc.Convert(arg2);
                        func_screen = dataFunc.X.scaleFunc.Convert(func);
                        point.Set(func, arg2, arg1);
                        pointScreen.Set(func_screen, arg2_screen, arg1_screen);

                        if ((func_screen - dataFunc.X.scaleAxe.LScreen) * (func_screen - dataFunc.X.scaleAxe.HScreen) > 0)
                        {
                            listMerkers[index].obj_Point.gameObject.SetActive(false);
                            listMerkers[index].X.SetPosition(0, new Vector3(-func_screen, 0, 0));
                        }
                        else
                        {
                            listMerkers[index].obj_Point.gameObject.SetActive(true);
                            listMerkers[index].X.SetPosition(0, new Vector3(0, 0, 0));
                        }

                        listMerkers[index].X.transform.localPosition = new Vector3(0, 0, 0);
                        listMerkers[index].X.SetPosition(1, new Vector3(dataFunc.X.scaleAxe.HScreen - func_screen, 0, 0));
                        listMerkers[index].X_Label.color = axesParent.AxeX_label.color;
                        listMerkers[index].X_Label.SetText(((Mathf.Round(func * factor)) / factor).ToString() + " " + text);
                        listMerkers[index].X_Label.transform.localPosition = new Vector3(dataFunc.X.scaleAxe.HScreen - func_screen, 0, 0);

                        listMerkers[index].Z.transform.localPosition = new Vector3(dataFunc.X.scaleAxe.HScreen - func_screen, 0, -arg1_screen);
                        listMerkers[index].Z.SetPosition(1, new Vector3(0, 0, dataFunc.Z.scaleAxe.HScreen));
                        listMerkers[index].Z_Label.color = axesParent.AxeY_label.color;
                        listMerkers[index].Z_Label.SetText(((Mathf.Round((arg2 * factor))) / factor).ToString());
                        listMerkers[index].Z_Label.transform.localPosition = new Vector3(0, 0, dataFunc.Z.scaleAxe.HScreen - arg2);

                        listMerkers[index].Y.transform.localPosition = new Vector3(dataFunc.X.scaleAxe.HScreen - func_screen, -arg2_screen, 0);
                        listMerkers[index].Y.SetPosition(1, new Vector3(0, dataFunc.Y.scaleAxe.HScreen, 0));
                        listMerkers[index].Y_Label.color = axesParent.AxeZ_label.color;
                        listMerkers[index].Y_Label.SetText(((Mathf.Round((arg1 * factor))) / factor).ToString());
                        listMerkers[index].Y_Label.transform.localPosition = new Vector3(0, dataFunc.Y.scaleAxe.HScreen - arg1, 0);
                        break;
                    case Graph3D.TypeFunc.Y_ZX:
                        arg1_screen = dataFunc.Z.scaleFunc.Convert(arg1);
                        arg2_screen = dataFunc.X.scaleFunc.Convert(arg2);
                        func_screen = dataFunc.Y.scaleFunc.Convert(func);
                        point.Set(arg2, func, arg1);
                        pointScreen.Set(arg2_screen, func_screen, arg1_screen);

                        if ((func_screen - dataFunc.Y.scaleAxe.LScreen) * (func_screen - dataFunc.Y.scaleAxe.HScreen) > 0)
                        {
                            listMerkers[index].obj_Point.gameObject.SetActive(false);
                            listMerkers[index].Y.SetPosition(0, new Vector3(0, -func_screen, 0));
                        }
                        else
                        {
                            listMerkers[index].obj_Point.gameObject.SetActive(true);
                            listMerkers[index].Y.SetPosition(0, new Vector3(0, 0, 0));
                        }

                        listMerkers[index].Y.transform.localPosition = new Vector3(0, 0, 0);
                        listMerkers[index].Y.SetPosition(1, new Vector3(0, dataFunc.Y.scaleAxe.HScreen - func_screen, 0));
                        listMerkers[index].Y_Label.color = axesParent.AxeY_label.color;
                        listMerkers[index].Y_Label.SetText(((Mathf.Round((func * factor))) / factor).ToString() + " " + text);
                        listMerkers[index].Y_Label.transform.localPosition = new Vector3(0, dataFunc.Y.scaleAxe.HScreen - func_screen, 0);

                        listMerkers[index].X.transform.localPosition = new Vector3(-arg2_screen, dataFunc.Y.scaleAxe.HScreen - func_screen, 0);
                        listMerkers[index].X.SetPosition(1, new Vector3(dataFunc.X.scaleAxe.HScreen, 0, 0));
                        listMerkers[index].X_Label.color = axesParent.AxeZ_label.color;
                        listMerkers[index].X_Label.SetText(((Mathf.Round((arg1 * factor))) / factor).ToString());
                        listMerkers[index].X_Label.transform.localPosition = new Vector3(dataFunc.X.scaleAxe.HScreen - arg1, 0, 0);

                        listMerkers[index].Z.transform.localPosition = new Vector3(0, dataFunc.Y.scaleAxe.HScreen - func_screen, -arg1_screen);
                        listMerkers[index].Z.SetPosition(1, new Vector3(0, 0, dataFunc.Z.scaleAxe.HScreen));
                        listMerkers[index].Z_Label.color = axesParent.AxeX_label.color;
                        listMerkers[index].Z_Label.SetText(((Mathf.Round((arg2 * factor))) / factor).ToString());
                        listMerkers[index].Z_Label.transform.localPosition = new Vector3(0, 0, dataFunc.Z.scaleAxe.HScreen - arg2);
                        break;
                    case Graph3D.TypeFunc.Y_XZ:
                        arg1_screen = dataFunc.X.scaleFunc.Convert(arg1);
                        arg2_screen = dataFunc.Z.scaleFunc.Convert(arg2);
                        func_screen = dataFunc.Y.scaleFunc.Convert(func);
                        point.Set(arg1, func, arg2);
                        pointScreen.Set(arg1_screen, func_screen, arg2_screen);

                        if ((func_screen - dataFunc.Y.scaleAxe.LScreen) * (func_screen - dataFunc.Y.scaleAxe.HScreen) > 0)
                        {
                            listMerkers[index].obj_Point.gameObject.SetActive(false);
                            listMerkers[index].Y.SetPosition(0, new Vector3(0, -func_screen, 0));
                        }
                        else
                        {
                            listMerkers[index].obj_Point.gameObject.SetActive(true);
                            listMerkers[index].Y.SetPosition(0, new Vector3(0, 0, 0));
                        }

                        listMerkers[index].Y.SetPosition(1, new Vector3(0, dataFunc.Y.scaleAxe.HScreen - func_screen, 0));
                        listMerkers[index].Y_Label.color = axesParent.AxeY_label.color;
                        listMerkers[index].Y_Label.SetText(((Mathf.Round((func * factor))) / factor).ToString() + " " + text);
                        listMerkers[index].Y_Label.transform.localPosition = new Vector3(0, dataFunc.Y.scaleAxe.HScreen - func_screen, 0);

                        listMerkers[index].X.transform.localPosition = new Vector3(-arg1_screen, dataFunc.Y.scaleAxe.HScreen - func_screen, 0);
                        listMerkers[index].X.SetPosition(1, new Vector3(dataFunc.X.scaleAxe.HScreen, 0, 0));
                        listMerkers[index].X_Label.color = axesParent.AxeZ_label.color;
                        listMerkers[index].X_Label.SetText(((Mathf.Round((arg2 * factor))) / factor).ToString());
                        listMerkers[index].X_Label.transform.localPosition = new Vector3(dataFunc.X.scaleAxe.HScreen - arg2, 0, 0);

                        listMerkers[index].Z.transform.localPosition = new Vector3(0, dataFunc.Y.scaleAxe.HScreen - func_screen, -arg2_screen);
                        listMerkers[index].Z.SetPosition(1, new Vector3(0, 0, dataFunc.Z.scaleAxe.HScreen));
                        listMerkers[index].Z_Label.color = axesParent.AxeX_label.color;
                        listMerkers[index].Z_Label.SetText(((Mathf.Round((arg1 * factor))) / factor).ToString());
                        listMerkers[index].Z_Label.transform.localPosition = new Vector3(0, 0, dataFunc.Z.scaleAxe.HScreen - arg1);
                        break;
                    case Graph3D.TypeFunc.Z_XY:
                        arg1_screen = dataFunc.X.scaleFunc.Convert(arg1);
                        arg2_screen = dataFunc.Y.scaleFunc.Convert(arg2);
                        func_screen = dataFunc.Z.scaleFunc.Convert(func);
                        point.Set(arg1, arg2, func);
                        pointScreen.Set(arg1_screen, arg2_screen, func_screen);

                        if ((func_screen - dataFunc.Z.scaleAxe.LScreen) * (func_screen - dataFunc.Z.scaleAxe.HScreen) > 0)
                        {
                            listMerkers[index].obj_Point.gameObject.SetActive(false);
                            listMerkers[index].Z.SetPosition(0, new Vector3(0, 0, -func_screen));
                        }
                        else
                        {
                            listMerkers[index].obj_Point.gameObject.SetActive(true);
                            listMerkers[index].Z.SetPosition(0, new Vector3(0, 0, 0));
                        }

                        listMerkers[index].Z.transform.localPosition = new Vector3(0, 0, 0);
                        listMerkers[index].Z.SetPosition(1, new Vector3(0, 0, dataFunc.Z.scaleAxe.HScreen - func_screen));
                        listMerkers[index].Z_Label.color = axesParent.AxeZ_label.color;
                        listMerkers[index].Z_Label.SetText(((Mathf.Round((func * factor))) / factor).ToString() + " " + text);
                        listMerkers[index].Z_Label.transform.localPosition = new Vector3(0, 0, dataFunc.Z.scaleAxe.HScreen - func_screen);

                        listMerkers[index].X.transform.localPosition = new Vector3(-arg1_screen, 0, dataFunc.Z.scaleAxe.HScreen - func_screen);
                        listMerkers[index].X.SetPosition(1, new Vector3(dataFunc.X.scaleAxe.HScreen, 0, 0));
                        listMerkers[index].X_Label.color = axesParent.AxeY_label.color;
                        listMerkers[index].X_Label.SetText(((Mathf.Round((arg2 * factor))) / factor).ToString());
                        listMerkers[index].X_Label.transform.localPosition = new Vector3(dataFunc.X.scaleAxe.HScreen - arg2, 0, 0);

                        listMerkers[index].Y.transform.localPosition = new Vector3(0, -arg2_screen, dataFunc.Z.scaleAxe.HScreen - func_screen);
                        listMerkers[index].Y.SetPosition(1, new Vector3(0, dataFunc.Y.scaleAxe.HScreen, 0));
                        listMerkers[index].Y_Label.color = axesParent.AxeX_label.color;
                        listMerkers[index].Y_Label.SetText(((Mathf.Round((arg1 * factor))) / factor).ToString());
                        listMerkers[index].Y_Label.transform.localPosition = new Vector3(0, dataFunc.Y.scaleAxe.HScreen - arg1, 0);
                        break;
                    case Graph3D.TypeFunc.Z_YX:
                        arg1_screen = dataFunc.Y.scaleFunc.Convert(arg1);
                        arg2_screen = dataFunc.X.scaleFunc.Convert(arg2);
                        func_screen = dataFunc.Z.scaleFunc.Convert(func);
                        point.Set(arg2, arg1, func);
                        pointScreen.Set(arg2_screen, arg1_screen, func_screen);

                        if ((func_screen - dataFunc.Z.scaleAxe.LScreen) * (func_screen - dataFunc.Z.scaleAxe.HScreen) > 0)
                        {
                            listMerkers[index].obj_Point.gameObject.SetActive(false);
                            listMerkers[index].Z.SetPosition(0, new Vector3(0, 0, -func_screen));
                        }
                        else
                        {
                            listMerkers[index].obj_Point.gameObject.SetActive(true);
                            listMerkers[index].Z.SetPosition(0, new Vector3(0, 0, 0));
                        }

                        listMerkers[index].Z.transform.localPosition = new Vector3(0, 0, 0);
                        listMerkers[index].Z.SetPosition(1, new Vector3(0, 0, dataFunc.Z.scaleAxe.HScreen - func_screen));
                        listMerkers[index].Z_Label.color = axesParent.AxeZ_label.color;
                        listMerkers[index].Z_Label.SetText(((Mathf.Round((func * factor))) / factor).ToString() + " " + text);
                        listMerkers[index].Z_Label.transform.localPosition = new Vector3(0, 0, dataFunc.Z.scaleAxe.HScreen - func_screen);

                        listMerkers[index].X.transform.localPosition = new Vector3(-arg2_screen, 0, dataFunc.Z.scaleAxe.HScreen - func_screen);
                        listMerkers[index].X.SetPosition(1, new Vector3(dataFunc.X.scaleAxe.HScreen, 0, 0));
                        listMerkers[index].X_Label.color = axesParent.AxeY_label.color;
                        listMerkers[index].X_Label.SetText(((Mathf.Round((arg1 * factor))) / factor).ToString());
                        listMerkers[index].X_Label.transform.localPosition = new Vector3(dataFunc.X.scaleAxe.HScreen - arg1, 0, 0);

                        listMerkers[index].Y.transform.localPosition = new Vector3(0, -arg1_screen, dataFunc.Z.scaleAxe.HScreen - func_screen);
                        listMerkers[index].Y.SetPosition(1, new Vector3(0, dataFunc.Y.scaleAxe.HScreen, 0));
                        listMerkers[index].Y_Label.color = axesParent.AxeX_label.color;
                        listMerkers[index].Y_Label.SetText(((Mathf.Round((arg2 * factor))) / factor).ToString());
                        listMerkers[index].Y_Label.transform.localPosition = new Vector3(0, dataFunc.Y.scaleAxe.HScreen - arg2, 0);
                        break;
                    default:
                        break;

                }

                Merker carrMerker = listMerkers[index];
                carrMerker.point.pos.Set(point.x, point.y, point.z);
                carrMerker.point.posScreen.Set(pointScreen.x, pointScreen.y, pointScreen.z);
                listMerkers[index] = carrMerker;
                listMerkers[index].obj.localPosition = pointScreen;

                if (dataFunc.typeData == Graph3D.TypeData.Function) //(ver1.1)
                {
                    CurvesFuncCreate(name);
                    listMerkers[index].curveArg1.Show(Color.yellow);
                    listMerkers[index].curveArg2.Show(Color.yellow);
                }
            }
            else Debug.Log("Merker with this name does not exist * Меркер с таким именем не существует");        
    }   //</ver1.1>

    //Show/hide merker * Показать/скрыть меркер
    public void Show(string name, bool active = true, Color? color = null)   //(ver1.1)
    {
        if (listMerkers == null) return;
        int index = listMerkers.FindIndex(m => m.name == name);
        if (index >= 0)
        {
            listMerkers[index].obj.gameObject.SetActive(active);

            if (color != null)
            {
                listMerkers[index].obj_Point.GetComponent<Image>().color = (Color)color;
            }
        }
        else
        {
            Debug.Log("Меркер с именем " + name + " не существует");
            Debug.Log("Merker with the name " + name + " does not exist");
        }
    }

    //Destroy the merker from the list and remove its object * Уничтожить меркер из списка и удалить его объект
    public void Destroy(string name)
    {
        if (listMerkers == null) return;
        int index = listMerkers.FindIndex(m => m.name == name);
        if (index >= 0)
        {
            listMerkers[index].curveArg1.Destroy();
            listMerkers[index].curveArg2.Destroy();
            Destroy(listMerkers[index].obj);
            listMerkers.RemoveAt(index);
        }
        else
        {
            Debug.Log("Меркер с именем " + name + " не существует");
            Debug.Log("Merker with the name " + name + " does not exist");
        }
    }

    //Destroy all merkers from the list and delete its objects * Уничтожить все меркеры из списка и удаленить его объекты
    public void DestroyAll()
    {
        if (listMerkers == null) return;
        for (int i = 0; i < listMerkers.Count; i++)
        {
            listMerkers[i].curveArg1.Destroy();
            listMerkers[i].curveArg2.Destroy();
            Destroy(listMerkers[i].obj);
            listMerkers.RemoveAt(i);
        }       
    }

    //Assign activity to curves of the functions passing through the marker  * Назначить кривым функции, проходящих через меркер активность
    public void SetCurvesActive(string name, bool arg1 = true, bool arg2 = true)
    {
        if (listMerkers == null) return;
        int index = listMerkers.FindIndex(m => m.name == name);
        if (index >= 0)
        {
            foreach (Graph3D.tCurve curve in listMerkers[index].curveArg1.listCurves)
            {
                curve.objLine.SetActive(arg1);
            }
            foreach (Graph3D.tCurve curve in listMerkers[index].curveArg2.listCurves)
            {              
                curve.objLine.SetActive(arg2);
            }
        }
        else
        {
            Debug.Log("Меркер с именем " + name + " не существует");
            Debug.Log("Merker with the name " + name + " does not exist");
        }
    }

    //Assign activity to auxiliary lines along the axes * Назначить вспомогательным линиям по осям активность
    public void SetAxeLinesActive(string name, bool func = true, bool arg1 = true , bool arg2 = true)
    {
        if (listMerkers == null) return;
        int index = listMerkers.FindIndex(m => m.name == name);
        if (index >= 0)
        {    
            if (listMerkers[index].obj != null)
            {
                switch (dataFunc.typeFunc)
                {
                    case Graph3D.TypeFunc.X_YZ:
                        listMerkers[index].X.enabled = func;
                        listMerkers[index].Y.enabled = arg1;
                        listMerkers[index].Z.enabled = arg2;
                        break;
                    case Graph3D.TypeFunc.X_ZY:
                        listMerkers[index].X.enabled = func;
                        listMerkers[index].Y.enabled = arg2;
                        listMerkers[index].Z.enabled = arg1;
                        break;
                    case Graph3D.TypeFunc.Y_ZX:
                        listMerkers[index].X.enabled = arg2;
                        listMerkers[index].Y.enabled = func;
                        listMerkers[index].Z.enabled = arg1;
                        break;
                    case Graph3D.TypeFunc.Y_XZ:
                        listMerkers[index].X.enabled = arg1;
                        listMerkers[index].Y.enabled = func;
                        listMerkers[index].Z.enabled = arg2;
                        break;
                    case Graph3D.TypeFunc.Z_XY:
                        listMerkers[index].X.enabled = arg1;
                        listMerkers[index].Y.enabled = arg2;
                        listMerkers[index].Z.enabled = func;
                        break;
                    case Graph3D.TypeFunc.Z_YX:
                        listMerkers[index].X.enabled = arg2;
                        listMerkers[index].Y.enabled = arg1;
                        listMerkers[index].Z.enabled = func;
                        break;
                    default:
                        break;
                }               
            }

        }
        else
        {
            Debug.Log("Меркер с именем " + name + " не существует");
            Debug.Log("Merker with the name " + name + " does not exist");
        }
    }

    //Assign activity to merker value  * Назначить вывод значений меркера активность
    public void SetValuesActive(string name, bool func = true, bool arg1 = true, bool arg2 = true)
    {
        if (listMerkers == null) return;
        int index = listMerkers.FindIndex(m => m.name == name);
        if (index >= 0)
        {
            if (listMerkers[index].obj != null)
            {
                switch (dataFunc.typeFunc)
                {
                    case Graph3D.TypeFunc.X_YZ:
                        listMerkers[index].X_Label.gameObject.SetActive(func);
                        listMerkers[index].Y_Label.gameObject.SetActive(arg2);
                        listMerkers[index].Z_Label.gameObject.SetActive(arg1);
                        break;
                    case Graph3D.TypeFunc.X_ZY:
                        listMerkers[index].X_Label.gameObject.SetActive(func);
                        listMerkers[index].Y_Label.gameObject.SetActive(arg1);
                        listMerkers[index].Z_Label.gameObject.SetActive(arg2);
                        break;
                    case Graph3D.TypeFunc.Y_ZX:
                        listMerkers[index].X_Label.gameObject.SetActive(arg1);
                        listMerkers[index].Y_Label.gameObject.SetActive(func);
                        listMerkers[index].Z_Label.gameObject.SetActive(arg2);
                        break;
                    case Graph3D.TypeFunc.Y_XZ:
                        listMerkers[index].X_Label.gameObject.SetActive(arg2);
                        listMerkers[index].Y_Label.gameObject.SetActive(func);
                        listMerkers[index].Z_Label.gameObject.SetActive(arg1);
                        break;
                    case Graph3D.TypeFunc.Z_XY:
                        listMerkers[index].X_Label.gameObject.SetActive(arg2);
                        listMerkers[index].Y_Label.gameObject.SetActive(arg1);
                        listMerkers[index].Z_Label.gameObject.SetActive(func);
                        break;
                    case Graph3D.TypeFunc.Z_YX:
                        listMerkers[index].X_Label.gameObject.SetActive(arg1);
                        listMerkers[index].Y_Label.gameObject.SetActive(arg2);
                        listMerkers[index].Z_Label.gameObject.SetActive(func);
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            Debug.Log("Меркер с именем " + name + " не существует");
            Debug.Log("Merker with the name " + name + " does not exist");
        }
    }

    //Assign color to auxiliary lines of the merker * Назначить цвет вспомогательным линиям меркера
    public void SetLinesColor(string name, Color funcLine, Color arg1Line, Color arg2Line)
    {
        if (listMerkers == null) return;
        int index = listMerkers.FindIndex(m => m.name == name);
        if (index >= 0)
        {
            if (listMerkers[index].obj != null)
            {
#pragma warning disable 0618
                switch (dataFunc.typeFunc)
                {
                    case Graph3D.TypeFunc.X_YZ:
                        listMerkers[index].X.SetColors(funcLine, funcLine);
                        listMerkers[index].Y.SetColors(arg1Line, arg1Line);
                        listMerkers[index].Z.SetColors(arg2Line, arg2Line);
                        break;
                    case Graph3D.TypeFunc.X_ZY:
                        listMerkers[index].X.SetColors(funcLine, funcLine);
                        listMerkers[index].Y.SetColors(arg2Line, arg2Line);
                        listMerkers[index].Z.SetColors(arg1Line, arg1Line);
                        break;
                    case Graph3D.TypeFunc.Y_ZX:
                        listMerkers[index].X.SetColors(arg2Line, arg2Line);
                        listMerkers[index].Y.SetColors(funcLine, funcLine);
                        listMerkers[index].Z.SetColors(arg1Line, arg1Line);
                        break;
                    case Graph3D.TypeFunc.Y_XZ:
                        listMerkers[index].X.SetColors(arg1Line, arg1Line);
                        listMerkers[index].Y.SetColors(funcLine, funcLine);
                        listMerkers[index].Z.SetColors(arg2Line, arg2Line);
                        break;
                    case Graph3D.TypeFunc.Z_XY:
                        listMerkers[index].X.SetColors(arg1Line, arg1Line);
                        listMerkers[index].Y.SetColors(arg2Line, arg2Line);
                        listMerkers[index].Z.SetColors(funcLine, funcLine);
                        break;
                    case Graph3D.TypeFunc.Z_YX:
                        listMerkers[index].X.SetColors(arg2Line, arg2Line);
                        listMerkers[index].Y.SetColors(arg1Line, arg1Line);
                        listMerkers[index].Z.SetColors(funcLine, funcLine);
                        break;
                    default:
                        break;
                }
#pragma warning restore
            }
        }
        else
        {
            Debug.Log("Меркер с именем " + name + " не существует");
            Debug.Log("Merker with the name " + name + " does not exist");
        }
    }

    //Assign color to merker values * Назначить цвет значениям меркера
    public void SetValuesColor(string name, Color funcValue, Color arg1Value, Color arg2Value)
    {
        if (listMerkers == null) return;
        int index = listMerkers.FindIndex(m => m.name == name);
        if (index >= 0)
        {
            if (listMerkers[index].obj != null)
            {
                switch (dataFunc.typeFunc)
                {
                    case Graph3D.TypeFunc.X_YZ:
                        listMerkers[index].X_Label.color = funcValue;
                        listMerkers[index].Y_Label.color = arg1Value;
                        listMerkers[index].Z_Label.color = arg2Value;
                        break;
                    case Graph3D.TypeFunc.X_ZY:
                        listMerkers[index].X_Label.color = funcValue;
                        listMerkers[index].Y_Label.color = arg2Value;
                        listMerkers[index].Z_Label.color = arg1Value;
                        break;
                    case Graph3D.TypeFunc.Y_ZX:
                        listMerkers[index].X_Label.color = arg2Value;
                        listMerkers[index].Y_Label.color = funcValue;
                        listMerkers[index].Z_Label.color = arg1Value;
                        break;
                    case Graph3D.TypeFunc.Y_XZ:
                        listMerkers[index].X_Label.color = arg1Value;
                        listMerkers[index].Y_Label.color = funcValue;
                        listMerkers[index].Z_Label.color = arg2Value;
                        break;
                    case Graph3D.TypeFunc.Z_XY:
                        listMerkers[index].X_Label.color = arg1Value;
                        listMerkers[index].Y_Label.color = arg2Value;
                        listMerkers[index].Z_Label.color = funcValue;
                        break;
                    case Graph3D.TypeFunc.Z_YX:
                        listMerkers[index].X_Label.color = arg2Value;
                        listMerkers[index].Y_Label.color = arg1Value;
                        listMerkers[index].Z_Label.color = funcValue;
                        break;
                    default:
                        break;
                }               
            }
        }
        else
        {
            Debug.Log("Меркер с именем " + name + " не существует");
            Debug.Log("Merker with the name " + name + " does not exist");
        }
    }

    //Assign the color to the mutually perpendicular curves of the Mercker function * Назначить цвет взаимоперпендикулярным кривым функции меркера
    public void SetCurvesColor(string name, Color func_arg1, Color func_arg2)
    {
        if (listMerkers == null) return;
        int index = listMerkers.FindIndex(m => m.name == name);
        if (index >= 0)
        {
#pragma warning disable
            foreach (Graph3D.tCurve curve in listMerkers[index].curveArg1.listCurves)
            {
                if (curve.lineRender != null)
                    curve.lineRender.SetColors(func_arg1, func_arg1);
            }
            foreach (Graph3D.tCurve curve in listMerkers[index].curveArg2.listCurves)
            {
                if (curve.lineRender != null) 
                    curve.lineRender.SetColors(func_arg2, func_arg2);
            }
#pragma warning restore
        }
        else
        {
            Debug.Log("Меркер с именем " + name + " не существует");
            Debug.Log("Merker with the name " + name + " does not exist");
        }
    }

    public struct Merker
    {
        public Transform obj;
        public Transform obj_Point;
        public LineRenderer X;
        public TextMeshPro X_Label;
        public LineRenderer Y;
        public TextMeshPro Y_Label;
        public LineRenderer Z;
        public TextMeshPro Z_Label;
        //public TMPro.tmu

        public string name;
        public Point point;
        public Graph3D.Curve curveArg1;
        public Graph3D.Curve curveArg2;

        public Merker(Transform obj, string name)
        {
            this.obj = obj;
            this.obj_Point = obj.Find("Point");
            this.X = obj.transform.Find("X").GetComponent<LineRenderer>();
            this.Y = obj.transform.Find("Y").GetComponent<LineRenderer>();
            this.Z = obj.transform.Find("Z").GetComponent<LineRenderer>();
            this.X_Label = X.transform.Find("Label").GetComponent<TextMeshPro>();
            this.Y_Label = Y.transform.Find("Label").GetComponent<TextMeshPro>();
            this.Z_Label = Z.transform.Find("Label").GetComponent<TextMeshPro>();

            this.name = name;
            this.point = default(Point);
            this.curveArg1 = null;
            this.curveArg2 = null;
        }
    }   

    public struct Point
    {
        public Vector3 pos;
        public Vector3 posScreen;        
    }  
}


