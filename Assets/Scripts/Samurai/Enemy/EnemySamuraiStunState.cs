using JetBrains.Annotations;
using NSamurai;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySamuraiStunState : EState
{
    private static int anim_stun = Animator.StringToHash("bStun");
    public override State state => EState.State.Stun;


    private float fstunTime = 3f;
    private float fcurrentTime = 0f;
    public override void EndState(Enemy _enemy)
    {
        _enemy.SetStateLock(false);
        _enemy.anim.SetBool(anim_stun, false);
        
        SamuraiEnemy samuraiEnemy = _enemy as SamuraiEnemy;
        //samuraiEnemy.SetActiveExecuteState(false);
    }

    public override void StartState(Enemy _enemy)
    {
        _enemy.SetStateLock(true);
        _enemy.anim.SetBool(anim_stun,true);     
        SamuraiEnemy samuraiEnemy = _enemy as SamuraiEnemy;
        samuraiEnemy.SFXPlayStun();
        //samuraiEnemy.SetActiveExecuteState(true);
    }

    public override void UpdateState(Enemy _enemy)
    {
        if(fcurrentTime <= fstunTime)
        {
            fcurrentTime += Time.deltaTime;
        }
        else
        {
            EndState(_enemy);
        }
    }

}
