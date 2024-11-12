using NSamurai;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiAirExecuteState : AState
{
    public override State state => State.Execute;
    public override void EndState(Actor actor)
    {
        actor.SetStateLock(false);
    }

    public override void StartState(Actor actor)
    {
        actor.SetStateLock(true);
        //락온 상태이면 락온해제
        ActorCamera camera = actor.GetActorCamera();
        if (camera.IsLockOn && camera.LockOnTarget == enemy.transform)
        {
            camera.SetLookAt(null, false);
        }

        SamuraiEnemy samuraiEnemy = null;
        samuraiEnemy = enemy as SamuraiEnemy;
        Samurai samurai = actor as Samurai;

        ExecuteDirector executeDirector = samurai.AirExecute(enemy);

        if (samuraiEnemy != null)
        {
            samuraiEnemy.Executed(actor, executeDirector);
        }

        executeDirector.playableDirector.stopped += (director) =>
        {
            if (samurai.ltriggerEnemies.Contains(samuraiEnemy))
                samurai.ltriggerEnemies.RemoveAll(x => x == enemy);

            EndState(actor);
            samuraiEnemy.Kill();
        };
    }
    public override void UpdateState(Actor actor)
    {

    }

    private void TimelineSet()
    {

    }

}
