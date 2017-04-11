using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/*
 * Split a mesh into two meshes 
 * 
*/
public class MeshSplitter
{
	private Color red = new Color(1f,0,0);
	private List<MeshZone> meshZones;
	private RayLine splitLine;
	private GameObject objectBeingSplit;
	private float widthOfSplitLine = 0.2f;

	public MeshSplitter(List<MeshZone> meshZones, RayLine splitLine, GameObject objectBeingSplit){
		this.meshZones = meshZones;
		this.splitLine = splitLine;
		this.objectBeingSplit = objectBeingSplit;
	}

	/*
	 * Break all the meshes apart if the provided line crosses it
	*/
	public List<MeshZone> breakUpMeshes(){
		List<MeshZone> updatedMeshZone = new List<MeshZone> ();
		bool hasUpdateHappened = false;
		foreach (MeshZone meshZone in meshZones) {
			List<MeshZone> result = meshZone.breakMesh(splitLine, objectBeingSplit);
			if (result.Count != 0) {
				hasUpdateHappened = true;
				updatedMeshZone.AddRange (result);
			}
		}
		if (hasUpdateHappened) {
			createLine ();
		}
		return updatedMeshZone;
	}

	private void createLine(){
		Vector3 pointStart = splitLine.p1;
		Vector3 pointEnd = splitLine.p2;
		//create cube
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//place the box in the middle of the end and start positions
		Vector3 boxPos = new Vector3 ((pointEnd.x + pointStart.x) / 2, (pointEnd.y + pointStart.y) / 2, 0f);
		cube.gameObject.transform.position = boxPos;

		//rotate
		float angle = Mathf.Atan2(pointEnd.y-pointStart.y, pointEnd.x - pointStart.x) * 180 / Mathf.PI - 90;
		cube.gameObject.transform.Rotate(new Vector3(0, 0, angle));

		//scale
		float distance = Mathf.Pow(Mathf.Pow(pointEnd.y - pointStart.y, 2) + Mathf.Pow(pointEnd.x - pointStart.x, 2), 0.5f);
		cube.gameObject.transform.localScale = new Vector3 (widthOfSplitLine, distance , 1f);;
		cube.GetComponent<Renderer> ().material.color = red;
	}
}
