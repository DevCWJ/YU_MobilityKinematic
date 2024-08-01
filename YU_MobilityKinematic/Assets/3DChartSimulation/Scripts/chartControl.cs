using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro;
public class chartControl : MonoBehaviour
{
    //canvas rect area
    private Rect canvasRectArea;

    //chartGroup
    public static GameObject titleLineGroup;
    public static GameObject gridLineGroup;
    public static GameObject dataLineGroup;
    public static GameObject markerGroup;
    public static GameObject cubeGroup;

    //canvas
    public static RectTransform textLayer;
    private RectTransform scaleLayer;
    private RectTransform chartAreaBckImg;

    //camera
    private GameObject chartCameraGroup;
    public static Camera camera3D;
    public static Camera cameraXY;
    public static Camera cameraUsed;

    //some parameters about the axis area
    float paraCameraSize;
    float paraAxisZoom;
    static Vector3 paraAxisRate;
    static Vector3 paraAxisShift;

    // tick name , title name setting
    public struct titleTick
    {
        public float max;
        public float min;
        public float[] tickData;
        public int tickNum;
        public float rate;//real number to 1000f
    }
    public static titleTick titleTickX;
    public static titleTick titleTickY;
    public static titleTick titleTickZ;

    //line material
    private static Material lineMaterial1;
    private static Material lineMaterial2;
    private static Material lineMaterial3;
    private static Material lineMaterial4;
    private static Material lineMaterial5;
    private static Material lineMaterial6;

    //cube prefab
    private static GameObject cubePrefab;
    private static int shiningColorFlag;

    //maker prefab
    private static GameObject markerPrefab;

    //chart text prefab
    private static GameObject chartTextPrefab;
    private static GameObject chartTextPrefab2;

    //rate xyz inputfield
    public static InputField rateX;
    public static InputField rateY;
    public static InputField rateZ;

    //set data for plot
    public static List<Vector3> dataForPlotAll;//for calculating the axis range
    public static List<Vector3> dataForPlotLine1;
    public static List<Vector3> dataForPlotLine2;
    public static List<Vector3> dataForPlotLine3;
    public static List<Vector3> dataForPlotLine4;
    public static List<Vector3> dataForPlotLine5;

    //time step for plot
    public static int timeLabel;

    //color group
    public static Color[] colorGroup;

    //set axis scale
    private static float realAxisRateZX;//axis z:axis x
    private static float realAxisRateYX;//axis y:axis x
    public static int flagIfFixRate;
    public static float[] inputRateXYZ;

    void Awake()
    {
        textLayer = GameObject.Find("canvasLabel/textLayer").GetComponent<RectTransform>();
        chartAreaBckImg = GameObject.Find("canvasBck/chartAreaBckImg").GetComponent<RectTransform>();

        titleLineGroup = GameObject.Find("chartGroup/titleLine").gameObject;
        gridLineGroup = GameObject.Find("chartGroup/gridLine").gameObject;
        dataLineGroup = GameObject.Find("chartGroup/dataLine").gameObject;
        markerGroup = GameObject.Find("chartGroup/markerGroup").gameObject;
        cubeGroup = GameObject.Find("chartGroup/cubeGroup").gameObject;

        chartCameraGroup = GameObject.Find("chartCameraGroup").gameObject;
        camera3D = chartCameraGroup.transform.Find("camera3D").GetComponent<Camera>();
        cameraXY = chartCameraGroup.transform.Find("cameraXY").GetComponent<Camera>();
        camera3D.gameObject.SetActive(true);
        cameraXY.gameObject.SetActive(false);

        lineMaterial1 = (Material)Resources.Load("chartLineMaterial1");
        lineMaterial2 = (Material)Resources.Load("chartLineMaterial2");
        lineMaterial3 = (Material)Resources.Load("chartLineMaterial3");
        lineMaterial4 = (Material)Resources.Load("chartLineMaterial4");
        lineMaterial5 = (Material)Resources.Load("chartLineMaterial5");
        lineMaterial6 = (Material)Resources.Load("chartLineMaterial6");

        cubePrefab = (GameObject)Resources.Load("cubePrefab");
        markerPrefab = (GameObject)Resources.Load("markerPrefab");
        chartTextPrefab = (GameObject)Resources.Load("chartTextPrefab");
        chartTextPrefab2 = (GameObject)Resources.Load("chartTextPrefab2");

        scaleLayer = GameObject.Find("canvasLabel/scaleLayer").GetComponent<RectTransform>();
        rateX = scaleLayer.transform.Find("rateX").GetComponent<InputField>();
        rateY = scaleLayer.transform.Find("rateY").GetComponent<InputField>();
        rateZ = scaleLayer.transform.Find("rateZ").GetComponent<InputField>();

    }

    // Start is called before the first frame update
    void Start()
    {
        canvasRectArea.x = 20f;
        canvasRectArea.y = 50f;
        canvasRectArea.width = 800f - 20f - canvasRectArea.x;
        canvasRectArea.height = 800f / Screen.width * Screen.height - 45f - canvasRectArea.y;

        textLayer.offsetMin = new Vector2(canvasRectArea.x, canvasRectArea.y);
        textLayer.offsetMax = new Vector2(canvasRectArea.width + canvasRectArea.x, canvasRectArea.height + canvasRectArea.y);
        chartAreaBckImg.offsetMin = new Vector2(canvasRectArea.x, canvasRectArea.y);
        chartAreaBckImg.offsetMax = new Vector2(canvasRectArea.width + canvasRectArea.x, canvasRectArea.height + canvasRectArea.y);

        cameraUsed = camera3D;
        camera3D.rect = new Rect(canvasRectArea.x / 800f, canvasRectArea.y / (800f / Screen.width * Screen.height),
                                        canvasRectArea.width / 800f, canvasRectArea.height / (800f / Screen.width * Screen.height));
        cameraXY.rect = new Rect((canvasRectArea.x + 15f) / 800f, (canvasRectArea.y + 10f) / (800f / Screen.width * Screen.height),
                                             canvasRectArea.width / 800f, canvasRectArea.height / (800f / Screen.width * Screen.height));

        paraCameraSize = 200f;
        paraAxisZoom = paraCameraSize * 0.15f;
        paraAxisRate = new Vector3(paraCameraSize - paraAxisZoom, paraCameraSize - paraAxisZoom,
                           paraCameraSize / canvasRectArea.height * canvasRectArea.width - paraAxisZoom);
        paraAxisShift = new Vector3(-paraAxisRate.x / 2, -paraAxisRate.y / 2, -paraAxisRate.z / 2);

        initLine();

        titleTickX = new titleTick();
        titleTickY = new titleTick();
        titleTickZ = new titleTick();
        titleTickX.tickData = new float[6];
        titleTickY.tickData = new float[6];
        titleTickZ.tickData = new float[6];


        dataForPlotAll = new List<Vector3>();
        dataForPlotLine1 = new List<Vector3>();
        dataForPlotLine2 = new List<Vector3>();
        dataForPlotLine3 = new List<Vector3>();
        dataForPlotLine4 = new List<Vector3>();
        dataForPlotLine5 = new List<Vector3>();

        timeLabel = 0;

        colorGroup = new Color[7];
        colorGroup[0] = new Color(0.6350f, 0.0780f, 0.1840f, 0.5f);//red
        colorGroup[1] = new Color(0f, 0.4470f, 0.7410f, 0.5f);//blue
        colorGroup[2] = new Color(0.8500f, 0.3250f, 0.0980f, 0.5f);//orange
        colorGroup[3] = new Color(0.9290f, 0.6940f, 0.1250f, 0.5f);//yellow
        colorGroup[4] = new Color(0.4940f, 0.1840f, 0.5560f, 0.5f);//violet
        colorGroup[5] = new Color(0.4660f, 0.6740f, 0.1880f, 0.5f);//green
        colorGroup[6] = new Color(0.3010f, 0.7450f, 0.9330f, 0.5f);//lt blue

        shiningColorFlag = 0;

        realAxisRateZX = 1;
        realAxisRateYX = 1;
        flagIfFixRate = 0;
        inputRateXYZ = new float[3];


        createCubeAndMaker();
        createTitleName();
        createTickName();

    }

    // Update is called once per frame
    void Update()
    {
        //time label - extract data for plot
        timeLabel++;
        timeLabel = timeLabel % inputData.lineData1.Count;


        updateDataForPlot();
        calculateAxisRange();
        updateTickBaseOnRule3D();
        createAxisLine();
        createGridLine();
        updateMarker();
        if (timeLabel > 1)
        {
            createDataLine();
            updateCube();
        }
        updateTitleName();
        createTickName();
        updateTickName();

    }

    void initLine()
    {
        //initialize axisLine
        string lineNum;
        int layer = 9;
        lineNum = "axisX0";
        createLine(titleLineGroup, lineNum, layer, lineMaterial1);
        lineNum = "axisX1Z";
        createLine(titleLineGroup, lineNum, layer, lineMaterial1);
        lineNum = "axisX1Y";
        createLine(titleLineGroup, lineNum, layer, lineMaterial1);
        lineNum = "axisZ0";
        createLine(titleLineGroup, lineNum, layer, lineMaterial1);
        lineNum = "axisZ1X";
        createLine(titleLineGroup, lineNum, layer, lineMaterial1);
        lineNum = "axisZ1Y";
        createLine(titleLineGroup, lineNum, layer, lineMaterial1);
        lineNum = "axisY0";
        createLine(titleLineGroup, lineNum, layer, lineMaterial1);
        lineNum = "axisY1X";
        createLine(titleLineGroup, lineNum, layer, lineMaterial1);
        lineNum = "axisY1Z";
        createLine(titleLineGroup, lineNum, layer, lineMaterial1);

        //initialize gridLine
        //xy plane
        lineNum = "gridlineXY1";
        createLine(gridLineGroup, lineNum, layer, lineMaterial2);
        lineNum = "gridlineXY2";
        createLine(gridLineGroup, lineNum, layer, lineMaterial2);
        //yz plane
        lineNum = "gridlineYZ1";
        createLine(gridLineGroup, lineNum, layer, lineMaterial2);
        lineNum = "gridlineYZ2";
        createLine(gridLineGroup, lineNum, layer, lineMaterial2);
        //zx plane
        lineNum = "gridlineZX1";
        createLine(gridLineGroup, lineNum, layer, lineMaterial2);
        lineNum = "gridlineZX2";
        createLine(gridLineGroup, lineNum, layer, lineMaterial2);

        //initialize dataLine
        lineNum = "Line_1";
        createLine(dataLineGroup, lineNum, layer, lineMaterial1);
        lineNum = "Line_1_fill";
        createLine(dataLineGroup, lineNum, layer, lineMaterial3);
        lineNum = "Line_2";
        createLine(dataLineGroup, lineNum, layer, lineMaterial4);
        lineNum = "Line_2_fill";
        createLine(dataLineGroup, lineNum, layer, lineMaterial3);
        lineNum = "Line_3";
        createLine(dataLineGroup, lineNum, layer, lineMaterial5);
        lineNum = "Line_3_2";
        createLine(dataLineGroup, lineNum, layer, lineMaterial6);
        lineNum = "Line_4";
        createLine(dataLineGroup, lineNum, layer, lineMaterial1);
        lineNum = "Line_5";
        createLine(dataLineGroup, lineNum, layer, lineMaterial1);
    }


    void createLine(GameObject chartGroup, string lineNum, int layer, Material lineMaterial)
    {
        GameObject Line = new GameObject(lineNum);
        Line.AddComponent<MeshFilter>();
        Line.AddComponent<MeshRenderer>();
        Line.layer = layer;
        Line.transform.SetParent(chartGroup.transform);
        Line.GetComponent<MeshRenderer>().material = lineMaterial;
        Line.name = lineNum;
    }

    void createCubeAndMaker()
    {
        GameObject obj;
        for (int i = 0; i < 9; i++)
        {
            obj = GameObject.Instantiate(cubePrefab, cubeGroup.transform);
            obj.name = "Cube_" + i;
        }
        for (int i = 0; i < 5; i++)
        {
            if (i != 3)
            {
                obj = GameObject.Instantiate(markerPrefab, markerGroup.transform);
                obj.name = "Marker_" + i;
            }

        }

    }

    public static void updateDataForPlot()
    {

        //extract points 

        //extract for calculating axis range
        dataForPlotAll.Clear();
        dataForPlotLine1.Clear();
        dataForPlotLine2.Clear();
        dataForPlotLine3.Clear();
        dataForPlotLine4.Clear();
        dataForPlotLine5.Clear();


        //add former points
        for (int j = 0; j < timeLabel - 1; j++)
        {

            dataForPlotLine1.Add(new Vector3(inputData.lineData1[j].posX,
                                inputData.lineData1[j].posY,
                                inputData.lineData1[j].posZ));
            dataForPlotLine2.Add(new Vector3(inputData.lineData2[j].posX,
                                inputData.lineData2[j].posY,
                                inputData.lineData2[j].posZ));
            dataForPlotLine3.Add(new Vector3(inputData.lineData3[j].posX,
                                inputData.lineData3[j].posY,
                                inputData.lineData3[j].posZ));
            dataForPlotLine4.Add(new Vector3(inputData.lineData4[j].posX,
                                inputData.lineData4[j].posY,
                                inputData.lineData4[j].posZ));
            dataForPlotLine5.Add(new Vector3(inputData.lineData5[j].posX,
                                inputData.lineData5[j].posY,
                                inputData.lineData5[j].posZ));


            dataForPlotAll.Add(new Vector3(inputData.lineData1[j].posX,
                                inputData.lineData1[j].posY,
                                inputData.lineData1[j].posZ));
            dataForPlotAll.Add(new Vector3(inputData.lineData2[j].posX,
                                inputData.lineData2[j].posY,
                                inputData.lineData2[j].posZ));
            dataForPlotAll.Add(new Vector3(inputData.lineData3[j].posX,
                                inputData.lineData3[j].posY,
                                inputData.lineData3[j].posZ));
            dataForPlotAll.Add(new Vector3(inputData.lineData4[j].posX,
                                inputData.lineData4[j].posY,
                                inputData.lineData4[j].posZ));
            dataForPlotAll.Add(new Vector3(inputData.lineData5[j].posX,
                                inputData.lineData5[j].posY,
                                inputData.lineData5[j].posZ));


        }



        //add the last points
        dataForPlotLine1.Add(new Vector3(inputData.lineData1[timeLabel].posX,
                                    inputData.lineData1[timeLabel].posY,
                                    inputData.lineData1[timeLabel].posZ));
        dataForPlotLine2.Add(new Vector3(inputData.lineData2[timeLabel].posX,
                                    inputData.lineData2[timeLabel].posY,
                                    inputData.lineData2[timeLabel].posZ));
        dataForPlotLine3.Add(new Vector3(inputData.lineData3[timeLabel].posX,
                                    inputData.lineData3[timeLabel].posY,
                                    inputData.lineData3[timeLabel].posZ));
        dataForPlotLine4.Add(new Vector3(inputData.lineData4[timeLabel].posX,
                                    inputData.lineData4[timeLabel].posY,
                                    inputData.lineData4[timeLabel].posZ));
        dataForPlotLine5.Add(new Vector3(inputData.lineData5[timeLabel].posX,
                                    inputData.lineData5[timeLabel].posY,
                                    inputData.lineData5[timeLabel].posZ));

        dataForPlotAll.Add(new Vector3(inputData.lineData1[timeLabel].posX,
                                    inputData.lineData1[timeLabel].posY,
                                    inputData.lineData1[timeLabel].posZ));
        dataForPlotAll.Add(new Vector3(inputData.lineData2[timeLabel].posX,
                                    inputData.lineData2[timeLabel].posY,
                                    inputData.lineData2[timeLabel].posZ));
        dataForPlotAll.Add(new Vector3(inputData.lineData3[timeLabel].posX,
                                    inputData.lineData3[timeLabel].posY,
                                    inputData.lineData3[timeLabel].posZ));
        dataForPlotAll.Add(new Vector3(inputData.lineData4[timeLabel].posX,
                                    inputData.lineData4[timeLabel].posY,
                                    inputData.lineData4[timeLabel].posZ));
        dataForPlotAll.Add(new Vector3(inputData.lineData5[timeLabel].posX,
                                    inputData.lineData5[timeLabel].posY,
                                    inputData.lineData5[timeLabel].posZ));




    }

    public static void calculateAxisRange()
    {


        int plotDataNum = dataForPlotAll.Count;

        if (plotDataNum > 0)
        {

            //define data
            List<Vector3> lineData = new List<Vector3>();
            List<float> LineDataX = new List<float>();
            List<float> LineDataY = new List<float>();
            List<float> LineDataZ = new List<float>();
            Vector3 dataTemp = new Vector3();
            for (int i = 0; i < plotDataNum; i++)
            {
                dataTemp = dataForPlotAll[i];
                lineData.Add(dataForPlotAll[i]);
                LineDataX.Add(dataTemp[0]);
                LineDataY.Add(dataTemp[1]);
                LineDataZ.Add(dataTemp[2]);
            }

            //define tick label
            float temp;
            float tickRangeTemp;
            float edgeRate = 0.1f;//edge precent


            temp = (float)Math.Ceiling((float)Convert.ToInt32((LineDataX.Max() * 100f).ToString("0")) / 100f);
            titleTickX.max = temp;
            temp = (float)Math.Floor((float)Convert.ToInt32((LineDataX.Min() * 100f).ToString("0")) / 100f);
            titleTickX.min = temp;


            titleTickX.tickData = new float[10];
            titleTickY.tickData = new float[10];
            titleTickZ.tickData = new float[10];

            titleTickX.tickNum = 5;
            tickRangeTemp = titleTickX.max - titleTickX.min;
            if (tickRangeTemp == 0)
            {
                titleTickX.max = titleTickX.max + 1;
                titleTickX.min = titleTickX.min - 1;
                tickRangeTemp = titleTickX.max - titleTickX.min;
            }
            titleTickX.max = titleTickX.max + tickRangeTemp * edgeRate;
            titleTickX.min = titleTickX.min - tickRangeTemp * edgeRate;
            tickRangeTemp = titleTickX.max - titleTickX.min;
            for (int i = 0; i < titleTickX.tickNum + 1; i++)
            {
                titleTickX.tickData[i] = (i * tickRangeTemp / titleTickX.tickNum + titleTickX.min);
            }
            titleTickX.rate = 1000f / tickRangeTemp;

            titleTickY.tickNum = 5;
            temp = (float)Math.Ceiling((float)Convert.ToInt32((LineDataY.Max() * 100f).ToString("0")) / 100f);
            titleTickY.max = temp;
            temp = (float)Math.Floor((float)Convert.ToInt32((LineDataY.Min() * 100f).ToString("0")) / 100f);
            titleTickY.min = temp;
            tickRangeTemp = titleTickY.max - titleTickY.min;
            if (tickRangeTemp == 0)
            {
                titleTickY.max = titleTickY.max + 1;
                titleTickY.min = titleTickY.min - 1;
                tickRangeTemp = titleTickY.max - titleTickY.min;
            }
            titleTickY.max = titleTickY.max + tickRangeTemp * edgeRate;
            titleTickY.min = titleTickY.min - tickRangeTemp * edgeRate;
            tickRangeTemp = titleTickY.max - titleTickY.min;
            for (int i = 0; i < titleTickY.tickNum + 1; i++)
            {
                titleTickY.tickData[i] = (i * tickRangeTemp / titleTickY.tickNum + titleTickY.min);

            }
            titleTickY.rate = 1000f / tickRangeTemp;


            temp = (float)Math.Ceiling((float)Convert.ToInt32((LineDataZ.Max() * 100f).ToString("0")) / 100f);
            titleTickZ.max = temp;
            temp = (float)Math.Floor((float)Convert.ToInt32((LineDataZ.Min() * 100f).ToString("0")) / 100f);
            titleTickZ.min = temp;
            titleTickZ.tickNum = 5;
            tickRangeTemp = titleTickZ.max - titleTickZ.min;
            if (tickRangeTemp == 0)
            {
                titleTickZ.max = titleTickZ.max + 1;
                titleTickZ.min = titleTickZ.min - 1;
                tickRangeTemp = titleTickZ.max - titleTickZ.min;
            }
            titleTickZ.max = titleTickZ.max + tickRangeTemp * edgeRate;
            titleTickZ.min = titleTickZ.min - tickRangeTemp * edgeRate;
            tickRangeTemp = titleTickZ.max - titleTickZ.min;
            for (int i = 0; i < titleTickZ.tickNum + 1; i++)
            {
                titleTickZ.tickData[i] = (i * tickRangeTemp / titleTickZ.tickNum + titleTickZ.min);
            }
            titleTickZ.rate = 1000f / tickRangeTemp;






        }


    }

    public static void createAxisLine()
    {

        string lineNum;
        float lineWidth = 0.6f;
        GameObject Line;
        Color colorLine;
        List<Vector3> lineData = new List<Vector3>();
        colorLine = new Color(96f / 255f, 110f / 255f, 133f / 255f, 1f);
        lineNum = "axisX0";
        Line = titleLineGroup.transform.Find(lineNum).gameObject;
        lineData.Clear();
        lineData.Add(new Vector3(0, 0, 0) + paraAxisShift);
        lineData.Add(Vector3.Scale(new Vector3(0, 0, 1), paraAxisRate) + paraAxisShift);
        defineAxisLine(Line, lineData, lineWidth, colorLine, cameraUsed);

        lineNum = "axisX1Z";
        Line = titleLineGroup.transform.Find(lineNum).gameObject;
        lineData.Clear();
        lineData.Add(Vector3.Scale(new Vector3(1, 0, 0), paraAxisRate) + paraAxisShift);
        lineData.Add(Vector3.Scale(new Vector3(1, 0, 1), paraAxisRate) + paraAxisShift);
        defineAxisLine(Line, lineData, lineWidth, colorLine, cameraUsed);

        lineNum = "axisX1Y";
        Line = titleLineGroup.transform.Find(lineNum).gameObject;
        lineData.Clear();
        lineData.Add(Vector3.Scale(new Vector3(0, 1, 0), paraAxisRate) + paraAxisShift);
        lineData.Add(Vector3.Scale(new Vector3(0, 1, 1), paraAxisRate) + paraAxisShift);
        defineAxisLine(Line, lineData, lineWidth, colorLine, cameraUsed);

        lineNum = "axisZ0";
        Line = titleLineGroup.transform.Find(lineNum).gameObject;
        lineData.Clear();
        lineData.Add(new Vector3(0, 0, 0) + paraAxisShift);
        lineData.Add(Vector3.Scale(new Vector3(1, 0, 0), paraAxisRate) + paraAxisShift);
        defineAxisLine(Line, lineData, lineWidth, colorLine, cameraUsed);

        lineNum = "axisZ1X";
        Line = titleLineGroup.transform.Find(lineNum).gameObject;
        lineData.Clear();
        lineData.Add(Vector3.Scale(new Vector3(0, 0, 1), paraAxisRate) + paraAxisShift);
        lineData.Add(Vector3.Scale(new Vector3(1, 0, 1), paraAxisRate) + paraAxisShift);
        defineAxisLine(Line, lineData, lineWidth, colorLine, cameraUsed);

        lineNum = "axisZ1Y";
        Line = titleLineGroup.transform.Find(lineNum).gameObject;
        lineData.Clear();
        lineData.Add(Vector3.Scale(new Vector3(0, 1, 0), paraAxisRate) + paraAxisShift);
        lineData.Add(Vector3.Scale(new Vector3(1, 1, 0), paraAxisRate) + paraAxisShift);
        defineAxisLine(Line, lineData, lineWidth, colorLine, cameraUsed);

        lineNum = "axisY0";
        Line = titleLineGroup.transform.Find(lineNum).gameObject;
        lineData.Clear();
        lineData.Add(Vector3.Scale(new Vector3(0, 0, 0), paraAxisRate) + paraAxisShift);
        lineData.Add(Vector3.Scale(new Vector3(0, 1, 0), paraAxisRate) + paraAxisShift);
        defineAxisLine(Line, lineData, lineWidth, colorLine, cameraUsed);

        lineNum = "axisY1X";
        Line = titleLineGroup.transform.Find(lineNum).gameObject;
        lineData.Clear();
        lineData.Add(Vector3.Scale(new Vector3(0, 0, 1), paraAxisRate) + paraAxisShift);
        lineData.Add(Vector3.Scale(new Vector3(0, 1, 1), paraAxisRate) + paraAxisShift);
        defineAxisLine(Line, lineData, lineWidth, colorLine, cameraUsed);

        lineNum = "axisY1Z";
        Line = titleLineGroup.transform.Find(lineNum).gameObject;
        lineData.Clear();
        lineData.Add(Vector3.Scale(new Vector3(1, 0, 0), paraAxisRate) + paraAxisShift);
        lineData.Add(Vector3.Scale(new Vector3(1, 1, 0), paraAxisRate) + paraAxisShift);
        defineAxisLine(Line, lineData, lineWidth, colorLine, cameraUsed);

    }

    public static void createGridLine()
    {

        if (titleTickX.tickData != null && titleTickY.tickData != null && titleTickZ.tickData != null)
        {
            float lineWidth;
            string lineNum;
            GameObject Line;
            Color colorLine;
            lineWidth = 0.3f;
            colorLine = new Color(96f / 255f, 110f / 255f, 133f / 255f, 1f);
            List<Vector3> lineData = new List<Vector3>();


            lineNum = "gridlineXY1";
            Line = gridLineGroup.transform.Find(lineNum).gameObject;
            lineData.Clear();
            for (int i = 1; i < titleTickY.tickNum; i++)
            {
                float tempHere = (titleTickY.tickData[i] - titleTickY.min) * titleTickY.rate / 1000f;
                lineData.Add(Vector3.Scale(new Vector3(0, 0, tempHere), paraAxisRate) + paraAxisShift);
                lineData.Add(Vector3.Scale(new Vector3(0, 1, tempHere), paraAxisRate) + paraAxisShift);
            }
            defineGridLine(Line, lineData, lineWidth, colorLine, cameraUsed);

            lineNum = "gridlineXY2";
            Line = gridLineGroup.transform.Find(lineNum).gameObject;
            lineData.Clear();
            for (int i = 1; i < titleTickX.tickNum; i++)
            {
                float tempHere = (titleTickX.tickData[i] - titleTickX.min) * titleTickX.rate / 1000f;
                lineData.Add(Vector3.Scale(new Vector3(0, tempHere, 0), paraAxisRate) + paraAxisShift);
                lineData.Add(Vector3.Scale(new Vector3(0, tempHere, 1), paraAxisRate) + paraAxisShift);
            }
            defineGridLine(Line, lineData, lineWidth, colorLine, cameraUsed);

            lineNum = "gridlineYZ1";
            Line = gridLineGroup.transform.Find(lineNum).gameObject;
            lineData.Clear();
            for (int i = 1; i < titleTickY.tickNum; i++)
            {
                float tempHere = (titleTickY.tickData[i] - titleTickY.min) * titleTickY.rate / 1000f;
                lineData.Add(Vector3.Scale(new Vector3(tempHere, 0, 0), paraAxisRate) + paraAxisShift);
                lineData.Add(Vector3.Scale(new Vector3(tempHere, 1, 0), paraAxisRate) + paraAxisShift);
            }
            defineGridLine(Line, lineData, lineWidth, colorLine, cameraUsed);

            lineNum = "gridlineYZ2";
            Line = gridLineGroup.transform.Find(lineNum).gameObject;
            lineData.Clear();
            for (int i = 1; i < titleTickZ.tickNum; i++)
            {
                float tempHere = (titleTickZ.tickData[i] - titleTickZ.min) * titleTickZ.rate / 1000f;
                lineData.Add(Vector3.Scale(new Vector3(0, tempHere, 0), paraAxisRate) + paraAxisShift);
                lineData.Add(Vector3.Scale(new Vector3(1, tempHere, 0), paraAxisRate) + paraAxisShift);
            }
            defineGridLine(Line, lineData, lineWidth, colorLine, cameraUsed);

            lineNum = "gridlineZX1";
            Line = gridLineGroup.transform.Find(lineNum).gameObject;
            lineData.Clear();
            for (int i = 1; i < titleTickX.tickNum; i++)
            {

                float tempHere = (titleTickX.tickData[i] - titleTickX.min) * titleTickX.rate / 1000f;
                lineData.Add(Vector3.Scale(new Vector3(tempHere, 0, 0), paraAxisRate) + paraAxisShift);
                lineData.Add(Vector3.Scale(new Vector3(tempHere, 0, 1), paraAxisRate) + paraAxisShift);
            }
            defineGridLine(Line, lineData, lineWidth, colorLine, cameraUsed);

            lineNum = "gridlineZX2";
            Line = gridLineGroup.transform.Find(lineNum).gameObject;
            lineData.Clear();
            for (int i = 1; i < titleTickZ.tickNum; i++)
            {
                float tempHere = (titleTickZ.tickData[i] - titleTickZ.min) * titleTickZ.rate / 1000f;

                lineData.Add(Vector3.Scale(new Vector3(0, 0, tempHere), paraAxisRate) + paraAxisShift);
                lineData.Add(Vector3.Scale(new Vector3(1, 0, tempHere), paraAxisRate) + paraAxisShift);
            }
            defineGridLine(Line, lineData, lineWidth, colorLine, cameraUsed);


        }
    }

    public static void createDataLine()
    {
        float lineWidth;
        string lineNum;
        GameObject Line;
        Color colorLine;

        List<Vector3> LineData = new List<Vector3>();
        List<Vector3> lineDataTemp = new List<Vector3>();
        lineWidth = 2f;
        LineData = dataForPlotLine1;
        lineNum = "Line_1";
        colorLine = new Color(219f / 255f, 94f / 255f, 94f / 255f, 1f);
        Line = dataLineGroup.transform.Find(lineNum).gameObject;
        lineDataTemp = processLineData(LineData, lineNum);
        defineLine(Line, lineDataTemp, lineWidth, colorLine, cameraUsed);
        colorLine = new Color(219f / 255f, 94f / 255f, 94f / 255f, 0.4f);
        lineNum = "Line_1_fill";
        Line = dataLineGroup.transform.Find(lineNum).gameObject;
        defineLineFill(Line, lineDataTemp, lineWidth, colorLine, cameraUsed);


        lineWidth = 2f;
        LineData = dataForPlotLine2;
        lineNum = "Line_2";
        colorLine = Color.white;
        Line = dataLineGroup.transform.Find(lineNum).gameObject;
        lineDataTemp = processLineData(LineData, lineNum);
        defineLineRainbow(Line, lineDataTemp, lineWidth, colorLine, cameraUsed);
        lineNum = "Line_2_fill";
        colorLine = colorGroup[(int)((float)timeLabel / inputData.lineData1.Count * colorGroup.Length)];
        Line = dataLineGroup.transform.Find(lineNum).gameObject;
        defineLineFill(Line, lineDataTemp, lineWidth, colorLine, cameraUsed);

        lineWidth = 3f;
        LineData = dataForPlotLine3;
        lineNum = "Line_3";
        colorLine = Color.white;
        Line = dataLineGroup.transform.Find(lineNum).gameObject;
        lineDataTemp = processLineData(LineData, lineNum);
        defineLineDash(Line, lineDataTemp, lineWidth, colorLine, cameraUsed, 40);
        lineWidth = 2.5f;
        LineData = dataForPlotLine3;
        lineNum = "Line_3_2";
        colorLine = Color.white;
        Line = dataLineGroup.transform.Find(lineNum).gameObject;
        lineDataTemp = processLineData(LineData, lineNum);
        lineDataTemp = shiftLineData(lineDataTemp);
        defineLineDash(Line, lineDataTemp, lineWidth, colorLine, cameraUsed, 20);

        lineWidth = 2f;
        LineData = dataForPlotLine5;
        lineNum = "Line_5";
        colorLine = colorGroup[((int)((float)timeLabel / inputData.lineData1.Count * colorGroup.Length) + 1) % colorGroup.Length];
        Line = dataLineGroup.transform.Find(lineNum).gameObject;
        lineDataTemp = processLineData(LineData, lineNum);
        defineLine(Line, lineDataTemp, lineWidth, colorLine, cameraUsed);

    }


    public static void updateCube()
    {
        float[] posX = new float[9];
        posX[0] = (titleTickX.tickData[0] + titleTickX.tickData[1]) / 2;
        posX[1] = titleTickX.tickData[1];
        posX[2] = (titleTickX.tickData[1] + titleTickX.tickData[2]) / 2;
        posX[3] = titleTickX.tickData[2];
        posX[4] = (titleTickX.tickData[2] + titleTickX.tickData[3]) / 2;
        posX[5] = titleTickX.tickData[3];
        posX[6] = (titleTickX.tickData[3] + titleTickX.tickData[4]) / 2;
        posX[7] = titleTickX.tickData[4];
        posX[8] = (titleTickX.tickData[4] + titleTickX.tickData[5]) / 2;


        GameObject obj;
        int dataPoint = timeLabel;
        Color tempColor = Color.white;
        float tempColorMax = 240f / 255f;
        float tempColorMin = 100f / 255f;
        float shiningSpeed = 0.005f;
        for (int i = 0; i < 9; i++)
        {
            obj = cubeGroup.transform.Find("Cube_" + i).gameObject;
            int space = (int)((float)inputData.lineData4.Count / 18f) * i;
            dataPoint = (timeLabel + space) % inputData.lineData4.Count;

            obj.transform.GetChild(0).localScale = new Vector3(5, inputData.lineData4[dataPoint].posY / 4, 5)*100;
            obj.transform.localPosition = new Vector3(
                (inputData.lineData4[0].posZ - titleTickZ.min) * titleTickZ.rate / 1000f * paraAxisRate.x + paraAxisShift.x,
                obj.transform.GetChild(0).localScale.y/ 100 * 4 / 2 + paraAxisShift.y,
                (posX[i] - titleTickX.min) * titleTickX.rate / 1000f * paraAxisRate.z + paraAxisShift.z);
            obj.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector3(
                0, obj.transform.GetChild(0).localScale.y / 100 * 4);
            obj.transform.GetChild(1).GetComponent<TMP_Text>().text = inputData.lineData4[dataPoint].posY.ToString("0.0");

            //shining effect
            if (i == 0)
            {
                tempColor = obj.transform.GetChild(0).GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
                float tempColorA = tempColor.r;
                if (tempColorA != tempColorMax && tempColorA != tempColorMin)
                {
                    tempColorA = tempColorA + shiningSpeed * shiningColorFlag;
                    if (tempColorA > tempColorMax) tempColorA = tempColorMax;
                    else if (tempColorA < tempColorMin) tempColorA = tempColorMin;
                }
                else if (tempColorA == tempColorMax)
                {
                    shiningColorFlag = -1;
                    tempColorA = tempColorA + shiningSpeed * shiningColorFlag;
                }
                else if (tempColorA == tempColorMin)
                {
                    shiningColorFlag = 1;
                    tempColorA = tempColorA + shiningSpeed * shiningColorFlag;
                }
                tempColor = new Color(tempColorA, tempColorA, tempColorA, 1);
                obj.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", tempColor);
            }
            else
            {
                obj.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", tempColor);
            }
        }

    }

    public static void updateMarker()
    {
        GameObject obj;
        List<Vector3> LineData = new List<Vector3>();
        Color colorMarker;
        obj = markerGroup.transform.Find("Marker_0").gameObject;
        LineData.Add(dataForPlotLine1[dataForPlotLine1.Count - 1]);
        colorMarker = colorGroup[0];
        LineData = processLineData(LineData, "");
        obj.transform.localPosition = LineData[LineData.Count - 1];
        obj.GetComponent<MeshRenderer>().material.color = colorMarker;
        obj.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", colorMarker);
        obj.transform.localEulerAngles = new Vector3(-inputData.lineData1[timeLabel].angTheta, 0, 0);

        obj = markerGroup.transform.Find("Marker_1").gameObject;
        LineData.Add(dataForPlotLine2[dataForPlotLine2.Count - 1]);
        colorMarker = colorGroup[1];
        LineData = processLineData(LineData, "");
        obj.transform.localPosition = LineData[LineData.Count - 1];
        obj.GetComponent<MeshRenderer>().material.color = colorMarker;
        obj.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", colorMarker);
        obj.transform.localEulerAngles = new Vector3(-inputData.lineData2[timeLabel].angTheta, -180, 0);

        obj = markerGroup.transform.Find("Marker_2").gameObject;
        LineData.Add(dataForPlotLine3[dataForPlotLine3.Count - 1]);
        colorMarker = colorGroup[2];
        LineData = processLineData(LineData, "");
        obj.transform.localPosition = LineData[LineData.Count - 1];
        obj.GetComponent<MeshRenderer>().material.color = colorMarker;
        obj.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", colorMarker);
        obj.transform.localEulerAngles = new Vector3(-inputData.lineData3[timeLabel].angTheta, 0, 0);

        obj = markerGroup.transform.Find("Marker_4").gameObject;
        LineData.Add(dataForPlotLine5[dataForPlotLine5.Count - 1]);
        colorMarker = colorGroup[3];
        LineData = processLineData(LineData, "");
        obj.transform.localPosition = LineData[LineData.Count - 1];
        obj.GetComponent<MeshRenderer>().material.color = colorMarker;
        obj.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", colorMarker);
        obj.transform.localEulerAngles = new Vector3(-inputData.lineData5[timeLabel].angTheta, 0, 0);
    }

    public static void defineLineRainbow(GameObject Line, List<Vector3> lineData, float lineWidth, Color color, Camera cameraChart)
    {

        int pointCount = lineData.Count;

        MeshFilter lineMeshFilter = Line.GetComponent<MeshFilter>();

        int pointNum1 = pointCount - 1;
        if (pointNum1 < 0) pointNum1 = 0;
        int pointNum2 = pointCount - 2;
        if (pointNum2 < 0) pointNum2 = 0;

        Vector3[] m_Vertices = new Vector3[(4 * pointNum1 + 4 * pointNum2)];
        int[] m_triangles = new int[(6 * pointNum1 + 6 * pointNum2)];
        Vector2[] m_UV = new Vector2[(4 * pointNum1 + 4 * pointNum2)];

        float lineLength = inputData.lengthLineData2;

        Vector3 viewDir;
        Vector3 lineDir;
        Vector3 lineMeshDirection;
        int trianglesCurrentCount = 0;


        float length = Mathf.Sqrt(Mathf.Pow((lineData[0].x - lineData[1].x), 2) +
         Mathf.Pow((lineData[0].y - lineData[1].y), 2) +
         Mathf.Pow((lineData[0].z - lineData[1].z), 2));

        float uvOffset = 0;
        float uvOffset2 = 0;
        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 pointHeadPos = lineData[i];
            Vector3 pointTailPos = lineData[i + 1];

            viewDir = cameraChart.transform.position - pointHeadPos;
            lineDir = pointHeadPos - pointTailPos;
            lineMeshDirection = Vector3.Cross(viewDir, lineDir).normalized;


            m_Vertices[i * 4] = pointTailPos - lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 1] = pointTailPos + lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 2] = pointHeadPos - lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 3] = pointHeadPos + lineMeshDirection * lineWidth / 2;

            uvOffset = i / lineLength * length * 2;
            uvOffset = uvOffset % 1;
            uvOffset2 = (i) / lineLength * length * 2;
            uvOffset2 = uvOffset2 % 1;

            m_UV[i * 4] = new Vector2(0, uvOffset2);
            m_UV[i * 4 + 1] = new Vector2(1, uvOffset2);
            m_UV[i * 4 + 2] = new Vector2(0, uvOffset);
            m_UV[i * 4 + 3] = new Vector2(1, uvOffset);


            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
            m_triangles[trianglesCurrentCount++] = i * 4 + 1;
            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 2;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
        }


        for (int i = 1; i < pointCount - 1; i++)
        {

            m_Vertices[(i - 1) * 4 + 2 + 4 * (pointCount - 1)] = m_Vertices[(i - 1) * 4];
            m_Vertices[(i - 1) * 4 + 3 + 4 * (pointCount - 1)] = m_Vertices[(i - 1) * 4 + 1];

            m_Vertices[(i - 1) * 4 + 0 + 4 * (pointCount - 1)] = m_Vertices[(i) * 4 + 2];
            m_Vertices[(i - 1) * 4 + 1 + 4 * (pointCount - 1)] = m_Vertices[(i) * 4 + 3];

            uvOffset = i / lineLength * length * 2;
            uvOffset = uvOffset % 1;
            uvOffset2 = (i) / lineLength * length * 2;
            uvOffset2 = uvOffset2 % 1;

            m_UV[(i - 1) * 4 + 2 + 4 * (pointCount - 1)] = new Vector2(0, uvOffset2);
            m_UV[(i - 1) * 4 + 3 + 4 * (pointCount - 1)] = new Vector2(1, uvOffset2);
            m_UV[(i - 1) * 4 + 0 + 4 * (pointCount - 1)] = new Vector2(0, uvOffset);
            m_UV[(i - 1) * 4 + 1 + 4 * (pointCount - 1)] = new Vector2(1, uvOffset);


            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 3 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 1 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 2 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 3 + 4 * (pointCount - 1);
        }


        lineMeshFilter.mesh .Clear();
        lineMeshFilter.mesh.vertices = m_Vertices;
        lineMeshFilter.mesh.triangles = m_triangles;
        lineMeshFilter.mesh.uv = m_UV;
        lineMeshFilter.mesh.RecalculateNormals();

    }

    public static void defineLineDash(GameObject Line, List<Vector3> lineData, float lineWidth, Color color, Camera cameraChart, float scale)
    {


        int pointCount = lineData.Count;

        MeshFilter lineMeshFilter = Line.GetComponent<MeshFilter>();

        int pointNum1 = pointCount - 1;
        if (pointNum1 < 0) pointNum1 = 0;
        int pointNum2 = pointCount - 2;
        if (pointNum2 < 0) pointNum2 = 0;

        Vector3[] m_Vertices = new Vector3[(4 * pointNum1 + 4 * pointNum2)];
        int[] m_triangles = new int[(6 * pointNum1 + 6 * pointNum2)];
        Vector2[] m_UV = new Vector2[(4 * pointNum1 + 4 * pointNum2)];

        float lineLength = inputData.lengthLineData2;

        Vector3 viewDir;
        Vector3 lineDir;
        Vector3 lineMeshDirection;
        int trianglesCurrentCount = 0;


        float length = Mathf.Sqrt(Mathf.Pow((lineData[0].x - lineData[1].x), 2) +
         Mathf.Pow((lineData[0].y - lineData[1].y), 2) +
         Mathf.Pow((lineData[0].z - lineData[1].z), 2));

        float uvOffset = 0;
        float uvOffset2 = 0;
        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 pointHeadPos = lineData[i];
            Vector3 pointTailPos = lineData[i + 1];

            viewDir = cameraChart.transform.position - pointHeadPos;
            lineDir = pointHeadPos - pointTailPos;
            lineMeshDirection = Vector3.Cross(viewDir, lineDir).normalized;


            m_Vertices[i * 4] = pointTailPos - lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 1] = pointTailPos + lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 2] = pointHeadPos - lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 3] = pointHeadPos + lineMeshDirection * lineWidth / 2;

            uvOffset = i / lineLength * length * scale;
            uvOffset = uvOffset % 1;
            uvOffset2 = (i + 1) / lineLength * length * scale;
            uvOffset2 = uvOffset2 % 1;
            m_UV[i * 4] = new Vector2(0, uvOffset2);
            m_UV[i * 4 + 1] = new Vector2(1, uvOffset2);
            m_UV[i * 4 + 2] = new Vector2(0, uvOffset);
            m_UV[i * 4 + 3] = new Vector2(1, uvOffset);


            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
            m_triangles[trianglesCurrentCount++] = i * 4 + 1;
            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 2;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
        }


        for (int i = 1; i < pointCount - 1; i++)
        {

            m_Vertices[(i - 1) * 4 + 2 + 4 * (pointCount - 1)] = m_Vertices[(i - 1) * 4];
            m_Vertices[(i - 1) * 4 + 3 + 4 * (pointCount - 1)] = m_Vertices[(i - 1) * 4 + 1];

            m_Vertices[(i - 1) * 4 + 0 + 4 * (pointCount - 1)] = m_Vertices[(i) * 4 + 2];
            m_Vertices[(i - 1) * 4 + 1 + 4 * (pointCount - 1)] = m_Vertices[(i) * 4 + 3];

            uvOffset = i / lineLength * length * scale;
            uvOffset = uvOffset % 1;
            uvOffset2 = (i) / lineLength * length * scale;
            uvOffset2 = uvOffset2 % 1;

            m_UV[(i - 1) * 4 + 2 + 4 * (pointCount - 1)] = new Vector2(0, uvOffset2);
            m_UV[(i - 1) * 4 + 3 + 4 * (pointCount - 1)] = new Vector2(1, uvOffset2);
            m_UV[(i - 1) * 4 + 0 + 4 * (pointCount - 1)] = new Vector2(0, uvOffset);
            m_UV[(i - 1) * 4 + 1 + 4 * (pointCount - 1)] = new Vector2(1, uvOffset);


            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 3 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 1 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 2 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 3 + 4 * (pointCount - 1);
        }


        lineMeshFilter.mesh.Clear();
        lineMeshFilter.mesh.vertices = m_Vertices;
        lineMeshFilter.mesh.triangles = m_triangles;
        lineMeshFilter.mesh.uv = m_UV;
        lineMeshFilter.mesh.RecalculateNormals();

    }

    public static void defineLine(GameObject Line, List<Vector3> lineData, float lineWidth, Color color, Camera cameraChart)
    {
        int pointCount = lineData.Count;
        Line.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
        Line.GetComponent<MeshRenderer>().material.color = color;

        MeshFilter lineMeshFilter = Line.GetComponent<MeshFilter>();

        int pointNum1 = pointCount - 1;
        if (pointNum1 < 0) pointNum1 = 0;
        int pointNum2 = pointCount - 2;
        if (pointNum2 < 0) pointNum2 = 0;

        Vector3[] m_Vertices = new Vector3[(4 * pointNum1 + 4 * pointNum2)];
        int[] m_triangles = new int[(6 * pointNum1 + 6 * pointNum2)];

        Vector3 viewDir;
        Vector3 lineDir;
        Vector3 lineMeshDirection;
        int trianglesCurrentCount = 0;

        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 pointHeadPos = lineData[i];
            Vector3 pointTailPos = lineData[i + 1];

            viewDir = cameraChart.transform.position - pointHeadPos;
            lineDir = pointHeadPos - pointTailPos;
            lineMeshDirection = Vector3.Cross(viewDir, lineDir).normalized;

            m_Vertices[i * 4] = pointTailPos - lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 1] = pointTailPos + lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 2] = pointHeadPos - lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 3] = pointHeadPos + lineMeshDirection * lineWidth / 2;

            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
            m_triangles[trianglesCurrentCount++] = i * 4 + 1;
            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 2;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
        }

        for (int i = 1; i < pointCount - 1; i++)
        {

            m_Vertices[(i - 1) * 4 + 2 + 4 * (pointCount - 1)] = m_Vertices[(i - 1) * 4];
            m_Vertices[(i - 1) * 4 + 3 + 4 * (pointCount - 1)] = m_Vertices[(i - 1) * 4 + 1];

            m_Vertices[(i - 1) * 4 + 0 + 4 * (pointCount - 1)] = m_Vertices[(i) * 4 + 2];
            m_Vertices[(i - 1) * 4 + 1 + 4 * (pointCount - 1)] = m_Vertices[(i) * 4 + 3];

            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 3 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 1 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 2 + 4 * (pointCount - 1);
            m_triangles[trianglesCurrentCount++] = (i - 1) * 4 + 3 + 4 * (pointCount - 1);
        }

        lineMeshFilter.mesh.Clear();
        lineMeshFilter.mesh.vertices = m_Vertices;
        lineMeshFilter.mesh.triangles = m_triangles;


    }

    public static void defineLineFill(GameObject Line, List<Vector3> lineData, float lineWidth, Color color, Camera cameraChart)
    {
        int pointCount = lineData.Count;

        Line.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
        Line.GetComponent<MeshRenderer>().material.color = color;

        MeshFilter lineMeshFilter = Line.GetComponent<MeshFilter>();

        int pointNum1 = pointCount - 1;
        if (pointNum1 < 0) pointNum1 = 0;

        Vector3[] m_Vertices = new Vector3[4 * pointNum1 * 2];
        int[] m_triangles = new int[6 * pointNum1 * 2];


        int trianglesCurrentCount = 0;

        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 pointHeadPos = lineData[i];
            Vector3 pointTailPos = lineData[i + 1];



            m_Vertices[i * 4 + 2] = pointTailPos - new Vector3(0, lineWidth / 2f, 0);
            m_Vertices[i * 4 + 3] = new Vector3(pointTailPos.x, paraAxisShift.y, pointTailPos.z);
            m_Vertices[i * 4] = pointHeadPos - new Vector3(0, lineWidth / 2f, 0);
            m_Vertices[i * 4 + 1] = new Vector3(pointHeadPos.x, paraAxisShift.y, pointHeadPos.z);

            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
            m_triangles[trianglesCurrentCount++] = i * 4 + 1;
            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 2;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
        }
        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 pointHeadPos = lineData[i];
            Vector3 pointTailPos = lineData[i + 1];



            m_Vertices[i * 4 + 2 + 4 * (pointCount - 1)] = pointTailPos - new Vector3(0, lineWidth / 2f, 0);
            m_Vertices[i * 4 + 3 + 4 * (pointCount - 1)] = new Vector3(pointTailPos.x, paraAxisShift.y, pointTailPos.z);
            m_Vertices[i * 4 + 4 * (pointCount - 1)] = pointHeadPos - new Vector3(0, lineWidth / 2f, 0);
            m_Vertices[i * 4 + 1 + 4 * (pointCount - 1)] = new Vector3(pointHeadPos.x, paraAxisShift.y, pointHeadPos.z);

            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 1;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
            m_triangles[trianglesCurrentCount++] = i * 4 + 2;
        }

        lineMeshFilter.mesh .Clear();
        lineMeshFilter.mesh.vertices = m_Vertices;
        lineMeshFilter.mesh.triangles = m_triangles;


    }


    public static void defineAxisLine(GameObject Line, List<Vector3> lineData, float lineWidth, Color color, Camera cameraChart)
    {

        int pointCount = lineData.Count;

        Line.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
        MeshFilter lineMeshFilter = Line.GetComponent<MeshFilter>();


        Vector3[] m_Vertices = new Vector3[4 * (pointCount - 1)];
        int[] m_triangles = new int[6 * (pointCount - 1)];


        Vector3 viewDir = new Vector3();
        Vector3 lineDir = new Vector3();
        Vector3 lineMeshDirection = new Vector3();
        int trianglesCurrentCount = 0;

        Vector3 pointHeadPos = new Vector3();
        Vector3 pointTailPos = new Vector3();
        for (int i = 0; i < pointCount - 1; i++)
        {
            pointHeadPos = lineData[i];
            pointTailPos = lineData[i + 1];

            viewDir = cameraChart.transform.position - pointHeadPos;
            lineDir = pointHeadPos - pointTailPos;
            lineMeshDirection = Vector3.Cross(viewDir, lineDir).normalized;


            m_Vertices[i * 4] = pointTailPos - lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 1] = pointTailPos + lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 2] = pointHeadPos - lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 3] = pointHeadPos + lineMeshDirection * lineWidth / 2;


            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
            m_triangles[trianglesCurrentCount++] = i * 4 + 1;
            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 2;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
        }


        lineMeshFilter.mesh.Clear();
        lineMeshFilter.mesh.vertices = m_Vertices;
        lineMeshFilter.mesh.triangles = m_triangles;

    }

    public static void defineGridLine(GameObject Line, List<Vector3> lineData, float lineWidth, Color color, Camera cameraChart)
    {

        int pointCount = lineData.Count;

        Line.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
        Line.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(0, 0));
        MeshFilter lineMeshFilter = Line.GetComponent<MeshFilter>();


        Vector3[] m_Vertices = new Vector3[4 * (pointCount) / 2 * 4 * (pointCount - 1) * 2];
        int[] m_triangles = new int[6 * (pointCount) / 2 * 4 * (pointCount - 1) * 2];
        Vector2[] m_UV = new Vector2[4 * (pointCount) / 2 * 4 * (pointCount - 1) * 2];

        float lineLength = Mathf.Sqrt(Mathf.Pow((lineData[0].x - lineData[lineData.Count - 1].x), 2) +
            Mathf.Pow((lineData[0].y - lineData[lineData.Count - 1].y), 2) +
            Mathf.Pow((lineData[0].z - lineData[lineData.Count - 1].z), 2));


        Vector3 viewDir;
        Vector3 lineDir;
        Vector3 lineMeshDirection;
        int trianglesCurrentCount = 0;

        for (int i = 0; i < pointCount - 1; i = i + 2)
        {
            Vector3 pointHeadPos = lineData[i];
            Vector3 pointTailPos = lineData[i + 1];

            viewDir = cameraChart.transform.position - pointHeadPos;
            lineDir = pointHeadPos - pointTailPos;
            lineMeshDirection = Vector3.Cross(viewDir, lineDir).normalized;


            m_Vertices[i * 4] = pointTailPos - lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 1] = pointTailPos + lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 2] = pointHeadPos - lineMeshDirection * lineWidth / 2;
            m_Vertices[i * 4 + 3] = pointHeadPos + lineMeshDirection * lineWidth / 2;


            m_UV[i * 4] = new Vector2(0, 0);
            m_UV[i * 4 + 1] = new Vector2(1, 0);
            m_UV[i * 4 + 2] = new Vector2(0, lineLength / 5f);
            m_UV[i * 4 + 3] = new Vector2(1, lineLength / 5f);


            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
            m_triangles[trianglesCurrentCount++] = i * 4 + 1;
            m_triangles[trianglesCurrentCount++] = i * 4;
            m_triangles[trianglesCurrentCount++] = i * 4 + 2;
            m_triangles[trianglesCurrentCount++] = i * 4 + 3;
        }

        lineMeshFilter.mesh .Clear();
        lineMeshFilter.mesh.vertices = m_Vertices;
        lineMeshFilter.mesh.triangles = m_triangles;
        lineMeshFilter.mesh.uv = m_UV;
        lineMeshFilter.mesh.RecalculateNormals();

    }


    public static List<Vector3> processLineData(List<Vector3> LineData, string lineNum)
    {
        List<Vector3> LineDataForPlot = new List<Vector3>();
        Vector3 temp = new Vector3();
        for (int i = 0; i < LineData.Count; i++)
        {

            temp = LineData[i];


            if (camera3D.gameObject.activeSelf == true)
            {
                LineDataForPlot.Add(new Vector3((temp[2] - titleTickZ.min) * titleTickZ.rate / 1000f * paraAxisRate.x + paraAxisShift.x,
                                                    (temp[1] - titleTickY.min) * titleTickY.rate / 1000f * paraAxisRate.y + paraAxisShift.y,
                                                    (temp[0] - titleTickX.min) * titleTickX.rate / 1000f * paraAxisRate.z + paraAxisShift.z));

            }
            else if (cameraXY.gameObject.activeSelf == true)
            {

                if (lineNum == "Line_5")
                {
                    LineDataForPlot.Add(new Vector3(1,
                                                    (temp[1] - titleTickY.min) * titleTickY.rate / 1000f * paraAxisRate.y + paraAxisShift.y,
                                                    (temp[0] - titleTickX.min) * titleTickX.rate / 1000f * paraAxisRate.z + paraAxisShift.z));
                }
                else
                {
                    LineDataForPlot.Add(new Vector3(0,
                                                    (temp[1] - titleTickY.min) * titleTickY.rate / 1000f * paraAxisRate.y + paraAxisShift.y,
                                                    (temp[0] - titleTickX.min) * titleTickX.rate / 1000f * paraAxisRate.z + paraAxisShift.z));
                }
            }
        }

        return LineDataForPlot;
    }

    public static List<Vector3> shiftLineData(List<Vector3> LineData)
    {
        List<Vector3> LineDataForPlot = new List<Vector3>();
        Vector3 temp = new Vector3();
        for (int i = 0; i < LineData.Count; i++)
        {

            temp = LineData[i];

            LineDataForPlot.Add(new Vector3(temp[0],
         temp[1] + (titleTickY.tickData[1] - titleTickY.min) / 2 * titleTickY.rate / 1000f * paraAxisRate.y,
                                                temp[2]));


        }

        return LineDataForPlot;
    }

    void createTitleName()
    {
        GameObject titleName = textLayer.transform.Find("titleName").gameObject;
        GameObject title;
        for (int i = 0; i < titleName.transform.childCount; i++)
        {
            Destroy(titleName.transform.GetChild(i).gameObject);
        }

        title = GameObject.Instantiate(chartTextPrefab);
        title.GetComponent<TMP_Text>().text = "X (m)";
        title.transform.SetParent(titleName.transform);
        title.transform.localScale = new Vector3(1, 1, 1);
        title.transform.localPosition = new Vector3(0, 0, 0);
        title.name = "titleX";


        title = GameObject.Instantiate(chartTextPrefab);
        title.GetComponent<TMP_Text>().text = "Y (m)";
        title.transform.SetParent(titleName.transform);
        title.transform.localScale = new Vector3(1, 1, 1);
        title.transform.localPosition = new Vector3(0, 0, 0);
        title.name = "titleY";


        title = GameObject.Instantiate(chartTextPrefab);
        title.GetComponent<TMP_Text>().text = "Z (m)";
        title.transform.SetParent(titleName.transform);
        title.transform.localScale = new Vector3(1, 1, 1);
        title.transform.position = new Vector3(0, 0, 0);
        title.transform.localPosition = new Vector3(0, 0, 0);
        title.name = "titleZ";


    }

    public static void updateTitleName()
    {
        GameObject titleName = textLayer.transform.Find("titleName").gameObject;
        GameObject title;
        Vector3 tickNamePos = new Vector3();
        Vector2 screenPos = new Vector2();


        //x title
        title = titleName.transform.Find("titleX").gameObject;
        float tempHere = 0;
        if (camera3D.gameObject.activeSelf == true)
        {
            tempHere = (titleTickX.max - titleTickX.min) * titleTickX.rate / 1000f;
            tickNamePos = Vector3.Scale(new Vector3(0, 0, tempHere), paraAxisRate) + new Vector3(0, 0, 15) + paraAxisShift;
        }
        else
        {
            tempHere = (titleTickX.max - titleTickX.min) / 2 * titleTickX.rate / 1000f;
            tickNamePos = Vector3.Scale(new Vector3(0, 0, tempHere), paraAxisRate) + new Vector3(0, -15, 0) + paraAxisShift;
        }

        screenPos = cameraUsed.WorldToScreenPoint(tickNamePos);
        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(800f - screenPos.x / Screen.width * 800f),
                                    -(Screen.height - screenPos.y) / Screen.width * 800f)
            + (new Vector2(800, 800f / Screen.width * Screen.height) - textLayer.GetComponent<RectTransform>().offsetMax);


        //y title
        title = titleName.transform.Find("titleY").gameObject;

        if (camera3D.gameObject.activeSelf == true)
        {
            tempHere = (titleTickY.max - titleTickY.min) * titleTickY.rate / 1000f;
            tickNamePos = Vector3.Scale(new Vector3(0, tempHere, 0), paraAxisRate) + new Vector3(0, 15, 0) + paraAxisShift;
        }
        else
        {
            tempHere = (titleTickY.max - titleTickY.min) / 2f * titleTickY.rate / 1000f;
            tickNamePos = Vector3.Scale(new Vector3(0, tempHere, 0), paraAxisRate) + new Vector3(0, 0, -13) + paraAxisShift;
        }
        screenPos = cameraUsed.WorldToScreenPoint(tickNamePos);
        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(800f - screenPos.x / Screen.width * 800f),
                                    -(Screen.height - screenPos.y) / Screen.width * 800f)
            + (new Vector2(800, 800f / Screen.width * Screen.height) - textLayer.GetComponent<RectTransform>().offsetMax);


        //z title
        title = titleName.transform.Find("titleZ").gameObject;
        tempHere = (titleTickZ.max - titleTickZ.min) * titleTickZ.rate / 1000f;

        tickNamePos = Vector3.Scale(new Vector3(tempHere, 0, 0), paraAxisRate) + new Vector3(15, 0, 0) + paraAxisShift;
        screenPos = cameraUsed.WorldToScreenPoint(tickNamePos);
        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(800f - screenPos.x / Screen.width * 800f),
                                    -(Screen.height - screenPos.y) / Screen.width * 800f)
            + (new Vector2(800, 800f / Screen.width * Screen.height) - textLayer.GetComponent<RectTransform>().offsetMax);

    }

    public static void createTickName()
    {
        if (titleTickX.tickData != null && titleTickY.tickData != null && titleTickZ.tickData != null)
        {
            GameObject tickName;
            GameObject textTick;

            tickName = textLayer.transform.Find("tickName/tickX").gameObject;
            for (int i = 0; i < titleTickX.tickNum + 1; i++)
            {
                if (textLayer.transform.Find("tickName/tickX/textTickX" + i) == false)
                {
                    textTick = GameObject.Instantiate(chartTextPrefab2);
                    textTick.transform.SetParent(tickName.transform);
                    textTick.transform.localScale = new Vector3(1, 1, 1);
                    textTick.transform.localPosition = new Vector3(0, 0, 0);
                    textTick.name = "textTickX" + i;
                }
            }

            tickName = textLayer.transform.Find("tickName/tickY").gameObject;
            for (int i = 0; i < titleTickY.tickNum + 1; i++)
            {
                if (textLayer.transform.Find("tickName/tickY/textTickY" + i) == false)
                {
                    textTick = GameObject.Instantiate(chartTextPrefab2);
                    textTick.transform.SetParent(tickName.transform);
                    textTick.transform.localScale = new Vector3(1, 1, 1);
                    textTick.transform.localPosition = new Vector3(0, 0, 0);
                    textTick.name = "textTickY" + i;
                }
            }

            tickName = textLayer.transform.Find("tickName/tickZ").gameObject;
            for (int i = 0; i < titleTickX.tickNum + 1; i++)
            {
                if (textLayer.transform.Find("tickName/tickZ/textTickZ" + i) == false)
                {
                    textTick = GameObject.Instantiate(chartTextPrefab2);
                    textTick.transform.SetParent(tickName.transform);
                    textTick.transform.localScale = new Vector3(1, 1, 1);
                    textTick.transform.localPosition = new Vector3(0, 0, 0);
                    textTick.name = "textTickZ" + i;
                }
            }

        }
    }

    public static void updateTickName()
    {
        if (titleTickX.tickData != null && titleTickY.tickData != null && titleTickZ.tickData != null)
        {
            GameObject textTick;
            Vector3 tickNamePos = new Vector3();
            Vector2 screenPos = new Vector2();

            //x
            for (int i = 0; i < titleTickX.tickNum + 1; i++)
            {
                textTick = textLayer.transform.Find("tickName/tickX/textTickX" + i).gameObject;

                float tempHere = (titleTickX.tickData[i] - titleTickX.min) * titleTickX.rate / 1000f;
                if (camera3D.gameObject.activeSelf == true)
                {
                    tickNamePos = Vector3.Scale(new Vector3(1, 0, tempHere), paraAxisRate) + new Vector3(5, 0, 5) + paraAxisShift;
                }
                else if (cameraXY.gameObject.activeSelf == true)
                {
                    tickNamePos = Vector3.Scale(new Vector3(0, 0, tempHere), paraAxisRate) + new Vector3(-8, -8, -5) + paraAxisShift;
                }

                screenPos = cameraUsed.WorldToScreenPoint(tickNamePos);
                textTick.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(800f - screenPos.x / Screen.width * 800f),
                                            -(Screen.height - screenPos.y) / Screen.width * 800f)
                     + (new Vector2(800, 800f / Screen.width * Screen.height) - textLayer.GetComponent<RectTransform>().offsetMax);

                textTick.GetComponent<TMP_Text>().text = (titleTickX.tickData[i]).ToString("0.00");

            }
            //y
            for (int i = 0; i < titleTickY.tickNum + 1; i++)
            {
                textTick = textLayer.transform.Find("tickName/tickY/textTickY" + i).gameObject;

                float tempHere = (titleTickY.tickData[i] - titleTickY.min) * titleTickY.rate / 1000f;
                if (camera3D.gameObject.activeSelf == true)
                {
                    tickNamePos = Vector3.Scale(new Vector3(1, tempHere, 0), paraAxisRate) + new Vector3(5, 5, 0) + paraAxisShift;
                }
                else if (cameraXY.gameObject.activeSelf == true)
                {
                    tickNamePos = Vector3.Scale(new Vector3(1, tempHere, 0), paraAxisRate) + new Vector3(-8, 5, -8) + paraAxisShift;
                }
                screenPos = cameraUsed.WorldToScreenPoint(tickNamePos);
                textTick.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(800f - screenPos.x / Screen.width * 800f),
                                            -(Screen.height - screenPos.y) / Screen.width * 800f)
                     + (new Vector2(800, 800f / Screen.width * Screen.height) - textLayer.GetComponent<RectTransform>().offsetMax);

                textTick.GetComponent<TMP_Text>().text = (titleTickY.tickData[i]).ToString("0.00");


            }
            //z
            for (int i = 0; i < titleTickZ.tickNum + 1; i++)
            {
                textTick = textLayer.transform.Find("tickName/tickZ/textTickZ" + i).gameObject;

                float tempHere = (titleTickZ.tickData[i] - titleTickZ.min) * titleTickZ.rate / 1000f;
                tickNamePos = Vector3.Scale(new Vector3(tempHere, 0, 1), paraAxisRate) + new Vector3(0, 0, 5) + paraAxisShift;
                screenPos = cameraUsed.WorldToScreenPoint(tickNamePos);
                textTick.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(800f - screenPos.x / Screen.width * 800f),
                                            -(Screen.height - screenPos.y) / Screen.width * 800f)
                     + (new Vector2(800, 800f / Screen.width * Screen.height) - textLayer.GetComponent<RectTransform>().offsetMax);


                textTick.GetComponent<TMP_Text>().text = (titleTickZ.tickData[i]).ToString("0.00");

            }
        }
    }

    public static void updateTickBaseOnRule3D()
    {

        float rateBlockZX = paraAxisRate.z / paraAxisRate.x;
        float rateBlockYX = paraAxisRate.z / paraAxisRate.y;

        float xzRate = (titleTickX.max - titleTickX.min) / (titleTickZ.max - titleTickZ.min) / rateBlockZX;
        float xyRate = (titleTickX.max - titleTickX.min) / (titleTickY.max - titleTickY.min) / rateBlockYX;

        if (flagIfFixRate == 0)
        {
            if (xzRate >= 1 && xyRate >= 1)
            {
                rateX.text = "1.0";
                rateZ.text = xzRate.ToString("0.0");
                rateY.text = xyRate.ToString("0.0");
                inputRateXYZ[0] = 1;
                inputRateXYZ[2] = xzRate;
                inputRateXYZ[1] = xyRate;
            }
            else if ((1f / xzRate) > 1 && (xyRate / xzRate) >= 1)
            {
                rateX.text = (1f / xzRate).ToString("0.0");
                rateZ.text = "1.0";
                rateY.text = (xyRate / xzRate).ToString("0.0");
                inputRateXYZ[0] = 1f / xzRate;
                inputRateXYZ[2] = 1;
                inputRateXYZ[1] = xyRate / xzRate;
            }
            else
            {
                rateX.text = (1f / xyRate).ToString("0.0");
                rateZ.text = (xzRate / xyRate).ToString("0.0");
                rateY.text = "1.0";
                inputRateXYZ[0] = 1f / xyRate;
                inputRateXYZ[2] = xzRate / xyRate;
                inputRateXYZ[1] = 1;
            }

        }
        else if (flagIfFixRate == 1)
        {
            float inputRateXZ = inputRateXYZ[2] / inputRateXYZ[0];// z/x
            float inputRateXY = inputRateXYZ[1] / inputRateXYZ[0];// z/x

            int flagEq = 0;
            if (inputRateXZ == 1 && inputRateXY == 1)
            {
                if (xzRate >= 1 && xyRate >= 1)
                    flagEq = 1;
                else if ((1f / xzRate) > 1 && (xyRate / xzRate) >= 1)
                    flagEq = 2;
                else 
                    flagEq = 3;
            }
            else
            {
                if (inputRateXZ == 1 || inputRateXY == 1 || inputRateXZ / inputRateXY == 1)
                {
                    if (xzRate >= 1 && xyRate >= 1)
                        flagEq = 1;
                    else if ((1f / xzRate) > 1 && (xyRate / xzRate) >= 1)
                        flagEq = 2;
                    else 
                        flagEq = 3;
                }

            }





            if ((inputRateXZ > 1 && inputRateXY > 1) || flagEq == 1)
            {
                float tickRangeTemp;
                float edgeRate = 0.1f;
                float mid;

                tickRangeTemp = (titleTickX.max - titleTickX.min);
                mid = (titleTickZ.max + titleTickZ.min) / 2;
                titleTickZ.max = mid + tickRangeTemp / 2 / (inputRateXZ * rateBlockZX);
                titleTickZ.min = mid - tickRangeTemp / 2 / (inputRateXZ * rateBlockZX);
                tickRangeTemp = titleTickZ.max - titleTickZ.min;
                for (int i = 0; i < titleTickZ.tickNum + 1; i++)
                {
                    titleTickZ.tickData[i] = (i * tickRangeTemp / titleTickZ.tickNum + titleTickZ.min);
                }
                titleTickZ.rate = 1000f / tickRangeTemp;

                tickRangeTemp = (titleTickX.max - titleTickX.min);
                mid = (titleTickY.max + titleTickY.min) / 2;
                titleTickY.max = mid + tickRangeTemp / 2 / (inputRateXY * rateBlockYX);
                titleTickY.min = mid - tickRangeTemp / 2 / (inputRateXY * rateBlockYX);
                tickRangeTemp = titleTickY.max - titleTickY.min;
                for (int i = 0; i < titleTickY.tickNum + 1; i++)
                {
                    titleTickY.tickData[i] = (i * tickRangeTemp / titleTickY.tickNum + titleTickY.min);
                }
                titleTickY.rate = 1000f / tickRangeTemp;

            }
            else if (((1f / inputRateXZ) > 1 && (inputRateXY / inputRateXZ) > 1) || flagEq == 2)
            {
                float tickRangeTemp;
                float edgeRate = 0.1f;
                float mid;

                tickRangeTemp = titleTickZ.max - titleTickZ.min;
                mid = (titleTickX.max + titleTickX.min) / 2;
                titleTickX.max = mid + tickRangeTemp / 2 / (1 / (inputRateXZ * rateBlockZX));
                titleTickX.min = mid - tickRangeTemp / 2 / (1 / (inputRateXZ * rateBlockZX));
                tickRangeTemp = titleTickX.max - titleTickX.min;
                for (int i = 0; i < titleTickX.tickNum + 1; i++)
                {
                    titleTickX.tickData[i] = (i * tickRangeTemp / titleTickX.tickNum + titleTickX.min);
                }
                titleTickX.rate = 1000f / tickRangeTemp;


                tickRangeTemp = titleTickZ.max - titleTickZ.min;
                mid = (titleTickY.max + titleTickY.min) / 2;
                titleTickY.max = mid + tickRangeTemp / 2 / ((inputRateXY * rateBlockYX) / (inputRateXZ * rateBlockZX));
                titleTickY.min = mid - tickRangeTemp / 2 / ((inputRateXY * rateBlockYX) / (inputRateXZ * rateBlockZX));
                tickRangeTemp = titleTickY.max - titleTickY.min;
                for (int i = 0; i < titleTickY.tickNum + 1; i++)
                {
                    titleTickY.tickData[i] = (i * tickRangeTemp / titleTickY.tickNum + titleTickY.min);
                }
                titleTickY.rate = 1000f / tickRangeTemp;

            }
            else if(((inputRateXZ > 1 && inputRateXY > 1) == false &&
                ((1f / inputRateXZ) > 1 && (inputRateXY / inputRateXZ) > 1) == false) ||
                      flagEq == 3)
            {
                float tickRangeTemp;
                float edgeRate = 0.1f;
                float mid;

                tickRangeTemp = titleTickY.max - titleTickY.min;
                mid = (titleTickX.max + titleTickX.min) / 2;
                titleTickX.max = mid + tickRangeTemp / 2 / (1 / (inputRateXY * rateBlockYX));
                titleTickX.min = mid - tickRangeTemp / 2 / (1 / (inputRateXY * rateBlockYX));
                tickRangeTemp = titleTickX.max - titleTickX.min;
                for (int i = 0; i < titleTickX.tickNum + 1; i++)
                {
                    titleTickX.tickData[i] = (i * tickRangeTemp / titleTickX.tickNum + titleTickX.min);
                }
                titleTickX.rate = 1000f / tickRangeTemp;

                tickRangeTemp = titleTickY.max - titleTickY.min;
                mid = (titleTickZ.max + titleTickZ.min) / 2;
                titleTickZ.max = mid + tickRangeTemp / 2 / ((inputRateXZ * rateBlockZX) / (inputRateXY * rateBlockYX));
                titleTickZ.min = mid - tickRangeTemp / 2 / ((inputRateXZ * rateBlockZX) / (inputRateXY * rateBlockYX));
                tickRangeTemp = titleTickZ.max - titleTickZ.min;
                for (int i = 0; i < titleTickZ.tickNum + 1; i++)
                {
                    titleTickZ.tickData[i] = (i * tickRangeTemp / titleTickZ.tickNum + titleTickZ.min);
                }
                titleTickZ.rate = 1000f / tickRangeTemp;
            }


        }

        realAxisRateZX = (titleTickX.max - titleTickX.min) / (titleTickZ.max - titleTickZ.min) / rateBlockZX;
        realAxisRateYX = (titleTickX.max - titleTickX.min) / (titleTickY.max - titleTickY.min) / rateBlockYX;



    }


}
