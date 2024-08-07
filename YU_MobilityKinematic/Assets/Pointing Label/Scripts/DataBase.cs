using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poitinglabel
{
    public class DataBase : MonoBehaviour
    {

        // The Database containing a serie of 1 object Name, 1 description, 1 object Name, 1 description,...
        public string[] database;

        private Dictionary<string, string> dic = new Dictionary<string, string>();

        // Use this for initialization
        void Start()
        {
            // Fill the dictionnary with database parameter
            for (int i = 0; i < database.Length; i++)
            {
                dic.Add(database[i], database[i + 1]);
                i++;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        // Returns the description associated to the object name.
        public string getText(string name)
        {
            return dic[name];
        }
    }
}