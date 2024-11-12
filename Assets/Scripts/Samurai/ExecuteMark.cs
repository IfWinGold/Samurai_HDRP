using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
public class ExecuteMark : MonoBehaviour
{
    private Transform stunTarget;
    private RectTransform rectTransform;
    private Vector3 screenPoint = Vector3.zero;

    [SerializeField] private SamuraiEnemy samuraiEnemy;
    public void SetTarget(Transform target)
    {
        samuraiEnemy = target.GetComponent<SamuraiEnemy>();
        stunTarget = target;
        screenPoint = Camera.main.WorldToScreenPoint(target.position);        
        rectTransform.anchoredPosition = screenPoint;
    }
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Update()
    {
        screenPoint = Camera.main.WorldToScreenPoint(stunTarget.position);
        rectTransform.anchoredPosition = screenPoint;
        if(samuraiEnemy)
        {
            if (samuraiEnemy.State == EState.State.Executed)
                Destroy(this.gameObject);
        }
    }
    private void OnDestroy()
    {
        Addressables.ReleaseInstance(this.gameObject);
    }

}
