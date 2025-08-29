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

        // CC 게이트: Root/Frozen은 평타 불가 (요구사항에 맞게 조정)
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

    // 애니메이션 이벤트에서 호출
    public void InstantiateProjecile()
    {
        var spawnPos = transform.position;
        var targetPos = _character.target
            ? _character.target.transform.position
            : spawnPos + new Vector3(_character.isFacingRight ? 8f : -8f, 0f, 0f); // 타겟 없을 때 폴백

        GameObject proj = Instantiate(Projectile, spawnPos, Quaternion.identity);
        proj.GetComponent<Projectile>().LaunchAtTarget(targetPos);
    }

    public void ChaneStateIdle()
    {
        _character.ActionState.ChangeState(CharacterStatements.ActionStatement.Idle);
    }
}