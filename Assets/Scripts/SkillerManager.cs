using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//透過這個腳本管理所有的技能,此腳本放的物件(SkillManager)上會有所有遊戲中的技能腳本
public class SkillerManager : MonoBehaviour
{
    public static SkillerManager instance;

    public Dash_Skill dash { get; private set; }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        dash = GetComponent<Dash_Skill>();
    }
}
