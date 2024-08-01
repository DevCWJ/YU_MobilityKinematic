using UnityEngine;

public class GraphDataCSV_2 : MonoBehaviour
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
        chart.data.typeForm = Graph3D.TypeForm.Points_Pins;

        chart.data.tableCSV = parseCSV;

        chart.data.autoScaleAxeFunc = true;
        chart.data.autoRangeArrayArg1 = true;
        chart.data.autoRangeArrayArg2 = true;
    }

    private void Start()
    {
        chart.DataSet();
        chart.Show();

        chart.Merker.Set("Au", 11f, 6f, 196.9665f, 4,"(Au)");
        chart.Merker.SetAxeLinesActive("Au", true, false, false);
        chart.Merker.SetValuesActive("Au", true, false, false);
        chart.Merker.SetValuesColor("Au", Color.yellow, Color.yellow, Color.yellow);
        chart.Merker.Show("Au", true, Color.yellow);
        
    }    
}
