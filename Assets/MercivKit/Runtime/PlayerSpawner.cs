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

        [SerializeField]
        private CameraController _camera;
        public CameraController Camera
        {
            get => _camera;
            set => _camera = value;
        }

        public void PlayerJoined(PlayerRef player)
        {
            if (player == Runner.LocalPlayer)
            {
                var playerNetworkObject = Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
                var playerController = playerNetworkObject.GetComponent<PlayerController>();
                Camera.target = playerController.transform;
            }
        }
    }
}