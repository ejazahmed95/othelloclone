using EAUnity.Core;
using EAUnity.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace EAUnity.Event {
    public class GameEventListener : MonoBehaviour {
        [Tooltip("Event to register with.")] 
        public GameEvent gameEvent;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent<IGameEventData> response;

        private void OnEnable() {
            gameEvent.RegisterListener(this);
        }

        private void OnDisable() {
            gameEvent.UnregisterListener(this);
        }
        
        public void OnEventRaised(IGameEventData data = null) {
            Log.Trace($"GameObject {gameObject.name.Italic()} handling event {gameEvent.name.Italic()}", "EventListener");
            response.Invoke(data);
        }
    }
}