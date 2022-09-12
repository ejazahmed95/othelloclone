using UnityEngine;

namespace EAUnity.Grid {
    public abstract class CellBehavior<T> : MonoBehaviour where T : CellInfo {
        protected T Info = null;
        
        public virtual CellBehavior<T> Init(T info) {
            Info = info;
            Info.AddListener(OnCellInfoChange);
            return this;
        }

        protected void OnEnable() {
            Info?.AddListener(OnCellInfoChange);
        }

        protected void OnDisable() {
            Info?.RemoveListener(OnCellInfoChange);
        }

        private void OnCellInfoChange() {
            UpdateCellInfo(Info);
        }

        protected virtual void UpdateCellInfo(T info) { }
    }
}