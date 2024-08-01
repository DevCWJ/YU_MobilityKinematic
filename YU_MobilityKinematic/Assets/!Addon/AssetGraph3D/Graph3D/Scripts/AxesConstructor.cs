using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[ExecuteInEditMode]
public class AxesConstructor : MonoBehaviour
{
    public Transform panelAxes;
    public Transform axesPivot;
    public Transform axesCenter;
    public string LabelX, LabelY, LabelZ;

    [SerializeField]
    private float _sizeX = 500, _sizeY = 500, _sizeZ = 500; 

    private Transform X, Xf, Xb, Xd;
    private Transform Y, Yl, Yf, Yr;
    private Transform Z, Zf, Zb, Zd;
    private Transform labelX, labelY, labelZ;
    private Transform XScaleL, XScaleH;
    private Transform YScaleL, YScaleH;
    private Transform ZScaleL, ZScaleH;

    private void Awake()
    {        
        Init();
    }

    private void Init()
    {
        X = axesCenter.Find("X").GetComponent<Transform>();
        Xf = X.Find("Xf").GetComponent<Transform>();
        Xb = X.Find("Xb").GetComponent<Transform>();
        Xd = X.Find("Xd").GetComponent<Transform>();

        Y = axesCenter.Find("Y").GetComponent<Transform>();
        Yl = Y.Find("Yl").GetComponent<Transform>();
        Yf = Y.Find("Yf").GetComponent<Transform>();
        Yr = Y.Find("Yr").GetComponent<Transform>();

        Z = axesCenter.Find("Z").GetComponent<Transform>();
        Zf = Z.Find("Zf").GetComponent<Transform>();
        Zb = Z.Find("Zb").GetComponent<Transform>();
        Zd = Z.Find("Zd").GetComponent<Transform>();

        labelX = X.Find("Label");
        labelY = Y.Find("Label");
        labelZ = Z.Find("Label");

        XScaleL = X.Find("ScaleL");
        XScaleH = X.Find("ScaleH");
        YScaleL = Yf.Find("ScaleL");
        YScaleH = Yf.Find("ScaleH");
        ZScaleL = Zd.Find("ScaleL");
        ZScaleH = Zd.Find("ScaleH");
    }


    public float sizeX 
    {
        set {
            _sizeX = value;
            if (value < 1) _sizeX = 1;
            DataChanged();
        }
        get { return _sizeX; }
    }   

    public float sizeY
    {
        set {
            _sizeY = value;
            if (value < 1) _sizeY = 1;
            DataChanged();
        }
        get { return _sizeY; }
    }   
    public float sizeZ
    {
        set {
            _sizeZ = value;
            if (value < 1) _sizeZ = 1;
            DataChanged();
        }
        get { return _sizeZ; }
    }   

    private void UpdateAxes()
    {
        //CenterAxe
        axesCenter.localPosition = new Vector3(axesPivot.localPosition.x - sizeX / 2, axesPivot.localPosition.y - sizeY / 2, axesPivot.localPosition.z - sizeZ / 2);
        //X
        X.position = axesCenter.position;
        X.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
        X.GetComponent<LineRenderer>().SetPosition(1, new Vector3(sizeX, 0, 0));
        //Xf
        Xf.position = X.position;
        Xf.localPosition += new Vector3(0, sizeY, 0);          
        Xf.GetComponent<LineRenderer>().SetPosition(0, X.GetComponent<LineRenderer>().GetPosition(0));
        Xf.GetComponent<LineRenderer>().SetPosition(1, X.GetComponent<LineRenderer>().GetPosition(1));
        //Xb
        Xb.position = X.position;
        Xb.localPosition += new Vector3(0, sizeY, sizeZ);
        Xb.GetComponent<LineRenderer>().SetPosition(0, X.GetComponent<LineRenderer>().GetPosition(0));
        Xb.GetComponent<LineRenderer>().SetPosition(1, X.GetComponent<LineRenderer>().GetPosition(1));
        //Xd
        Xd.position = X.position;
        Xd.localPosition += new Vector3(0, 0, sizeZ);
        Xd.GetComponent<LineRenderer>().SetPosition(0, X.GetComponent<LineRenderer>().GetPosition(0));
        Xd.GetComponent<LineRenderer>().SetPosition(1, X.GetComponent<LineRenderer>().GetPosition(1));
        //Y
        Y.position = axesCenter.position;
        Y.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
        Y.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, sizeY, 0));
        //Yl
        Yl.position = Y.position;
        Yl.localPosition += new Vector3(sizeX, 0, 0);
        Yl.GetComponent<LineRenderer>().SetPosition(0, Y.GetComponent<LineRenderer>().GetPosition(0));
        Yl.GetComponent<LineRenderer>().SetPosition(1, Y.GetComponent<LineRenderer>().GetPosition(1));
        //Yr
        Yr.position = Y.position;
        Yr.localPosition += new Vector3(0, 0, sizeZ);
        Yr.GetComponent<LineRenderer>().SetPosition(0, Y.GetComponent<LineRenderer>().GetPosition(0));
        Yr.GetComponent<LineRenderer>().SetPosition(1, Y.GetComponent<LineRenderer>().GetPosition(1));
        //Yf
        Yf.position = Y.position;
        Yf.localPosition += new Vector3(sizeX, 0, sizeZ);
        Yf.GetComponent<LineRenderer>().SetPosition(0, Y.GetComponent<LineRenderer>().GetPosition(0));
        Yf.GetComponent<LineRenderer>().SetPosition(1, Y.GetComponent<LineRenderer>().GetPosition(1));
        //Z
        Z.position = axesCenter.position;
        Z.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
        Z.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, sizeZ));
        X.GetComponent<LineRenderer>().SetPosition(1, new Vector3(sizeX, 0, 0));
        //Zf
        Zf.position = Z.position;
        Zf.localPosition += new Vector3(sizeX, sizeY, 0);
        Zf.GetComponent<LineRenderer>().SetPosition(0, Z.GetComponent<LineRenderer>().GetPosition(0));
        Zf.GetComponent<LineRenderer>().SetPosition(1, Z.GetComponent<LineRenderer>().GetPosition(1));
        //Zb
        Zb.position = Z.position;
        Zb.localPosition += new Vector3(0, sizeY, 0);
        Zb.GetComponent<LineRenderer>().SetPosition(0, Z.GetComponent<LineRenderer>().GetPosition(0));
        Zb.GetComponent<LineRenderer>().SetPosition(1, Z.GetComponent<LineRenderer>().GetPosition(1));
        //Zd
        Zd.position = Z.position;
        Zd.localPosition += new Vector3(sizeX, 0, 0);
        Zd.GetComponent<LineRenderer>().SetPosition(0, Z.GetComponent<LineRenderer>().GetPosition(0));
        Zd.GetComponent<LineRenderer>().SetPosition(1, Z.GetComponent<LineRenderer>().GetPosition(1));
    }    

    private void DataChanged()
    {            
        Init();
        UpdateAxes();

        float sizeXZ = Mathf.Sqrt(sizeX * sizeX + sizeZ * sizeZ);
        float sizeXYZ = Mathf.Sqrt(sizeX * sizeX + sizeZ * sizeZ + sizeY * sizeY);
        panelAxes.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeXZ, sizeXYZ);
        panelAxes.localPosition = new Vector3(panelAxes.localPosition.x, panelAxes.localPosition.y, -sizeXYZ * 1.5f);       
    }
   
    void OnValidate ( ) 
    {
        //Debug.Log("OnValidate ( )");
        sizeX = _sizeX ;
        sizeY = _sizeY ;
        sizeZ = _sizeZ ;   

        labelX.GetComponent<Text>().text = LabelX; 
        labelY.GetComponent<Text>().text = LabelY;
        labelZ.GetComponent<Text>().text = LabelZ;

        labelX.localPosition = new Vector3(X.GetComponent<LineRenderer>().GetPosition(1).x, Y.GetComponent<LineRenderer>().GetPosition(1).y, 0);
        labelY.localPosition = new Vector3(0, Y.GetComponent<LineRenderer>().GetPosition(1).y, 0);
        labelZ.localPosition = new Vector3(0, Y.GetComponent<LineRenderer>().GetPosition(1).y, Z.GetComponent<LineRenderer>().GetPosition(1).z);

        XScaleL.localPosition = new Vector3(X.GetComponent<LineRenderer>().GetPosition(0).x + 40f, 0, 0);
        XScaleH.localPosition = new Vector3(X.GetComponent<LineRenderer>().GetPosition(1).x - 40f, 0, 0);
        YScaleL.localPosition = new Vector3(0, Yf.GetComponent<LineRenderer>().GetPosition(0).y + 40f, 0);
        YScaleH.localPosition = new Vector3(0, Yf.GetComponent<LineRenderer>().GetPosition(1).y - 40f, 0);
        ZScaleL.localPosition = new Vector3(0, 0, Zd.GetComponent<LineRenderer>().GetPosition(0).z + 40f);
        ZScaleH.localPosition = new Vector3(0, 0, Zd.GetComponent<LineRenderer>().GetPosition(1).z - 40f);        
    }  

}


