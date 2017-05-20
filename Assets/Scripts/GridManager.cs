using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

	public int ScaleX = 10;
	public int ScaleY = 10;

	public GameObject GRIDTEMPLATE;

	public GameObject[] ElementTypes;

	public GameObject MapEdge;

	public GameObject Flag_Blue;
	public GameObject Flag_Orange;

	// Use this for initialization
	void Start () {

		// Create the MAP
		for (int forX = -2; forX <= ScaleX*2+2; forX = forX +2)
		{
			for (int forY = -2; forY <= ScaleY*2+2; forY = forY +2)
			{
				Grid GRID_PIECE_GRIDDITY;

				//EDGECHECK FIRST
				if (forX == -2 || forX == ScaleX * 2 + 2) {
					GRID_PIECE_GRIDDITY = this.CreateGrid (GetElementByName("Map_Edge"), (this.transform.position + new Vector3 (forX, 0, forY)));
				} else if (forY == -2 || forY == ScaleY * 2 + 2) {
					GRID_PIECE_GRIDDITY = this.CreateGrid (GetElementByName("Map_Edge"), (this.transform.position + new Vector3 (forX, 0, forY)));
				} else {
					GRID_PIECE_GRIDDITY = this.CreateGrid (GetElementByName("Ground_Grass"), (this.transform.position + new Vector3 (forX, 0, forY)));
				}

				//Grid GRID_PIECE_GRIDDITY = this.CreateGrid (GetElementByName("GROUND_STANDARD"), (this.transform.position + new Vector3 (forX, 0, forY)));

				GRID_PIECE_GRIDDITY.x = forX;
				GRID_PIECE_GRIDDITY.y = forY;

				if ((forX == ScaleX*2) & (forY == ScaleY)) {
					GameObject GRID_PIECE = (GameObject)Instantiate (Flag_Blue, (this.transform.position + new Vector3 (forX, 0, forY)), this.transform.rotation);
				}
				else if ((forX == 0) & (forY == ScaleY)) {
					GameObject GRID_PIECE = (GameObject)Instantiate (Flag_Orange, (this.transform.position + new Vector3 (forX, 0, forY)), this.transform.rotation);
				}
			}
		}

	}

    /*public void CreateGrid (GameObject TypeToChange, Vector3 NewGridLocation, Grid OldGrid)
	{

		Grid NewGrid = this.CreateGrid (TypeToChange, NewGridLocation);

		NewGrid.x = OldGrid.x;
		NewGrid.y = OldGrid.y;

		NewGrid.UpdateName ();

	}*/

    public Grid CreateGrid(GameObject TypeToChange, Vector3 NewGridLocation)
    {
        GameObject GRID_PIECE = (GameObject)Instantiate(GRIDTEMPLATE, NewGridLocation, this.transform.rotation);
        GRID_PIECE.transform.parent = this.transform;

        Grid GRID_PIECE_GRIDDITY = GRID_PIECE.GetComponent<Grid>();

        GRID_PIECE_GRIDDITY.UseThisModel(TypeToChange);

        return GRID_PIECE_GRIDDITY;
    }

    public Grid CreateGrid (GameObject TypeToChange, Vector3 NewGridLocation, Grid OldGrid)
	{
		GameObject GRID_PIECE = (GameObject)Instantiate (GRIDTEMPLATE, NewGridLocation, this.transform.rotation);
		GRID_PIECE.transform.parent = this.transform;

		Grid GRID_PIECE_GRIDDITY = GRID_PIECE.GetComponent<Grid> ();

		foreach (Transform AlaOsuus in GRID_PIECE.GetComponentsInChildren<Transform>()) {
		
			AlaOsuus.rotation = this.transform.rotation;
		
		}

		//GRID_PIECE_GRIDDITY.gameObject.AddComponent(TypeToChange) as GameObject;

	//	GameObject NewLook = GRID_PIECE_GRIDDITY.gameObject.AddComponent("GameObject") as GameObject;

		GRID_PIECE_GRIDDITY.UseThisModel (TypeToChange);

        //GameObject NewLook = GRID_PIECE.AddComponent<GameObject>();

        GRID_PIECE_GRIDDITY.x = OldGrid.x;
        GRID_PIECE_GRIDDITY.y = OldGrid.y;


        //GRID_PIECE_GRIDDITY.ChangeTo (Type);


        return GRID_PIECE_GRIDDITY;
	}

	// Update is called once per frame
	void Update () {
		
	}

    public GameObject GetElementByName(string name)
    {
        foreach (GameObject type in ElementTypes)
            if (type.name == name)
                return type;
        return null;
    }




}
