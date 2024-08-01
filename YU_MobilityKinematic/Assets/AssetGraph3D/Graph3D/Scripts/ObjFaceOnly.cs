using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [ExecuteInEditMode]
public class ObjFaceOnly : MonoBehaviour
{
     private void Update()
     {
        var dir = Camera.main.transform.position - transform.position;
        transform.LookAt(transform.position - dir);
     }
     
}
