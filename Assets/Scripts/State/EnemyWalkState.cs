using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalkState : EState
{
    
    private static int ANIM_WALK = Animator.StringToHash("bWalk");
    public override State state => State.Walk;

    public override void EndState(Enemy _enemy)
    {
        _enemy.animator.SetBool(ANIM_WALK, false);
        _enemy.agent.isStopped = true;
    }

    public override void StartState(Enemy _enemy)
    {
        _enemy.agent.isStopped = false;
        _enemy.agent.SetDestination(_enemy.target.transform.position);
        //_enemy.agent.destination = _enemy.target.transform.position;
        _enemy.agent.stoppingDistance = _enemy.stopDistance;
        _enemy.animator.SetBool(ANIM_WALK, true);
    }

    public override void UpdateState(Enemy _enemy)
    {
        _enemy.agent.SetDestination(_enemy.target.transform.position);
    }
}
