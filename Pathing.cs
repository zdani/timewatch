using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathing : MonoBehaviour {

	public float destination = 0f;
	public float Speed = 50;
	private Transform myTransform;
	private float startX;
	private float endX;
	// Use this for initialization
	void Start () {
		myTransform = this.transform;
		endX = destination;
		startX = myTransform.transform.position.x;
	}

	// Update is called once per frame
	void Update () {
		float moveAmount = Speed * Time.deltaTime;
		if (myTransform.position.x.Equals(endX)){
			float temp = endX;
			endX = startX;
			startX = temp;
		}
		Vector3 dest = new Vector3 (endX, myTransform.position.y, myTransform.position.z);
		myTransform.position = Vector3.MoveTowards(myTransform.position, dest, moveAmount);
	}

	
}
