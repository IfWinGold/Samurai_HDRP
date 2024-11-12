using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySamuraiExecutedState : EState
{
    public override State state => EState.State.Executed;

    public override void EndState(Enemy _enemy)
    {

    }

    public override void StartState(Enemy _enemy)
    {
        _enemy.SetStateLock(true);
        SamuraiEnemy samuraiEnemy = _enemy as SamuraiEnemy;

        samuraiEnemy.capsuleCollider.enabled = false;

        samuraiEnemy.agent.enabled = false;
        //samuraiEnemy.enabled = false;
    }

    public override void UpdateState(Enemy _enemy)
    {
        
    }
}
