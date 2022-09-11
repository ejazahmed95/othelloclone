using System;
using UnityEngine;

namespace Othello.Scripts.EAUnity.Core {
    [RequireComponent(typeof(Camera))]
    public class FitCameraToObject : MonoBehaviour {
        [SerializeField] private SpriteRenderer spriteToFit;
        [SerializeField] private Vector2 border;

        private Camera _cam;

        private void Awake() {
            _cam = GetComponent<Camera>();
            
            var bounds = spriteToFit.bounds;
            var requiredSize = border + new Vector2(bounds.size.x, bounds.size.y);

            UpdateCamSize(requiredSize);
        }
        
        private void UpdateCamSize(Vector2 requiredSize) {
            var height = requiredSize.y;
            if (height * _cam.aspect < requiredSize.x) {
                height = requiredSize.x / _cam.aspect;
            }
            _cam.orthographicSize = height/2;
        }
    }
}