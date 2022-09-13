using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Othello.Scripts {
    public class CoinObjectPool : MonoBehaviour {
        [SerializeField] private Coin coinPrefab;
        private List<Coin> _freeCoins;

        private void Awake() {
            _freeCoins = new List<Coin>();
        }
        
        public Coin GetNewCoin() {
            if (_freeCoins.Count > 0) {
                Coin coin = _freeCoins[0];
                _freeCoins.RemoveAt(0);
                coin.gameObject.SetActive(true);
                return coin;
            };
            Coin freeCoin = Instantiate(coinPrefab, gameObject.transform);
            return freeCoin;
        }

        public void DisableCoin(Coin coin) {
            coin.gameObject.SetActive(false);
            _freeCoins.Add(coin);
        }
    }
}