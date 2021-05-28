using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Anim
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;
    private Transform tr;
    public float moveSpeed = 10.0f;
    public float rotSpeed = 100.0f;

    public Anim anim;
    private Animation _animation;

    public int hp = 100;


    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "PUNCH")
        {
            hp -= 10;
            Debug.Log("Player HP = " + hp.ToString());
            if(hp <= 0)
            {
                PlayerDie();
            }

        }
    }

    void PlayerDie()
    {
        Debug.Log("Player Die!!");
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        foreach(GameObject monster in monsters)
        {
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();

        _animation = GetComponentInChildren<Animation>();
        _animation.clip = anim.idle;
        _animation.Play();

    }


    // Update is called once per frame
    void Update() // 매 프레임 마다 호출이 되는데, 컴퓨터의 성능에 따라서 초당 몇백번~몇십번 호출이 될수 있음
    {
        //Time.deltaTime 은 이를 초당 균일한 속도 (60fps)로 맞추기 위한 수학적인 게산을 해준 값으로 생각 해야함
        //따라서 어떤 컴퓨터에서도 동일한 속도로 이동이나 회전을 하고 싶을때는 Time.deltatime 을 항상 계산해줘야함
        
     
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Debug.Log("h=" + ((int)h).ToString());
        Debug.Log("v=" + ((int)v).ToString());

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        //tr.Translate(Vector3.right * moveSpeed * h * Time.deltaTime, Space.Self);
        tr.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);

        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        if(v >= 0.1f)
        {
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if(v <= -0.1f)
        {
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        {
            _animation.CrossFade(anim.idle.name, 0.3f);
        }

    }
}
