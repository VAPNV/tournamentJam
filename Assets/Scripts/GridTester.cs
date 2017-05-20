using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTester : MonoBehaviour {

	public Grid GridToTest;


	void Start () {
		GridToTest.GridModel = GridToTest.Mother.GetElementByName("Ground");
	
	
	}

	// Update is called once per frame
	void Update () {

//
//		if (GridToTest != null)
//			GridToTest.CycleType ();
	}
}
