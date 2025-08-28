using UnityEngine;
public class Health : CharacterModule, IDamageable
{
    public float maxHP = 100;

    public float curHP;
    bool dead;

    public override void Initialization()
    {
        this.curHP = maxHP;
    }
    void DIe()
    {
        if (dead) return;
        dead = true;
    }

    public void ApplyDamage(float damage)
    {
        if (curHP <= 0f) return;
        curHP -= damage;
        if(curHP <= 0f)
        {
            curHP = 0f;
            DIe();
        }
    }
}
