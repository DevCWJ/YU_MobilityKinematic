using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poitinglabel
{
    public class TurningCamera : MonoBehaviour
    {

        public Transform target;

        public void Update()
        {
            transform.LookAt(target);
            transform.Translate(Vector3.right * Time.deltaTime);
        }

    }
}