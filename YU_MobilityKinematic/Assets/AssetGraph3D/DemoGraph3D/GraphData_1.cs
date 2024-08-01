using UnityEngine;

public class GraphData_1 : MonoBehaviour
{   
    public Graph3D Graph3D;

    private Graph3D.Surface surf;   

    private void Awake()
    {   
        surf = new Graph3D.Surface(Graph3D);

        //surf.data.typeFunc = Graph3D.TypeFunc.Y_XZ;
        //surf.data.typeFunc = Graph3D.TypeFunc.Y_ZX;
        //surf.data.typeFunc = Graph3D.TypeFunc.Z_YX;
        //surf.data.typeFunc = Graph3D.TypeFunc.Z_XY;
        surf.data.typeFunc = Graph3D.TypeFunc.X_YZ;
        //surf.data.typeFunc = Graph3D.TypeFunc.X_ZY;

        surf.data.typeGraph = Graph3D.TypeGraph.Surface;
        surf.data.typeData = Graph3D.TypeData.Function;
        surf.data.typeForm = Graph3D.TypeForm.Lines;

        surf.data.func = func_1;

        surf.data.Z.scale.L = -100;
        surf.data.Z.scale.H = 100;
        surf.data.Z.segments = 100;        
        surf.data.Z.curves = 20;

        surf.data.Y.scale.L = -100;
        surf.data.Y.scale.H = 100;
        surf.data.Y.segments = 100;       
        surf.data.Y.curves = 20;

        surf.data.X.scale.L = -100;
        surf.data.X.scale.H = 100;
        surf.data.X.segments = 100;         
        surf.data.X.curves = 20;

        surf.data.autoScaleAxeFunc = true;             
    }

    private void Start()
    {              
        surf.DataSet();
        surf.Show(Color.blue);
    }

    private float func_1(float x, float z)
    {
        return 0.02f * (x * x + z * z);
    }    

    private float func_2(float x, float z)
    {
        return (0.01f * x * x) / (1 + 0.01f * z * z);
    }

    private float func_3(float x, float z)
    {
        return Mathf.Sin(0.0001f * x * z) * Mathf.Cos(0.0001f * x * z);
    }

    private float func_4(float x, float z)
    {
        return Mathf.Sin((0.001f * x * x) / (1 + 0.001f * z * z));
    }

    private float func_5(float x, float z)
    {
        return (Mathf.Sin(x * x + z * z) - Mathf.Cos(x * x + z * z)) / (1f + 0.001f * (x * x + z * z));
    }
}
