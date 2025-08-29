using UnityEngine;

[DisallowMultipleComponent]
public class PatternBot : MonoBehaviour, IInputDriver
{
    [Header("Targeting")]
    public LayerMask enemyLayers;
    public float detectionRadius = 60f;  // ���� ũ�� ��� �Ÿ� ����

    [Header("Pattern Durations")]
    public Vector2 moveBeforeAttack = new Vector2(2f, 5f); // 2~5�� �̵� �� ��Ÿ
    public Vector2 moveBeforeSkill = new Vector2(1f, 4f); // ��Ÿ �� 1~4�� �̵� �� ��ų

    [Header("Skill Casting")]
    public float castTime = 0.35f;       // ��ų ĳ���� ���� ����
    public bool faceTargetOnFire = true; // �߻�/���� ���� Ÿ�� �������� ȸ��

    [Header("Collision (Platform)")]
    public LayerMask platformMask;       // Platform ���̾� ����
    public float frontProbeDistance = 0.15f; // ���� Ž�� �Ÿ�(ª��)
    public float wallFlipCooldown = 0.25f;   // ���� �� ƨ�� ���� �ð�

    private Character _ch;
    private BoxCollider2D _col;
    private int _dir = 1;                // -1: ����, 1: ������
    private float _phaseUntil;
    private float _nextWallFlipAllowed;

    private enum Phase { MoveA, Attack, MoveB, Skill, Casting }
    private Phase _phase = Phase.MoveA;

    void Awake()
    {
        _ch = GetComponent<Character>();
        _col = GetComponent<BoxCollider2D>();
        if (platformMask.value == 0) platformMask = LayerMask.GetMask("Platform"); // ���ǻ� �⺻��
        _dir = (_ch && _ch.isFacingRight) ? 1 : -1;
        _phaseUntil = Time.time + Random.Range(moveBeforeAttack.x, moveBeforeAttack.y);
    }

    public InputSnapshot CollectInput(Character self)
    {
        var s = InputSnapshot.Empty;
        float now = Time.time;

        // �Ÿ� ������� "���ʿ��� ���� ��"�� ���� �� ������ ����
        var tgt = TargetingUtil2D.AcquireFrontmost(self.transform, self.isFacingRight, detectionRadius, enemyLayers);
        if (tgt != null)
            self.target = tgt.gameObject;  // ��Ÿ/��ų �����ӿ� ���������� ���

        switch (_phase)
        {
            // ��ų ĳ���� ������ ������
            case Phase.Casting:
                s.Horizontal = 0f;
                if (now >= _phaseUntil) StartMoveA(now);
                break;

            // 2~5�� �̵�
            case Phase.MoveA:
                s.Horizontal = _dir;
                HandleWallFlip(now, ref s);
                if (now >= _phaseUntil)
                    _phase = Phase.Attack;
                break;

            // ��Ÿ 1ȸ (�Ÿ� ����)
            case Phase.Attack:
                if (faceTargetOnFire && self.target)
                    self.isFacingRight = self.target.transform.position.x >= self.transform.position.x;

                s.NormalAttack = true; // �� �����Ӹ� true
                StartMoveB(now);
                break;

            // 1~4�� �̵�
            case Phase.MoveB:
                s.Horizontal = _dir;
                HandleWallFlip(now, ref s);
                if (now >= _phaseUntil)
                    _phase = Phase.Skill;
                break;

            // ��ų 1ȸ (�Ÿ� ����)
            case Phase.Skill:
                if (faceTargetOnFire && self.target)
                    self.isFacingRight = self.target.transform.position.x >= self.transform.position.x;

                s.Skill3 = true; // �� �����Ӹ� true

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
        // ������ ����. ���� ��ŸƮ�� ���ϸ� ���� �� ���:
        // _dir = Random.value < 0.5f ? -1 : 1;
    }

    void StartMoveB(float now)
    {
        _phase = Phase.MoveB;
        _phaseUntil = now + Random.Range(moveBeforeSkill.x, moveBeforeSkill.y);
    }

    // ���� Platform �� üũ �� �ε����� ����
    void HandleWallFlip(float now, ref InputSnapshot s)
    {
        if (_col == null) return;
        if (now < _nextWallFlipAllowed) return;

        Vector2 dir = (_dir > 0) ? Vector2.right : Vector2.left;

        var b = _col.bounds;
        Vector2 size = new Vector2(b.size.x * 0.95f, b.size.y * 0.8f);   // ��¦ ���� ũ��
        Vector2 center = b.center + new Vector3(dir.x * 0.01f, 0f, 0f);    // �̼� ������
        var hit = Physics2D.BoxCast(center, size, 0f, dir, frontProbeDistance, platformMask);

        if (hit.collider != null)
        {
            _dir *= -1;                               // ����
            _nextWallFlipAllowed = now + wallFlipCooldown;
            s.Horizontal = _dir;                      // ��� �ݿ�
        }
    }
}
