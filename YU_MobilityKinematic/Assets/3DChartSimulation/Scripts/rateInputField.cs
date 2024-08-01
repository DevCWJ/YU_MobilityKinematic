using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rateInputField : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region rate inputfield

        chartControl.rateX.onValueChanged.AddListener(delegate
        {
            float temp = 0;
            OnInputFieldValueChangedFlt(chartControl.rateX, ref temp);
            if (temp != 0 && temp > 0)
                chartControl.inputRateXYZ[0] = temp;

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


        });
        chartControl.rateZ.onValueChanged.AddListener(delegate
        {
            float temp = 0;
            OnInputFieldValueChangedFlt(chartControl.rateZ, ref temp);
            if (temp != 0 && temp > 0)
                chartControl.inputRateXYZ[2] = temp;


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
        });
        chartControl.rateY.onValueChanged.AddListener(delegate
        {
            float temp = 0;
            OnInputFieldValueChangedFlt(chartControl.rateY, ref temp);
            if (temp != 0 && temp > 0)
                chartControl.inputRateXYZ[1] = temp;


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
        });
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnInputFieldValueChangedFlt(InputField InputField, ref float output)
    {
        string str = InputField.text;
        str = System.Text.RegularExpressions.Regex.Replace(str, @"[^\d.\d]", " ");

        if (float.TryParse(str, out float temp))
            output = temp;
    }

}
