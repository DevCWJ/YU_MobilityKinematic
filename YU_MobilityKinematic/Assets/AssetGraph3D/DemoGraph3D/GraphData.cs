using UnityEngine;

public class GraphData : MonoBehaviour
{   
    public Graph3D Graph3D;

    private Graph3D.Curves curves;  
    
    private void Awake()
    {
        curves = new Graph3D.Curves(Graph3D);

        curves.data.typeFunc = Graph3D.TypeFunc.Y_XZ;
        curves.data.typeGraph = Graph3D.TypeGraph.ZCurves;
        //curves.data.typeGraph = Graph3D.TypeGraph.XCurves; 
        curves.data.typeData = Graph3D.TypeData.Function;
        curves.data.typeForm = Graph3D.TypeForm.Lines;
        curves.data.func = funcXYZ;

        curves.data.X.scale.L = 0;
        curves.data.X.scale.H = 5;
        curves.data.X.segments = 40;        
        curves.data.X.curves = 30;

        curves.data.Z.scale.L = -5;
        curves.data.Z.scale.H = 5;
        //curves.data.Z.segments = 40;  //For XCurves
        //curves.data.Z.argConst = 1;   //For XCurves
        //curves.data.Z.curves = 30;    //For XCurves

        curves.data.Y.scale.L = 0;
        curves.data.Y.scale.H = 3;      

        //curves.data.autoScaleAxeFunc = true;
    }

    private void Start()
    {
        curves.DataSet();
        curves.Show(Color.green);

        curves.Merker.Set("1", 1.2f, -1.8f, 10);
        curves.Merker.SetAxeLinesActive("1", true, false, false);
        curves.Merker.SetValuesActive("1", true, false, false);
        curves.Merker.Show("1", true);

        curves.Merker.Set("2", 2.2f, 2f, 10);
        curves.Merker.SetCurvesActive("2",false, false);
        curves.Merker.SetAxeLinesActive("2", true, false, false);
        curves.Merker.SetValuesActive("2", true, false, false);
        curves.Merker.Show("2", true);

        curves.Merker.Set("3", 4.55f, 0.7f, 10);
        curves.Merker.SetCurvesActive("3", false, false);
        curves.Merker.SetAxeLinesActive("3", true, false, false);
        curves.Merker.SetValuesActive("3", true, false, false);
        curves.Merker.Show("3", true);
    }  
    
    private float funcXYZ(float x, float z)
    {
        return Mathf.Sin(x * x)/ (x * 2 + z* z * 2 + 0.3f) + 1; 
    }
}
