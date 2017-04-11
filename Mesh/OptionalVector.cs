using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OptionalVector
{
	bool valueExists;
	Vector3 vector;

	public OptionalVector (Vector3 vector){
		this.valueExists = true;
		this.vector = vector;
	}

	public OptionalVector (){
		valueExists = false;
	}

	public bool hasResult(){
		return valueExists;
	}
	public Vector3 getResult(){
		if (!valueExists){
			throw new Exception("Tried to retrive an optional value when no value exists");
		}
		return vector;
	}
}
