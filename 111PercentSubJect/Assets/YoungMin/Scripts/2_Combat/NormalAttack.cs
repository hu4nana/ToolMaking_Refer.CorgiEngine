using UnityEngine;

public class NormalAttack : CharacterModule
{
    public GameObject Projectile;

    bool _inputNormalAttack;
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
    public void ChaneStateIdle()
    {
        _character.ActionState.ChangeState(CharacterStatements.ActionStatement.Idle);
    }
    public void InstantiateProjecile()
    {
        GameObject _normalProj=Instantiate(Projectile,this.transform.position,Quaternion.identity);
        _normalProj.GetComponent<Projectile>().LaunchAtTarget(_character.target.transform.position);
    }
}