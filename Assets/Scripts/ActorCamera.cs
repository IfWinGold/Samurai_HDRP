using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NSamurai;


public class ActorCamera : MonoBehaviour
{
    [SerializeField] private Actor actor;
    [SerializeField] private CinemachineFreeLook freelookCamera;
    [SerializeField] private CinemachineVirtualCamera executeReadyCamera;
    [SerializeField] private CinemachineVirtualCamera executeCamera;
    [Header("LockOnCamera")]
    [SerializeField] private CinemachineVirtualCamera lockOnCamera;
    [SerializeField] private float flockOnRadius = 100f;
    [SerializeField] private Transform lockOnMid;
    [SerializeField] private Transform testLockOnEnemy;
    private Transform lockOnTarget;

    private bool isLockOn = false;
    private Vector3 midPositionVelocity = Vector3.zero;

    [SerializeField] private CinemachineTargetGroup targetGroup;
    
    public Camera camera { get => Camera.main; }
    public bool IsLockOn { get { return isLockOn; } }
    public Transform LockOnTarget { get { return lockOnTarget; } }
    public void SetLookAt(Transform target,bool _active)
    {
        if(_active)
        {
            lockOnTarget = target;
            UpdateLockOnMidPosition();
            lockOnCamera.gameObject.SetActive(true);
            freelookCamera.gameObject.SetActive(false);
        }
        else
        {
            lockOnTarget = null;
            freelookCamera.m_XAxis.Value = actor.transform.eulerAngles.y ;
            lockOnCamera.gameObject.SetActive(false);
            freelookCamera.gameObject.SetActive(true);
            UIManager.instance.SetActiveLockOnMark(null);
        }
        isLockOn = _active;
        actor.anim.SetBool("bLockOn", isLockOn);
    }
    public void SetExecuteReadyCameraLookAt(Transform transform)
    {
        executeReadyCamera.LookAt = transform;
    }
    public void SetExecuteCameraLookAt(Transform transform)
    {
        executeCamera.LookAt = transform;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isLockOn)
            {
                SetLookAt(null, false);
                isLockOn = false;
            }
            else
            {
                Collider enemy = null;
                if(CanCameraLockOn(out enemy))
                {
                    SetLookAt(enemy.transform, true);
                    UIManager.instance.SetActiveLockOnMark(enemy.transform);
                    isLockOn = true;
                }
            }
        }

        if (isLockOn)
        {
            UpdateLockOnMidPosition();
        }                   
    }
    private void UpdateLockOnMidPosition()
    {
        Vector3 midPosition = (actor.transform.position + lockOnTarget.transform.position) / 2f;
        midPosition.y = 0f;
        lockOnMid.transform.position = Vector3.SmoothDamp(lockOnMid.transform.position, midPosition, ref midPositionVelocity,2f);
    }

    public bool CanCameraLockOn(out Collider enemy)
    {
        Collider[] enemis = Physics.OverlapSphere(transform.position, flockOnRadius, 1 << LayerMask.NameToLayer("Enemy"));
        if (enemis.Length <= 0)
        {
            enemy = null;
            return false;
        }
        else
        {
            float minDistance = flockOnRadius;
            Collider minTarget = null;
            foreach (Collider c in enemis)
            {
                float distance = Vector3.Distance(actor.transform.position, c.gameObject.transform.position);
                if (distance <= minDistance)
                {
                    minDistance = distance;
                    minTarget = c;
                }
            }
            enemy = minTarget;
            return true;
        }
    }
}
