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
		for (int forX = 0; forX <= ScaleX; forX = forX +2)
		{
			for (int forY = 0; forY <= ScaleY; forY = forY +2)
			{
				GameObject GRID_PIECE = (GameObject)Instantiate (GRIDTEMPLATE, (this.transform.position + new Vector3(forX, 0, forY)), this.transform.rotation);
				GRID_PIECE.transform.parent = this.transform;
				Grid GRID_PIECE_GRIDDITY = GRID_PIECE.GetComponent<Grid> ();

				GRID_PIECE_GRIDDITY.x = forX;
				GRID_PIECE_GRIDDITY.y = forY;
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}






}
