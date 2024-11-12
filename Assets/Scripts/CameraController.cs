using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace NSamurai
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance;
        [SerializeField] private ActorCamera actorCamera;
        [SerializeField] private float flockOnRadius = 100f;
        [SerializeField] private Actor actor;

        private Transform lockonTarget;

        private bool isCameraLockon = false;
        public bool IsCameraLockOn { get => IsCameraLockOn; }
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);
        }
        void Update()
        {

        }


    }

}
