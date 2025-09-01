using UnityEngine;

public class CharacterModule : MonoBehaviour
{
    protected Character _character;
    protected HorizontalMove _horizontalMove;
    protected Health _health;
    protected Skill _skill;

    protected Rigidbody2D _rigid;

    public CharacterStatements.ActionStatement[] BlockingActionsStates;
    public CharacterStatements.ConditionStatement[] BlockingConditionStates;

    public virtual void Initialization()
    {
        _character = this.gameObject.GetComponentInParent<Character>();
        _horizontalMove = this.GetComponentInParent<HorizontalMove>();
        _rigid=this.GetComponentInParent<Rigidbody2D>();
        _health= this.GetComponentInParent<Health>();
        _skill = this.GetComponentInParent<Skill>();
        
    }
    public virtual void HandleInput() { }

    protected virtual void EarlyProcessModule() { }
    public virtual void ProcessModule() { }
    public virtual void LateProcessModule() { }

    public virtual bool ModuleAuthorized
    {
        get
        { 
            if (_character != null)
            {
                if ((BlockingActionsStates != null) && (BlockingActionsStates.Length > 0))
                {
                    for (int i = 0; i < BlockingActionsStates.Length; i++)
                    {
                        if (BlockingActionsStates[i] == (_character.ActionState.CurrentState))
                        {
                            return false;
                        }
                    }
                }

                if ((BlockingConditionStates != null) && (BlockingConditionStates.Length > 0))
                {
                    for (int i = 0; i < BlockingConditionStates.Length; i++)
                    {
                        if (BlockingConditionStates[i] == _character.ConditionState.CurrentState)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
