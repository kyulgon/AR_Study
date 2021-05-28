using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Threading;

public class MonsterCtrl : MonoBehaviour
{
    public enum MonsterState { idle, trace, attack, die};
    public MonsterState monsterState = MonsterState.idle;
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    public float traceDist = 10.0f;
    public float attackDist = 2.0f;
    private bool isDie = false;

    private int hp = 100;

    private Thread thread = null;
    private float distance = 0f;
    private object lockObject = new object();
    private static Queue<MonsterState> TaskQueue = new Queue<MonsterState>();

    // Start is called before the first frame update
    void Start()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //nvAgent.destination = playerTr.position;

        //StartCoroutine(this.CheckMonsterState());
        //StartCoroutine(this.MonsterAction());
        thread = new Thread(CheckMonsterState);
        thread.Start();
    }

    void CheckMonsterState()
    {
        while(isDie == false)
        {
            // yield return new WaitForSeconds(0.2f);

            Thread.Sleep(200);


            if(monsterState == MonsterState.die)
            {
                ;
            }
            else if (distance <= attackDist)
            {
                lock(lockObject)
                {
                    TaskQueue.Enqueue(MonsterState.attack);
                }
                Debug.Log("CheckMonsterState(): attack!");
            }
            else if(distance <= traceDist)
            {
                lock (lockObject)
                {
                    TaskQueue.Enqueue(MonsterState.trace);
                }
                Debug.Log("CheckMonsterState(): trace!");
            }
            else
            {
                lock (lockObject)
                {
                    TaskQueue.Enqueue(MonsterState.idle);
                }
                Debug.Log("CheckMonsterState(): idle!");
            }
        }
    }

    IEnumerator MonsterAction()
    {
        while(!isDie)
        {
            switch(monsterState)
            {
                case MonsterState.idle:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsTrace", false);

                    break;
                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.isStopped = false;
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);

                    break;
                case MonsterState.attack:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsAttack", true);
                    break;
            }
            yield return null;
        }
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();
        nvAgent.isStopped = true;
        animator.SetTrigger("IsPlayerDie");
    }


   


    private void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "BULLET")
        {
            Destroy(coll.gameObject);
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;

            if(hp <= 0)
            {
                MonsterDie();
            }
            else
            {
                animator.SetTrigger("IsHit");
            }
            
        }
    }

    void MonsterDie()
    {
        StopAllCoroutines();
        isDie = true;
        monsterState = MonsterState.die;
        nvAgent.isStopped = true;
        animator.SetTrigger("IsDie");

        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }
    }

   

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(playerTr.position, monsterTr.position);

        if(TaskQueue.Count > 0)
        {
            lock(lockObject)
            {
                monsterState = TaskQueue.Dequeue();
            }
            switch (monsterState)
            {
                case MonsterState.idle:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsTrace", false);
                    break;
                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.isStopped = false;
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);
                    break;
                case MonsterState.attack:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsAttack", true);
                    break;
            }
        }
    }
}
