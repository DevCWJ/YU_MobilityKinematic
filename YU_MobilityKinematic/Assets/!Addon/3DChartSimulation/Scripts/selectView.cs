using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectView : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSelectView(int flag)
    {
        //flag 0 3D ; 1 XY
        if (flag == 0)
        {
            chartControl.cameraUsed = chartControl.camera3D;
            chartControl.camera3D.gameObject.SetActive(true);
            chartControl.cameraXY.gameObject.SetActive(false);
            chartControl.textLayer.transform.Find("tickName/tickX").gameObject.SetActive(true);
            chartControl.textLayer.transform.Find("tickName/tickY").gameObject.SetActive(true);
            chartControl.textLayer.transform.Find("tickName/tickZ").gameObject.SetActive(true);
            chartControl.textLayer.transform.Find("titleName/titleX").gameObject.SetActive(true);
            chartControl.textLayer.transform.Find("titleName/titleY").gameObject.SetActive(true);
            chartControl.textLayer.transform.Find("titleName/titleZ").gameObject.SetActive(true);
        }
        else if (flag == 1)
        {
            chartControl.cameraUsed = chartControl.cameraXY;
            chartControl.camera3D.gameObject.SetActive(false);
            chartControl.cameraXY.gameObject.SetActive(true);
            chartControl.textLayer.transform.Find("tickName/tickX").gameObject.SetActive(true);
            chartControl.textLayer.transform.Find("tickName/tickY").gameObject.SetActive(true);
            chartControl.textLayer.transform.Find("tickName/tickZ").gameObject.SetActive(false);
            chartControl.textLayer.transform.Find("titleName/titleX").gameObject.SetActive(true);
            chartControl.textLayer.transform.Find("titleName/titleY").gameObject.SetActive(true);
            chartControl.textLayer.transform.Find("titleName/titleZ").gameObject.SetActive(false);
        }


    }

    public void OnFixRate()
    {

        GameObject imageYes = this.transform.Find("ImageYes").gameObject;
        GameObject imageNo = this.transform.Find("ImageNo").gameObject;


        if (imageNo.activeSelf == true && imageYes.activeSelf == false)
        {
            imageNo.SetActive(false);
            imageYes.SetActive(true);

            chartControl.flagIfFixRate = 1;


        }
        else if (imageNo.activeSelf == false && imageYes.activeSelf == true)
        {
            imageNo.SetActive(true);
            imageYes.SetActive(false);

            chartControl.flagIfFixRate = 0;

        }
        chartControl.updateDataForPlot();
        chartControl.calculateAxisRange();
        chartControl.updateTickBaseOnRule3D();
        chartControl.createAxisLine();
        chartControl.createGridLine();
        chartControl.updateMarker();
        if (chartControl.timeLabel > 1)
        {
            chartControl.createDataLine();
            chartControl.updateCube();
        }
        chartControl.updateTitleName();
        chartControl.createTickName();
        chartControl.updateTickName();

    }
}
