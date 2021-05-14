using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayCast_dragon : MonoBehaviour
{
    Animator anim;
    public int atkPnt = 200;
    public int hltPnt = 5000;
    float timeElapsed;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 1000;

        Debug.DrawRay(transform.position, forward, Color.green);

        if (Physics.Raycast(transform.position, forward, out hit))
        {
            //Debug.Log("드래곤이 로봇을 겨냥함!");
            anim.SetBool("isHit", true);

            if (timeElapsed >= 3)
            {
                timeElapsed += Time.deltaTime;
                hit.transform.GetComponent<rayCast_robot>().hltPnt -= atkPnt;
                timeElapsed = 0f;
            }

            
            
        }
        else
        {
            anim.SetBool("isHit", false);
        }

        if (hltPnt <= 0)
        {
            anim.SetBool("isDead", true);
        }
    }
}
