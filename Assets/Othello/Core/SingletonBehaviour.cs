using System;
using UnityEngine;

namespace RangerRPG.Core {
    /// <summary>
    /// Only once instance of this can be present at any given point
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBehaviour<T> : MonoBehaviour where T: SingletonBehaviour<T> {

        public static T Instance => DI.Get<T>();
        
        public virtual void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                Log.Err($"Singleton type {typeof(T)} already exists");
            } else {
                Log.Info($"Singleton type {typeof(T)} is created");
                DI.Register(this);
            }
        }

        public virtual void OnDestroy() {
            DI.DeRegister(this);
        }
    }
}