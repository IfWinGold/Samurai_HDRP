using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CustomTargetMovePlayableBehavior : PlayableBehaviour
{
    public Transform player1;
    public Transform player2;
    public Vector3 startOffset;
    public Vector3 endOffset;
    public AnimationCurve moveCurveX;
    public AnimationCurve moveCurveY;
    public AnimationCurve moveCurveZ;

    private Vector3 initialPosition = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;

    // 타임라인이 시작될 때 호출되는 메서드
    public override void OnGraphStart(Playable playable)
    {
        if (player1 != null && player2 != null)
        {
            initialPosition = player1.position + startOffset;
            targetPosition = player2.position + endOffset;
        }
    }
    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);
        if(player1 != null && player2 != null)
        {
            //player1.transform.position = initialPosition;
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        ProcessFrame(playable, info,null);
    }

    // 타임라인이 매 프레임마다 호출되는 메서드
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (player1 != null)
        {
            double normalizedTime = playable.GetTime() / playable.GetDuration();            
            float xT = moveCurveX.Evaluate((float)normalizedTime);
            float yT = moveCurveY.Evaluate((float)normalizedTime);
            float zT = moveCurveZ.Evaluate((float)normalizedTime);

            float posX = Mathf.LerpUnclamped(initialPosition.x, targetPosition.x, xT);
            float posY = Mathf.LerpUnclamped(initialPosition.y, targetPosition.y, yT);
            float posZ = Mathf.LerpUnclamped(initialPosition.z, targetPosition.z, zT);
            Vector3 movePosition = new Vector3(posX, posY, posZ);
            //player1.position = Vector3.Lerp(initialPosition, targetPosition,moveCurve.Evaluate((float)normalizedTime));
            player1.position = movePosition;
        }
    }
}
