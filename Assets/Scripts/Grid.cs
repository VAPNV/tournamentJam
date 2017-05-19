using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Script what deals with single GRID: Where it is and what it contains!
/// </summary>
public class Grid : MonoBehaviour {

	public int x;
	public int y;
		// these could be inherited from transform but meh

	string WhatIam = "Ground";
	public GridManager Mother;

	public GameObject GridModel;

	private 


	// Use this for initialization
	void Start () {
		this.UpdateName();

		Mother = this.GetComponentInParent<GridManager>();	//Insert check which FAILS ROAYALLY IF THIS DOES NOT WORK!
	}
	

	void ChangeTo (GameObject WhatToChangeTo)
	{
		this.GridModel = WhatToChangeTo;

		WhatIam = WhatToChangeTo.name;

		this.UpdateName ();
	}

	void UpdateName()
	{
		this.name = (x + ":" + y + ": " + WhatIam);

	}


}
