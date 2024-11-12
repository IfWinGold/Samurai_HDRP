using NSamurai;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.VFX;
public class SamuraiEnemy : Enemy
{
    [Header("TrainingSetting")]
    [SerializeField] private bool isTranining = false;
    [SerializeField] private string defaultState;
    [Space(20)]
    [SerializeField] private AudioClip[] sfxswords;
    [SerializeField] private AudioClip[] sfxdameged;
    [SerializeField] private AudioClip[] sfxstun;
    [SerializeField] private float assassinationDistance = 3f;
    

    [Header("UI")]
    [SerializeField] private Image imgExecutionGuage;
    [SerializeField] private VisualEffect vfxBlood;

    [SerializeField] private SignalReceiver executeReceiver;

    private ExecuteMark m_executeMark;
    private bool canExecute = false;

    private AudioSource audioSource;
    private CapsuleCollider m_capsuleCollider;

    public ExecuteMark executeMark { get => m_executeMark; set => m_executeMark = value; }
    public bool CanExecute { get => canExecute; set => canExecute = value; }
    public SignalReceiver ExecuteReceiver { get => executeReceiver; }    
    public CapsuleCollider capsuleCollider { get => m_capsuleCollider; }
    public override void Initialized()
    {
        base.Initialized();
        m_capsuleCollider = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();
    }
    public override void Damaged(int _dmg)
    {
        if (currentState.state == EState.State.Executed) 
            return;


        if (currentState.state != EState.State.Dameged)
        {
            SetStateLock(false);
            vfxBlood.Play();
            SwitchState<EnemyDamagedState>();
            Debug.Log("Damaeged");

            //TODO > Dameged
        }
        UpdateExecutionGuage(_dmg);
        SFXPlayDameged();

        if (Mathf.Approximately(fexecutionGuage, EXECUTION_MAX))
        {
            SwitchState<EnemySamuraiStunState>();
        }
    }
    public void Executed(Actor actor,ExecuteDirector executeDirector)
    {
        SwitchState<EnemySamuraiExecutedState>(actor);

        //Position 세팅
        Vector3 moveDir = actor.transform.forward;
        Vector3 pos = actor.transform.position + (moveDir * executeDirector.distance);
        pos.y = this.transform.position.y;

        this.transform.position = pos;


        //Rotate 세팅
        Vector3 lookPos = actor.transform.position;
        lookPos.y = this.transform.position.y;

        if(executeDirector.state == ExecuteDirector.State.Normal)
        {
            this.transform.forward = -actor.transform.forward;
        }
        else
        {
            this.transform.forward = actor.transform.forward;
        }

        return;
    }
    public void Kill()
    {
        animator.enabled = false;
        agent.enabled = false;
        this.enabled = false;
    }
    public override void Attack()
    {
        if (bisStock) return;
        for (int i = 0; i < ltriggerActors.Count; ++i)
        {
            if (ltriggerActors[i] != null)
            {
                Samurai samurai = ltriggerActors[i] as Samurai;
                if (samurai.CanParry(this))
                {
                    //Stock이 걸리면 Samurai는 Parry 함수를 호출
                    Stock();
                    samurai.Parry(this);                    
                }
                else if (samurai.IsGuard())
                {                    
                    
                }
                else
                {
                    ltriggerActors[i].Damaged(attackDmg);
                    Debug.Log($"Enemy : Attack");
                }

            }
        }
    }
    protected override void Stock()
    {
        base.Stock();
        UpdateExecutionGuage(80f);
        //if(Mathf.Approximately(fexecutionGuage, EXECUTION_MAX))
        //{
        //    SwitchState<EnemySamuraiStunState>();
        //}
    }
    protected override void Blocked()
    {
        base.Blocked();
    }
    public void SFXPlaySwrod()
    {
        audioSource.PlayOneShot(sfxswords[Random.Range(0, sfxswords.Length)]);
    }
    public void SFXPlayDameged()
    {
        audioSource.PlayOneShot(sfxdameged[Random.Range(0, sfxdameged.Length)]);
    }
    public void SFXPlayStun()
    {
        audioSource.PlayOneShot(sfxstun[Random.Range(0, sfxstun.Length)]);
    }
    private void UpdateExecutionGuage(float value)
    {
        fexecutionGuage = Mathf.Clamp(fexecutionGuage + value, 0f, EXECUTION_MAX);
        imgExecutionGuage.fillAmount = Mathf.InverseLerp(0, EXECUTION_MAX, fexecutionGuage);
    }
    public void SetActiveExecuteState(bool _active)
    {
        if(_active && canExecute != true)
        {
            canExecute = true;
            UIManager.instance.SetActiveExecuteMark(this);
        }
        else if(!_active) 
        {
            UIManager.instance.DestroyExecuteMark(this);
            canExecute = false;            
        }
    }
    protected override void Tick()
    {
        if (tmpUgui != null)
        {
            tmpUgui.text = $"State:{currentState.state}";
            canvas.transform.LookAt(Camera.main.transform);
        }


        //입력이 없을경우 Idle 상태로 변경
        if (bStateFance || currentState.state == EState.State.Executed) return;


        float distance = GetTargetDistance();
        
        if (distance > fdetectDistance) return;
        if (distance > stopDistance && IsDetected(target.transform))
        {
            //walk
            if (currentState.state != EState.State.Walk)
            {
                SwitchState<EnemyWalkState>();
                //SetActiveExecuteState(false);
            }
            return;
        }
        else if(IsDetected(target.transform))
        {
            //attack
            if (currentState.state != EState.State.Attack)
            {
                SwitchState<EnemyAttackState>();
            }
            //SetActiveExecuteState(false);            
        }
        else if(!IsDetected(target.transform) && distance <= assassinationDistance)
        {
            //SetActiveExecuteState(true);        
        }
    }

    public void VFXPlayBlood()
    {
        vfxBlood.Play();
    }

    
}
