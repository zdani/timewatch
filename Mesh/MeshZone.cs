using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeshZone
{
	public List<RayLine> lines;
	private MeshCreator meshCreate;
	private static int numberOfExistingMeshSplits = 1; 
	private String meshName = "UserMeshSplit_";

	public MeshZone (GameObject parentObject, String name, List<RayLine> lines)
	{
		this.lines = lines;
		meshCreate = new MeshCreator(parentObject,  name, convertLinesToPoints(lines));
	}

	public MeshZone (GameObject parentObject, String name, Vector3[] points)
	{
		this.lines = convertPointsToLine(points);
		meshCreate = new MeshCreator(parentObject,  name, points);
	}

	public void renderMesh(){
		meshCreate.renderPoly ();
	}

	public void unrender(){
		meshCreate.unrender ();
	}

	private Vector3[] convertLinesToPoints(List<RayLine> lines){
		Vector3[] points = new Vector3[lines.Count];
		int counter = 0;
		foreach (RayLine line in lines) {
			points [counter] = line.getOrigin ();
			counter++;
		}
		return points;
	}

	private List<RayLine> convertPointsToLine(Vector3[] points){
		List<RayLine> result = new List<RayLine> ();
		for (int i = 0; i < points.Length - 1; i++){
			result.Add(new RayLine(points[i], points[ i+ 1]));
		}
		result.Add(new RayLine(points[points.Length - 1], points [0]));
		return result;
	}
		
	public LinePointDistance getMinDistanceLine (Vector3 point)
	{
		LinePointDistance minLine = lines.First().calculateMinDistanceToPoint (point);
		foreach (RayLine line in lines) {
			LinePointDistance result = line.calculateMinDistanceToPoint (point);
			if (result.getDistance() < minLine.getDistance()) {
				minLine = result;
			}
		}
		return minLine;
	}

	public List<int> getIntersectLines(RayLine userLine){
		List<int> result = new List<int> ();
		int counter = 0;
		foreach (RayLine line in lines) {
			if (line.getIntersectPoint(userLine).hasResult()) {
				result.Add (counter);
			}
			counter++;
		}
		return result;
	}

	public List<MeshZone> breakMesh(RayLine splitLine, GameObject objectBeingSplit){
		unrender ();
		List<int> intersectLines = getIntersectLines (splitLine);
		if (intersectLines.Count > 2) {
			throw new Exception ("Identified " + intersectLines.Count + " intersect points. There should never be more than 2");
		} else if (intersectLines.Count <= 1) {
			return new List<MeshZone> ();
		}
		return breakMesh (intersectLines.First (), intersectLines.Last (), splitLine, objectBeingSplit);
		//todo add test to see if the line connects all the way around
	}

	private List<MeshZone> breakMesh(int pos1, int pos2, RayLine splitLine, GameObject objectBeingSplit){
		List<MeshZone> result = new List<MeshZone> ();
		result.Add(createMeshFromSplit(pos1, pos2, splitLine, objectBeingSplit));
		result.Add(createMeshFromSplit(pos2, pos1, splitLine, objectBeingSplit));
		return result;
	}

	private MeshZone createMeshFromSplit(int pos1, int pos2, RayLine splitLine, GameObject objectBeingSplit){
		List<RayLine> linesOfNewMesh = getLinesForNewMesh (pos1, pos2, splitLine);
		MeshZone zone = new MeshZone (objectBeingSplit, getNextMeshName(), linesOfNewMesh);
		zone.renderMesh ();
		return zone;
	}

	private List<RayLine> getLinesForNewMesh(int startPos, int endPos, RayLine splitLine){
		int currentPos = startPos;
		List<RayLine> result = new List<RayLine> ();
		// add all lines except for first and last
		// they will be added later
		currentPos = updateLineCounter (currentPos, lines.Count);
		while (currentPos != endPos) {
			result.Add (lines.ElementAt (currentPos));
			currentPos = updateLineCounter (currentPos, lines.Count);
		}

		RayLine firstEle = lines.ElementAt (startPos);
		Vector3 intersectFirst = firstEle.getIntersectPoint (splitLine).getResult();
		RayLine updatedFirstLine = new RayLine (intersectFirst, firstEle.getEnd());

		RayLine lastEle = lines.ElementAt (endPos);
		Vector3 intersectLast = lastEle.getIntersectPoint (splitLine).getResult ();
		RayLine updatedLastLine = new RayLine (lastEle.getOrigin(), intersectLast);

		RayLine EndsConnector = new RayLine (intersectLast, intersectFirst);

		result.Insert (0, updatedFirstLine);
		result.Add (updatedLastLine);
		result.Add (EndsConnector);
		return result;
	}
		
	private String getNextMeshName(){
		numberOfExistingMeshSplits++;
		return meshName + (numberOfExistingMeshSplits - 1);
	}

	private int updateLineCounter(int currentPos, int size){
		return currentPos = (currentPos == size - 1) ? 0 : currentPos + 1;
	}
}
