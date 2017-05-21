using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {
    public float buildTime;
    [SyncVar]
    private float buildTimeLeft;
    [SyncVar]
    private float fightText;

	[SyncVar]
	public int Score_Orange;
	[SyncVar]
	public int Score_Blue;

	// Use this for initialization
	void Start () {
        buildTimeLeft = buildTime;

	}
	
	// Update is called once per frame
	void Update () {

		//ANNOUNCEMENTS!

//		foreach (Combat FightingBot in this.GetComponentsInChildren<Combat>())
//		{
//			if (FightingBot.team == Combat.Team.Blue)
//			else if (FightingBot.team == Combat.Team.Blue)
//
//		}

       // GameObject.Find("Announcement").GetComponent<Text>().text = "";
		if (buildTimeLeft <= 30 && buildTimeLeft > 0)
			AnnounceMessage ("Battle stats in " + (int)buildTimeLeft);
		else if (buildTimeLeft > 0) 
			AnnounceMessage ("BUILD YOUR TRENCHES!");
		else if (fightText > 0 )
			AnnounceMessage ("FIGHT!");
        else if (!isServer)
            return;

        if (buildTimeLeft < 0)
        {
            buildTimeLeft = 0;
            PlayerMove[] players = FindObjectsOfType<PlayerMove>();
            foreach (PlayerMove plr in players)
            {
                if (plr.debug)
                    continue;
                plr.fighting = true;
                plr.WhatToBuild = mod(plr.WhatToBuild, plr.fighting ? plr.fightTools.Length : plr.buildTools.Length); ;
            }
            fightText = 2;
        }
        else if (buildTimeLeft > 0)
            buildTimeLeft -= Time.deltaTime;
        if (fightText > 0)
            fightText -= Time.deltaTime;
    }


	public void AnnounceMessage(string WhatToSend)
	{
		Debug.Log (WhatToSend);

		GameObject.Find ("Announcement").GetComponent<Text> ().text = WhatToSend;
	}

    int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

}
