using Fusion;
using UnityEngine;

public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;

    public void PlayerJoined(PlayerRef player)
    {
        if (!Runner.IsServer)
            return;

        SpawnPlayer(player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (!Runner.IsServer)
            return;

        DespawnPlayer(player);
    }

    private void SpawnPlayer(PlayerRef playerRef)
    {
        if (!Runner.IsServer)
            return;

        if (Runner.TryGetPlayerObject(playerRef, out NetworkObject existingPlayer))
        {
            return;
        }

        int index = playerRef.PlayerId % spawnPoints.Length;
        Vector3 spawnPoint = spawnPoints[index].position;

        NetworkObject playerObject = Runner.Spawn(playerNetworkPrefab, spawnPoint, Quaternion.identity, playerRef);

        Runner.SetPlayerObject(playerRef, playerObject);
    }

    private void DespawnPlayer(PlayerRef playerRef)
    {
        if (Runner.TryGetPlayerObject(playerRef, out NetworkObject playerNetworkObject))
        {
            Runner.Despawn(playerNetworkObject);
        }

        Runner.SetPlayerObject(playerRef, null);
    }
}
