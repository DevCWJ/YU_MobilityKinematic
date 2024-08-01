using UnityEngine;
using System.Collections.Generic;

public class GraphData_2 : MonoBehaviour
{  
    public Graph3D Graph3D;

    private Graph3D.Surface surf;
    private Graph3D.Curve curve;

    private List<Graph3D.funcXYZ> listFunc;
    private Graph3D.funcXYZ carrFunc;
    private int numFunc;

    private void Awake()
    {
        numFunc = 0;
        listFunc = new List<Graph3D.funcXYZ>();

        listFunc.Add(water);
        listFunc.Add(func_1);
        listFunc.Add(func_2);
        listFunc.Add(func_3);
        listFunc.Add(func_4);
        listFunc.Add(func_5);
        listFunc.Add(func_6);
        listFunc.Add(func_7);
        listFunc.Add(func_8);
        listFunc.Add(func_9);
        listFunc.Add(func_10);
        listFunc.Add(func_11);

        surf = new Graph3D.Surface(Graph3D);

        surf.data.typeFunc = Graph3D.TypeFunc.Y_XZ;
        surf.data.typeGraph = Graph3D.TypeGraph.Surface;
        surf.data.typeData = Graph3D.TypeData.Function;
        surf.data.typeForm = Graph3D.TypeForm.Lines;

        surf.data.func = listFunc[0];

        surf.data.Z.scale.L = -3;
        surf.data.Z.scale.H = 3;
        surf.data.Z.segments = 100;
        surf.data.Z.curves = 40;

        surf.data.X.scale.L = -4.5f;
        surf.data.X.scale.H = 4.5f;
        surf.data.X.segments = 100;
        surf.data.X.curves = 60;

        surf.data.Y.scale.L = -0.5f;
        surf.data.Y.scale.H = 2.5f;
        //surf.data.Y.segments = 100;
        //surf.data.Y.argConst = 0;
        //surf.data.Y.curves = 20;

        //surf.data.autoScaleAxeFunc = true;        

        //---------------------------
        curve = new Graph3D.Curve(Graph3D);
    }

    private void Start()
    {
        surf.DataSet();
        surf.Show(Color.cyan);

        surf.Merker.Set("1", 1.7f, 0, 3);
        //surf.Merker.SetCurvesActive("1", false, true);
        surf.Merker.Show("1", true);

        if (numFunc == 0)
        {
            curve.data = surf.data;
            curve.data.func = waterPlus;
            curve.data.typeGraph = Graph3D.TypeGraph.Curve_Xconst;
            curve.data.X.argConst = -1.7f;
            curve.DataSet();
            //curve.Show(Color.green);    //The curve in this example will be drawn with a merker * Кривая в данном примере будет нарисована меркером
            curve.Merker.Set("1", -1.7f, 0, 3);
            curve.Merker.SetCurvesActive("1", false, true);
            curve.Merker.SetCurvesColor("1", Color.green, Color.green);
            curve.Merker.Show("1", true);
        }
        else curve.Destroy();
    }

    public void decGraf()
    {
        surf.Destroy();
        numFunc--;
        if (numFunc < 0) numFunc = listFunc.Count - 1;
        surf.data.func = listFunc[numFunc];
        Start();
    }

    public void incGraf()
    {
        surf.Destroy();
        numFunc++;
        if (numFunc >= listFunc.Count) numFunc = 0;
        surf.data.func = listFunc[numFunc];
        Start();
    }

    private float water(float x, float z)
    {
        return (Mathf.Cos(x * x + z * z)) / (1 + (x * x + z * z));
    }

    private float waterPlus(float x, float z)
    {
        return (Mathf.Cos(x * x + z * z)) / (1 + (x * x + z * z)) + 1.3f;
    }

    private float func_1(float x, float z)
    {
        return (x * z) / (1f - Mathf.Sqrt(z * x + x * x + z * z) / 1000);
    }

    private float func_2(float x, float z)
    {
        return 0.2f * (x * x + z * z) + (Mathf.Sin(x * x * 2 + z * z * 2) / (1 + x * x + z * z) - Mathf.Cos(x * x * 2 + z * z * 2) / (1 + x * x + z * z)) - 0.5f;
    }

    private float func_3(float x, float z)
    {
        return 0.02f * Mathf.Sin((x * x * 5) / (1 + z * z * 0.5f));
    }

    private float func_4(float x, float z)
    {
        return 0.2f * Mathf.Sin(x * z) * Mathf.Cos(x * z);
    }

    private float func_5(float x, float z)
    {
        return (0.5f * x * x) / (1 + z * z);
    }

    private float func_6(float x, float z)
    {
        return 1 / (0.5f + x * x + z * z);
    }

    private float func_7(float x, float z)
    {
        return 1.2f * x * z / (0.5f + x * x + z * z) + 1;
    }

    private float func_8(float x, float z)
    {
        return (x + z) / (1 + x * z);
    }

    private float func_9(float x, float z)
    {
        return 1 / (1f + (x - 1) * (x - 1) + z * z) - 1 / (1f + (x + 1) * (x + 1) + z * z) + 1;
    }

    private float func_10(float x, float z)
    {
        return 0.3f * Mathf.Cos(x * 2) * Mathf.Sin(z * 2) + 1;
    }

    private float func_11(float x, float z)
    {
        return Mathf.Sin(x * x) / (x * 2 + z * z * 2 + 0.3f) + 1;
    }

}
