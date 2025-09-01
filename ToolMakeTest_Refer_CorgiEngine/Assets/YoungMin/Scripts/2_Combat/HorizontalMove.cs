using UnityEngine;

public class HorizontalMove : CharacterModule
{
    public float moveSpeed = 10f;

    float inputX;

    public override void Initialization()
    {
        base.Initialization();
    }

    public override void ProcessModule()
    {
        HandleInput();
    }

    public override void HandleInput()
    {
        if (!ModuleAuthorized) return;

        // CC: Root/Frozen이면 이동 금지
        var cc = _character.ConditionState.CurrentState;
        if (cc == CharacterStatements.ConditionStatement.Root ||
            cc == CharacterStatements.ConditionStatement.Frozen)
        {
            _rigid.linearVelocity = Vector2.up * _rigid.linearVelocityY; // 수평 0
            return;
        }

        inputX = _character.Input_Horizontal;
        if (_character.isGrounded)
        {
            Move();

            if (inputX != 0)
            {
                ChangeState();
                ChangeDirection();
            }
        }
    }

    public virtual void Move()
    {
        _rigid.linearVelocity = Vector2.right* inputX * moveSpeed + Vector2.up * _rigid.linearVelocityY;
    }

    public virtual void ChangeState()
    {
        _character.ActionState.ChangeState(CharacterStatements.ActionStatement.Move);
    }

    public virtual void ChangeDirection()
    {
        if (inputX > 0)
            _character.isFacingRight = true;
        else if (inputX < 0)
            _character.isFacingRight = false;
    }
}
