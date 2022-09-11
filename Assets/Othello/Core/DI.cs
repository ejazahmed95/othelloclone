using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

namespace RangerRPG.Core {
    /// <summary>
    /// Create and register objects on this for easy access later
    /// </summary>
    public static class DI {
        private static Dictionary<Type, Object> instances = new Dictionary<Type, object>();

        static DI(){ 
            Log.Info("Initializing the dependency injector", "DI");
        }
        
        public static void Register<T>(T component){
            Log.Info($"Registering component type {typeof(T)}", "DI");
            instances[component.GetType()] = component;
        }
        
        public static void DeRegister<T>(T component) {
            instances.Remove(component.GetType());
        }
        
        /// <summary>
        /// Get the registered object from type
        /// </summary>
        /// <typeparam name="T"> Required Object Type </typeparam>
        /// <returns></returns>
        public static T Get<T>(){
            //PrintDi();
            T i;
            try{
                i = instances.Select(component => component.Value).OfType<T>().FirstOrDefault();
            }
            catch (Exception e){
                Debug.LogError( $"[DI] exception in getting the type of {typeof(T).FullName}; Exception {e.Message}");
                return default;
            }
            Log.Debug($"successfully got the type {typeof(T).FullName}");
            return i;
        }

        private static void PrintDi(){
            foreach (var (key, value) in instances.Select(x => (x.Key, x.Value))){
                Debug.Log($"[DI] Type={key}, Value={value}");
            }
        }
    }
}