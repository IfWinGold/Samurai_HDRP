using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NSamurai
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance = null;

        [SerializeField] private InGameMarkUI ingameMarkUI; 
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);
        }
        public void SetActiveLockOnMark(Transform target)
        {
            if (target == null)
                ingameMarkUI.SetLockOnMark(null);
            else
                ingameMarkUI.SetLockOnMark(target);
        }
        public void SetActiveExecuteMark(SamuraiEnemy target)
        {
            ingameMarkUI.SetExecuteMark(target);
        }
        public void DestroyExecuteMark(SamuraiEnemy target)
        {
            ingameMarkUI.DestroyExecuteMark(target);
        }
    }

}
