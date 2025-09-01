using System.Collections;
using Unity.XR.OpenVR;
using UnityEngine;

public class Skill : CharacterModule
{

    public float coolTime;

    
    protected float curCoolTime = 0;

    bool skillActivates = true;
    
    public bool CheckSkillActivates() => skillActivates;

    public void ActivateSkill()
    {
        this.curCoolTime = 0;
        this.skillActivates = true;
    }
    public void UsedSkill()
    {
        SetSkillCoolTime(coolTime);
        DeactivateSkill();
        StartCoroutine(WaitSkillCoolTime());
    }
    public void SetSkillCoolTime(float coolTime)
    {
        this.curCoolTime = coolTime;
    }
    public void DeactivateSkill()
    {
        if (!this.skillActivates)
            return;
        this.skillActivates = false;
    }
    public void CoolingDownSkill()
    {
        if (this.curCoolTime <= 0)
            return;

        this.curCoolTime -= Time.deltaTime;
    }
    IEnumerator WaitSkillCoolTime()
    {
        while(curCoolTime > 0f)
        {
            CoolingDownSkill();

            yield return null;
        }

        ActivateSkill();
    }

    public void DirectShot()
    {

    }
    public override void ProcessModule()
    {
        base.ProcessModule();
    }
}
