using System;
using System.Collections;
using EAUnity.Core;
using UnityEngine;

namespace Game {
    public class Coin : MonoBehaviour {
        [SerializeField] private SpriteRenderer coinSprite;

        private PlayerInfo _owner;
        private float _flipSpeed = 3f;

        public int Id => _owner.id;
        
        public Coin Init(PlayerInfo pInfo) {
            _owner = pInfo;
            _owner.score++;
            coinSprite.sprite = _owner.coinSprite;
            // coinSprite.color = _owner.color;
            return this;
        }

        public void FlipCoin(PlayerInfo pInfo) {
            if (_owner == pInfo) {
                return;
            }
            _owner.score--;
            _owner = pInfo;
            _owner.score++;
            StartCoroutine(FlippingCoin());
        }

        private IEnumerator FlippingCoin() {
            float elapsedTime = 0;
            var newScale = 1.0;
            while (newScale > 0) {
                yield return null;
                elapsedTime += Time.deltaTime * _flipSpeed;
                newScale = Mathf.Lerp(1, 0, elapsedTime*elapsedTime);
                gameObject.transform.localScale = new Vector3((float)newScale, 1, 1);
            }
            newScale = 0f;
            coinSprite.sprite = _owner.coinSprite;
            // coinSprite.color = _owner.color;
            elapsedTime = 0;
            while (newScale<1) {
                yield return null;
                elapsedTime += Time.deltaTime * _flipSpeed;
                newScale = Mathf.Lerp(0, 1, elapsedTime*elapsedTime);
                gameObject.transform.localScale = new Vector3((float)newScale, 1, 1);
            }
        }

        private void OnDisable() {
            _owner.score--;
        }

    }
}