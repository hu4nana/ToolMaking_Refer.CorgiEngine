using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatements
{
    public enum ConditionStatement
    {
        Normal,
        Silence,
        Root,
        Frozen,
        Airborne
    }
    public enum ActionStatement
    {
        Idle,
        Move,
        Jump,
        NormalAttack,
        Casting1,
        Casting2,
        Casting3,
        Casting4,
        Death
    }
}

public class StateMachine<T>
{
    public GameObject Target;

    public virtual T CurrentState { get; private set; }
    public virtual T PreviousState { get; private set; }
    public void ChangeState(T newState)
    {
        if(EqualityComparer<T>.Default.Equals(newState, CurrentState))
        {
            return;
        }

        PreviousState = CurrentState;
        CurrentState = newState;
    }
}

public static class EnumUtil<T> where T : struct, Enum
{
    public static readonly T[] Values = (T[])Enum.GetValues(typeof(T));
    public static readonly int Count = Values.Length;
    public static T FromIndex(int i) => Values[i];
    public static int ToIndex(T e) => Array.IndexOf(Values, e);
}