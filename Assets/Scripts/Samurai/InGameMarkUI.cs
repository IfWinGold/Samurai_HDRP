using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.VisualScripting;


namespace NSamurai
{
    public class InGameMarkUI : MonoBehaviour
    {
        [SerializeField] private RawImage lockOnMark;
        [SerializeField] private Transform canvasPivot;
        private Vector3 lockOnScreenPoint = Vector3.zero;        
        private Transform lockOnTarget;


        private const string ADDR_EXECUTE_MARK_UI = "Samurai_ExecuteMark";

        public void SetLockOnMark(Transform target)
        {
            if (target == null)
            {
                lockOnMark.gameObject.SetActive(false);
                return;
            }


            lockOnTarget = target;
            lockOnScreenPoint = Camera.main.WorldToScreenPoint(lockOnTarget.position);
            lockOnMark.rectTransform.anchoredPosition = lockOnScreenPoint;
            lockOnMark.gameObject.SetActive(true);
        }
        public void SetExecuteMark(SamuraiEnemy target)
        {
            if (target.executeMark != null) return;

            AsyncOperationHandle handle =  Addressables.InstantiateAsync(ADDR_EXECUTE_MARK_UI,canvasPivot);
            handle.Completed += (h) => 
            {
                GameObject obj = handle.Result as GameObject;
                ExecuteMark mark = obj.GetComponent<ExecuteMark>();
                mark.SetTarget(target.transform);


                target.executeMark = mark;
            };
        }
        public void DestroyExecuteMark(SamuraiEnemy samuraiEnemy)
        {
            if(samuraiEnemy.executeMark != null)
            {
                Destroy(samuraiEnemy.executeMark.gameObject);
                samuraiEnemy.executeMark = null;
            }
        }

        private void Update()
        {
            if(lockOnMark.gameObject.activeSelf)
            {
                
                lockOnScreenPoint = Camera.main.WorldToScreenPoint(lockOnTarget.position);
                lockOnMark.rectTransform.anchoredPosition = lockOnScreenPoint;
            }
        }
    }

}

