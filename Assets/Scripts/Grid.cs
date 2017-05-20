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

	public string WhatIam = "NOTHING";
	public GridManager Mother;

	public GameObject GridModel;


	public int Health = 100;
	public int Armour = 10;
	public GameObject BecomeThisAfterDeath;


	// Use this for initialization
	void Start () {
		Mother = this.GetComponentInParent<GridManager>();	//Insert check which FAILS ROAYALLY IF THIS DOES NOT WORK!



		this.UpdateName ();
	}

	public void UseThisModel (GameObject ModelToUse)
	{
		
		GridModel = (GameObject)Instantiate (ModelToUse, (this.transform.position + new Vector3(0, 0, 0)), this.transform.rotation);
		GridModel.transform.parent = this.transform;


		this.WhatIam = ModelToUse.name;


		this.UpdateName ();
	}

	/// <summary>
	/// Damages Grid. Returns TRUE if died!
	/// </summary>
	/// <param name="Amount">Amount.</param>
	public bool Damage(int Amount)
	{
		this.Health = this.Health + Mathf.Min(0,(this.Armour - Amount));

		if (Health < 0)
			return true;

		return false;
	
	
	}

	public void ChangeTo (GameObject WhatToChangeTo)
	{
		Mother.CreateGrid (WhatToChangeTo, this.transform.position, this);
		Destroy (this.gameObject);

		//this.GridModel = WhatToChangeTo;
//
//		GameObject NewModel = (GameObject)Instantiate (WhatToChangeTo, (this.transform.position + new Vector3(0, 0, 0)), this.transform.rotation);
//		NewModel.transform.parent = this.transform;
//		Destroy (GridModel);
//
//		this.GridModel = NewModel;
//
//		WhatIam = GridModel.name;
//
//		this.UpdateName ();


		//other.CreateNew
	}

	public void CycleType ()
	{

		this.ChangeTo (Mother.GetElementByName("TRENCH_LOW"));

//		if (GridModel == Mother.Ground)
//			this.ChangeTo (Mother.Trench_Low);
//		else if (GridModel == Mother.Trench_Low)
//			this.ChangeTo (Mother.Trench_Deep);
//		else if (GridModel == Mother.Trench_Deep)
//			this.ChangeTo (Mother.Ground);
			
		this.UpdateName ();
	}

	public void UpdateName()
	{
		this.name = (x + ":" + y + ": " + WhatIam);

	}



}
