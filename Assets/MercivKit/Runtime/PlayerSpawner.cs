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

        public void PlayerJoined(PlayerRef player)
        {
            if (player == Runner.LocalPlayer)
            {
                Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
            }
        }
    }
}