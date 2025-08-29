using UnityEngine;

public class PlayerSkill3 : CharacterModule
{
    public GameObject CarrotRain;
    public float CooldownTime = 2f;  // ��ų ��Ÿ��
    private float _cooldownTimer = 0f;
    private bool _isCasting = false; // ��ų ��� �� ����

    public float GetCoolDownTimer()
    {
        return _cooldownTimer;
    }
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

        if (_cooldownTimer==0f && _character.Input_Skill3) // ����: ��ų3 �Է�
        {
            UseSkill();
        }
    }

    protected virtual void UseSkill()
    {
        Debug.Log("Skill3 Used!");

        // ���� ��ȯ �� �ٸ� ��� ����
        _character.ActionState.ChangeState(CharacterStatements.ActionStatement.Skill1);
        _rigid.linearVelocity = Vector2.zero;
        _character.isFacingRight = true;
        // ��ų �ߵ�
        _isCasting = true;

        // ��Ÿ�� ����
        _cooldownTimer = CooldownTime;

        // ��: ����ü ���� / �ִϸ��̼� ����
        Instantiate(CarrotRain, Vector2.right * _character.target.transform.position.x+Vector2.up*8.5f
            , Quaternion.identity);

        // ���� �ð� �� Idle�� ���ͽ�Ű�� ������ �ڷ�ƾ ���
        _character.StartCoroutine(BackToIdleAfter(1f));
    }

    private System.Collections.IEnumerator BackToIdleAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _character.ActionState.ChangeState(CharacterStatements.ActionStatement.Idle);
    }
}
