using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Playables;

namespace NSamurai
{
    [Serializable]
    public class ExecuteDirector
    {
        /// <summary>
        /// 처형 방향 ex)일반,기습 등등
        /// </summary>
        public enum State
        {
            Normal,
            Back,
        }
        public PlayableDirector playableDirector;
        public State state;
        /// <summary>
        /// 플레이어와의 거리
        /// </summary>
        public float distance;
    }

}
