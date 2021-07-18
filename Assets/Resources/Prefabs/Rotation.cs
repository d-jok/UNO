using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
	private int rotationSpeed;
	private GameObject mObj;

    // Start is called before the first frame update
    void Start()
    {
		rotationSpeed = 20;
		mObj = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
		mObj.transform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);
    }
}
