using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParseCSV : MonoBehaviour
{  
    // The csv file can be dragged throughthe inspector * Перетащите в инсепкторе файл csv 
    public TextAsset csvFile;
    // The csvData in which the CSV File would be parsed * Данные из файла csvFile будут спарсены в csvData   
    public string[,] csvData;

    void Awake()
    {
        csvData = getCSVData(csvFile.text);
        //DisplayCSVData(csvData);
    }


    static public string[,] getCSVData(string csvText)
    { 
        //Split the data on split line character * Разделим данные на символьные строки
        string[] delimiterChars = { "\n"};
        string[] lines = csvText.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);

        //Find the max number of columns * Определим максимальное количество колонок
        int totalColumns = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(';');
            totalColumns = Mathf.Max(totalColumns, row.Length);
        }

        //Сreates new 2D string array * Создаем новый двумерный массив строк    
        string[,] outputData = new string[lines.Length, totalColumns];
        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = lines[y].Split(';');
            for (int x = 0; x < row.Length; x++)
            {
                outputData[y, x] = row[x];
            }
        }
        return outputData;
    }

    //Logging parsed data * Вывод в лог спарсенных данных
    static public void DisplayCSVData(string[,] csvData)
    {
        string textOutput = "";
        for (int x = 0; x <= csvData.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= csvData.GetUpperBound(1); y++)
            {

                textOutput += csvData[x, y];
                textOutput += ",";
            }
            textOutput += "\n";
        }
        Debug.Log(textOutput);       
    }
    
}
