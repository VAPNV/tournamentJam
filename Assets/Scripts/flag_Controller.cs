using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class flag_Controller : MonoBehaviour
{


    int flag_cooldown = 5; //Flag cooldown in seconds untiol it can be picked back up
    int flag_to_home_cooldown = 10;

    Combat.Team flag_team;

    bool flagOnCooldown = false;
    bool is_flag_home = true;

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
        if (col.name == "PlayerCube(Clone)")
        {

            var target = col.gameObject.GetComponent<PlayerMove>();

            if (target.playerIsHoldingFlag && target.GetComponent<Combat>().team == flag_team)
            {
                Debug.Log("SCORE!");
                target.playerIsHoldingFlag = false;
                target.flag_ref.GetComponent<flag_Controller>().returnToStart();
                target.flag_ref = null;
            }

            else if (!flagOnCooldown && target.GetComponent<Combat>().team != flag_team)
            {
                //We need to pass reference to the flag to activate it again once dropped
                target.playerFlagSwitch(true, this.gameObject);
                if (is_flag_home)
                {
                    is_flag_home = false;
                }
                else
                {
                    CancelInvoke("return_to_base_CD");
                }
                this.gameObject.SetActive(false);
            }
        }
    }


    public void dropFlag(Vector3 pos)
    {
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

