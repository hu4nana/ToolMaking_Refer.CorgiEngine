using UnityEngine;

public class NormalAttack : CharacterModule
{
    public GameObject Projectile;

    bool _inputNormalAttack;

    public override void ProcessModule()
    {
        HandleInput();
    }

    public override void HandleInput()
    {
        if (!ModuleAuthorized) return;

        // CC ����Ʈ: Root/Frozen�� ��Ÿ �Ұ� (�䱸���׿� �°� ����)
        var cc = _character.ConditionState.CurrentState;
        if (cc == CharacterStatements.ConditionStatement.Root ||
            cc == CharacterStatements.ConditionStatement.Frozen)
            return;

        if (_character.isGrounded)
        {
            _inputNormalAttack = _character.Input_NormalAttack;
            if (_inputNormalAttack)
            {
                _rigid.linearVelocity = Vector2.zero;
                _character.ActionState.ChangeState(CharacterStatements.ActionStatement.NormalAttack);
            }
        }
    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ��
    public void InstantiateProjecile()
    {
        var spawnPos = transform.position;
        var targetPos = _character.target
            ? _character.target.transform.position
            : spawnPos + new Vector3(_character.isFacingRight ? 8f : -8f, 0f, 0f); // Ÿ�� ���� �� ����

        GameObject proj = Instantiate(Projectile, spawnPos, Quaternion.identity);
        proj.GetComponent<Projectile>().LaunchAtTarget(targetPos);
    }

    public void ChaneStateIdle()
    {
        _character.ActionState.ChangeState(CharacterStatements.ActionStatement.Idle);
    }
}