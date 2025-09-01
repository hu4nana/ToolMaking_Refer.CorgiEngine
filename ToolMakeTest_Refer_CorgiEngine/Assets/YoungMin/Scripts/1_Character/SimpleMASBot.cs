using UnityEngine;

[DisallowMultipleComponent]
public class SimpleMASBot : MonoBehaviour, IInputDriver
{
    [Header("Detection / Targeting")]
    public LayerMask enemyLayers;
    public float detectionRadius = 12f;

    [Header("Range")]
    public float preferredRange = 6f;  // �� �Ÿ� ���̸� ���߰� ����/��ų
    public float stopRange = 5.2f;     // �̵� ���ߴ� ���

    [Header("Attack / Skill")]
    public float attackCooldown = 0.7f; // ��Ÿ ��
    public float skillCooldown = 6f;   // ��ų1 ��
    public float castTime = 0.35f; // ��ų �����ð�(���� �̵� �Է� 0)

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

        // ĳ���� ��
        if (_phase == Phase.Casting)
        {
            s.Horizontal = 0f;
            if (now >= _castUntil) _phase = Phase.Free;
            return s;
        }

        // Ÿ�� ���� (�ٶ󺸴� ���� ���� "���� ���� ��")
        if (_tgt == null || !_tgt.gameObject.activeInHierarchy)
            _tgt = TargetingUtil2D.AcquireFrontmost(self.transform, self.isFacingRight, detectionRadius, enemyLayers);

        // ������ ����
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

        // Ÿ���� ������ �Ÿ��� ���� �̵�/����/��ų
        float dx = _tgt.position.x - self.transform.position.x;
        float adx = Mathf.Abs(dx);

        // �þ� ����
        if (!Mathf.Approximately(dx, 0f))
            self.isFacingRight = dx > 0f;

        // ����/����
        if (adx > stopRange)
            s.Horizontal = Mathf.Sign(dx);
        else
            s.Horizontal = 0f;

        // CC ����Ʈ: ���¿� ���� �Է� ���� (��: Root/Frozen)
        var cc = self.ConditionState.CurrentState;
        bool canUseSkills = cc != CharacterStatements.ConditionStatement.Silence
                           && cc != CharacterStatements.ConditionStatement.Root
                           && cc != CharacterStatements.ConditionStatement.Frozen;
        bool canNormalAtk = cc != CharacterStatements.ConditionStatement.Root
                           && cc != CharacterStatements.ConditionStatement.Frozen;
        bool canMove = cc != CharacterStatements.ConditionStatement.Root
                           && cc != CharacterStatements.ConditionStatement.Frozen;

        if (!canMove) s.Horizontal = 0f;

        // ��Ÿ� ���̸� �ൿ
        if (adx <= preferredRange)
        {
            // 1) ��Ÿ ����
            if (canNormalAtk && now >= _nextAtk)
            {
                self.target = _tgt.gameObject;  // ��Ÿ �����ӿ� Ÿ�� ����
                s.NormalAttack = true;
                _nextAtk = now + attackCooldown;
                return s; // ��Ÿ �������� �̹� �������� ��
            }

            // 2) ��ų1
            if (canUseSkills && now >= _nextSkill)
            {
                self.target = _tgt.gameObject;  // ���� ���� ��ǥ ��������
                s.Skill1 = true;
                _nextSkill = now + skillCooldown;

                // ĳ���� �ð�
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
