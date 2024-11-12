using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class CustomTargetMovePlayableAsset : PlayableAsset
{
    public ExposedReference<Transform> player1;
    public ExposedReference<Transform> player2;
    public Vector3 startOffset;
    public Vector3 endOffset;
    public AnimationCurve moveCurveX;
    public AnimationCurve moveCurveY;
    public AnimationCurve moveCurveZ;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CustomTargetMovePlayableBehavior>.Create(graph);

        var behaviour = playable.GetBehaviour();
        behaviour.player1 = player1.Resolve(graph.GetResolver());
        behaviour.player2 = player2.Resolve(graph.GetResolver());
        behaviour.startOffset = startOffset;
        behaviour.endOffset = endOffset;
        behaviour.moveCurveX = moveCurveX;
        behaviour.moveCurveY = moveCurveY;
        behaviour.moveCurveZ = moveCurveZ;
        return playable;
    }
}
