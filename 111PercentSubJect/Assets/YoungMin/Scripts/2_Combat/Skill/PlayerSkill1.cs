using UnityEngine;

public class PlayerSkill1 : CharacterModule
{
    public GameObject Laser;
    public float CooldownTime = 2f;  // ��ų ��Ÿ��
    private float _cooldownTimer = 0f;
    private bool _isCasting = false; // ��ų ��� �� ����

    public override void ProcessModule()
    {
        // ��Ÿ�� ����
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0f)
            {
                _cooldownTimer = 0f;
                // ��Ÿ�� �� �� �ٽ� ��� ����
                _isCasting = false;
            }
        }

        HandleInput();
    }

    public override void HandleInput()
    {
        if (!ModuleAuthorized) return;
        if (_isCasting) return; // ��ų ���� �߿� ����
        if (_cooldownTimer > 0f) return; // ��Ÿ�� �߿� ����

        if (_cooldownTimer==0f && _character.Input_Skill1) // ����: ��ų1 �Է�
        {
            UseSkill();
        }
    }

    protected virtual void UseSkill()
    {
        Debug.Log("Skill Used!");

        // ���� ��ȯ �� �ٸ� ��� ����
        _character.ActionState.ChangeState(CharacterStatements.ActionStatement.Skill1);
        _rigid.linearVelocity = Vector2.zero;
        _character.isFacingRight = true;
        // ��ų �ߵ�
        _isCasting = true;

        // ��Ÿ�� ����
        _cooldownTimer = CooldownTime;

        // ��: ����ü ���� / �ִϸ��̼� ����
        Instantiate(Laser, transform.position, Quaternion.identity);

        // ���� �ð� �� Idle�� ���ͽ�Ű�� ������ �ڷ�ƾ ���
        _character.StartCoroutine(BackToIdleAfter(5f));
    }

    private System.Collections.IEnumerator BackToIdleAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _character.ActionState.ChangeState(CharacterStatements.ActionStatement.Idle);
    }
}
