using UnityEngine;

[DisallowMultipleComponent]
public class PatternBot : MonoBehaviour, IInputDriver
{
    [Header("Targeting")]
    public LayerMask enemyLayers;
    public float detectionRadius = 60f;  // 아주 크게 잡아 거리 무시

    [Header("Pattern Durations")]
    public Vector2 moveBeforeAttack = new Vector2(2f, 5f); // 2~5초 이동 후 평타
    public Vector2 moveBeforeSkill = new Vector2(1f, 4f); // 평타 후 1~4초 이동 후 스킬

    [Header("Skill Casting")]
    public float castTime = 0.35f;       // 스킬 캐스팅 동안 정지
    public bool faceTargetOnFire = true; // 발사/시전 직전 타겟 방향으로 회전

    [Header("Collision (Platform)")]
    public LayerMask platformMask;       // Platform 레이어 지정
    public float frontProbeDistance = 0.15f; // 전방 탐지 거리(짧게)
    public float wallFlipCooldown = 0.25f;   // 반전 후 튕김 방지 시간

    private Character _ch;
    private BoxCollider2D _col;
    private int _dir = 1;                // -1: 왼쪽, 1: 오른쪽
    private float _phaseUntil;
    private float _nextWallFlipAllowed;

    private enum Phase { MoveA, Attack, MoveB, Skill, Casting }
    private Phase _phase = Phase.MoveA;

    void Awake()
    {
        _ch = GetComponent<Character>();
        _col = GetComponent<BoxCollider2D>();
        if (platformMask.value == 0) platformMask = LayerMask.GetMask("Platform"); // 편의상 기본값
        _dir = (_ch && _ch.isFacingRight) ? 1 : -1;
        _phaseUntil = Time.time + Random.Range(moveBeforeAttack.x, moveBeforeAttack.y);
    }

    public InputSnapshot CollectInput(Character self)
    {
        var s = InputSnapshot.Empty;
        float now = Time.time;

        // 거리 상관없이 "앞쪽에서 가장 앞"의 적을 매 프레임 갱신
        var tgt = TargetingUtil2D.AcquireFrontmost(self.transform, self.isFacingRight, detectionRadius, enemyLayers);
        if (tgt != null)
            self.target = tgt.gameObject;  // 평타/스킬 프레임에 스냅샷으로 사용

        switch (_phase)
        {
            // 스킬 캐스팅 동안은 가만히
            case Phase.Casting:
                s.Horizontal = 0f;
                if (now >= _phaseUntil) StartMoveA(now);
                break;

            // 2~5초 이동
            case Phase.MoveA:
                s.Horizontal = _dir;
                HandleWallFlip(now, ref s);
                if (now >= _phaseUntil)
                    _phase = Phase.Attack;
                break;

            // 평타 1회 (거리 무시)
            case Phase.Attack:
                if (faceTargetOnFire && self.target)
                    self.isFacingRight = self.target.transform.position.x >= self.transform.position.x;

                s.NormalAttack = true; // 한 프레임만 true
                StartMoveB(now);
                break;

            // 1~4초 이동
            case Phase.MoveB:
                s.Horizontal = _dir;
                HandleWallFlip(now, ref s);
                if (now >= _phaseUntil)
                    _phase = Phase.Skill;
                break;

            // 스킬 1회 (거리 무시)
            case Phase.Skill:
                if (faceTargetOnFire && self.target)
                    self.isFacingRight = self.target.transform.position.x >= self.transform.position.x;

                s.Skill3 = true; // 한 프레임만 true

                if (castTime > 0f)
                {
                    _phase = Phase.Casting;
                    _phaseUntil = now + castTime;
                    s.Horizontal = 0f;
                }
                else
                {
                    StartMoveA(now);
                }
                break;
        }

        return s;
    }

    void StartMoveA(float now)
    {
        _phase = Phase.MoveA;
        _phaseUntil = now + Random.Range(moveBeforeAttack.x, moveBeforeAttack.y);
        // 방향은 유지. 랜덤 스타트를 원하면 다음 줄 사용:
        // _dir = Random.value < 0.5f ? -1 : 1;
    }

    void StartMoveB(float now)
    {
        _phase = Phase.MoveB;
        _phaseUntil = now + Random.Range(moveBeforeSkill.x, moveBeforeSkill.y);
    }

    // 전방 Platform 벽 체크 → 부딪히면 반전
    void HandleWallFlip(float now, ref InputSnapshot s)
    {
        if (_col == null) return;
        if (now < _nextWallFlipAllowed) return;

        Vector2 dir = (_dir > 0) ? Vector2.right : Vector2.left;

        var b = _col.bounds;
        Vector2 size = new Vector2(b.size.x * 0.95f, b.size.y * 0.8f);   // 살짝 줄인 크기
        Vector2 center = b.center + new Vector3(dir.x * 0.01f, 0f, 0f);    // 미세 오프셋
        var hit = Physics2D.BoxCast(center, size, 0f, dir, frontProbeDistance, platformMask);

        if (hit.collider != null)
        {
            _dir *= -1;                               // 반전
            _nextWallFlipAllowed = now + wallFlipCooldown;
            s.Horizontal = _dir;                      // 즉시 반영
        }
    }
}
