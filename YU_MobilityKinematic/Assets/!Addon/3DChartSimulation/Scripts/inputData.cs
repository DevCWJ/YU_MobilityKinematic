using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class inputData : MonoBehaviour
{
    TextAsset readT;
    public struct dataGroup
    {
        public float posX;
        public float posY;
        public float posZ;
        public float angTheta;
    }

    public static List<dataGroup> lineData1;
    public static List<dataGroup> lineData2;
    public static List<dataGroup> lineData3;
    public static List<dataGroup> lineData4;
    public static List<dataGroup> lineData5;

    public static float lengthLineData2;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        lineData1 = new List<dataGroup>();
        lineData2 = new List<dataGroup>();
        lineData3 = new List<dataGroup>();
        lineData4 = new List<dataGroup>();
        lineData5 = new List<dataGroup>();

        ImportLineData();

        lengthLineData2 = 0;
        for (int i = 0; i < lineData2.Count - 1; i++)
        {
            lengthLineData2 += Mathf.Sqrt(Mathf.Pow((lineData2[i].posX - lineData2[i + 1].posX), 2) +
            Mathf.Pow((lineData2[i].posY - lineData2[i + 1].posY), 2) +
            Mathf.Pow((lineData2[i].posZ - lineData2[i + 1].posZ), 2));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    void ImportLineData()
    {
        TextAsset readT = Resources.Load<TextAsset>("inputData");
        string readS = readT.text;

        //read data to list
        List<string> readSentences = new List<string>();
        int num = 0;
        for (int i = 0; i < readS.Length; i++)
        {
            string readC = readS.Substring(i, 1);
            if (readC == "\n")
            {
                num += 1;
                continue;
            }
            if (readSentences.Count <= num)
            {
                readSentences.Add(readC);
            }
            else
            {
                readSentences[readSentences.Count - 1] += readC;
            }
        }

        //read list to struct
        for (int i = 1; i < readSentences.Count; i++)
        {
            string[] msg = readSentences[i].Split(',');
            for (int j = 0; j < msg.Length; j = j + 4)
            {
                dataGroup dataGroup = new dataGroup();
                dataGroup.posX = (float)Convert.ToDouble(msg[j]);
                dataGroup.posY = (float)Convert.ToDouble(msg[j + 1]);
                dataGroup.posZ = (float)Convert.ToDouble(msg[j + 2]);
                dataGroup.angTheta = (float)Convert.ToDouble(msg[j + 3]);
                if (j == 0) lineData1.Add(dataGroup);
                else if (j == 4) lineData2.Add(dataGroup);
                else if (j == 8) lineData3.Add(dataGroup);
                else if (j == 12) lineData4.Add(dataGroup);
                else if (j == 16) lineData5.Add(dataGroup);
            }
        }

    }
}
