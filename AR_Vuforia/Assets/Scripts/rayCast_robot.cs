using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ryaCast_robot : MonoBehaviour
{
    Animator anim; // 애니메이션을 실행시키기 위한 변수
    public int atkpnt = 2000;
    public int hltpnt = 5000;
    float timeElapsed; // 시간 계산

    void Start()
    {
        anim = transform.GetComponent<Animator>();
    }

    void Update()
    {
        RaycastHit hit; // 부짇히는 물체의 정보를 담는 변수 hit 선언

        // Forward 라는 이름의 방향 변수 선언 및 대입
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 1000;

        // 가상의 레이인 레이캐스트를 테스트 중에는 보일 수 있도록 함
        Debug.DrawRay(transform.position, forward, Color.green);

        if(Physics.Raycast(transform.position, forward, out hit))
        {
            Debug.Log("드래곤을 발견"); // 레이 캐스터에 어떠한 물체가 맞으다면 hit라는 문구를 넣음
            anim.SetBool("isHit", true); // 애니메이션 실행

            if(timeElapsed >=3)
            {
                timeElapsed += Time.deltaTime;
                hit.transform.GetComponent<rayCast_Dragon>().hltpnt -= atkpnt;
            }
        }
        else
        {
            anim.SetBool("isHit", false);
        }

        if(hltpnt <=0)
        {
            anim.SetBool("isDead", true);
        }
    }
}
