using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*
 * Build a mesh from pre-set specifications
*/
public class MeshFactory
{

	public static MeshZone buildInitialMesh(GameObject gameObject){
		MeshZone initialMesh = new MeshZone (gameObject, "dan1", createInitialPolyVertices ());
		return initialMesh;
	}
		
	private static Vector3[] createInitialPolyVertices(){
		GameObject background = GameObject.Find ("Background");
		float x = background.transform.position.x;
		float y = background.transform.position.y;
		float scaleX = background.transform.lossyScale.x;
		float scaleY = background.transform.lossyScale.y;

		// Create Vector2 vertices
		Vector3[] vertices3D = new Vector3[] {
			new Vector3(x,               y,               0),
			new Vector3(x + 2.5f*scaleX, y,               0),
			new Vector3(x + 2.5f*scaleX, y + 2.5f*scaleY, 0),
			new Vector3(x,               y + 2.5f*scaleY, 0)
		};
		return vertices3D;
	}
}

