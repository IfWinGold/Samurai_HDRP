using NSamurai;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SamuraiExecuteReadyState : AState
{
    public override State state => State.ExecuteReady;

    PlayableDirector playableDirector;
    Samurai samurai;
    float progress = 0f;
    public override void EndState(Actor actor)
    {
        actor.SetStateLock(false);
    }

    public override void StartState(Actor actor)
    {
        actor.SetStateLock(true);
        samurai = actor as Samurai;
        ActorCamera ac = actor.GetActorCamera();
        if (ac.IsLockOn)
            ac.SetLookAt(null, false);

        ExecuteDirector director = samurai.SetExecuteReady(enemy);
        playableDirector = director.playableDirector;
        playableDirector.stopped += (x) =>
        {
            //TODO
            EndState(actor);
        };
    }

    public override void UpdateState(Actor actor)
    {
        progress = (float)(playableDirector.time / playableDirector.duration);

        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log($"InputDown");
            playableDirector.Stop();
            samurai.SetStateLock(false);
            samurai.SwitchState<SamuraiExecuteState>(enemy);
        }
        Debug.Log($"Ready Update State");
    }
}
