using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class AxesConstructor : MonoBehaviour
{
    public Transform panelAxes;
    public Transform axesPivot;
    public Transform axesCenter;
    public string LabelX, LabelY, LabelZ;

    [SerializeField]
    private float _sizeX = 500, _sizeY = 500, _sizeZ = 500; 

    private LineRenderer X, Xf, Xb, Xd;
    private LineRenderer Y, Yl, Yf, Yr;
    private LineRenderer Z, Zf, Zb, Zd;
    private TextMeshPro labelX, labelY, labelZ;
    private Transform XScaleL, XScaleH;
    private Transform YScaleL, YScaleH;
    private Transform ZScaleL, ZScaleH;

    private void Awake()
    {        
        Init();
    }

    private void Init()
    {
        X = axesCenter.Find("X").GetComponent<LineRenderer>();
        Xf = X.transform.Find("Xf").GetComponent<LineRenderer>();
        Xb = X.transform.Find("Xb").GetComponent<LineRenderer>();
        Xd = X.transform.Find("Xd").GetComponent<LineRenderer>();

        Y = axesCenter.Find("Y").GetComponent<LineRenderer>();
        Yl = Y.transform.Find("Yl").GetComponent<LineRenderer>();
        Yf = Y.transform.Find("Yf").GetComponent<LineRenderer>();
        Yr = Y.transform.Find("Yr").GetComponent<LineRenderer>();

        Z = axesCenter.Find("Z").GetComponent<LineRenderer>();
        Zf = Z.transform.Find("Zf").GetComponent<LineRenderer>();
        Zb = Z.transform.Find("Zb").GetComponent<LineRenderer>();
        Zd = Z.transform.Find("Zd").GetComponent<LineRenderer>();

        labelX = X.transform.Find("Label").GetComponent<TextMeshPro>();
        labelY = Y.transform.Find("Label").GetComponent<TextMeshPro>();
        labelZ = Z.transform.Find("Label").GetComponent<TextMeshPro>();

        XScaleL = X.transform.Find("ScaleL");
        XScaleH = X.transform.Find("ScaleH");
        YScaleL = Yf.transform.Find("ScaleL");
        YScaleH = Yf.transform.Find("ScaleH");
        ZScaleL = Zd.transform.Find("ScaleL");
        ZScaleH = Zd.transform.Find("ScaleH");
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
        X.transform.position = axesCenter.position;
        X.SetPosition(0, new Vector3(0, 0, 0));
        X.SetPosition(1, new Vector3(sizeX, 0, 0));
        //Xf
        Xf.transform.position = X.transform.position;
        Xf.transform.localPosition += new Vector3(0, sizeY, 0);
        Xf.SetPosition(0, X.GetPosition(0));
        Xf.SetPosition(1, X.GetPosition(1));
        //Xb
        Xb.transform.position = X.transform.position;
        Xb.transform.localPosition += new Vector3(0, sizeY, sizeZ);
        Xb.SetPosition(0, X.GetPosition(0));
        Xb.SetPosition(1, X.GetPosition(1));
        //Xd
        Xd.transform.position = X.transform.position;
        Xd.transform.localPosition += new Vector3(0, 0, sizeZ);
        Xd.SetPosition(0, X.GetPosition(0));
        Xd.SetPosition(1, X.GetPosition(1));
        //Y
        Y.transform.position = axesCenter.position;
        Y.SetPosition(0, new Vector3(0, 0, 0));
        Y.SetPosition(1, new Vector3(0, sizeY, 0));
        //Yl
        Yl.transform.position = Y.transform.position;
        Yl.transform.localPosition += new Vector3(sizeX, 0, 0);
        Yl.SetPosition(0, Y.GetPosition(0));
        Yl.SetPosition(1, Y.GetPosition(1));
        //Yr
        Yr.transform.position = Y.transform.position;
        Yr.transform.localPosition += new Vector3(0, 0, sizeZ);
        Yr.SetPosition(0, Y.GetPosition(0));
        Yr.SetPosition(1, Y.GetPosition(1));
        //Yf
        Yf.transform.position = Y.transform.position;
        Yf.transform.localPosition += new Vector3(sizeX, 0, sizeZ);
        Yf.SetPosition(0, Y.GetPosition(0));
        Yf.SetPosition(1, Y.GetPosition(1));
        //Z
        Z.transform.position = axesCenter.position;
        Z.SetPosition(0, new Vector3(0, 0, 0));
        Z.SetPosition(1, new Vector3(0, 0, sizeZ));
        X.SetPosition(1, new Vector3(sizeX, 0, 0));
        //Zf
        Zf.transform.position = Z.transform.position;
        Zf.transform.localPosition += new Vector3(sizeX, sizeY, 0);
        Zf.SetPosition(0, Z.GetPosition(0));
        Zf.SetPosition(1, Z.GetPosition(1));
        //Zb
        Zb.transform.position = Z.transform.position;
        Zb.transform.localPosition += new Vector3(0, sizeY, 0);
        Zb.SetPosition(0, Z.GetPosition(0));
        Zb.SetPosition(1, Z.GetPosition(1));
        //Zd
        Zd.transform.position = Z.transform.position;
        Zd.transform.localPosition += new Vector3(sizeX, 0, 0);
        Zd.SetPosition(0, Z.GetPosition(0));
        Zd.SetPosition(1, Z.GetPosition(1));
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

        labelX.SetText(LabelX);
        labelY.SetText(LabelY);
        labelZ.SetText(LabelZ);

        labelX.transform.localPosition = new Vector3(X.GetPosition(1).x, Y.GetPosition(1).y, 0);
        labelY.transform.localPosition = new Vector3(0, Y.GetPosition(1).y, 0);
        labelZ.transform.localPosition = new Vector3(0, Y.GetPosition(1).y, Z.GetPosition(1).z);

        XScaleL.localPosition = new Vector3(X.GetPosition(0).x + 40f, 0, 0);
        XScaleH.localPosition = new Vector3(X.GetPosition(1).x - 40f, 0, 0);
        YScaleL.localPosition = new Vector3(0, Yf.GetPosition(0).y + 40f, 0);
        YScaleH.localPosition = new Vector3(0, Yf.GetPosition(1).y - 40f, 0);
        ZScaleL.localPosition = new Vector3(0, 0, Zd.GetPosition(0).z + 40f);
        ZScaleH.localPosition = new Vector3(0, 0, Zd.GetPosition(1).z - 40f);        
    }  

}


