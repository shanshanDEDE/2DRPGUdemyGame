using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//這個類別代表所有技能的繼承父類別,這邊預設所有技能都有冷卻時間,即使可能有技能不需要冷卻,但到時我們可以把他設定冷卻為0
public class Skill : MonoBehaviour
{
    [SerializeField] protected float cooldown;
    protected float cooldownTimer;

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {

        if (cooldownTimer < 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }

        Debug.Log("Can't use skill");
        return false;
    }

    public virtual void UseSkill()
    {

    }

}
