// "(ver1.1), <ver1.1> ... </ver1.1>" - Update string or lines ver1.1 * Строка или строки обновления ver1.1

using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Graph3D : MonoBehaviour
{
    //--------------------------for Edotor inspector----------------------------------------
    public GameObject pPoint;   //(ver1.1) Prefab point * Префаб точки
    public GameObject pCurve;   //Prefab curve * Префаб кривой
    public GameObject pMerker;  //Prefab merker * Префаб меркера
    public RectTransform ParentCurves;  //Parent for Curves in the project hierarchy * Родитель для кривых в иерархии проекта
    public RectTransform ParentMerkers; //Parent for merkers in the project hierarchy * Родитель для меркеров в иерархии проекта
    public Transform AxeX;  //X axis object * Объект оси X
    public Transform AxeY;
    public Transform AxeZ;
    public Transform labelScale_Xmin, labelScale_Xmax;  //X-axis scale value objects minimum/maximum physical * Объекты значения шкалы оси X минимум/максимум физические
    public Transform labelScale_Ymin, labelScale_Ymax;
    public Transform labelScale_Zmin, labelScale_Zmax;

    //-------------------------------Inside-------------------------------------------------
    public tScaleAxe scaleX;  //X-scale in screen coordinates * Шкала X в координатах экрана
    public tScaleAxe scaleY;
    public tScaleAxe scaleZ;

    //--------------------------------Auxiliary---------------------------------------------
    //For autoscale * Для автомасштаба     
    private float ArgFuncMin = 3.4e38f, ArgFuncMax = -3.4e38f;  //Variables for sampling the minimum and maximum functions * Переменные для выборки минимума и максимума функции  
    private TypeGraph carrObjTypeGraph = TypeGraph.Not_assigned;   //The current type of graph to be created * Текущий тип создаваемого графика
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        InitGraph3D();
    }

    public void InitGraph3D()
    {
        scaleX.LScreen = AxeX.GetComponent<LineRenderer>().GetPosition(0).x;
        scaleX.HScreen = AxeX.GetComponent<LineRenderer>().GetPosition(1).x;
        scaleY.LScreen = AxeY.GetComponent<LineRenderer>().GetPosition(0).y;
        scaleY.HScreen = AxeY.GetComponent<LineRenderer>().GetPosition(1).y;
        scaleZ.LScreen = AxeZ.GetComponent<LineRenderer>().GetPosition(0).z;
        scaleZ.HScreen = AxeZ.GetComponent<LineRenderer>().GetPosition(1).z;
    }

    public struct tParamXYZ //Function Parameter Structure * Структура параметров графика функции
    {
        public TypeGraph typeGraph;   //Graph type (curve, array of curves, grid surface) * Вид графика (кривая, массив кривых, поверхность ввиде сетки)
        public TypeFunc typeFunc;     //Function type (Position of function and arguments relative to axes) * Тип функции (Положение функции и аргументов относительно осей)
        public TypeData typeData;     //(ver1.1) Data type (Function, Array fron CSV dable) * Тип данных (Функция, массив из таблицы CSV)  
        public TypeForm typeForm;     //(ver1.1) Display type (Lines, points, points with a pin) * Тип отображения (Линии, точки, точки с ножкой)  
        public funcXYZ func;          //Ref function * Ссылка на функцию
        public ParseCSV tableCSV;     //(ver1.1) Ссылка на таблицу CSV с данными
        public tParamAxe X;           //X axis parameters * Параметры оси X
        public tParamAxe Y;
        public tParamAxe Z;
        public bool autoScaleAxeFunc;   //Auto scale function * Автоматическая шкала функции  
        public bool autoRangeArrayArg1;   //(ver1.1) Autodetect axis limits for argument 1 * Автоопределение пределов оси аргумента 1 
        public bool autoRangeArrayArg2;   //(ver1.1) Autodetect axis limits for argument 2 * Автоопределение пределов оси аргумента 2
        public bool dataOK { private set; get; }   //Only read. Be set to 1 if Data correct * Только для чтения. Устанавливается в 1 если данные корректны
        public bool CheckCorrectDataSet()    //<ver1.1> Validating Data Fill * Проверка правильности заполнения Data
        {   // return 1 - OK
            bool result = true;           

            if (typeData == TypeData.Array)
            {
                if (tableCSV == null)
                {
                    Debug.Log("Error. No tableCSV assigned - data.tableCSV");
                    result = false;
                }
            }

            if (typeData == TypeData.Function)
            {
                if (func == null)
                {
                    Debug.Log("Error. No function assigned - data.func");
                    result = false;
                }
            }

            if (typeGraph == TypeGraph.Not_assigned)
            {
                Debug.Log("Error. No type graph assigned - data.typeGraph");
                result = false;
            }           

            if ( (typeFunc == TypeFunc.X_YZ || typeFunc == TypeFunc.X_ZY) && !(typeGraph == TypeGraph.Not_assigned) &&
                !(typeGraph == TypeGraph.Curve_Yconst || typeGraph == TypeGraph.Curve_Zconst ||
                  typeGraph == TypeGraph.YCurves || typeGraph == TypeGraph.ZCurves || typeGraph == TypeGraph.Surface) ||

                 (typeFunc == TypeFunc.Y_XZ || typeFunc == TypeFunc.Y_ZX) && !(typeGraph == TypeGraph.Not_assigned) &&
                !(typeGraph == TypeGraph.Curve_Xconst || typeGraph == TypeGraph.Curve_Zconst ||
                  typeGraph == TypeGraph.XCurves || typeGraph == TypeGraph.ZCurves || typeGraph == TypeGraph.Surface) ||

                 (typeFunc == TypeFunc.Z_XY || typeFunc == TypeFunc.Z_YX) && !(typeGraph == TypeGraph.Not_assigned) &&
                !(typeGraph == TypeGraph.Curve_Xconst || typeGraph == TypeGraph.Curve_Yconst ||
                  typeGraph == TypeGraph.XCurves || typeGraph == TypeGraph.YCurves || typeGraph == TypeGraph.Surface) )
            {
                Debug.Log("Error. data.typeFunc = " + typeFunc + " does not match data.typeGraph = " + typeGraph);
                result = false;
            }
                       
            dataOK = result;
            return result;
        }   //</ver1.1>          
    }

    public struct tScale    //Axis scale structure in physical units * Структура шкалы оси в физических единицах
    {
        public float L, H;
    }

    public struct tScaleAxe //Axis scale structure in screen units * Структура шкалы оси в единицах экрана
    {
        public float LScreen, HScreen;
    }

    public struct tParamAxe  //Axis data structure * Структура данных оси
    {
        public tScale scale;
        public tScaleAxe scaleAxe;
        public tScaleFunc scaleFunc;
        public int curves;      //The number of curves in the array (for Curves) * Количество кривых в массиве (для Curves)
        public int segments;    //Number of curve function segments(smoothness) * Количество сегментов функции кривой (плавность)
        public float argConst;  //The value of the immutable curve parameter (for Curve) * Значение неизменяемого параметра кривой (для Curve)
        public int argConstCSV;  //(ver1.1) Номер строки (Arg2) или колонки (Arg1)  аргумента кривой в массиве CSV (для Curve)
    }

    public enum TypeGraph { Not_assigned, Curve_Xconst, Curve_Yconst, Curve_Zconst, XCurves, YCurves, ZCurves, Surface };   //Type of graph * Тип графика
    //(Not assigned, Curve where one parameter is constant, Array of curves in the direction of one of the axes, Curved grid surface)  
    //(Не назначена, Кривая где один из аргуметнов - константа, Мвссив кривых в направлении одной оси, Поверхность ввиде сетки кривых) 

    public enum TypeFunc { Y_XZ, Y_ZX, X_YZ, X_ZY, Z_XY, Z_YX }  //Function type (Position of function and arguments relative to axes) * Тип функции (Положение функции и аргументов относительно осей)
                                                                 //For example Y_XZ : Y - function result, X - arg1, Z - arg2
                                                                 //Например Y_XZ : Y - результат функции, X - arg1, Z - arg2

    public enum TypeData { Function, Array }  //(ver1.1) Тип данных для графика (Function - Функция описанная формулой F=f(arg1, arg2), Array - набор данных из таблицы CSV)

    public enum TypeForm { Lines, Points, Points_Pins }  //(ver1.1) Тип отображения графика (Линии, Точки)

    public delegate float funcXYZ(float arg1, float arg2);  //Delegate of function

    public struct tNodes    //Curve nodes in physical units and screen units * Узлы кривой в физических единицах и единицах экрана
    {
        public Vector3[] point;
        public Vector3[] pointScreen;
    }

    public struct tCurve    //Curve Data Structure * Структура данных кривой
    {
        public GameObject[] objPoints;   //(ver1.1) Ссылка на массив объектов-точек кривой (для типа кривой Array) 
        public GameObject objLine;           //Ref to the prefab Curve object * Ссылка на обьект префаба кривой
        public LineRenderer lineRender;      //Ref to LineRender Curve * Ссылка на LineRender кривой
        public tNodes nodes;
    }

    //------------------------------------------- class Curve -------------------------------------------------------------------
    //This class is used to build all types of graphs (curve, array of curves, surface)
    //Данный класс используется для построения всех типов графиков (кривая, массив кривых, поверхность) 
    public class Curve
    {
        public tNodes lineNodes { protected set; get; }      //Nodes of curve (preparatory) * Узлы линии (предварительные)
        private Graph3D parent;         //Parent (Graph3D panel) * Родитель (панель графика)
        public List<tCurve> listCurves;          //A curve or a list of curves(segments if the function crosses the graph area several times) * Кривая или список кривых (отрезков, если функция несколько раз пересекает область графика)
        public tParamXYZ data;                   //Curve parameters * Параметры кривой
        public MerkerChart3D Merker;   //Class for displaying the merkers on the chart (points on the chart and values) * Класс для вывода меркеров на график (точек на графике и значений)

        public Curve(Graph3D par)
        {
            this.parent = par;
            parent.InitDataParamXYZ(ref data);
            data.X.scaleAxe = parent.scaleX;
            data.Y.scaleAxe = parent.scaleY;
            data.Z.scaleAxe = parent.scaleZ;
            //Merker = null;
        }

        public void Destroy()   //Clearing the list and deleting objects (curves) of the list * Очистка списка и удалнеие объектов (кривых) списка
        {
            try { Merker.DestroyAll(); }
            catch { };
            if (listCurves != null && listCurves.Count > 0)
            {
                foreach (tCurve curve in listCurves)
                {
                    UnityEngine.Object.Destroy(curve.objLine);
                    if (curve.objPoints != null)    //<ver1.1>
                    {
                        for (int i = 0; i < curve.objPoints.Length; i++)   
                        {
                            UnityEngine.Object.Destroy(curve.objPoints[i]);
                        }
                    }   //</ver1.1>
                }                
                listCurves.RemoveRange(0, listCurves.Count - 1);
            }
            Debug.Log("Error deleting curve * Ошибка удаления кривой");
        }

        //Assign parameters to the curve and populate listCurves * Назначить кривой параметры и заполнить listCurves
        public void DataSet()
        {
            if (!data.CheckCorrectDataSet())    //Validation of parameters * Проверка на корректность параметров
            {
                Debug.Log("Parameters of curve not valid. No curve created * Параметры кривой не корректны. Кривая не создана");
                return;
            }
            //----------------------- 
            if (parent.carrObjTypeGraph == TypeGraph.Not_assigned)            
                parent.carrObjTypeGraph = data.typeGraph;  //What graph will be built as a result * Какой график будет построен в результате
                  
            data.X.scaleFunc = CreateScaleFuncAxeX(data);
            data.Y.scaleFunc = CreateScaleFuncAxeY(data);
            data.Z.scaleFunc = CreateScaleFuncAxeZ(data);

            lineNodes = CreateCurve(data);  //Creating preliminary curve nodes * Создание предварительных узлов кривой

            //If the graph type is a curve and autoscale is on, make the curve recalculation
            //Если тип графика - кривая и включен автомасштаб - сделать перерасчет кривой
            if (parent.carrObjTypeGraph == TypeGraph.Curve_Xconst || parent.carrObjTypeGraph == TypeGraph.Curve_Yconst || parent.carrObjTypeGraph == TypeGraph.Curve_Zconst)
            {
                bool reCreate = false;
                if (data.autoScaleAxeFunc)
                {
                    parent.AutoScaleRecalc(ref data, parent.ArgFuncMin, parent.ArgFuncMax);
                    reCreate = true;
                }
                if (data.autoRangeArrayArg1 || data.autoRangeArrayArg2) //<ver1.1>
                {                    
                    Curve[] crv = new Curve[1];                 
                    crv[0].lineNodes = lineNodes;
                    parent.AutoRangeArgRecalc(ref data, crv); 
                    reCreate = true;
                }   //</ver1.1>
                if (reCreate)
                {
                    data.X.scaleFunc = CreateScaleFuncAxeX(data);
                    data.Y.scaleFunc = CreateScaleFuncAxeY(data);
                    data.Z.scaleFunc = CreateScaleFuncAxeZ(data);

                    Destroy();
                    lineNodes = CreateCurve(data);                    
                }

                Merker = new MerkerChart3D(parent.pMerker, parent.ParentMerkers, parent, data);
                parent.carrObjTypeGraph = TypeGraph.Not_assigned;   //Mandatory reset after object creation * Обязаьельный сброс после создания объекта       
            }

            if (data.typeForm == TypeForm.Lines)    //(ver1.1)
            {
                //Checking the function of the preliminary curve for the intersection of the limits.Cutting off protruding parts and dividing into segments.Place segments on a list.
                //Проверка функции предварительной кривой на пересечение пределов. Отсечение выступающих частей и разбиение на отрезки. Помещение отрезков в список.
                if (data.typeFunc == TypeFunc.Y_XZ || data.typeFunc == TypeFunc.Y_ZX)
                    listCurves = SeparationCurve(TypeFunc.Y_XZ, lineNodes, data.Y.scaleAxe.LScreen, data.Y.scaleAxe.HScreen);
                if (data.typeFunc == TypeFunc.X_YZ || data.typeFunc == TypeFunc.X_ZY)
                    listCurves = SeparationCurve(TypeFunc.X_YZ, lineNodes, data.X.scaleAxe.LScreen, data.X.scaleAxe.HScreen);
                if (data.typeFunc == TypeFunc.Z_XY || data.typeFunc == TypeFunc.Z_YX)
                    listCurves = SeparationCurve(TypeFunc.Z_XY, lineNodes, data.Z.scaleAxe.LScreen, data.Z.scaleAxe.HScreen);
            }
            else if (data.typeForm == TypeForm.Points || data.typeForm == TypeForm.Points_Pins) //<ver1.1>
            {
                listCurves = CreateObjectsOfPointsCSV(lineNodes);
            }   //</ver1.1>                
        }



        //The method of creating a curve in the form of nodes after preparing the parameters of the curve.
        //Used as an internal method, but can be used for your own purposes.
        //To create a graph, the user should not be used (it is used by the DataSet() method)
        //Метод создания кривой ввиде узлов после подготовки параметров кривой. 
        //Используется как внутренний метод, но можно использовать для своих целей.
        //Для создания графика пользователем не должен использоваться (его использует метод DataSet())


        public tNodes CreateCurve(tParamXYZ param)
        {
            tNodes nodes = new tNodes();

            if (param.dataOK)
            {
                float arg_step = 10;                    //Function Variable Argument Step * Шаг переменного аргумента функции
                float arg1_min = 0, arg1_max = 100;     //Range of Variable Argument Values * Диапазон значений переменного аргумента
                float arg2_const = 0;                   //The value of the second argument (constant) * Значение второго аргумента (неизменяемого)   


                if (param.typeData == TypeData.Function)
                {
                    //Variable Argument Definition Logic and Data Retrieval * Логика определения переменного аргумента и извлечение данных
                    //Segment pitch calculation (physical units) * Расчет шага сегмента (физические единицы)
                    //Y
                    if ((param.typeFunc == TypeFunc.X_YZ || param.typeFunc == TypeFunc.X_ZY || param.typeFunc == TypeFunc.Z_YX || param.typeFunc == TypeFunc.Z_XY) && (param.typeGraph != TypeGraph.Curve_Yconst))
                    {
                        nodes.point = new Vector3[param.Y.segments + 1];
                        nodes.pointScreen = new Vector3[param.Y.segments + 1];
                        arg_step = (param.Y.scale.H - param.Y.scale.L) / (param.Y.segments);
                        arg1_min = param.Y.scale.L;
                        arg1_max = param.Y.scale.H;
                    }
                    //Z
                    if ((param.typeFunc == TypeFunc.X_ZY || param.typeFunc == TypeFunc.X_YZ || param.typeFunc == TypeFunc.Y_ZX || param.typeFunc == TypeFunc.Y_XZ) && (param.typeGraph != TypeGraph.Curve_Zconst))
                    {
                        nodes.point = new Vector3[param.Z.segments + 1];
                        nodes.pointScreen = new Vector3[param.Z.segments + 1];
                        arg_step = (param.Z.scale.H - param.Z.scale.L) / (param.Z.segments);
                        arg1_min = param.Z.scale.L;
                        arg1_max = param.Z.scale.H;
                    }
                    //X
                    if ((param.typeFunc == TypeFunc.Z_XY || param.typeFunc == TypeFunc.Z_YX || param.typeFunc == TypeFunc.Y_XZ || param.typeFunc == TypeFunc.Y_ZX) && (param.typeGraph != TypeGraph.Curve_Xconst))
                    {
                        nodes.point = new Vector3[param.X.segments + 1];
                        nodes.pointScreen = new Vector3[param.X.segments + 1];
                        arg_step = (param.X.scale.H - param.X.scale.L) / (param.X.segments);
                        arg1_min = param.X.scale.L;
                        arg1_max = param.X.scale.H;
                    }
                    //Defining a constant argument * Определение постоянного аргумента
                    if (param.typeGraph == TypeGraph.Curve_Xconst) arg2_const = param.X.argConst;
                    if (param.typeGraph == TypeGraph.Curve_Yconst) arg2_const = param.Y.argConst;
                    if (param.typeGraph == TypeGraph.Curve_Zconst) arg2_const = param.Z.argConst;
                }

                int indexFinish = 1; //(ver1.1)                                    

                if (param.typeData == TypeData.Array)   //<ver1.1>
                {
                    indexFinish = GetMaxIndexCSV(param, TypeIndex.Elements);                      
                    nodes.point = new Vector3[0];   //The arrays will expand in the process of adding points * Массивы будут увеличиваться в процессе добавления точек
                    nodes.pointScreen = new Vector3[0];
                }	//</ver1.1>					

                int i = 0;  //Function Node Number * Номер узла функции   

                //Helper Variables * Вспомогательные переменные
                bool j_en = true;
                bool j_en_mem = true;
                bool first_point = true;    //Flag of function first point * Флаг первой точки функции
                Vector3 point_mem = new Vector3();
                Vector3 point_new = new Vector3();
                
                float arg1_carr = arg1_min;
                float arg1_carr_finish = (arg1_max + arg_step / 2);
                int indexCarr = 0;  //(ver1.1) The index of the point in the array, if the data type is Array (will be increased immediately by 1 before the first use)
                                    //Индекс точки в массиве, если тип данных - Array (будет увеличен сразу на 1 перед первым использованием) 

                //<ver1.1> Проверка условия окончания заполнения узлов кривой
                bool CondFillNodes()
                {
                    if (param.typeData == TypeData.Function)
                    {
                        if (arg1_carr <= arg1_carr_finish)
                        {
                            arg1_carr += arg_step;
                            return true;
                        }
                        else return false;
                    }
                    if (param.typeData == TypeData.Array)
                    {
                        if (indexCarr < indexFinish)
                        {
                            indexCarr++;
                            return true;
                        }
                        else return false;
                    }
                    return false;
                }   //</ver1.1>

                //Filling the curve nodes (physical units and screen units) according to the step size of the segment (TypeData.Function) 
                //Заполнение узлов кривой (физические единицы и единицы экрана) согласно величине шага сегмента (TypeData.Function)  
                //(ver1.1) Or import parsed points from a CSV table (Type Data.Array)
                //(ver1.1) Либо импорт спарсенных точек из таблицы CSV (TypeData.Array) 
                while (CondFillNodes()) //(ver1.1)
                {
                    float func = 0;     //Function value in the node * Значение функции в узле

                    if (param.typeData == TypeData.Function)    //(ver1.1)
                    {
                        switch (param.typeFunc)
                        {
                            case TypeFunc.X_YZ:
                                if (param.typeGraph == TypeGraph.Curve_Zconst)
                                {
                                    nodes.point[i].x = param.func(arg1_carr, arg2_const);
                                    nodes.point[i].y = arg1_carr;
                                    nodes.point[i].z = arg2_const;
                                    func = nodes.point[i].x;
                                }
                                if (param.typeGraph == TypeGraph.Curve_Yconst)
                                {
                                    nodes.point[i].x = param.func(arg2_const, arg1_carr);
                                    nodes.point[i].y = arg2_const;
                                    nodes.point[i].z = arg1_carr;
                                    func = nodes.point[i].x;
                                }
                                break;
                            case TypeFunc.X_ZY:
                                if (param.typeGraph == TypeGraph.Curve_Yconst)
                                {
                                    nodes.point[i].x = param.func(arg1_carr, arg2_const);
                                    nodes.point[i].y = arg2_const;
                                    nodes.point[i].z = arg1_carr;
                                    func = nodes.point[i].x;
                                }
                                if (param.typeGraph == TypeGraph.Curve_Zconst)
                                {
                                    nodes.point[i].x = param.func(arg2_const, arg1_carr);
                                    nodes.point[i].y = arg1_carr;
                                    nodes.point[i].z = arg2_const;
                                    func = nodes.point[i].x;
                                }
                                break;
                            case TypeFunc.Y_ZX:
                                if (param.typeGraph == TypeGraph.Curve_Xconst)
                                {
                                    nodes.point[i].x = arg2_const;
                                    nodes.point[i].y = param.func(arg1_carr, arg2_const);
                                    nodes.point[i].z = arg1_carr;
                                    func = nodes.point[i].y;
                                }
                                if (param.typeGraph == TypeGraph.Curve_Zconst)
                                {
                                    nodes.point[i].x = arg1_carr;
                                    nodes.point[i].y = param.func(arg2_const, arg1_carr);
                                    nodes.point[i].z = arg2_const;
                                    func = nodes.point[i].y;
                                }
                                break;
                            case TypeFunc.Y_XZ:
                                if (param.typeGraph == TypeGraph.Curve_Zconst)
                                {
                                    nodes.point[i].x = arg1_carr;
                                    nodes.point[i].y = param.func(arg1_carr, arg2_const);
                                    nodes.point[i].z = arg2_const;
                                    func = nodes.point[i].y;
                                }
                                if (param.typeGraph == TypeGraph.Curve_Xconst)
                                {
                                    nodes.point[i].x = arg2_const;
                                    nodes.point[i].y = param.func(arg2_const, arg1_carr);
                                    nodes.point[i].z = arg1_carr;
                                    func = nodes.point[i].y;
                                }
                                break;
                            case TypeFunc.Z_XY:
                                if (param.typeGraph == TypeGraph.Curve_Yconst)
                                {
                                    nodes.point[i].x = arg1_carr;
                                    nodes.point[i].y = arg2_const;
                                    nodes.point[i].z = param.func(arg1_carr, arg2_const);
                                    func = nodes.point[i].z;
                                }
                                if (param.typeGraph == TypeGraph.Curve_Xconst)
                                {
                                    nodes.point[i].x = arg2_const;
                                    nodes.point[i].y = arg1_carr;
                                    nodes.point[i].z = param.func(arg2_const, arg1_carr);
                                    func = nodes.point[i].z;
                                }
                                break;
                            case TypeFunc.Z_YX:
                                if (param.typeGraph == TypeGraph.Curve_Xconst)
                                {
                                    nodes.point[i].x = arg2_const;
                                    nodes.point[i].y = arg1_carr;
                                    nodes.point[i].z = param.func(arg1_carr, arg2_const);
                                    func = nodes.point[i].z;
                                }
                                if (param.typeGraph == TypeGraph.Curve_Yconst)
                                {
                                    nodes.point[i].x = arg1_carr;
                                    nodes.point[i].y = arg2_const;
                                    nodes.point[i].z = param.func(arg2_const, arg1_carr);
                                    func = nodes.point[i].z;
                                }
                                break;
                        }
                    }
                    else if (param.typeData == TypeData.Array)  //<ver1.1>
                    {
                        Vector3 pointTemp = new Vector3();
                        if (ImportCSVPointToNode(param, indexCarr, out func, out pointTemp))    // If the point contains the full coordinate... * Если точка содержит полную координату...
                        {                           
                            Array.Resize(ref nodes.point, nodes.point.Length + 1);  //increasing the array before adding a point * увеличиваем массив перед добавлении точки
                            Array.Resize(ref nodes.pointScreen, nodes.pointScreen.Length + 1);  //increasing the array before adding a point * увеличиваем массив перед добавлении точки
                            nodes.point[i] = pointTemp; //writing a point to a node * записываем точку в узел
                            //Debug.Log("i, nodes.point[i] = " + i + ", " + pointTemp);                      
                        }
                        else continue;  //If the array does not contain any of the parameters (x, y, z) of the point, we return to checking the condition (go to the next point)
                                        //Если массив не содержит любой из параметров (x, y, z) точки, возвращаемся к проверке условия (переходим к следующей точке)
                    }   //</ver1.1>

                    //Selection of function extrema if autoscale is used * Отбор экстремумов функции если используется автомасштаб
                    if (func > parent.ArgFuncMax) parent.ArgFuncMax = func;
                    if (func < parent.ArgFuncMin) parent.ArgFuncMin = func;

                    //Convert a curve node to screen coordinates * Преобоазование узла кривой в координаты экрана
                    float x = param.X.scaleFunc.Convert(nodes.point[i].x);
                    float y = param.Y.scaleFunc.Convert(nodes.point[i].y);
                    float z = param.Z.scaleFunc.Convert(nodes.point[i].z);

                    point_new.Set(x, y, z);

                    //The logic for determining the overflow of the scale in the parameters (to typeForm.line) * Логика определения выхода функции за границы шкалы в параметрах (для typeForm.line)
                    if (param.typeForm == TypeForm.Lines)    //(ver1.1)
                    {
                        j_en = true;
                        if (param.typeFunc == TypeFunc.X_YZ || param.typeFunc == TypeFunc.X_ZY)
                        {
                            if (param.X.scaleAxe.LScreen <= param.X.scaleAxe.HScreen)
                            {
                                if (x < param.X.scaleAxe.LScreen)
                                    j_en = false;
                                if (x > param.X.scaleAxe.HScreen)
                                    j_en = false;
                            }
                            else
                            {
                                if (x > param.X.scaleAxe.LScreen)
                                    j_en = false;
                                if (x < param.X.scaleAxe.HScreen)
                                    j_en = false;
                            }
                        }
                        if (param.typeFunc == TypeFunc.Y_XZ || param.typeFunc == TypeFunc.Y_ZX)
                        {
                            if (param.Y.scaleAxe.LScreen <= param.Y.scaleAxe.HScreen)
                            {
                                if (y < param.Y.scaleAxe.LScreen)
                                    j_en = false;
                                if (y > param.Y.scaleAxe.HScreen)
                                    j_en = false;
                            }
                            else
                            {
                                if (y > param.Y.scaleAxe.LScreen)
                                    j_en = false;
                                if (y < param.Y.scaleAxe.HScreen)
                                    j_en = false;
                            }
                        }
                        if (param.typeFunc == TypeFunc.Z_XY || param.typeFunc == TypeFunc.Z_YX)
                        {
                            if (param.Z.scaleAxe.LScreen <= param.Z.scaleAxe.HScreen)
                            {
                                if (z < param.Z.scaleAxe.LScreen)
                                    j_en = false;
                                if (z > param.Z.scaleAxe.HScreen)
                                    j_en = false;
                            }
                            else
                            {
                                if (z > param.Z.scaleAxe.LScreen)
                                    j_en = false;
                                if (z < param.Z.scaleAxe.HScreen)
                                    j_en = false;
                            }
                        }

                        //The auxiliary function of creating a node at the intersection of the function segment and one of the limits of the scale (for precise edge)
                        //Вспомогательная функция создания узла в месте пересечения сегмента функции и одной из границ шкалы (для ровного края)
                        void CreatePointOnAxe(Vector3 v_mem, Vector3 v_new, float f_mem, float f_new, float LScale, float HScale)
                        {                            
                            Vector3 vector = v_new - v_mem;
                            float K;
                            void addPointOnAxe()
                            {
                                //Debug.Log("CreatePointOnAxe() i = " + i);
                                //Debug.Log("CreatePointOnAxe() nodes.pointScreen.Lenght = " + nodes.pointScreen.Length);

                                nodes.pointScreen[i] = (vector * K + v_mem);
                                Vector3 point = new Vector3();
                                point.x = data.X.scaleFunc.UnConvert(nodes.pointScreen[i].x);
                                point.y = data.Y.scaleFunc.UnConvert(nodes.pointScreen[i].y);
                                point.z = data.Z.scaleFunc.UnConvert(nodes.pointScreen[i].z);
                                Vector3 oldPoint = nodes.point[i];  //(ver1.1)
                                nodes.point[i] = point;
                                Array.Resize(ref nodes.pointScreen, nodes.pointScreen.Length + 1);
                                Array.Resize(ref nodes.point, nodes.point.Length + 1);
                                Debug.Log(" nodes.point[i] = " + nodes.point[i]);
                                Debug.Log(" nodes.pointScreen[i] = " + nodes.pointScreen[i]);
                                nodes.point[i+1] = oldPoint; //(ver1.1)
                                i++;
                                
                            }

                            if (!j_en_mem && j_en)  //If now the function enters the scale zone * Если сейчас Функция входит в зону шкалы
                            {
                                if (Mathf.Abs(LScale - f_mem) < Mathf.Abs(HScale - f_mem))    //If function passed through LScale * Если функция прошла через LScale
                                {
                                    K = (LScale - f_mem) / (f_new - f_mem);
                                    addPointOnAxe();
                                }
                                if (Mathf.Abs(HScale - f_mem) < Mathf.Abs(LScale - f_mem))    //If function passed through HScale * Если функция прошла через HScale
                                {
                                    K = (HScale - f_mem) / (f_new - f_mem);
                                    addPointOnAxe();
                                }

                            }
                            if (j_en_mem && !j_en)  //If now the Function leaves the scale area * Если сейчас Функция выходит в зону шкалы
                            {
                                if (Mathf.Abs(LScale - f_new) < Mathf.Abs(HScale - f_new))    //If function passed through LScale * Если функция прошла через LScale
                                {
                                    K = (LScale - f_mem) / (f_new - f_mem);
                                    addPointOnAxe();
                                }
                                if (Mathf.Abs(HScale - f_new) < Mathf.Abs(LScale - f_new))    //If function passed through HScale * Если функция прошла через HScale
                                {
                                    K = (HScale - f_mem) / (f_new - f_mem);
                                    addPointOnAxe();
                                }
                            }
                            
                        }

                        if (!first_point)
                        {
                            //Create a node on the border of the scale if the function segment crossed one of the limit of the scale
                            //Cоздаем узел на границе шкалы, если сегмент функции пересек одну из границ шкалы
                            if (param.typeFunc == TypeFunc.Y_XZ || param.typeFunc == TypeFunc.Y_ZX)
                                CreatePointOnAxe(point_mem, point_new, point_mem.y, y, param.Y.scaleAxe.LScreen, param.Y.scaleAxe.HScreen);
                            if (param.typeFunc == TypeFunc.X_YZ || param.typeFunc == TypeFunc.X_ZY)
                                CreatePointOnAxe(point_mem, point_new, point_mem.x, x, param.X.scaleAxe.LScreen, param.X.scaleAxe.HScreen);
                            if (param.typeFunc == TypeFunc.Z_XY || param.typeFunc == TypeFunc.Z_YX)
                                CreatePointOnAxe(point_mem, point_new, point_mem.z, z, param.Z.scaleAxe.LScreen, param.Z.scaleAxe.HScreen);
                        }

                        j_en_mem = j_en;
                        point_mem = point_new;
                    }
                    //Fill the node with coordinates in screen units * Заполняем узел координатами в единицах экрана
                    nodes.pointScreen[i].x = x;
                    nodes.pointScreen[i].y = y;
                    nodes.pointScreen[i].z = z;

                    i++;
                    first_point = false;
                }
            }
            return nodes;
        }       

        //Checking the function of the preliminary curve for the intersection of the limits.Cutting off protruding parts and dividing into segments. Returns a list of line segments.
        //Проверка функции предварительной кривой на пересечение пределов. Отсечение выступающих частей и разбиение на отрезки. Возвращает список отрезков.
        private List<tCurve> SeparationCurve(TypeFunc typeFunc, tNodes nodes, float funcMin, float funcMax)
        {
            List<tCurve> list_Curves = new List<tCurve>();
            bool curveCreate = false;
            int j = 0;  //Segments points * Точки отрезков           

            //We pass all the nodes of the preliminary curve * Проходим все узлы предварительной кривой
            for (int i = 0; i < nodes.pointScreen.Length; i++)
            {
                float func = 0;
                if (typeFunc == TypeFunc.Y_XZ || typeFunc == TypeFunc.Y_ZX) func = nodes.pointScreen[i].y;
                if (typeFunc == TypeFunc.X_YZ || typeFunc == TypeFunc.X_ZY) func = nodes.pointScreen[i].x;
                if (typeFunc == TypeFunc.Z_XY || typeFunc == TypeFunc.Z_YX) func = nodes.pointScreen[i].z;

                if ((func - funcMin) * (func - funcMax) <= 0)  //If the point is between the limits  ((a - L) * (a - H) <= 0) * Если точка находится между пределами
                {
                    if (!curveCreate)     //If you just entered the zone - create a segment * Если только что вошли в зону - создаем отрезок
                    {
                        //First, we resize the previous segment * Сначала делаем ресайз предыдущего отрезка
                        ResizeLastCurve(list_Curves, j);

                        tCurve curve = new tCurve();
                        curve.nodes = new tNodes();
                      
                        curve.nodes.point = new Vector3[nodes.pointScreen.Length]; //(ver1.1) fixed
                        curve.nodes.pointScreen = new Vector3[nodes.pointScreen.Length];                                       
                        
                        curve.objLine = Instantiate(parent.pCurve, parent.ParentCurves);
                        curve.lineRender = curve.objLine.GetComponent<LineRenderer>();
                        curve.lineRender.useWorldSpace = false;                        

                        list_Curves.Add(curve);

                        curveCreate = true;     //Setting the segment creation flag * Устанавливаем флаг создания отрезка
                        j = 0;  //We start numbering a new segment from 0 * Начинаем нумерацию нового отрезка с 0
                    }
                    if (curveCreate)    //If a new segment is in the process of being created * Если новый отрезок в процессе создания
                    {
                        list_Curves[list_Curves.Count - 1].nodes.point[j] = nodes.point[i];
                        list_Curves[list_Curves.Count - 1].nodes.pointScreen[j] = nodes.pointScreen[i];
                        j++;
                    }
                }
                else      //If we exit the zone, reset the flag for creating a segment * Если выходим из зоны сбрасывеем флаг создания отрезка
                {
                    curveCreate = false;
                }
            }
            //Do the last segment resize * Делаем ресайз последнего отрезка
            ResizeLastCurve(list_Curves, j);
            return list_Curves;
        }

        //Trimming excess nodes of the last curve (resize node array) * Обрезка лишних узлов последней кривой (ресайз массива узлов) 
        private void ResizeLastCurve(List<tCurve> list_Curves, int j)
        {
            if (list_Curves.Count > 0)
            {
                tCurve curve_resize = new tCurve();
                curve_resize = list_Curves[list_Curves.Count - 1];
                Array.Resize(ref curve_resize.nodes.point, j);  //(ver1.1) fixed
                Array.Resize(ref curve_resize.nodes.pointScreen, j);
                list_Curves.RemoveAt(list_Curves.Count - 1);
                list_Curves.Add(curve_resize);
            }
        }

        //<ver1.1> Creating a point object for Type Data.Array * Создание объекта точки (pPoint) для TypeData.Array
        private List<tCurve> CreateObjectsOfPointsCSV(tNodes nodes) 
        {
            List<tCurve> list_Curves = new List<tCurve>();
            tCurve curve = new tCurve();
            curve.nodes = nodes;

            GameObject[] objPoints = new GameObject[nodes.pointScreen.Length];
            for (int i = 0; i < nodes.pointScreen.Length; i++)
            {
                objPoints[i] = Instantiate(parent.pPoint, parent.ParentCurves);
                objPoints[i].transform.localPosition = curve.nodes.pointScreen[i];
                objPoints[i].SetActive(false);
            }
            curve.objPoints = objPoints;
            list_Curves.Add(curve);
            return list_Curves;
        }   //</ver1.1>

        //Display the finished curve(s) from the list to the screen * Вывод готовой кривой (кривых) из списка на экран
        public void Show(Color? color = null, bool active = true)
        {
            for (int i = 0; i < listCurves.Count; i++)
            {
                if (active)
                {
                    if (data.typeData == TypeData.Array)    //<ver1.1>
                    {          
                        if (data.typeForm == TypeForm.Lines)
                        {
                            Vector3[] pointsReal = PointInsideGraph(i); //Selecting only points inside the Graph3D area * Отбор только точек внутри области Graph3D
                            listCurves[i].lineRender.enabled = true;
                            listCurves[i].lineRender.positionCount = pointsReal.Length;
                            listCurves[i].lineRender.SetPositions(pointsReal);
                            if (color != null)
                            {
                                listCurves[i].lineRender.SetColors((Color)color, (Color)color);
                            }
                        }
                        else if (data.typeForm == TypeForm.Points || data.typeForm == TypeForm.Points_Pins)
                        {
                            for (int j = 0; j < listCurves[i].objPoints.Length; j++)
                            {
                                if (listCurves[i].objPoints[j] != null)
                                {
                                    listCurves[i].objPoints[j].transform.GetComponent<pPointController>().Show(data, listCurves[i].objPoints[j].transform.localPosition, color, active);                                   
                                }
                            }
                        }
                    }   //</ver1.1>
                    else if (data.typeData == TypeData.Function)    //(ver1.1)
                    {
                        if (data.typeForm == TypeForm.Lines)    //(ver1.1)
                        {
                            listCurves[i].lineRender.enabled = true;
                            listCurves[i].lineRender.positionCount = listCurves[i].nodes.pointScreen.Length;
                            listCurves[i].lineRender.SetPositions(listCurves[i].nodes.pointScreen);
                            if (color != null)
                            {
                                listCurves[i].lineRender.SetColors((Color)color, (Color)color);
                            }
                        }
                        else if (data.typeForm == TypeForm.Points || data.typeForm == TypeForm.Points_Pins) //(ver1.1)
                        {
                            for (int j = 0; j < listCurves[i].objPoints.Length; j++)
                            {
                                if (listCurves[i].objPoints[j] != null)
                                {
                                    listCurves[i].objPoints[j].transform.GetComponent<pPointController>().Show(data, listCurves[i].objPoints[j].transform.localPosition, color, active);
                                }
                            }
                        }
                    }                   
                }
                else
                {
                    listCurves[i].lineRender.enabled = false;                   
                }
            }
            if (active)
            {
                parent.labelScale_Xmin.GetComponent<Text>().text = data.X.scale.L.ToString();
                parent.labelScale_Xmax.GetComponent<Text>().text = data.X.scale.H.ToString();
                parent.labelScale_Ymin.GetComponent<Text>().text = data.Y.scale.L.ToString();
                parent.labelScale_Ymax.GetComponent<Text>().text = data.Y.scale.H.ToString();
                parent.labelScale_Zmin.GetComponent<Text>().text = data.Z.scale.L.ToString();
                parent.labelScale_Zmax.GetComponent<Text>().text = data.Z.scale.H.ToString();
            }
        }

        //<ver1.1> Returns CSV array points inside the Graph3D area * Возвращает точки массива CSV внутри области Graph3D
        Vector3[] PointInsideGraph(int i)   
        {
            Vector3[] newPointScreen = new Vector3[0];
            int c = 0;
            for (int j = 0; j < listCurves[i].nodes.pointScreen.Length; j++)
            {
                if (listCurves[i].nodes.pointScreen[j].x >= data.X.scaleAxe.LScreen && listCurves[i].nodes.pointScreen[j].x <= data.X.scaleAxe.HScreen &&
                    listCurves[i].nodes.pointScreen[j].y >= data.Y.scaleAxe.LScreen && listCurves[i].nodes.pointScreen[j].y <= data.Y.scaleAxe.HScreen &&
                    listCurves[i].nodes.pointScreen[j].z >= data.Z.scaleAxe.LScreen && listCurves[i].nodes.pointScreen[j].z <= data.Z.scaleAxe.HScreen)
                {
                    Array.Resize(ref newPointScreen, newPointScreen.Length + 1);
                    newPointScreen[c] = listCurves[i].nodes.pointScreen[j];
                    c++;
                }
            }
            return newPointScreen;
        }   //</ver1.1>
    }

    //------------------------------------------- class Curves -------------------------------------------------------------------
    //This class is used to build an array of curves in the direction of one of the axes, as well as to build a surface (mesh)
    //Данный класс используется для построения массива кривых в направлении одной из осей, а также при построении поверхности (сетки) 
    public class Curves
    {
        public Curve[] curves;
        public tParamXYZ data;
        public Graph3D parent;         //Parent (Graph3D panel) * Родитель (панель графика)
        public MerkerChart3D Merker;   //Class for displaying the merkers on the chart (points on the chart and values) * Класс для вывода меркеров на график (точек на графике и значений)

        public Curves(Graph3D par)
        {
            this.parent = par;
            parent.InitDataParamXYZ(ref data);
            data.X.scaleAxe = parent.scaleX;
            data.Y.scaleAxe = parent.scaleY;
            data.Z.scaleAxe = parent.scaleZ;
            //Merker = null;
        }        

        //Assign parameters to the Curves and populate array of curves * Назначить массиву кривых параметры и заполнить массив кривых
        public void DataSet()
        {
            //Validation of parameters * Проверка на корректность параметров
            if (!data.CheckCorrectDataSet())
            {
                Debug.Log("No graph created");
                return;
            }
            //-----------------------
            if (parent.carrObjTypeGraph == TypeGraph.Not_assigned)    
                parent.carrObjTypeGraph = data.typeGraph;    

            data.X.scaleFunc = CreateScaleFuncAxeX(data);
            data.Y.scaleFunc = CreateScaleFuncAxeY(data);
            data.Z.scaleFunc = CreateScaleFuncAxeZ(data);

            parent.labelScale_Xmin.GetComponent<Text>().text = data.X.scale.L.ToString();
            parent.labelScale_Xmax.GetComponent<Text>().text = data.X.scale.H.ToString();
            parent.labelScale_Ymin.GetComponent<Text>().text = data.Y.scale.L.ToString();
            parent.labelScale_Ymax.GetComponent<Text>().text = data.Y.scale.H.ToString();
            parent.labelScale_Zmin.GetComponent<Text>().text = data.Z.scale.L.ToString();
            parent.labelScale_Zmax.GetComponent<Text>().text = data.Z.scale.H.ToString();

            curves = CreateCurves(data); //Creating an array of curves according to the specified parameters * Создание массива кривых по заданным параметрам

            //If the graph type is a Сurves and autoscale is on, make the array of curves recalculation.
            //Если тип графика - Carves и включен автомасштаб - сделать перерасчет массива кривых.
            if (parent.carrObjTypeGraph == TypeGraph.XCurves || parent.carrObjTypeGraph == TypeGraph.YCurves || parent.carrObjTypeGraph == TypeGraph.ZCurves)
            {
                bool reCreate = false;
                if (data.autoScaleAxeFunc)
                {
                    parent.AutoScaleRecalc(ref data, parent.ArgFuncMin, parent.ArgFuncMax);
                    reCreate = true;
                }
                if (data.autoRangeArrayArg1 || data.autoRangeArrayArg2)
                {
                    parent.AutoRangeArgRecalc(ref data, curves); //(ver1.1)    
                    reCreate = true;
                }
                if (reCreate)
                {
                    data.X.scaleFunc = CreateScaleFuncAxeX(data);
                    data.Y.scaleFunc = CreateScaleFuncAxeY(data);
                    data.Z.scaleFunc = CreateScaleFuncAxeZ(data);

                    Destroy();
                    curves = CreateCurves(data);            
                }
                
                Merker = new MerkerChart3D(parent.pMerker, parent.ParentMerkers, parent, data);
                parent.carrObjTypeGraph = TypeGraph.Not_assigned;   //Mandatory reset after object creation * Обязаьельный сброс после создания объекта           
            }           
        }

        //The method of creating a array of curves after preparing the parameters of the curves.
        //Used as an internal method, but can be used for your own purposes.
        //To create a graph, the user should not be used (it is used by the DataSet() method)
        //Метод создания массива кривых после подготовки параметров кривых. 
        //Используется как внутренний метод, но можно использовать для своих целей.
        //Для создания графика пользователем не должен использоваться (его использует метод DataSet())
        public Curve[] CreateCurves(tParamXYZ param)
        {
            Curve[] cv = new Curve[0];
            if (param.dataOK)
            {
                int сurve_carr = 0;  //Number of the created curve in the array * Номер создаваемой кривой в массиве 
                                     //We prepare the parameters for constructing the curve in the array and fill the array of curves
                                     //Готовим параметры для построения кривой в массиве и заполняем массив кривых

                if (param.typeData == TypeData.Function)    //(ver1.1)
                {
                    float cv_step = 10;     //The step between the curves (calculated) * Шаг между кривыми (расчитывается)
                    float cv_min = 0, cv_max = 100;

                    //Depending on the type of curve array, the necessary parameters are selected * В зависимости от типа массива кривых выбираются необходимые параметры
                    if (param.typeGraph == TypeGraph.XCurves)
                    {
                        cv = new Curve[param.X.curves + 1];
                        cv_step = (param.X.scale.H - param.X.scale.L) / (param.X.curves);
                        cv_min = param.X.scale.L;
                        cv_max = param.X.scale.H;
                    }
                    if (param.typeGraph == TypeGraph.YCurves)
                    {
                        cv = new Curve[param.Y.curves + 1];
                        cv_step = (param.Y.scale.H - param.Y.scale.L) / (param.Y.curves);
                        cv_min = param.Y.scale.L;
                        cv_max = param.Y.scale.H;
                    }
                    if (param.typeGraph == TypeGraph.ZCurves)
                    {
                        cv = new Curve[param.Z.curves + 1];
                        cv_step = (param.Z.scale.H - param.Z.scale.L) / (param.Z.curves);
                        cv_min = param.Z.scale.L;
                        cv_max = param.Z.scale.H;
                    }
                   
                    for (float ln_carr = cv_min; ln_carr <= (cv_max + cv_step / 2); ln_carr += cv_step)
                    {
                        cv[сurve_carr] = new Curve(parent);
                        cv[сurve_carr].data = data;

                        if (param.typeGraph == TypeGraph.XCurves)
                        {
                            cv[сurve_carr].data.X.argConst = ln_carr;
                            cv[сurve_carr].data.typeGraph = TypeGraph.Curve_Xconst;
                        }
                        if (param.typeGraph == TypeGraph.YCurves)
                        {
                            cv[сurve_carr].data.Y.argConst = ln_carr;
                            cv[сurve_carr].data.typeGraph = TypeGraph.Curve_Yconst;
                        }
                        if (param.typeGraph == TypeGraph.ZCurves)
                        {
                            cv[сurve_carr].data.Z.argConst = ln_carr;
                            cv[сurve_carr].data.typeGraph = TypeGraph.Curve_Zconst;
                        }

                        cv[сurve_carr].DataSet();
                        сurve_carr++;
                        if (сurve_carr >= cv.Length) break;
                    }
                }
                else if (param.typeData == TypeData.Array)  //<ver1.1>
                {
                    int maxIndexCurve = GetMaxIndexCSV(param, TypeIndex.Carves);
                    cv = new Curve[maxIndexCurve];  
                    for (int ln_carr = 1; ln_carr <= maxIndexCurve; ln_carr++)
                    {
                        cv[сurve_carr] = new Curve(parent);
                        cv[сurve_carr].data = data;

                        if (param.typeGraph == TypeGraph.XCurves)
                        {
                            cv[сurve_carr].data.X.argConstCSV = ln_carr;
                            cv[сurve_carr].data.typeGraph = TypeGraph.Curve_Xconst;
                        }
                        if (param.typeGraph == TypeGraph.YCurves)
                        {
                            cv[сurve_carr].data.Y.argConstCSV = ln_carr;
                            cv[сurve_carr].data.typeGraph = TypeGraph.Curve_Yconst;
                        }
                        if (param.typeGraph == TypeGraph.ZCurves)
                        {
                            cv[сurve_carr].data.Z.argConstCSV = ln_carr;
                            cv[сurve_carr].data.typeGraph = TypeGraph.Curve_Zconst;
                        }

                        cv[сurve_carr].DataSet();                        
                        if (сurve_carr >= cv.Length) break;
                        сurve_carr++;
                    }
                }   //</ver1.1>
            }
            return cv;
        }
       
    


        //Display the created array of curves on the screen * Вывод созданного массива кривых на экран 
        public void Show(Color? color = null, bool active = true)
        {
            for (int l = 0; l < curves.Length; l++)
            {
                curves[l].Show(color, active);
            }

        }

        public void Destroy()   //Clearing the list and deleting objects (curves) of the list * Очистка списка и удалнеие объектов (кривых) списка
        {
            try { Merker.DestroyAll(); }
            catch { };
            if (curves != null)
            {
                for (int l = 0; l < curves.Length; l++)
                {
                    curves[l].Destroy();
                }
            }
        }
    }

    //------------------------------------------- class Surface -------------------------------------------------------------------
    //This class is used to build a surface(mesh)
    //Данный класс используется для построения поверхности (сетки) 
    public class Surface
    {
        Surf surf;  
        public tParamXYZ data;
        public Graph3D parent;         //Parent (Graph3D panel) * Родитель (панель графика)
        public MerkerChart3D Merker;   //Class for displaying the merkers on the chart (points on the chart and values) * Класс для вывода меркеров на график (точек на графике и значений)
        public Surface(Graph3D par)
        {
            this.parent = par;
            parent.InitDataParamXYZ(ref data);
            data.X.scaleAxe = parent.scaleX;
            data.Y.scaleAxe = parent.scaleY;
            data.Z.scaleAxe = parent.scaleZ;
            //Merker = null;
        }

        private bool pointFormType; //(ver1.1)

        //Assign parameters to the Surface and populate structur Surf surf * Назначить поверхности параметры и заполнить структуру Surf surf
        public void DataSet()
        {
            //Validation of parameters * Проверка на корректность параметров
            if (!data.CheckCorrectDataSet())
            {
                Debug.Log("No graph created");
                return;
            }
            //-----------------------

            //<ver1.1> Condition flag (Array && Surface && Points*). It is necessary to optimize the construction of the surface of points. We will create by type Curves.
            //Флаг условия (Array && Surface && Points*). Необходим для оптимизации построения поверхности точек. Будем создавать по типу Curves. 
            pointFormType = (data.typeData == TypeData.Array && data.typeGraph == TypeGraph.Surface && (data.typeForm == TypeForm.Points || data.typeForm == TypeForm.Points_Pins));   //(ver1.1)
            //</ver1.1>

            if (parent.carrObjTypeGraph == TypeGraph.Not_assigned)           
                parent.carrObjTypeGraph = data.typeGraph;
               
            data.X.scaleFunc = CreateScaleFuncAxeX(data);
            data.Y.scaleFunc = CreateScaleFuncAxeY(data);
            data.Z.scaleFunc = CreateScaleFuncAxeZ(data);  

            parent.labelScale_Xmin.GetComponent<Text>().text = data.X.scale.L.ToString();            
            parent.labelScale_Xmax.GetComponent<Text>().text = data.X.scale.H.ToString();
            parent.labelScale_Ymin.GetComponent<Text>().text = data.Y.scale.L.ToString();
            parent.labelScale_Ymax.GetComponent<Text>().text = data.Y.scale.H.ToString();
            parent.labelScale_Zmin.GetComponent<Text>().text = data.Z.scale.L.ToString();
            parent.labelScale_Zmax.GetComponent<Text>().text = data.Z.scale.H.ToString();

            surf = CreateSurface(data); //Creating a surface by parameters * Создание поверхности по параметрам           

            //If the graph type is a Surface and autoscale is on, make the surface recalculation.
            //Если тип графика - Surface и включен автомасштаб - сделать перерасчет поверхности.
            if (parent.carrObjTypeGraph == TypeGraph.Surface)
            {
                bool reCreate = false;
                if (data.autoScaleAxeFunc)
                {
                    parent.AutoScaleRecalc(ref data, parent.ArgFuncMin, parent.ArgFuncMax);
                    reCreate = true;
                }
                if (data.autoRangeArrayArg1 || data.autoRangeArrayArg2)
                {
                    parent.AutoRangeArgRecalc(ref data, surf.Curves1.curves); //(ver1.1)    
                    reCreate = true;
                }
                if (reCreate)
                {
                    data.X.scaleFunc = CreateScaleFuncAxeX(data);
                    data.Y.scaleFunc = CreateScaleFuncAxeY(data);
                    data.Z.scaleFunc = CreateScaleFuncAxeZ(data);

                    Destroy();                    
                    surf = CreateSurface(data);
                }
                                
                Merker = new MerkerChart3D(parent.pMerker, parent.ParentMerkers, parent, data);
                parent.carrObjTypeGraph = TypeGraph.Not_assigned;   //Mandatory reset after object creation * Обязаьельный сброс после создания объекта            
            }            
        }

        //Method for creating a surface (grid of curves) after preparing curve parameters.
        //Used as an internal method, but can be used for your own purposes.
        //To create a graph, the user should not be used (it is used by the DataSet() method)
        //Метод создания поверхности (сетки кривых) после подготовки параметров кривых. 
        //Используется как внутренний метод, но можно использовать для своих целей.
        //Для создания графика пользователем не должен использоваться (его использует метод DataSet()) 
        public Surf CreateSurface(tParamXYZ param)
         {
            Surf surf = new Surf();
            //surf.Curves1 and surf.Curves2 are mutually perpendicular arrays of curves * surf.Curves1 и surf.Curves2 - взаимно перпендикулярные массивы кривых
            surf.Curves1 = new Curves(parent);
            if (!pointFormType) //(ver1.1)
            {
                surf.Curves2 = new Curves(parent);
            }
            //surf.isoLines = new Curves(parent);

            //Preparing parameters for creating a grid of curves * Подготовка параметров для построения сетки кривых 
            switch (param.typeFunc)
                {
                    case TypeFunc.X_YZ:
                        surf.Curves1.data = param;
                        surf.Curves1.data.typeGraph = TypeGraph.ZCurves;
                    if (pointFormType) break; //(ver1.1)                    
                        surf.Curves2.data = param;
                        surf.Curves2.data.typeGraph = TypeGraph.YCurves;                    
                        break;
                    case TypeFunc.X_ZY:
                        surf.Curves1.data = param;
                        surf.Curves1.data.typeGraph = TypeGraph.YCurves;
                    if (pointFormType) break; //(ver1.1)  
                        surf.Curves2.data = param;
                        surf.Curves2.data.typeGraph = TypeGraph.ZCurves;
                        break;
                    case TypeFunc.Y_ZX:
                        surf.Curves1.data = param;
                        surf.Curves1.data.typeGraph = TypeGraph.XCurves;
                    if (pointFormType) break; //(ver1.1)  
                        surf.Curves2.data = param;
                        surf.Curves2.data.typeGraph = TypeGraph.ZCurves;
                        break;
                    case TypeFunc.Y_XZ:
                        surf.Curves1.data = param;
                        surf.Curves1.data.typeGraph = TypeGraph.ZCurves;
                    if (pointFormType) break; //(ver1.1)  
                        surf.Curves2.data = param;
                        surf.Curves2.data.typeGraph = TypeGraph.XCurves;
                        break;
                    case TypeFunc.Z_XY:
                        surf.Curves1.data = param;
                        surf.Curves1.data.typeGraph = TypeGraph.YCurves;
                    if (pointFormType) break; //(ver1.1)  
                        surf.Curves2.data = param;
                        surf.Curves2.data.typeGraph = TypeGraph.XCurves;
                        break;
                    case TypeFunc.Z_YX:
                        surf.Curves1.data = param;
                        surf.Curves1.data.typeGraph = TypeGraph.XCurves;
                    if (pointFormType) break; //(ver1.1)  
                        surf.Curves2.data = param;
                        surf.Curves2.data.typeGraph = TypeGraph.YCurves;
                        break;
                }
                surf.Curves1.DataSet();
                if (!pointFormType) //(ver1.1)  
                surf.Curves2.DataSet();

                return surf;
        }

        //Display created arrays of curves (mutually perpendicular) on the screen * Вывод созданных массивов кривых (взаимноперпендикулярных) на экран 
        public void Show(Color? color = null, bool active = true)
        {
            surf.Curves1.Show(color, active);
            if (!pointFormType) //(ver1.1)  
                surf.Curves2.Show(color, active);
        }

        public void Destroy()   //Clearing the list and deleting objects (curves) of the list * Очистка списка и удалнеие объектов (кривых) списка
        {
            try { Merker.DestroyAll(); }
            catch { };
            if (surf.Curves1 != null) surf.Curves1.Destroy();
            if (!pointFormType && surf.Curves2 != null) //(ver1.1)  
                surf.Curves2.Destroy();
        }
    }

    //Surface Data Structure (Mesh) * Структура данных поверхности (сетки) 
    public struct Surf
    {
        public Curves Curves1;
        public Curves Curves2;
        //public Curves isoLines;
    }

    //Structure of the scale conversion function * Структура функции конвертации масштаба
    //y = K * x + B;
    public struct tScaleFunc
    {
        private float K;
        private float B;
        public float Convert(float val)
        {
           return K * val + B;
        }  
         public float UnConvert(float val) 
        {
           return (val - B) / K;
        }            

        public static tScaleFunc CreateScaleFunc(float LScale, float HScale, float LScaleNew, float HScaleNew) //Сoefficients  calculation * Расчет коэффициентов
        {
            tScaleFunc scaleFunc = new tScaleFunc();
            scaleFunc.K = (HScaleNew - LScaleNew) / (HScale - LScale);
            scaleFunc.B = -HScaleNew * LScale / (HScale - LScale);
            return scaleFunc;
        }
    }

    //Functions for creating scale conversion structures by chart parameters (conversion of physical units to screen units)
    //Функции для создания структур конвертации масштабов по параметрам графика (конвертация физических единиц в единицы экрана)
    public static tScaleFunc CreateScaleFuncAxeX(tParamXYZ param)
    {
        tScaleFunc scale = tScaleFunc.CreateScaleFunc(param.X.scale.L, param.X.scale.H, param.X.scaleAxe.LScreen, param.X.scaleAxe.HScreen);
        return scale;
    }
    public static tScaleFunc CreateScaleFuncAxeY(tParamXYZ param)
    {
        tScaleFunc scale = tScaleFunc.CreateScaleFunc(param.Y.scale.L, param.Y.scale.H, param.Y.scaleAxe.LScreen, param.Y.scaleAxe.HScreen);
        return scale;
    }
    public static tScaleFunc CreateScaleFuncAxeZ(tParamXYZ param)
    {
        tScaleFunc scale = tScaleFunc.CreateScaleFunc(param.Z.scale.L, param.Z.scale.H, param.Z.scaleAxe.LScreen, param.Z.scaleAxe.HScreen);
        return scale;
    }

    //The function of recalculating the limits of the scale of the graph function(when the autoscale is turned on)
    //Функция перерасчета лимитов шкалы функции графика (при включении автомасштаба)
    private void AutoScaleRecalc(ref tParamXYZ data, float minFunc, float maxFunc)
    {
        float scale, dL, dH;
        if (data.typeFunc == TypeFunc.X_YZ || data.typeFunc == TypeFunc.X_ZY)
        {
            scale = data.X.scale.H - data.X.scale.L;
            dL = minFunc - data.X.scale.L;
            dH = data.X.scale.H - maxFunc;

            //if (dL / scale > 0.05)
            {
                data.X.scale.L = minFunc;
            }
           // if (dH / scale > 0.05)
            {
                data.X.scale.H = maxFunc;
            }
        }
        if (data.typeFunc == TypeFunc.Z_YX || data.typeFunc == TypeFunc.Z_XY)
        {
            scale = data.Z.scale.H - data.Z.scale.L;
            dL = minFunc - data.Z.scale.L;
            dH = data.Z.scale.H - maxFunc;

            //if (dL / scale > 0.05)
            {
                data.Z.scale.L = minFunc;
            }
            //if (dH / scale > 0.05)
            {
                data.Z.scale.H = maxFunc;
            }
        }
        if (data.typeFunc == TypeFunc.Y_XZ || data.typeFunc == TypeFunc.Y_ZX)
        {
            scale = data.Y.scale.H - data.Y.scale.L;
            dL = minFunc - data.Y.scale.L;
            dH = data.Y.scale.H - maxFunc;

            //if (dL / scale > 0.05)
            {
                data.Y.scale.L = minFunc;
            }
            //if (dH / scale > 0.05)
            {
                data.Y.scale.H = maxFunc;
            }
        }       
    }

    //(ver1.1) Function for recalculating the limits of argument 1 or 2 (when auto Range Array Arg* = true)
    //(ver1.1) Функция перерасчета пределов аргумета 1 или 2 (при включении autoRangeArrayArg*)
    private void AutoRangeArgRecalc(ref tParamXYZ data, Curve[] curves)//, tNodes nodes) //<ver1.1>
    {
        if (data.typeData == TypeData.Array)
        {
            if (data.autoRangeArrayArg1)
            {
                if (data.typeFunc == TypeFunc.Y_XZ || data.typeFunc == TypeFunc.Z_XY)
                {
                    data.X.scale.L = GetMinArg1(data, curves);
                    data.X.scale.H = GetMaxArg1(data, curves);
                }
                else if (data.typeFunc == TypeFunc.Y_ZX || data.typeFunc == TypeFunc.X_ZY)
                {
                    data.Z.scale.L = GetMinArg1(data, curves);
                    data.Z.scale.H = GetMaxArg1(data, curves);
                }
                else if (data.typeFunc == TypeFunc.X_YZ || data.typeFunc == TypeFunc.Z_YX)
                {
                    data.Y.scale.L = GetMinArg1(data, curves);
                    data.Y.scale.H = GetMaxArg1(data, curves);
                }
            }
            if (data.autoRangeArrayArg2)
            {
                if (data.typeFunc == TypeFunc.Y_XZ || data.typeFunc == TypeFunc.X_YZ)
                {
                    data.Z.scale.L = GetMinArg2(data, curves);
                    data.Z.scale.H = GetMaxArg2(data, curves);
                }
                else if (data.typeFunc == TypeFunc.Y_ZX || data.typeFunc == TypeFunc.Z_YX)
                {
                    data.X.scale.L = GetMinArg2(data, curves);
                    data.X.scale.H = GetMaxArg2(data, curves);
                }
                else if (data.typeFunc == TypeFunc.X_ZY || data.typeFunc == TypeFunc.Z_XY)
                {
                    data.Y.scale.L = GetMinArg2(data, curves);
                    data.Y.scale.H = GetMaxArg2(data, curves);
                }
            }
        }
    }   //</ver1.1>

    //Initialization function of chart parameters * Функция начальной инициализации параметров графика
    private void InitDataParamXYZ(ref tParamXYZ data)
    {
        InitGraph3D();

        data.typeGraph = TypeGraph.Not_assigned;
        data.typeFunc = TypeFunc.Y_XZ;
        data.func = null;
        data.autoScaleAxeFunc = false;

        data.X.argConst = 0.0f;
        data.X.curves = 20;
        data.X.segments = 20;
        data.X.scale.L = 0.0f;
        data.X.scale.H = 100.0f;
        data.Y.argConst = 0.0f;
        data.Y.curves = 20;
        data.Y.segments = 20;
        data.Y.scale.L = 0.0f;
        data.Y.scale.H = 100.0f;
        data.Z.argConst = 0.0f;
        data.Z.curves = 20;
        data.Z.segments = 20;
        data.Z.scale.L = 0.0f;
        data.Z.scale.H = 100.0f;
    }

    //<ver1.1> Importing a point from a CSV table * Импорт точки из таблицы CSV
    public static bool ImportCSVPointToNode(tParamXYZ data, int indexCarr, out float func, out Vector3 point)   
    {
        point = new Vector3(0,0,0);
        func = 0;
       
        Debug.Log("ImportCSVData()"); 
        Vector3 result = new Vector3();

        //If the point does not contain at least one ordinate, then we send a message and then exit the parent method with the result 0
        //Если точка не содержит хоть одну ординату, то отправляем сообщение и далее выходим из метода родителя с результатом 0
        bool SendLogMess(int row, int col)
        {
            Debug.Log("Отсутствует значение в csvData(" + row + ", "+col+")");            
            return false;
        }

        if (data.tableCSV.csvFile != null)
        {
            switch (data.typeFunc)
            {
                case TypeFunc.X_YZ:

                    if (data.typeGraph == TypeGraph.Curve_Zconst)
                    {
                        int indexConst = data.Z.argConstCSV;
                        if (!float.TryParse(data.tableCSV.csvData[indexConst, indexCarr], out result.x)) return SendLogMess(indexConst, indexConst);
                        if (!float.TryParse(data.tableCSV.csvData[0, indexCarr], out result.y)) return SendLogMess(0, indexCarr);
                        if (!float.TryParse(data.tableCSV.csvData[indexConst, 0], out result.z)) return SendLogMess(indexConst, 0);
                    }
                    if (data.typeGraph == TypeGraph.Curve_Yconst)
                    {
                        int indexConst = data.Y.argConstCSV;
                        if (!float.TryParse(data.tableCSV.csvData[indexCarr, indexConst], out result.x)) return SendLogMess(indexCarr, indexConst);
                        if (!float.TryParse(data.tableCSV.csvData[0, indexConst], out result.y)) return SendLogMess(0, indexConst);
                        if (!float.TryParse(data.tableCSV.csvData[indexCarr, 0], out result.z)) return SendLogMess(indexCarr, 0);
                    }
                    func = result.x;
                    break;
                case TypeFunc.X_ZY:
                    if (data.typeGraph == TypeGraph.Curve_Yconst)
                    {
                        int indexConst = data.Y.argConstCSV;
                        if (!float.TryParse(data.tableCSV.csvData[indexConst, indexCarr], out result.x)) return SendLogMess(indexConst, indexConst);
                        if (!float.TryParse(data.tableCSV.csvData[indexConst, 0], out result.y)) return SendLogMess(indexConst, 0);
                        if (!float.TryParse(data.tableCSV.csvData[0, indexCarr], out result.z)) return SendLogMess(0, indexCarr);
                    }
                    if (data.typeGraph == TypeGraph.Curve_Zconst)
                    {
                        int indexConst = data.Z.argConstCSV;
                        if (!float.TryParse(data.tableCSV.csvData[indexCarr, indexConst], out result.x)) return SendLogMess(indexCarr, indexConst);
                        if (!float.TryParse(data.tableCSV.csvData[indexCarr, 0], out result.y)) return SendLogMess(indexCarr, 0);
                        if (!float.TryParse(data.tableCSV.csvData[0, indexConst], out result.z)) return SendLogMess(0, indexConst);
                    }
                    func = result.x;
                    break;
                case TypeFunc.Y_ZX:
                    if (data.typeGraph == TypeGraph.Curve_Xconst)
                    {
                        int indexConst = data.X.argConstCSV;
                        if (!float.TryParse(data.tableCSV.csvData[indexConst, 0], out result.x)) return SendLogMess(indexConst, 0);
                        if (!float.TryParse(data.tableCSV.csvData[indexConst, indexCarr], out result.y)) return SendLogMess(indexConst, indexConst);
                        if (!float.TryParse(data.tableCSV.csvData[0, indexCarr], out result.z)) return SendLogMess(0, indexCarr);
                    }
                    if (data.typeGraph == TypeGraph.Curve_Zconst)
                    {
                        int indexConst = data.Z.argConstCSV;
                        if (!float.TryParse(data.tableCSV.csvData[indexCarr, 0], out result.x)) return SendLogMess(indexCarr, 0);
                        if (!float.TryParse(data.tableCSV.csvData[indexCarr, indexConst], out result.y)) return SendLogMess(indexCarr, indexConst);
                        if (!float.TryParse(data.tableCSV.csvData[0, indexConst], out result.z)) return SendLogMess(0, indexConst);
                    }
                    func = result.y;
                    break;
                case TypeFunc.Y_XZ:
                    if (data.typeGraph == TypeGraph.Curve_Zconst)
                    {
                        int indexConst = data.Z.argConstCSV;
                        if (!float.TryParse(data.tableCSV.csvData[0, indexCarr], out result.x)) return SendLogMess(0, indexCarr);
                        if (!float.TryParse(data.tableCSV.csvData[indexConst, indexCarr], out result.y)) return SendLogMess(indexConst, indexConst);
                        if (!float.TryParse(data.tableCSV.csvData[indexConst, 0], out result.z)) return SendLogMess(indexConst, 0);
                    }
                    if (data.typeGraph == TypeGraph.Curve_Xconst)
                    {
                        int indexConst = data.X.argConstCSV;
                        if (!float.TryParse(data.tableCSV.csvData[0, indexConst], out result.x)) return SendLogMess(0, indexConst);
                        if (!float.TryParse(data.tableCSV.csvData[indexCarr, indexConst], out result.y)) return SendLogMess(indexCarr, indexConst);
                        if (!float.TryParse(data.tableCSV.csvData[indexCarr, 0], out result.z)) return SendLogMess(indexCarr, 0);
                    }
                    func = result.y;
                    break;
                case TypeFunc.Z_XY:
                    if (data.typeGraph == TypeGraph.Curve_Yconst)
                    {
                        int indexConst = data.Y.argConstCSV;
                        if (!float.TryParse(data.tableCSV.csvData[0, indexCarr], out result.x)) return SendLogMess(0, indexCarr);
                        if (!float.TryParse(data.tableCSV.csvData[indexConst, 0], out result.y)) return SendLogMess(indexConst, 0);
                        if (!float.TryParse(data.tableCSV.csvData[indexConst, indexCarr], out result.z)) return SendLogMess(indexConst, indexCarr);
                    }
                    if (data.typeGraph == TypeGraph.Curve_Xconst)
                    {
                        int indexConst = data.X.argConstCSV;
                        if (!float.TryParse(data.tableCSV.csvData[0, indexConst], out result.x)) return SendLogMess(0, indexConst);
                        if (!float.TryParse(data.tableCSV.csvData[indexCarr, 0], out result.y)) return SendLogMess(indexCarr, 0);
                        if (!float.TryParse(data.tableCSV.csvData[indexCarr, indexConst], out result.z)) return SendLogMess(indexCarr, indexConst);
                    }
                    func = result.z;
                    break;
                case TypeFunc.Z_YX:
                    if (data.typeGraph == TypeGraph.Curve_Xconst)
                    {
                        int indexConst = data.X.argConstCSV;
                        if (!float.TryParse(data.tableCSV.csvData[indexConst, 0], out result.x)) return SendLogMess(indexConst, 0);
                        if (!float.TryParse(data.tableCSV.csvData[0, indexCarr], out result.y)) return SendLogMess(0, indexCarr);
                        if (!float.TryParse(data.tableCSV.csvData[indexConst, indexCarr], out result.z)) return SendLogMess(indexConst, indexCarr);
                    }
                    if (data.typeGraph == TypeGraph.Curve_Yconst)
                    {
                        int indexConst = data.Y.argConstCSV;
                        if (!float.TryParse(data.tableCSV.csvData[indexCarr, 0], out result.x)) return SendLogMess(indexCarr, 0);
                        if (!float.TryParse(data.tableCSV.csvData[0, indexConst], out result.y)) return SendLogMess(0, indexConst);
                        if (!float.TryParse(data.tableCSV.csvData[indexCarr, indexConst], out result.z)) return SendLogMess(indexCarr, indexConst);
                    }
                    func = result.z;
                    break;
            }
            point = result;
            return true;
        }
        else
        {
            Debug.Log("Срипт ParseCSV не подключен , либо не указан файл CSV");
            return false;
        }       
    }   //</ver1.1>

    enum TypeIndex { Elements, Carves };    //(ver1.1) The index of the element or the index of the curve * Индекс элемент или индекс кривой.
    //<ver1.1> Returns the maximum index of the element or the maximum index of the curve from the CSV table according to the parameter settings.
    //0921 Возвращает максимальный индекс элемента или максимальный индекс кривой из таблицы CSV согласно настройкам параметров.
    private static int GetMaxIndexCSV(tParamXYZ param, TypeIndex type)	
	{
		//Определяем максимальный индекс элементов в выбранной строке (arg1) или максимальный индекс элементов в выбранной колоноке (arg2) в таблице CSV
		int maxRow = param.tableCSV.csvData.GetUpperBound(0);   //0 - первая размерность массива
        int maxCol = param.tableCSV.csvData.GetUpperBound(1);   //1 - вторая размерность массива


        if ((param.typeFunc == TypeFunc.X_YZ || param.typeFunc == TypeFunc.Y_XZ) && (param.typeGraph == TypeGraph.Curve_Zconst || param.typeGraph == TypeGraph.ZCurves) ||
			(param.typeFunc == TypeFunc.X_ZY || param.typeFunc == TypeFunc.Z_XY) && (param.typeGraph == TypeGraph.Curve_Yconst || param.typeGraph == TypeGraph.YCurves) ||
			(param.typeFunc == TypeFunc.Z_YX || param.typeFunc == TypeFunc.Y_ZX) && (param.typeGraph == TypeGraph.Curve_Xconst || param.typeGraph == TypeGraph.XCurves))
		{
			if (type == TypeIndex.Elements) return maxCol;  
			else if (type == TypeIndex.Carves) return maxRow;                      
		}
		else if
			((param.typeFunc == TypeFunc.X_ZY || param.typeFunc == TypeFunc.Y_ZX) && (param.typeGraph == TypeGraph.Curve_Zconst || param.typeGraph == TypeGraph.ZCurves) ||
			 (param.typeFunc == TypeFunc.X_YZ || param.typeFunc == TypeFunc.Z_YX) && (param.typeGraph == TypeGraph.Curve_Yconst || param.typeGraph == TypeGraph.YCurves) || 
			 (param.typeFunc == TypeFunc.Z_XY || param.typeFunc == TypeFunc.Y_XZ) && (param.typeGraph == TypeGraph.Curve_Xconst)|| param.typeGraph == TypeGraph.XCurves)
        {
			if (type == TypeIndex.Elements) return maxRow;
			else if (type == TypeIndex.Carves) return maxCol;
        }
        return 0;
	}   //</ver1.1>	

    public static float GetMinArg1(tParamXYZ param, Curve[] curves) //<ver1.1>
    {
        float minElement = GetOrdinateNode(param, curves[0].lineNodes, 0).arg1;
        for (int i = 0; i < curves.Length; i++)
        {
            for (int j = 0; j < curves[i].lineNodes.point.Length; j++)
            {
                float arg = GetOrdinateNode(param, curves[i].lineNodes, j).arg1;
                if (minElement > arg) minElement = arg;
            }
        }
        return minElement;
    }   //</ver1.1>

    public static float GetMaxArg1(tParamXYZ param, Curve[] curves) //<ver1.1>
    {
        float maxElement = GetOrdinateNode(param, curves[0].lineNodes, 0).arg1;
        for (int i = 0; i < curves.Length; i++)
        {
            for (int j = 0; j < curves[i].lineNodes.point.Length; j++)
            {
                float arg = GetOrdinateNode(param, curves[i].lineNodes, j).arg1;
                if (maxElement < arg) maxElement = arg;
            }
        }
        return maxElement;
    }   //</ver1.1>

    public static float GetMinArg2(tParamXYZ param, Curve[] curves) //<ver1.1>
    {
        float minElement = GetOrdinateNode(param, curves[0].lineNodes, 0).arg2;
        for (int i = 0; i < curves.Length; i++)
        {
            for (int j = 0; j < curves[i].lineNodes.point.Length; j++)
            {
                float arg = GetOrdinateNode(param, curves[i].lineNodes, j).arg2;
                if (minElement > arg) minElement = arg;
            }
        }
        return minElement;
    }   //</ver1.1>

    public static float GetMaxArg2(tParamXYZ param, Curve[] curves) //<ver1.1>
    {
        float maxElement = GetOrdinateNode(param, curves[0].lineNodes, 0).arg2;
        for (int i = 0; i < curves.Length; i++)
        {
            for (int j = 0; j < curves[i].lineNodes.point.Length; j++)
            {
                float arg = GetOrdinateNode(param, curves[i].lineNodes, j).arg2;
                if (maxElement < arg) maxElement = arg;
            }
        }
        return maxElement;
    }   //</ver1.1>


    public static float GetMinArg1(tParamXYZ param, tNodes nodesCurve)  //<ver1.1>
    {          
        float minElement = GetOrdinateNode(param, nodesCurve, 0).arg1;
        for (int i = 0; i < nodesCurve.point.Length; i++)
        {
            float arg = GetOrdinateNode(param, nodesCurve, i).arg1;
            if (minElement > arg) minElement = arg;            
        }
        return minElement;
    }   //</ver1.1>

    public static float GetMaxArg1(tParamXYZ param, tNodes nodesCurve)  //<ver1.1>
    {
        float maxElement = GetOrdinateNode(param, nodesCurve, 0).arg1;
        for (int i = 0; i < nodesCurve.point.Length; i++)
        {
            float arg = GetOrdinateNode(param, nodesCurve, i).arg1;
            if (maxElement < arg) maxElement = arg;
        }
        return maxElement;
    }   //</ver1.1>

    public static float GetMinArg2(tParamXYZ param, tNodes nodesCurve)  //<ver1.1>
    {
        float minElement = GetOrdinateNode(param, nodesCurve, 0).arg2;
        for (int i = 0; i < nodesCurve.point.Length; i++)
        {
            float arg = GetOrdinateNode(param, nodesCurve, i).arg2;
            if (minElement > arg) minElement = arg;
        }
        return minElement;
    }   //</ver1.1>

    public static float GetMaxArg2(tParamXYZ param, tNodes nodesCurve)  //<ver1.1>
    {
        float maxElement = GetOrdinateNode(param, nodesCurve, 0).arg2;
        for (int i = 0; i < nodesCurve.point.Length; i++)
        {
            float arg = GetOrdinateNode(param, nodesCurve, i).arg2;
            if (maxElement < arg) maxElement = arg;
        }
        return maxElement;
    }   //</ver1.1>

    struct Ords //<ver1.1>
    {
        public float func;
        public float arg1;
        public float arg2;
    }   //</ver1.1>

    private static Ords GetOrdinateNode(tParamXYZ param, tNodes nodesCurve, int index)  //<ver1.1> return (func, arg1, arg2)
    {
        Ords result = new Ords();

        switch(param.typeFunc)
        {
            case TypeFunc.Y_XZ:
                {
                    result.func = nodesCurve.point[index].y;
                    result.arg1 = nodesCurve.point[index].x;
                    result.arg2 = nodesCurve.point[index].z;
                    break;
                }
            case TypeFunc.Y_ZX:
                {
                    result.func = nodesCurve.point[index].y;
                    result.arg1 = nodesCurve.point[index].z;
                    result.arg2 = nodesCurve.point[index].x;
                    break;
                }
            case TypeFunc.X_YZ:
                {
                    result.func = nodesCurve.point[index].x;
                    result.arg1 = nodesCurve.point[index].y;
                    result.arg2 = nodesCurve.point[index].z;
                    break;
                }
            case TypeFunc.X_ZY:
                {
                    result.func = nodesCurve.point[index].x;
                    result.arg1 = nodesCurve.point[index].z;
                    result.arg2 = nodesCurve.point[index].y;
                    break;
                }
            case TypeFunc.Z_XY:
                {
                    result.func = nodesCurve.point[index].z;
                    result.arg1 = nodesCurve.point[index].x;
                    result.arg2 = nodesCurve.point[index].y;
                    break;
                }
            case TypeFunc.Z_YX:
                {
                    result.func = nodesCurve.point[index].z;
                    result.arg1 = nodesCurve.point[index].y;
                    result.arg2 = nodesCurve.point[index].x;
                    break;
                }
        }
        return result;
    }
}

