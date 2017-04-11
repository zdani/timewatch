using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public struct LinePointDistance : IComparable<LinePointDistance>
{
	private float distance;
	private RayLine line;
	private Vector2 point;

	public LinePointDistance(float distance, RayLine line, Vector2 point){
		this.distance = distance;
		this.line = line;
		this.point = point;
	}

	public LinePointDistance(RayLine line, Vector2 point, Vector3 intersect){
		this.distance = Vector3.Distance (intersect, point);
		this.line = line;
		this.point = point;
	}

	public float getDistance(){return distance;}
	public Vector2 getPoint(){return point;}
	public RayLine getLine(){return line;}

	public int CompareTo(LinePointDistance other){
		return distance.CompareTo(other.distance);
	}

	public bool isWithinAcceptableRange(float minDistanceAllowed){
		if (distance < minDistanceAllowed) {
			return true;
		} else {
			Debug.Log ("Min distance found / allowed: " + distance + " / " + minDistanceAllowed);
			return false;
		}
	}
}
