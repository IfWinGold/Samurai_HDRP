using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamagedState : EState
{
    public static int ANIM_DAMEGED = Animator.StringToHash("tDameged");
    public override State state => State.Dameged;

    private float fdamegeTime = 0.2f;
    private float fcurrentTime = 0f;


    public override void EndState(Enemy _enemy)
    {
        _enemy.SetStateLock(false);
    }

    public override void StartState(Enemy _enemy)
    {
        _enemy.anim.SetTrigger(ANIM_DAMEGED);
        _enemy.SetStateLock(true);
    }

    public override void UpdateState(Enemy _enemy)
    {
        fcurrentTime += Time.deltaTime;
        if (fcurrentTime >= fdamegeTime)
            _enemy.SetStateLock(false);
    }
}
