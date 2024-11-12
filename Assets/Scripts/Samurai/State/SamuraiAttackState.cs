using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SamuraiAttackState : AState
{
    struct AttackCombo
    {
        public AttackCombo(string _name, float _progress) { name = _name;progress = _progress; }
        public string name;
        public float progress;
    }


    private static int anim_combo = Animator.StringToHash("nCombo");
    private static int anim_attack = Animator.StringToHash("bAttack");
    private bool bisCombo = false;
    private int nCombat = -1;
    public override State state => State.Attack;

    private float lastAttackTime = 0f;
    private float comboDelay = 0.25f;

    private List<AttackCombo> comboList = new List<AttackCombo>() {
        new AttackCombo("Samurai_Attack_Combo_B1",0f),
        new AttackCombo("Samurai_Attack_Combo_B2",0f),
        new AttackCombo("Samurai_Attack_Combo_B3",0f),
        new AttackCombo("Samurai_Attack_Combo_B4",0f),
        new AttackCombo("Samurai_Attack_Combo_B5",0f),
    };
    private AttackCombo runAttack = new AttackCombo("Samurai_Attack_Sprint_Unequip", 0f);

    private AttackCombo currentAttack;
    private int currentAttackIndex = -1; 
    private bool bcomboReservation = false;
    private bool bfirstAttack = false; //처음 중복공격 방지

    private AnimatorStateInfo animStateInfo;
    private float animProgress = 0f;    

    public override void EndState(Actor actor)
    {
        actor.SetStateLock(false);
        actor.anim.SetInteger(anim_combo, -1);
        actor.anim.SetBool(anim_attack, false);
    }

    public override void StartState(Actor actor)
    {        
        actor.SetStateLock(true);
        currentAttackIndex = -1;
    }

    public override void UpdateState(Actor actor)
    {
        if(!bfirstAttack)
        {
            currentAttackIndex++;
            PlayAnim(actor);
            bfirstAttack = true;
            return;
        }
        animStateInfo = actor.anim.GetCurrentAnimatorStateInfo(0);
        animProgress = animStateInfo.normalizedTime % 1;

        if(animStateInfo.IsName(currentAttack.name))
        {
            currentAttack.progress = animProgress;
        }

        if (bcomboReservation&&currentAttack.progress >= 0.7f)
        {
            PlayAnim(actor);
            bcomboReservation = false;
        }        
        else if(currentAttack.progress >= 0.9f && !bcomboReservation)
        {
            EndState(actor);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Reservation();
        }
    }
    
    private void Reservation()
    {
        if(!bcomboReservation)
        {
            bcomboReservation = true;
            currentAttackIndex++;
            if (currentAttackIndex > comboList.Count-1)
                currentAttackIndex = 0;
        }
    }
    private void PlayAnim(Actor actor)
    {        
        actor.anim.SetInteger(anim_combo, currentAttackIndex);
        actor.anim.SetBool(anim_attack, true);
        bcomboReservation = false;

        if (!bfirstAttack && actor.PrevState.state == State.Run || actor.PrevState.state == State.Dash)
            currentAttack = runAttack;
        else
            currentAttack = comboList[currentAttackIndex];
    }
}
