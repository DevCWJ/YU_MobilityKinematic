using UnityEngine;

public class GraphDataCSV_3 : MonoBehaviour
{   
    public Graph3D Graph3D;
    public ParseCSV parseCSV_melt;
    public ParseCSV parseCSV_boil;

    private Graph3D.Surface chart_melt;
    private Graph3D.Surface chart_boil;

    private void Awake()
    {
        chart_melt = new Graph3D.Surface(Graph3D);
        chart_boil = new Graph3D.Surface(Graph3D);

        chart_melt.data.typeFunc = Graph3D.TypeFunc.Y_XZ;
        chart_melt.data.typeGraph = Graph3D.TypeGraph.Surface;
        chart_melt.data.typeData = Graph3D.TypeData.Array;
        chart_melt.data.typeForm = Graph3D.TypeForm.Points;
        chart_melt.data.autoRangeArrayArg1 = true;
        chart_melt.data.autoRangeArrayArg2 = true;
        chart_melt.data.Y.scale.L = -273f;
        chart_melt.data.Y.scale.H = 6000f;
        chart_melt.data.tableCSV = parseCSV_melt;

        chart_boil.data = chart_melt.data;  //We copy the configuration, change only the necessary settings * Копируем конфигурацию, меняем только необходимые настройки.
        chart_boil.data.tableCSV = parseCSV_boil;       
    }

    private void Start()
    {
        chart_melt.DataSet();
        chart_melt.Show(Color.red);

        chart_boil.DataSet();
        chart_boil.Show(Color.white);

        chart_melt.Merker.Set("1", 8f, 4f, 1535f, 5, "Fe (melting)");            
        chart_melt.Merker.Show("1", true, Color.red);       
    }    
}
