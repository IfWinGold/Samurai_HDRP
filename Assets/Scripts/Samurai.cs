using NSamurai;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class Samurai : Actor
{
    [SerializeField] private AudioClip[] sfxBlock;
    [SerializeField] private AudioClip[] sfxdameged;
    [SerializeField] private AudioClip[] sfxparrys;
    [SerializeField] private VisualEffect vfxKatanaParry;
    [SerializeField] private VisualEffect vfxKatanaBlock;
    [SerializeField] private VisualEffect vfxKatanaBlood;
    [SerializeField] private float fdetectRadius;
    [SerializeField] private float fassassinationNormalRadius;
    [SerializeField] private float fassassinationAirRadius;
    [SerializeField] private float fassassinationAirAngle = -45f;
    //9,10,11,30,31

    [SerializeField] protected SamuraiEnemy enemyPrefab;
    private AudioSource audioSource;
    private SamuraiAttackSystem attackSystem;
    private bool bendAnimFence = false;    

    public ExecuteDirector PlayedExecuteDirector { get => playedExecuteDirector; }
    public ExecuteDirector ExecuteReadyDirector { get => playedExecuteReadyDirector; }

    public List<SamuraiEnemy> detecteEnemies = new List<SamuraiEnemy>();
    public List<SamuraiEnemy> ltriggerEnemies = new List<SamuraiEnemy>();


    [SerializeField] private ExecuteDirector[] normalExecuteDirectors;
    [SerializeField] private ExecuteDirector[] backExecuteDirectors;
    [SerializeField] private ExecuteDirector[] airExecuteDirectors;
    [SerializeField] private ExecuteDirector[] executeReadyDirectors;

    private ExecuteDirector playedExecuteDirector = null;
    private ExecuteDirector playedExecuteReadyDirector = null;

    private List<SamuraiEnemy> prevDetectEnemies = new List<SamuraiEnemy>();
    protected override void Initialized()
    {
        base.Initialized(); 
        audioSource = GetComponent<AudioSource>();
    }
    protected override void Tick()
    {
        prevDetectEnemies = detecteEnemies;
        detecteEnemies = GetDetecteEnemy();
        //UpdateExecuteState(); //Execute상태를 풀려야 할 적은 풀어줌
        //입력이 없을경우 Idle 상태로 변경

        if (bstateFance) return;

        //Attack && Execute
        if (Input.GetMouseButtonDown(0))
        {
            List<SamuraiEnemy> air = GetAirExecuteEnemies(detecteEnemies);
            if(air.Count > 0)
            {
                float minDis = fassassinationAirRadius;
                SamuraiEnemy target = null;
                foreach(SamuraiEnemy enemy in air)
                {
                    float dis = Vector3.Distance(enemy.transform.position, this.transform.position);
                    if(dis <= minDis)
                    {
                        minDis = dis;
                        target = enemy;
                    }
                }
                if(CanAirAssassination(target))
                {
                    SwitchState<SamuraiAirExecuteState>(target);
                }                
            }

            SamuraiEnemy samuraiEnemy = null;
            if(CanExecuteInTriggerEnemy(out samuraiEnemy))
            {
                SwitchState<SamuraiExecuteState>(samuraiEnemy);
            }
            else
            {
                SwitchState<SamuraiAttackState>();
            }            
        }

        //TODO >> GUARD
        if (Input.GetMouseButtonDown(1)||Input.GetMouseButton(1))
        {
            SwitchState<SamuraiGuardState>();
        }        




        Vector3 inputDirection = GameUtility.GetInputDirection();        

        //Debug.Log($"InputDirection : {inputDirection} , state:{currentState.state} , velocity : {characterController.velocity}");

        if (inputDirection == Vector3.zero)
        {
            //Idle
            if (currentState.state != AState.State.Idle && !actorCamera.IsLockOn)
            {
                    SwitchState<SamuraiIdleState>();                
            }
                

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //HeavyAttack
                SwitchState<SamuraiHeavyAttackState>();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                //Dash
                SwitchState<SamuraiDashState>();
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                //Run
                SwitchState<RunState>();
            }
            else
            {
                //Walk
                SwitchState<SamuraiWalkState>();
            }


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //HeavyAttack
                SwitchState<SamuraiHeavyAttackState>();
            }

        }

    }
    #region Detect
    private List<SamuraiEnemy> GetDetecteEnemy()
    {
        List<SamuraiEnemy> samuraiEnemies = new List<SamuraiEnemy>();
        Collider[] detect = Physics.OverlapSphere(this.transform.position, fdetectRadius,1 << LayerMask.NameToLayer("Enemy"));
        foreach (var collider in detect) 
        {
            SamuraiEnemy se = collider.gameObject.GetComponent<SamuraiEnemy>();
            if (se != null)
            {
                samuraiEnemies.Add(se);
            }

            if (CanDetectAirAssassination(se))
            {
                se.SetActiveExecuteState(true);
            }
            else if (CanAssassination(se))
            {
                se.SetActiveExecuteState(true);
            }
            else if (!CanExecute(se))
            {
                se.SetActiveExecuteState(false);
            }
            else if(se.State == EState.State.Stun)
            {
                se.SetActiveExecuteState(true);
            }
        }
        return samuraiEnemies;
    }
    private List<SamuraiEnemy> GetAirExecuteEnemies(List<SamuraiEnemy> enimies)
    {
        List<SamuraiEnemy> result = new List<SamuraiEnemy>();
        foreach (var enemy in enimies)
        {
            if (CanDetectAirAssassination(enemy))
            {
                result.Add(enemy);            
            }
        }
        return result;
    }
    public bool CanDetectAirAssassination(SamuraiEnemy se)
    {
        Vector3 dis = se.transform.position - this.transform.position;
        float distance = dis.magnitude;
        float angle = Mathf.Atan2(dis.y, dis.z) * Mathf.Rad2Deg;
        if (distance <= fassassinationAirRadius && angle <= fassassinationAirAngle)
        {
            return true;
        }
        return false;
    }
    public bool CanAirAssassination(SamuraiEnemy se)
    {
        return CanDetectAirAssassination(se) && IsFall();
    }
    public bool CanAssassination(SamuraiEnemy se)
    {
        Vector3 forward = se.transform.forward;
        Vector3 dir = se.transform.position - this.transform.position;
        float distance = dir.magnitude;
        float dot = Vector3.Dot(se.transform.forward, dir.normalized);
        if (distance <= fassassinationNormalRadius && dot > 0)
        {
            return true;
        }
        else
            return false;
    }
    public bool CanExecute(Enemy enemy)
    {
        float guage = enemy.ExecutionGuage;
        return Mathf.Approximately(guage, Enemy.EXECUTION_MAX);
    }
    private List<SamuraiEnemy> GetAssassinationEnemies(List<SamuraiEnemy> enimies)
    {
        List<SamuraiEnemy> result = new List<SamuraiEnemy>();
        foreach (var enemy in enimies)
        {
            if(CanAssassination(enemy))
            {
                result.Add(enemy);
            }
        }
        return result;
    }
    /// <summary>
    /// Execute상태가 아닌 Enemy를 해제
    /// </summary>
    #endregion
    private bool CanExecuteInTriggerEnemy(out SamuraiEnemy enemy)
    {
        Enemy executeEnemy = ltriggerEnemies.Find(x => CanAssassination(x as SamuraiEnemy));
        enemy =  executeEnemy as SamuraiEnemy;
        return executeEnemy!=null;
    }
    public override void Damaged(int _dmg)
    {
        if (currentState.state == AState.State.Execute || CurrentState.state == AState.State.ExecuteReady) return;
        if(currentState.state != AState.State.Dameged)
        {
            SetStateLock(false);
            SwitchState<SamuraiDamegedState>();
            Debug.Log("Damaeged");
            SFXPlayDameged();
            //TODO > Dameged
        }
    }
    public void Attack()
    {
        for (int i = 0; i < ltriggerEnemies.Count; ++i)
        {           
            if (ltriggerEnemies[i] != null)
            {
                SamuraiEnemy samuraiEnemy = ltriggerEnemies[i] as SamuraiEnemy;
                if(samuraiEnemy != null)
                {
                    if (CanAssassination(samuraiEnemy))
                    {
                        SetStateLock(false);
                        SwitchState<SamuraiExecuteState>(samuraiEnemy);
                        return;
                    }
                    if(CanExecute(samuraiEnemy))
                    {
                        SetStateLock(false);
                        SwitchState<SamuraiExecuteState>(samuraiEnemy);
                        return;
                    }
                    ltriggerEnemies[i].Damaged(10);
                }               
            }
        }
    }

    public ExecuteDirector Execute(Enemy _enemy)
    {
        Vector3 dir = _enemy.transform.position - this.transform.position;
        float dot = Vector3.Dot(_enemy.transform.forward, dir.normalized);
        ExecuteDirector executeDirector;

        if (dot > 0)
            executeDirector = backExecuteDirectors[Random.Range(0, backExecuteDirectors.Length)];
        else
            executeDirector = normalExecuteDirectors[Random.Range(0, normalExecuteDirectors.Length)];

        //Rotate 세팅
        Vector3 lookPos = _enemy.transform.position;
        lookPos.y = this.transform.position.y;
        
        this.transform.LookAt(lookPos);        

        TimelineAsset timeline = executeDirector.playableDirector.playableAsset as TimelineAsset;
        SamuraiEnemy samuraiEnemy = _enemy as SamuraiEnemy;

        foreach (var track in timeline.GetOutputTracks())
        {
            if(track.name == "Enemy")
            {
                executeDirector.playableDirector.SetGenericBinding(track, _enemy.animator);                
            }
            else if(track.name == "Blood")
            {
                executeDirector.playableDirector.SetGenericBinding(track, samuraiEnemy.GetComponent<SignalReceiver>());
            }
        }
        actorCamera.SetExecuteCameraLookAt(_enemy.transform);
        executeDirector.playableDirector.Play();
        playedExecuteDirector = executeDirector;
        return executeDirector;
    }
    public ExecuteDirector AirExecute(Enemy _enemy)
    {
        ExecuteDirector executeDirector = airExecuteDirectors[Random.Range(0,airExecuteDirectors.Length)];

        //Rotate 세팅
        Vector3 lookPos = _enemy.transform.position;
        lookPos.y = this.transform.position.y;

        this.transform.LookAt(lookPos);
        TimelineAsset timeline = executeDirector.playableDirector.playableAsset as TimelineAsset;
        SamuraiEnemy samuraiEnemy = _enemy as SamuraiEnemy;

        foreach (var track in timeline.GetOutputTracks())
        {            
            if(track is PlayableTrack playable && track.name == "Playable")
            {
                var clips = track.GetClips();
                
                foreach(var clip in clips)
                {
                    CustomTargetMovePlayableAsset customClip = clip.asset as CustomTargetMovePlayableAsset;
                    ExposedReference<Transform> exposeRef = customClip.player1;
                    ExposedReference<Transform> exposeRef2 = customClip.player2;
                    Transform samurai = exposeRef.Resolve(executeDirector.playableDirector);
                    Transform enemy = exposeRef2.Resolve(executeDirector.playableDirector);
                    Debug.Log($"Player1Value:{samurai.name}");
                    Debug.Log($"Player2Value:{enemy.name}");
                    
                }
            }



            if (track.name == "Enemy")
            {
                executeDirector.playableDirector.SetGenericBinding(track, _enemy.animator);
            }
            else if (track.name == "Blood")
            {
                executeDirector.playableDirector.SetGenericBinding(track, samuraiEnemy.GetComponent<SignalReceiver>());
            }
        }
        actorCamera.SetExecuteCameraLookAt(_enemy.transform);
        executeDirector.playableDirector.Play();
        playedExecuteDirector = executeDirector;
        return executeDirector;
    }
    //public ExecuteDirector Assassination(Enemy _enemy)
    //{

    //}

    public ExecuteDirector SetExecuteReady(Enemy _enemy)
    {
        ExecuteDirector readyDirector = executeReadyDirectors[Random.Range(0, executeReadyDirectors.Length)];
        TimelineAsset timeline = readyDirector.playableDirector.playableAsset as TimelineAsset;
        actorCamera.SetExecuteReadyCameraLookAt(_enemy.transform);
        foreach (var track in timeline.GetOutputTracks())
        {
            SamuraiEnemy samuraiEnemy = _enemy as SamuraiEnemy;
            if (track.name == "Enemy")
            {
                readyDirector.playableDirector.SetGenericBinding(track, _enemy.animator);
            }
            else if (track.name == "Blood")
            {
                readyDirector.playableDirector.SetGenericBinding(track, samuraiEnemy.GetComponent<SignalReceiver>());
            }
        }
        readyDirector.playableDirector.Play();
        playedExecuteReadyDirector = readyDirector;
        return readyDirector;
    }

    /// <summary>
    /// ENEMY에서 호출
    /// </summary>
    /// <returns></returns>
    public override bool CanParry(Enemy enemy)
    {
        SamuraiGuardState guardState = currentState as SamuraiGuardState;
        if (guardState == null) return false;
        
        return guardState.CanParry();
    }
    public void Parry(Enemy se)
    {
        if(CanExecute(se))
        {
            SetStateLock(false);
            se.SwitchState<EnemySamuraiStunState>();
            SwitchState<SamuraiExecuteReadyState>(se);
            SamuraiEnemy samuraiEnemy = se as SamuraiEnemy;            
        }
        else
        {
            SetStateLock(false);
            SwitchState<SamuraiParryState>();
        }
    }
    public override bool IsGuard()
    {
        SamuraiGuardState guardState = currentState as SamuraiGuardState;
        guardState?.Guard(this);

        return guardState != null;
    }
    public override void UpdateRotateUsingCameraDirection(Vector3 _moveDir)
    {
        ActorCamera actorcamera = GetActorCamera();
        if(actorcamera.IsLockOn)
        {
            Vector3 dir = (actorcamera.LockOnTarget.position - this.transform.position).normalized;
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref fRotVelocity, fsmoothRotTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            base.UpdateRotateUsingCameraDirection(_moveDir);
        }                
    }    



    public void OnEndAttackAnimation()
    {
        if (bendAnimFence) 
            return;
        //Debug.Log("OnEndAttackAnimation");
        //currentState.EndState(this);
    }


    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 54;
        style.normal.textColor = Color.red;
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        float progresss = info.normalizedTime % 1; //루프 고려
        GUI.Label(new Rect(5, 525, Screen.width, 20), $"CurrentState:{currentState.state}",style);
        GUI.Label(new Rect(5, 425, Screen.width, 20), $"frotVelocity:{fRotVelocity}", style);
        GUI.Label(new Rect(5, 325, Screen.width, 20), $"fmoveYVelocity:{characterController.velocity.y}", style);
    }
    public void SFXPlayBlock()
    {
        audioSource.PlayOneShot(sfxBlock[Random.Range(0, sfxBlock.Length)]);
    }
    public void SFXPlayDameged()
    {
        audioSource.PlayOneShot(sfxdameged[Random.Range(0, sfxdameged.Length)]);
    }
    public void SFXPlayParry()
    {
        audioSource.PlayOneShot(sfxparrys[Random.Range(0, sfxparrys.Length)]);
    }
    public void VFXPlayParry()
    {
        vfxKatanaParry.Play();
    }
    public void VFXPlayBlock()
    {
        vfxKatanaBlock.Play();
    }
    public void SetEndAnimationLock(bool _active)
    {
        bendAnimFence = _active;
    }
    public bool IsHiding(SamuraiEnemy samuraiEnemy)
    {
        return !samuraiEnemy.IsDetected(this.transform);
    }
    public void OnTriggerEnter(Collider other)
    {
        SamuraiEnemy enemy = other.GetComponent<SamuraiEnemy>();
        if (enemy != null)
        {
            if (enemy.State == EState.State.Executed)
                return;

            ltriggerEnemies.Add(enemy);

        }
        Debug.Log($"{other.gameObject.name}");
    }
    public void OnTriggerStay(Collider other)
    {

    }
    public void OnTriggerExit(Collider other)
    {
        SamuraiEnemy enemy = other.GetComponent<SamuraiEnemy>();
        if (enemy != null)
        {
            if (ltriggerEnemies.Contains(enemy))
                ltriggerEnemies.Remove(enemy);
        }
    }
    private void OnDrawGizmos()
    {
        //White = detect
        //green = execute
        Gizmos.DrawWireSphere(transform.position, fdetectRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,fassassinationAirRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fassassinationNormalRadius);

    }
}
