using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BearingPrinter : MonoBehaviour {
    public Text bearingText;
    
	// Update is called once per frame
	void Update () {
		if (bearingText!= null) {
            //bearingText.text = Camera.main.transform.rotation.y.ToString("F2");
            string number= Camera.main.transform.rotation.y.ToString("F2");
            string dir = "N";
            float yRot = Camera.main.transform.rotation.y;


            var v = transform.forward;
            v.y = 0;
            v.Normalize();

            //if (Vector3.Angle(v, Vector3.forward) <= 45.0) {
            //    //Debug.Log("North");
            //    dir = "N";
            //}
            //else if (Vector3.Angle(v, Vector3.right) <= 45.0) {
            //    //Debug.Log("East");
            //    dir = "E";
            //}
            //else if (Vector3.Angle(v, Vector3.back) <= 45.0) {
            //    //Debug.Log("South");
            //    dir = "S";
            //}
            //else {
            //    //Debug.Log("West");
            //    dir = "W";
            //}

            if (Vector3.Angle(v, Vector3.forward) <= 45.0) {
                //Debug.Log("North");
                dir = "S";
            }
            else if (Vector3.Angle(v, Vector3.right) <= 45.0) {
                //Debug.Log("East");
                dir = "W";
            }
            else if (Vector3.Angle(v, Vector3.back) <= 45.0) {
                //Debug.Log("South");
                dir = "N";
            }
            else {
                //Debug.Log("West");
                dir = "E";
            }

            bearingText.text = dir + "(" + number + ")";
        }
	}
}
