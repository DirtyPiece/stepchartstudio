using UnityEngine;
using System.Collections;

public class CameraMovementScript : MonoBehaviour {
	public GameObject objectToRotateAbout;
	public float rotationDegreesPerSecond = 30.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float horizontalAxis = Input.GetAxis ("Horizontal");
		transform.RotateAround (objectToRotateAbout.transform.position, new Vector3 (0, 1, 0), -horizontalAxis * rotationDegreesPerSecond * Time.deltaTime);
	}
}
