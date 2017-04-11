using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/*
 * Registers and draws user created lines
 * Splits the mesh into seperate pieces
 * 
*/

public class Mesher : MonoBehaviour {
	

	private List<MeshZone> meshZones = new List<MeshZone>();
	private LineSelection currentState;

	void Start () {
		
		MeshZone initialMesh = MeshFactory.buildInitialMesh (this.gameObject);
		initialMesh.renderMesh ();
		meshZones.Add (initialMesh); 
		currentState = new NoSelection (gameObject, meshZones);
    }

	void Update(){
		if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonUp (0)) {
			doAction ();
		}
	}
		
	private void doAction(){
		currentState.doAction (this);
	}

	public void setState(LineSelection newState){
		currentState = newState;
	}
}
	

public abstract class LineSelection{
	public GameObject gameObject;
	public float minDistanceAllowed;
	public List<MeshZone> meshZones;
	private float ratioOfScreenToMinDistanceAllowed = 10f;
		
	public LineSelection(GameObject gameObject, List<MeshZone> meshZones){
		minDistanceAllowed = Camera.main.orthographicSize / ratioOfScreenToMinDistanceAllowed;
		this.meshZones = meshZones;
		this.gameObject = gameObject;
	}

	public abstract void doAction (Mesher newState);

	public LinePointDistance isMouseNearLine(){		
		Vector2 clickPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		LinePointDistance minDistanceFound = meshZones.First ().getMinDistanceLine (clickPoint);
		//LinePointDistance doesItWork = meshZones.Min(a =>a.getMinDistanceLine(clickPoint));
		foreach (MeshZone meshZone in meshZones) {
			LinePointDistance minDistance = meshZone.getMinDistanceLine (clickPoint);
			if (minDistance.getDistance() < minDistanceFound.getDistance()) {
				minDistanceFound = minDistance;
			}
		}
		Debug.Log ("Testing");
		return minDistanceFound;
	}
		
}

class NoSelection : LineSelection{
	public NoSelection( GameObject gameObject, List<MeshZone> meshZones) : base(gameObject, meshZones){
	}

	public override void doAction(Mesher state){
		LinePointDistance nearPoint = isMouseNearLine ();
		if (!nearPoint.isWithinAcceptableRange(minDistanceAllowed)) {
			return;	
		}
		Vector3 mouseStart = nearPoint.getPoint();
		state.setState (new MouseDown (mouseStart, gameObject, meshZones));
	}
}

class MouseDown : LineSelection{
	Vector3 mouseStart;

	public MouseDown(Vector3 mouseStart, GameObject gameObject, List<MeshZone> meshZones): base(gameObject, meshZones){
		this.mouseStart = mouseStart;	
	}

	public override void doAction(Mesher state){
		LinePointDistance nearPoint = isMouseNearLine ();
		if (!nearPoint.isWithinAcceptableRange(minDistanceAllowed)) {
			state.setState (new NoSelection (gameObject, meshZones));
			return;	
		}
		Vector3 mouseEnd = nearPoint.getPoint();
		RayLine userLine = new RayLine (mouseStart, mouseEnd);
		MeshSplitter meshSplitter = new MeshSplitter (meshZones, userLine, gameObject);
		meshZones = meshSplitter.breakUpMeshes ();
		state.setState (new NoSelection (gameObject, meshZones));
	}
}
