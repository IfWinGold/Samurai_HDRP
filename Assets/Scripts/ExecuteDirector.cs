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
        /// ó�� ���� ex)�Ϲ�,��� ���
        /// </summary>
        public enum State
        {
            Normal,
            Back,
        }
        public PlayableDirector playableDirector;
        public State state;
        /// <summary>
        /// �÷��̾���� �Ÿ�
        /// </summary>
        public float distance;
    }

}
