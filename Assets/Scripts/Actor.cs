using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField] protected ActorCamera actorCamera;
    [SerializeField] protected float fsmoothRotTime = 0.05f;
    [SerializeField] private float flockonDistance = 100f;

    protected CharacterController characterController;
    protected Animator animator;    
    [SerializeField] protected float fmoveSpeed = 3.0f;
    [SerializeField] protected float frunSpeed = 4f;
    [SerializeField] protected float fdashSpeed = 5f;
    [Header("Jump")]
    [SerializeField] protected float fjumpHeight = 3f;
    [SerializeField] protected float fjumpDetectDistance = 1.5f;
    [SerializeField] protected string groundCheckLayerName = "Ground";
    [SerializeField] protected string jumpAnimName = "bJump";
    private bool isJump = false;
    private bool isLanding = false;
    private float flandingCurrentTime = 0f;
    private float ftargetLandingTime = 0f;
    [Space(20)]
    protected AState.State estate = AState.State.Idle;
    protected AState currentState;
    protected AState prevState;
    protected bool bstateFance = false;
    protected static int groundLayer;
    protected static int anim_jump;
    

    public Animator anim { get => animator; }
    public CharacterController charController { get => characterController; }
    public float moveSpeed { get => fmoveSpeed; }
    public float PrevRotAngle { get => prevRotAngle; }
    public float runSpeed { get=> frunSpeed; }
    public float dashSpeed { get => fdashSpeed; }
    public AState PrevState { get => prevState; }
    public AState CurrentState { get => currentState; }
    public Vector3 PrevInputDirection { get => prevInputDirection; }
    public float RotVelocity { get => fRotVelocity; }

    protected Vector3 prevInputDirection = Vector3.zero;
    protected float prevRotAngle = 0f;

    protected float fRotVelocity = 0f;

    [SerializeField] private Renderer[] playerRenderers;

    protected Vector3 lastFixedPosition;
    protected Vector3 nextFixedPosition;
    private float horizontalInput;
    private float verticalInput;

    protected Vector3 actorVelocity = Vector3.zero;

    public ActorCamera GetActorCamera()
    {
        return actorCamera;
    }
    public void SwitchState<T>() where T : AState, new()
    {
        if (bstateFance) return;
        prevState = currentState;
        currentState.EndState(this);
        currentState = new T();
        currentState.StartState(this);
    }
    public void SwitchState<T>(Enemy _enemy) where T : AState, new()
    {
        if (bstateFance) return;
        prevState = currentState;
        currentState.EndState(this);
        currentState = new T();
        currentState.enemy = _enemy;
        currentState.StartState(this);        
    }

    protected virtual void Initialized()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        transform.rotation = Quaternion.identity;
        animator = GetComponentInChildren<Animator>();

        currentState = new IdleState();
        currentState.StartState(this);
    }
    public virtual void UpdateMove(Vector3 _moveDir, float _speed)
    {
        if (isLanding) 
            return;
        prevInputDirection = _moveDir;

        Vector3 dir = actorCamera.camera.transform.TransformDirection(_moveDir);
        if (characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
            else
            {
                if (isJump)
                    Landing();
                actorVelocity.y = 0f;            
            }
        } //IsGrounded 보완
        else if(IsCheckGrounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        actorVelocity.y += Physics.gravity.y * Time.deltaTime;
        dir *=  _speed *Time.deltaTime;
        dir.y = actorVelocity.y * Time.deltaTime;
        characterController.Move(dir);
    }   
    /// <summary>
    /// IsGround 보완용
    /// </summary>
    /// <returns></returns>
    protected bool IsCheckGrounded()
    {
        if (characterController.isGrounded) return true;
        var ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * fjumpDetectDistance, Color.red);
        return Physics.Raycast(ray, fjumpDetectDistance, groundLayer);
    }
    /// <summary>
    /// UpdateMove와 중복사용X
    /// </summary>
    public void UpdateGravity()
    {
        if(characterController.isGrounded)
        {
            actorVelocity.y = 0f;
        }
        else
        {
            actorVelocity.y += Physics.gravity.y * Time.deltaTime;
            characterController.Move(actorVelocity * Time.deltaTime);
        }
    }
    
    private void Jump()
    {
        actorVelocity.y += Mathf.Sqrt(fjumpHeight * -3.0f * Physics.gravity.y);
        animator.SetBool(anim_jump, true);
        isJump = true;
    }
    private void Landing()
    {
        animator.SetBool(anim_jump, false);
        isLanding = true;
        isJump = false;
    }
    protected bool IsFall()
    {
        return characterController.velocity.y < -5f;
    }
    public void OnEndLandingAnim()
    {
        isLanding = false;
    }
    public virtual void UpdateRotateUsingCameraDirection(Vector3 _moveDir)
    {
        Vector3 dir = actorCamera.camera.transform.TransformDirection(_moveDir);
        float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        prevRotAngle = targetAngle;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref fRotVelocity, fsmoothRotTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
    public virtual void UpdateRotateWithAngle(float targetAngle)
    {
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref fRotVelocity, fsmoothRotTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }



    protected virtual void Tick()
    {
        //입력이 없을경우 Idle 상태로 변경

        if (bstateFance) return;

        Vector3 inputDirection = GameUtility.GetInputDirection();
        if (inputDirection == Vector3.zero)
        {
            //Idle
            if(currentState.state != AState.State.Idle)
                SwitchState<IdleState>();
            return;
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                //Dash
                SwitchState<DashState>();
            }            
            else if(Input.GetKey(KeyCode.LeftShift))
            {
                //Run
                SwitchState<RunState>();
            }
            else
            {
                //Walk
                SwitchState<WalkState>();
            }
        }                                          
    }

    public virtual void Damaged(int _dmg)
    {
        if(currentState.state == AState.State.Dash)
        {
            //저스트 회피
            Debug.Log($"Not Damaged!");
        }
        else
        {
            //TODO > Damaged
        }       
    }
    public virtual bool CanParry(Enemy enemy)
    {
        return false;
    }
    public virtual bool IsGuard()
    {
        return false;
    }
    /// <summary>
    /// State 변경을 가능하게 하거나 잠그거나
    /// </summary>
    /// <param name="_active"></param>
    public void SetStateLock(bool _active)
    {
        bstateFance = _active;
    }

    void Start()
    {
        Initialized();
        groundLayer = 1 << LayerMask.NameToLayer(groundCheckLayerName);
        anim_jump = Animator.StringToHash(jumpAnimName);
    }

    void Update()
    {
        Tick();
        
        currentState.UpdateState(this);


        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach(var renderer in playerRenderers)
            {
                renderer.material.SetFloat("_RimPower", 10f);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (var renderer in playerRenderers)
            {
                renderer.material.SetFloat("_RimPower", 0f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, flockonDistance);
    }
}
