using UnityEngine;

[DisallowMultipleComponent]
public class SimpleMASBot : MonoBehaviour, IInputDriver
{
    [Header("Detection / Targeting")]
    public LayerMask enemyLayers;
    public float detectionRadius = 12f;

    [Header("Range")]
    public float preferredRange = 6f;  // 이 거리 안이면 멈추고 공격/스킬
    public float stopRange = 5.2f;     // 이동 멈추는 경계

    [Header("Attack / Skill")]
    public float attackCooldown = 0.7f; // 평타 쿨
    public float skillCooldown = 6f;   // 스킬1 쿨
    public float castTime = 0.35f; // 스킬 시전시간(동안 이동 입력 0)

    [Header("Patrol (No Target)")]
    public float patrolSwitchInterval = 2.5f;

    Character _ch;
    Transform _tgt;
    float _nextAtk, _nextSkill, _castUntil, _patrolUntil;
    int _patrolDir = 1;

    enum Phase { Free, Casting }
    Phase _phase = Phase.Free;

    void Awake()
    {
        _ch = GetComponent<Character>();
        _patrolUntil = Time.time + patrolSwitchInterval;
    }

    public InputSnapshot CollectInput(Character self)
    {
        var s = InputSnapshot.Empty;
        float now = Time.time;

        // 캐스팅 중
        if (_phase == Phase.Casting)
        {
            s.Horizontal = 0f;
            if (now >= _castUntil) _phase = Phase.Free;
            return s;
        }

        // 타겟 갱신 (바라보는 방향 기준 "가장 앞의 적")
        if (_tgt == null || !_tgt.gameObject.activeInHierarchy)
            _tgt = TargetingUtil2D.AcquireFrontmost(self.transform, self.isFacingRight, detectionRadius, enemyLayers);

        // 없으면 순찰
        if (_tgt == null)
        {
            if (now >= _patrolUntil)
            {
                _patrolDir = -_patrolDir;
                _patrolUntil = now + patrolSwitchInterval;
            }
            s.Horizontal = _patrolDir;
            return s;
        }

        // 타겟이 있으면 거리에 따라 이동/공격/스킬
        float dx = _tgt.position.x - self.transform.position.x;
        float adx = Mathf.Abs(dx);

        // 시야 방향
        if (!Mathf.Approximately(dx, 0f))
            self.isFacingRight = dx > 0f;

        // 접근/유지
        if (adx > stopRange)
            s.Horizontal = Mathf.Sign(dx);
        else
            s.Horizontal = 0f;

        // CC 게이트: 상태에 따라 입력 제한 (예: Root/Frozen)
        var cc = self.ConditionState.CurrentState;
        bool canUseSkills = cc != CharacterStatements.ConditionStatement.Silence
                           && cc != CharacterStatements.ConditionStatement.Root
                           && cc != CharacterStatements.ConditionStatement.Frozen;
        bool canNormalAtk = cc != CharacterStatements.ConditionStatement.Root
                           && cc != CharacterStatements.ConditionStatement.Frozen;
        bool canMove = cc != CharacterStatements.ConditionStatement.Root
                           && cc != CharacterStatements.ConditionStatement.Frozen;

        if (!canMove) s.Horizontal = 0f;

        // 사거리 안이면 행동
        if (adx <= preferredRange)
        {
            // 1) 평타 먼저
            if (canNormalAtk && now >= _nextAtk)
            {
                self.target = _tgt.gameObject;  // 평타 프레임에 타겟 주입
                s.NormalAttack = true;
                _nextAtk = now + attackCooldown;
                return s; // 평타 눌렀으면 이번 프레임은 끝
            }

            // 2) 스킬1
            if (canUseSkills && now >= _nextSkill)
            {
                self.target = _tgt.gameObject;  // 시전 시점 좌표 스냅샷용
                s.Skill1 = true;
                _nextSkill = now + skillCooldown;

                // 캐스팅 시간
                if (castTime > 0f)
                {
                    _phase = Phase.Casting;
                    _castUntil = now + castTime;
                    s.Horizontal = 0f;
                }
                return s;
            }
        }

        return s;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.25f);
        var p = transform.position;
        Gizmos.DrawSphere(p, detectionRadius);
    }
#endif
}
