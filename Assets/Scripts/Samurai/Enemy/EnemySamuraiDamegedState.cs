using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySamuraiDamegedState : EState
{
    public static int ANIM_DAMEGED = Animator.StringToHash("bDameged");
    public override State state => throw new System.NotImplementedException();

    public override void EndState(Enemy _enemy)
    {
        //_enemy.anim.SetBool(ANIM_DAMEGED, false);
    }

    public override void StartState(Enemy _enemy)
    {
       // _enemy.anim.SetBool(ANIM_DAMEGED, true);
    }

    public override void UpdateState(Enemy _enemy)
    {
        
    }

}
