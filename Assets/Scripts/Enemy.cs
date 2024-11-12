using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Enemy : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    [Header("Detect")]
    [SerializeField] protected float fdetectAngle = 90f;
    [SerializeField] protected float fdetectDistance = 15f;

    [SerializeField] protected Actor simpleTarget;

    [SerializeField] protected float fstopDistance;

    [SerializeField] protected BoxCollider attackCollider;
    [SerializeField] protected TextMeshProUGUI tmpUgui;
    [SerializeField] protected Canvas canvas;
    protected static int ANIM_WALK = Animator.StringToHash("bWalk");
    protected static int ANIM_ATTACK = Animator.StringToHash("bAttack");


    protected float fexecutionGuage = 0f;
    public const float EXECUTION_MAX = 100f;

    protected EState state;

    public Animator animator;

    public bool bStateFance = false;

    protected EState currentState;
    protected bool bstateFlag = false;

    public Animator anim { get => animator; }
    public NavMeshAgent agent { get => navMeshAgent; }
    public Actor target { get => simpleTarget; }
    public EState.State State { get => currentState.state; }
    public float stopDistance { get => fstopDistance; }
    public float ExecutionGuage { get => fexecutionGuage; }

    public bool bisAttack = false;
    public bool bisStock = false;
    public int attackDmg = 1;

    public List<Actor> ltriggerActors = new List<Actor>();


    public virtual void Initialized()
    {
        transform.rotation = Quaternion.identity;
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        currentState = new EnemyIdleState();
        currentState.StartState(this);
    }
    public void SetTarget(Actor actor)
    {
        simpleTarget = actor;
    }

    public void SwitchState<T>() where T : EState, new()
    {
        currentState.EndState(this);
        currentState = new T();
        currentState.StartState(this);
    }
    public void SwitchState<T>(Actor actor) where T : EState, new()
    {
        currentState.EndState(this);
        currentState = new T();
        currentState.actor = actor;
        currentState.StartState(this);
    }
    public void SetStateLock(bool _active)
    {
        bStateFance = _active;
    }


    public float GetTargetDistance()
    {
        return Vector3.Distance(this.transform.position, simpleTarget.transform.position);
    }


    public void OnStartAnimation()
    {
        //bStateFance = true;
    }
    public void OnEndAniamtion()
    {
        bisStock = false;
    }

    public virtual void ParryTime()
    {
        for(int i = 0; i < ltriggerActors.Count; i++)
        {
            if (ltriggerActors[i] != null)
            {
                Samurai samurai = ltriggerActors[i] as Samurai;
                if (samurai.CanParry(this))
                {
                    Stock();
                    samurai.Parry(this);
                }
            }
        }
    }
    protected virtual void Stock()
    {
        animator.SetTrigger("tCollide");
        bisStock = true;
    }
    protected virtual void Blocked()
    {

    }
    public virtual void Attack()
    {
        if (bisStock) return;
        for (int i = 0; i < ltriggerActors.Count; ++i)
        {
            if(ltriggerActors[i] != null)
            {
                if (ltriggerActors[i].CanParry(this))
                {
                    Stock();
                }
                else
                {
                    ltriggerActors[i].Damaged(attackDmg);
                    Debug.Log($"Enemy : Attack");
                }

            }                
        }            
    }
    public virtual void Damaged(int _dmg)
    {
        if (currentState.state != EState.State.Dameged)
        {
            SetStateLock(false);
            SwitchState<EnemyDamagedState>();
            Debug.Log("Damaeged");         
            
            //TODO > Dameged
        }
    }

    protected virtual void Tick()
    {
        if (tmpUgui != null)
        {
            tmpUgui.text = $"State:{currentState.state}";
            canvas.transform.LookAt(Camera.main.transform);
        }
            
        //입력이 없을경우 Idle 상태로 변경

        if (bStateFance) return;

        float distance = GetTargetDistance();
        if (distance > fdetectDistance) return;
        if (distance > stopDistance)
        {
            //walk
            if (currentState.state != EState.State.Walk)
            {
                SwitchState<EnemyWalkState>();
            }               
            return;
        }
        else
        {
            //attack
            if(currentState.state != EState.State.Attack)
            {
                SwitchState<EnemyAttackState>();
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Actor actor = other.GetComponent<Actor>();
        if(actor!= null)
        {
            ltriggerActors.Add(actor);
        }            
    }
    public void OnTriggerStay(Collider other)
    {
        
    }
    public void OnTriggerExit(Collider other)
    {
        Actor actor = other.GetComponent<Actor>();
        if (actor != null)
        {
            if (ltriggerActors.Contains(actor))
                ltriggerActors.RemoveAll(x=>x==actor);
        }
    }

    void Update()
    {
        Tick();
        currentState.UpdateState(this);
    }

    private void Start()
    {
        Initialized();
    }

    public bool IsDetected(Transform target)
    {
        Vector3 directionTarget = target.transform.position - this.transform.position;
        float targetAngle = Vector3.Angle(directionTarget, this.transform.forward);

        bool obstacleHide = Physics.Raycast(this.transform.position, directionTarget.normalized, fdetectDistance, 1 << LayerMask.NameToLayer("Obstacle"));
        bool result = Mathf.Abs(targetAngle) <= fdetectAngle * 0.5f && directionTarget.magnitude <= fdetectDistance && !obstacleHide;


        //TODO
        if(result)
        {
            
        }
        else
        {

        }

        return result;

    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, 15f);

        Gizmos.color = Color.cyan;

        float lookingAngle = transform.eulerAngles.y;
        Vector3 rightDir = GameUtility.AngleToDir(lookingAngle + fdetectAngle * 0.5f);
        Vector3 leftDir = GameUtility.AngleToDir(lookingAngle - fdetectAngle * 0.5f);
        Vector3 lookDir = GameUtility.AngleToDir(lookingAngle);

        Gizmos.DrawRay(transform.position, rightDir * fdetectDistance);
        Gizmos.DrawRay(transform.position, lookDir * fdetectDistance);
        Gizmos.DrawRay(transform.position, leftDir * fdetectDistance);


        Gizmos.color = Color.red;

        Vector3 directionTarget = target.transform.position - this.transform.position;
        float targetAngle = Vector3.Angle(directionTarget, this.transform.forward);

        Gizmos.DrawRay(transform.position, directionTarget.normalized * fdetectDistance);

    }
}
