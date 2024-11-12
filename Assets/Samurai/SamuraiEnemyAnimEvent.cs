using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiEnemyAnimEvent : MonoBehaviour
{
    [SerializeField] private SamuraiEnemy samuraiEnemy;

    public void OnParryTime()
    {
        samuraiEnemy.ParryTime();
    }
    public void OnAttack()
    {
        samuraiEnemy.Attack();
    }
    public void OnEndAnimation()
    {
        samuraiEnemy.OnEndAniamtion();
    }
}
