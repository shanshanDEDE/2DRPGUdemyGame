using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//這個類別放在SkillerManager的那個物件底下 當要使用技能的物件可以去透過單利模式來取得SkillerManager的那個物件底下的CreateClone
//並且透過CreateClone這個方法產生這個物件後再去取得這個物件底下的SetupClone方法 並將從當要使用技能的物件的位置也透過方法傳過去來設定
public class Clone_Skill : Skill
{
    [Header("Clone_Skill")]
    [SerializeField] private GameObject clonePrefab;

    //將參數放在Clone_Skill這裡,而不是寫在Clone_Skill_Controller,可以方便之後要設定技能可以都在在SkillerManager的那個物件底下設定
    [SerializeField] private float CloneDuration;

    [Space]
    [SerializeField] private bool canAttack;


    public void CreateClone(Transform _clonePosition)
    {
        GameObject newClone = Instantiate(clonePrefab);

        //調用clone_Skill_Controller方法並在那個方法中設定clonePrefab的位置
        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition, CloneDuration, canAttack);
    }
}
