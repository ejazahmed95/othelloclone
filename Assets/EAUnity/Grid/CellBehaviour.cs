using UnityEngine;

namespace EAUnity.Grid {
    public abstract class CellBehavior<T> : MonoBehaviour where T : CellInfo {
        private T _info = null;
        
        public virtual CellBehavior<T> Init(T info) {
            _info = info;
            _info.AddListener(OnCellInfoChange);
            return this;
        }

        protected void OnEnable() {
            _info?.AddListener(OnCellInfoChange);
        }

        protected void OnDisable() {
            _info?.RemoveListener(OnCellInfoChange);
        }

        private void OnCellInfoChange() {
            UpdateCellInfo(_info);
        }

        protected virtual void UpdateCellInfo(T info) { }
    }
}