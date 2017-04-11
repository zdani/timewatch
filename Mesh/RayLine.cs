using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RayLine
{
	public Vector3 p1;
	public Vector3 p2;
	public float slope;
	public float y_intercept;
	bool isParralelToYAxis;

	public RayLine(Vector3 p1, Vector3 p2){
		this.p1 = p1;
		this.p2 = p2;
		isParralelToYAxis = (p1.x == p2.x) ? true : false;
		slope = (p1.y - p2.y) / (p1.x - p2.x);
		y_intercept = p1.y - slope * p1.x;
	}		
		
	public Vector3 getOrigin(){
		return p1;
	}

	public Vector3 getEnd(){
		return p2;
	}

	public LinePointDistance calculateMinDistanceToPoint(Vector3 point){
		Vector3 projectedPoint = ProjectPointOnLine(point);
		Vector3 intersect = getClosestPointOnLine (projectedPoint);
		return new LinePointDistance (this, point, intersect);
	}	

	private Vector3 ProjectPointOnLine(Vector3 point){		
		Vector3 v1 = getEnd () - getOrigin ();
		Vector3 v2 = point - getOrigin();
		//project v2 onto v1
		Vector3 projection = Vector3.Dot(v1, v2) / (float)Math.Pow(v1.magnitude, 2f) * v1;
		return getOrigin() + projection;
	}

	public OptionalVector getIntersectPoint (RayLine otherRay){
		if (isParralelToYAxis && otherRay.isParralelToYAxis) {
			return new OptionalVector ();
		}
		Vector3 intersect;
		if (isParralelToYAxis){
			intersect =  getIntersectBetweenRegularAndYParallelLine (otherRay, this);
		} else if (otherRay.isParralelToYAxis) {
			intersect = getIntersectBetweenRegularAndYParallelLine (this, otherRay);
		} else {
			float x = (otherRay.y_intercept - y_intercept) / (slope - otherRay.slope);
			float y = slope * x + y_intercept;
			intersect = new Vector3(x, y, 0);
		}
		if (isWithinLine (intersect) && otherRay.isWithinLine(intersect)) {
			return new OptionalVector (intersect);
		} else {
			return new OptionalVector ();
		}
	}

	private Vector3 getIntersectBetweenRegularAndYParallelLine(RayLine regular, RayLine yParallel){
		float x = yParallel.p1.x;
		float y = yParallel.p1.x * regular.slope + regular.y_intercept;
		return new Vector3 (x, y, 0);
	}

	private bool isWithinLine(Vector3 point){
		return isPointBetweenEndPoints (point.x, p1.x, p2.x) && isPointBetweenEndPoints (point.y, p1.y, p2.y);
	}

	private Vector3 getClosestPointOnLine(Vector3 point){
		if (isWithinLine (point)) {
			return point;
		}
		OrderedPoints orderedPoints = getPointsInOrder ();
		return (point.x > orderedPoints.max.x || point.y > orderedPoints.max.y) ? orderedPoints.max : orderedPoints.min;
	}

	private struct OrderedPoints{
		public Vector3 min, max;
		public OrderedPoints(Vector3 min, Vector3 max){
			this.min = min;
			this.max = max;
		}
	}

	private OrderedPoints getPointsInOrder(){		
		if (p1.x < p2.x || p1.y < p2.y) {
			return new OrderedPoints (p1, p2);
		} else {
			return new OrderedPoints (p2, p1);
		}
	}

	private bool isPointBetweenEndPoints(float point, float endPoint1, float endPoint2){
		if (endPoint1 > endPoint2) {
			return isPointBetweenEndPoints (point, endPoint2, endPoint1);
		}
		return endPoint1 <= point && point <= endPoint2;
	}
}
	