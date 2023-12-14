using Fusion;
using UnityEngine;

namespace MercivKit
{
    public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
    {
        [SerializeField]
        private GameObject _playerPrefab;
        public GameObject PlayerPrefab
        {
            get => _playerPrefab;
            set => _playerPrefab = value;
        }

        void Awake()
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        public void PlayerJoined(PlayerRef player)
        {
            if (player == Runner.LocalPlayer)
            {
                Runner.Spawn(PlayerPrefab, transform.position, transform.rotation);
            }
        }
    }
}