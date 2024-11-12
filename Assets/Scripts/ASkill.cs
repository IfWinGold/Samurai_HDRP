using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Skill_Data",menuName = "Scriptable Object/Skill Data",order = int.MaxValue)]
public class ASkill : ScriptableObject
{
    [SerializeField] private string skillName;
    [SerializeField] private int damege;
    [SerializeField] private KeyCode keycode;
    [SerializeField] private AnimationClip[] animClips;

 

    


    public void Init()
    {

    }
    
}
