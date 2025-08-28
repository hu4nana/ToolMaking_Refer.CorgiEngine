using UnityEngine;

[CreateAssetMenu(fileName = "SkillStats", menuName = "Scriptable Objects/SkillStats")]
public class SkillStats : ScriptableObject
{
    public int damage;
    public float cool;


    public Sprite Icon;
}
