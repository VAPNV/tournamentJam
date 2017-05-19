using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

	public int ScaleX = 10;
	public int ScaleY = 10;

	public GameObject GRIDTEMPLATE;

	public GameObject Ground;
	public GameObject Trench_Low;
	public GameObject Trench_Deep;

	// Use this for initialization
	void Start () {

		for (int forX = 0; forX <= ScaleX*2; forX = forX +2)
		{
			for (int forY = 0; forY <= ScaleY*2; forY = forY +2)
			{
				Grid GRID_PIECE_GRIDDITY = this.CreateGrid (Ground, (this.transform.position + new Vector3 (forX, 0, forY)));

				GRID_PIECE_GRIDDITY.x = forX;
				GRID_PIECE_GRIDDITY.y = forY;
			}
		}

	}

	public void CreateGrid (GameObject TypeToChange, Vector3 NewGridLocation, Grid OldGrid)
	{

		Grid NewGrid = this.CreateGrid (TypeToChange, NewGridLocation);

		NewGrid.x = OldGrid.x;
		NewGrid.y = OldGrid.y;

		NewGrid.UpdateName ();

	}

	public Grid CreateGrid (GameObject TypeToChange, Vector3 NewGridLocation)
	{
		GameObject GRID_PIECE = (GameObject)Instantiate (GRIDTEMPLATE, NewGridLocation, this.transform.rotation);
		GRID_PIECE.transform.parent = this.transform;

		Grid GRID_PIECE_GRIDDITY = GRID_PIECE.GetComponent<Grid> ();

		//GRID_PIECE_GRIDDITY.gameObject.AddComponent(TypeToChange) as GameObject;

	//	GameObject NewLook = GRID_PIECE_GRIDDITY.gameObject.AddComponent("GameObject") as GameObject;

		GRID_PIECE_GRIDDITY.UseThisModel (TypeToChange);

		//GameObject NewLook = GRID_PIECE.AddComponent<GameObject>();




		//GRID_PIECE_GRIDDITY.ChangeTo (Type);


		return GRID_PIECE_GRIDDITY;
	}

	// Update is called once per frame
	void Update () {
		
	}






}
