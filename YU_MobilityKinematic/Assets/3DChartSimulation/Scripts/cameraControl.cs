using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour
{
    private Vector3 cameraInitPos;
    private Vector3 cameraInitAng;

    // Start is called before the first frame update
    void Start()
    {
        cameraInitPos = this.gameObject.transform.localPosition;
        cameraInitAng = this.gameObject.transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {

            this.transform.Translate(new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 2800));

        }

        if (Input.GetMouseButton(2))
        {
            this.gameObject.transform.Translate(Vector3.left * Input.GetAxis("Mouse X") * 5);
            this.gameObject.transform.Translate(Vector3.up * Input.GetAxis("Mouse Y") * -5);

        }

        if (Input.GetMouseButton(1))
        {
            this.gameObject.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, (Input.GetAxis("Mouse X") * 5));
            this.gameObject.transform.RotateAround(new Vector3(0, 0, 0), transform.right, Input.GetAxis("Mouse Y") * 5);

        }

        if (Input.GetKey(KeyCode.K))
        {

            this.gameObject.transform.localPosition = cameraInitPos;
            this.gameObject.transform.localEulerAngles = cameraInitAng;
        }
    }
}
