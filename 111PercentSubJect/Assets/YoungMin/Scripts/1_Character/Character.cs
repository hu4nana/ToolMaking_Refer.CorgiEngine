using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// Basic Movement is Left, Right That's it
// Hp, No Mp Just CoolTime
// Move, NormalAttack(_Arched Orbit_), Skill_1.2.3.4.5

// CC__Need CharacterConditionState....Maybe?
// Silence, Root, Frozen
// Sllence( Can  NormalAttack Only ), Root( Cant NormalAttack And Skills ),
// Frozen, Airborne ( Controlled )


// NormalAttack go to the enemy position at the moment of execution
// 화살이 발사될 때 가장 앞에 있는 적의 위치로 감

// Skill Casting Time

// 공통되는 행동
// Move, NormalAttack, Skill

public struct InputSnapshot
{
    public float Horizontal;     // -1 ~ 1
    public bool NormalAttack;    // 해당 프레임만 true
    public bool Skill1;
    public bool Skill2;
    public bool Skill3;
    public bool Skill4;

    public static InputSnapshot Empty => new InputSnapshot();
}

public interface IInputDriver
{
    // Character가 매 프레임 호출해서 입력을 가져옵니다.
    InputSnapshot CollectInput(Character self);
}


public class Character : MonoBehaviour
{
    public enum FacingDirection { Left, Right }
    public enum CharacterType { Player, AI}

    [Header("CharacterType")]
    public CharacterType _characterType;

    [Header("Name")]
    public string id;


    readonly List<CharacterModule> modules = new();

    protected Animator ani;
    public Rigidbody2D rigid { get; private set; }

    public FacingDirection InitialFacingDirection = FacingDirection.Right;

    public StateMachine<CharacterStatements.ConditionStatement> ConditionState;
    public StateMachine<CharacterStatements.ActionStatement> ActionState;

    public bool isFacingRight {  get; set; }

    public CharacterStatements.ActionStatement _curActionState;
    public CharacterStatements.ConditionStatement _curConditionState;


    public T FindModule<T>() where T : CharacterModule => modules.OfType<T>().FirstOrDefault();

    public GameObject target;

    #region About Player Input
    public float Input_Horizontal { get; private set;}
    public bool Input_NormalAttack { get; private set; }
    public bool Input_Skill1 { get; private set; }
    public bool Input_Skill2 { get; private set; }
    public bool Input_Skill3 { get; private set; }
    public bool Input_Skill4 { get; private set; }
    #endregion

    #region AI Input Driver
    private IInputDriver _inputDriver;
    #endregion

    #region About GroundCheck

    LayerMask groundMask;
    readonly float downOffset = 0.05f;
    readonly float edgeInset = 0f;
    readonly float maxSlopeAngle = 60f;


    protected BoxCollider2D col;
    float minGroundNormalY;
    protected RaycastHit2D groundHIt;

    [SerializeField]
    public bool isGrounded {  get; private set; }
    #endregion

    public void CacheModules()
    {
        modules.Clear();
        modules.AddRange(GetComponents<CharacterModule>());
        foreach (var module in modules)
        {
            module.Initialization();
            Debug.Log($"{module.GetType().Name}");
        }
        
    }

    private void Awake()
    {
        Initialization();
    }

    public void Initialization()
    {
        rigid=GetComponent<Rigidbody2D>();
        ani = gameObject.GetComponent<Animator>();
        ConditionState=new StateMachine<CharacterStatements.ConditionStatement>();
        ActionState=new StateMachine<CharacterStatements.ActionStatement>();
        if(InitialFacingDirection == FacingDirection.Right) isFacingRight = true;

        _curActionState = CharacterStatements.ActionStatement.Idle;
        _curConditionState = CharacterStatements.ConditionStatement.Normal;

        ActionState.ChangeState(CharacterStatements.ActionStatement.Idle);
        ConditionState.ChangeState(CharacterStatements.ConditionStatement.Normal);

        minGroundNormalY = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
        groundMask = LayerMask.GetMask("Platform");
        col = GetComponent<BoxCollider2D>();

        CacheModules();

        _inputDriver = GetComponent<IInputDriver>(); // 같은 GO에 붙은 봇/플레이어 입력 드라이버
    }

    private void Update()
    {
        EveryFrame();
    }


    protected virtual void EveryFrame()
    {
        // 1) 입력
        if (_characterType == CharacterType.Player)
        {
            InputHandler(); // 기존 플레이어 입력
        }
        else // AI
        {
            if (_inputDriver != null)
            {
                var s = _inputDriver.CollectInput(this);
                ApplyInputSnapshot(s);
            }
            else
            {
                ApplyInputSnapshot(InputSnapshot.Empty);
            }
        }

        // 2) 모듈 처리
        if (Time.timeScale != 0f)
        {
            ProcessModules();
            LateProcessModules();
        }

        // 3) 공용 시스템
        GroundCheck();
        UpdateAnimators();
        RotateModel();
    }

    public void ApplyInputSnapshot(InputSnapshot s)
    {
        Input_Horizontal = Mathf.Clamp(s.Horizontal, -1f, 1f);
        Input_NormalAttack = s.NormalAttack;
        Input_Skill1 = s.Skill1;
        Input_Skill2 = s.Skill2;
        Input_Skill3 = s.Skill3;
        Input_Skill4 = s.Skill4;

        if (!Mathf.Approximately(Input_Horizontal, 0f))
            isFacingRight = Input_Horizontal > 0f;
    }
    protected virtual void InputHandler()
    {
        Input_Horizontal = Input.GetAxisRaw("Horizontal");
        Input_NormalAttack = Input.GetButtonDown("NormalAttack");
        Input_Skill1 = Input.GetButtonDown("Skill1");
        Input_Skill2 = Input.GetButtonDown("Skill2");
        Input_Skill3 = Input.GetButtonDown("Skill3");
        Input_Skill4 = Input.GetButtonDown("Skill4");
    }

    #region About GroundCheck

    public virtual void GroundCheck()
    {
        isGrounded = Grounded(out groundHIt);
        //Debug.Log(isGrounded);
    }


    public bool Grounded(out RaycastHit2D hit)
    {
        var b = col.bounds;                       // 월드 AABB
        float y = b.min.y - downOffset;            // 하단보다 살짝 아래
        Vector2 a = new(b.min.x + edgeInset, y);   // 왼쪽 시작점(안쪽으로 살짝)
        Vector2 c = new(b.max.x - edgeInset, y);   // 오른쪽 끝점(안쪽으로 살짝)

        Debug.DrawLine(a, c, Color.cyan);          // 씬에서 시각화

        hit = Physics2D.BoxCast(b.center, b.size, 0f, Vector2.down, 0.05f, groundMask);
        if (hit.collider == null) return false;

        // 경사 필터(너무 가파른 면 제외)
        if (hit.normal.y < minGroundNormalY) return false;

        return true;
    }

    #endregion

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!col) col = GetComponent<BoxCollider2D>();
        var b = col.bounds;
        float y = b.min.y - downOffset;
        Vector3 L = new(b.min.x + edgeInset, y, 0f);
        Vector3 R = new(b.max.x - edgeInset, y, 0f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(L, R);
    }
#endif

    protected virtual void EarlyProcessModule() { }
    public virtual void ProcessModule() { }
    public virtual void LateProcessModule() { }

    public virtual void ProcessModules()
    {
        foreach (CharacterModule module in modules)
        {
            if (module.enabled)
            {
                module.ProcessModule();
            }
        }
    }

    public virtual void LateProcessModules()
    {
        foreach (CharacterModule module in modules)
        {
            if (module.enabled)
            {
                module.LateProcessModule();
            }
        }
    }

    public virtual void UpdateAnimators()
    {
        
        if (ani != null &&
            _curActionState != ActionState.CurrentState)
        {
            for (int i = 0; i < EnumUtil<CharacterStatements.ActionStatement>.Count; i++)
            {
                if (EnumUtil<CharacterStatements.ActionStatement>.FromIndex(i) != ActionState.CurrentState)
                {
                    ani.SetBool($"{EnumUtil<CharacterStatements.ActionStatement>.FromIndex(i)}", false);
                }
                else
                {
                    ani.SetBool($"{EnumUtil<CharacterStatements.ActionStatement>.FromIndex(i)}", true);
                }
            }
            _curActionState=ActionState.CurrentState;
        }
    }

    public virtual void ChangeActionStateToIdle()
    {
        if (ani.GetBool("Move") && Input_Horizontal==0 && isGrounded)
        {
            ActionState.ChangeState(CharacterStatements.ActionStatement.Idle);
            //Debug.Log("Animation Event Was Activated");
        }
    }
    protected virtual void RotateModel()
    {
        //Debug.Log(isFacingRight);
        if (isFacingRight)
            this.transform.rotation= Quaternion.Euler(0, 0, 0);
        else
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
