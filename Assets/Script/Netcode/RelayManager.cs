using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    public NetworkManager networkManager;
    public GameObject playerPrefab; // Player Prefab с CarController и PlayerInput
    public Transform spawnPoint;    // точка спавна

    // --- Создание Host ---
    public async Task<string> CreateRelayAndSpawn(int maxPlayers)
    {
        var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        var transport = networkManager.GetComponent<UnityTransport>();
        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );

        networkManager.StartHost();
        Debug.Log("Host started");

        // Спавн игрока на сервере
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);

        return joinCode;
    }

    // --- Подключение клиента ---
    public async Task JoinRelayAndSpawn(string joinCode)
    {
        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        var transport = networkManager.GetComponent<UnityTransport>();
        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData
        );

        networkManager.StartClient();
        Debug.Log("Client started");
    }

    // --- Server RPC для спавна игрока ---
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong clientId)
    {
        Vector3 spawnPos = spawnPoint ? spawnPoint.position : Vector3.zero;
        GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        // Spawn как PlayerObject для конкретного клиента
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

        Debug.Log($"Spawned player for ClientId {clientId}");
    }
}