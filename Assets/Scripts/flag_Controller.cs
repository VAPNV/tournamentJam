using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class flag_Controller : MonoBehaviour
{


    int flag_cooldown = 5; //Flag cooldown in seconds untiol it can be picked back up
    int flag_to_home_cooldown = 10;

    public Combat.Team flag_team;

    bool flagOnCooldown = false;
    bool is_flag_home = true;

	public GameManager Boss;

    private Vector3 startPos;

    // Use this for initialization
    void Start()
    {
		
	

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        //Are we colliding with a player=
//        
		// commented away as there is weird nonsynch stuff :(

//        if (col.tag == "Player")
//        {
//            var target = col.gameObject.GetComponent<PlayerMove>();
//
//            if (target.playerIsHoldingFlag && target.GetComponent<Combat>().team == flag_team)
//            {
//                Debug.Log("SCORE!");
//
//				GameObject.Find ("GameManager").GetComponent<GameManager>().AnnounceMessage (target.name + " SCORES! for " + flag_team + "!");
//
//				Combat GivePointsToScorer = col.gameObject.GetComponent<Combat>();
//
//				GivePointsToScorer.GetScore (10);
//
//                target.playerIsHoldingFlag = false;
//                target.flag_ref.GetComponent<flag_Controller>().returnToStart();
//                target.toggleFlagOnPlayerVisibility(false, target.flag_ref.GetComponent<flag_Controller>().flag_team);
//                target.flag_ref = null;
//            }
//
//            else if (!flagOnCooldown && target.GetComponent<Combat>().team != flag_team)
//            {
//
//
//
//				GameObject.Find ("GameManager").GetComponent<GameManager>().AnnounceMessage (flag_team + " flag taken!");
//
//                //We need to pass reference to the flag to activate it again once dropped
//                target.playerFlagSwitch(true, this.gameObject);
//                if (is_flag_home)
//                {
//                    is_flag_home = false;
//                }
//                else
//                {
//                    CancelInvoke("return_to_base_CD");
//                    flag_to_home_cooldown = 10;
//                }
//                this.gameObject.SetActive(false);
//            }
//        }
    }


    public void dropFlag(Vector3 pos)
    {
		GameObject.Find ("GameManager").GetComponent<GameManager>().AnnounceMessage (flag_team + " flag dropped!");

        this.gameObject.transform.position = pos;
        this.gameObject.SetActive(true);
        flagOnCooldown = true;
        InvokeRepeating("count_down_CD", 1.0f, 1.0f);
        InvokeRepeating("return_to_base_CD", 1.0f, 1.0f);
    }


    //Cooldown if the flag is dropped eitgher by killing a player or player dropping it
    private void count_down_CD()
    {
        if (--flag_cooldown == 0)
        {
            flagOnCooldown = false;
            CancelInvoke("count_down_CD");
            flag_cooldown = 5;
            Debug.Log("FLAG CAN BE CAPTURED AGAIN!");
        }
    }

    private void return_to_base_CD()
    {
        if (--flag_to_home_cooldown == 0)
        {
			GameObject.Find ("GameManager").GetComponent<GameManager>().AnnounceMessage (flag_team + " flag returned to base!");

            flagOnCooldown = false;
            CancelInvoke("return_to_base_CD");
            flag_to_home_cooldown = 10;
            this.returnToStart();
            Debug.Log("FLAG RETURNED HOME!");
        }
    }

    public void setFlagStartPos(Vector3 pos, Combat.Team team_ID)
    {
        this.startPos = pos;
        flag_team = team_ID;
    }

    private void returnToStart()
    {
        this.gameObject.transform.position = startPos;
        is_flag_home = true;
        this.gameObject.SetActive(true);

    }
}

