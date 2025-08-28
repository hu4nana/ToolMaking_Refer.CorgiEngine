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
        HandleHorizontalMovement();
        HandleInput();
    }

    public override void HandleInput()
    {

        inputX = _character.Input_Horizontal;
        if (!_character.isGrounded)
            return;

        Move();

        if (inputX != 0)
        {
            ChangeState();
            ChangeDirection();
        }
    }

    protected virtual void HandleHorizontalMovement()
    {
        if (!ModuleAuthorized
            || (_character._curConditionState != CharacterStatements.ConditionStatement.Normal))
        {
            return;
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
