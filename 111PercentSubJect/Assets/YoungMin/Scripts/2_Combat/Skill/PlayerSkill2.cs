using UnityEngine;

public class PlayerSkill2 : CharacterModule
{
    public GameObject Chest;
    public float CooldownTime = 2f;  // 스킬 쿨타임
    private float _cooldownTimer = 0f;
    private bool _isCasting = false; // 스킬 사용 중 여부


    public float GetCoolDownTimer()
    {
        return _cooldownTimer;
    }


    public override void ProcessModule()
    {
        // 쿨타임 갱신
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0f)
            {
                _cooldownTimer = 0f;
                // 쿨타임 끝 → 다시 사용 가능
                _isCasting = false;
            }
        }

        HandleInput();
    }

    public override void HandleInput()
    {
        if (!ModuleAuthorized) return;
        if (_isCasting) return; // 스킬 시전 중엔 무시
        if (_cooldownTimer > 0f) return; // 쿨타임 중엔 무시

        if (_cooldownTimer==0f && _character.Input_Skill2) // 예시: 스킬2 입력
        {
            UseSkill();
        }
    }

    protected virtual void UseSkill()
    {
        Debug.Log("Skill2 Used!");

        // 상태 전환 → 다른 모듈 차단
        _character.ActionState.ChangeState(CharacterStatements.ActionStatement.Skill1);
        _rigid.linearVelocity = Vector2.zero;
        _character.isFacingRight = true;
        // 스킬 발동
        _isCasting = true;

        // 쿨타임 시작
        _cooldownTimer = CooldownTime;

        // 예: 투사체 생성 / 애니메이션 실행
        Instantiate(Chest, Vector2.right * _character.target.transform.position.x, Quaternion.identity);

        // 일정 시간 뒤 Idle로 복귀시키고 싶으면 코루틴 사용
        _character.StartCoroutine(BackToIdleAfter(1f));
    }

    private System.Collections.IEnumerator BackToIdleAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _character.ActionState.ChangeState(CharacterStatements.ActionStatement.Idle);
    }
}
