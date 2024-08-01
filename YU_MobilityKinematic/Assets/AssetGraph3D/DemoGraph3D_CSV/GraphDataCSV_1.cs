using UnityEngine;

public class GraphDataCSV_1 : MonoBehaviour
{   
    public Graph3D Graph3D;
    public ParseCSV parseCSV;

    private Graph3D.Surface chart;

    private void Awake()
    {
        chart = new Graph3D.Surface(Graph3D);

        chart.data.typeFunc = Graph3D.TypeFunc.Y_XZ;
        chart.data.typeGraph = Graph3D.TypeGraph.Surface;
        chart.data.typeData = Graph3D.TypeData.Array;
        chart.data.typeForm = Graph3D.TypeForm.Lines;

        chart.data.tableCSV = parseCSV;

        chart.data.Y.scale.L = 5000f;
        chart.data.Y.scale.H = 7000f;

        //chart.data.autoScaleAxeFunc = true;
        chart.data.autoRangeArrayArg1 = true;
        chart.data.autoRangeArrayArg2 = true;
    }

    private void Start()
    {
        chart.DataSet();
        chart.Show();       
    }  
    
}
