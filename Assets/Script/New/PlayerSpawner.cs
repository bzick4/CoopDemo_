// using UnityEngine;
// using Unity.Netcode;

// public class PlayerSpawner : MonoBehaviour
// {
//     [Header("Список персонажей (SOData)")]
//     [SerializeField] private SOData[] Characters; // Массив твоих SOData с VisualPrefab внутри

//     [Header("Точка спавна")]
//     [SerializeField] private Transform SpawnPoint;

//     // Синглтон для удобного доступа
//     public static PlayerSpawner Instance;

//     private void Awake()
//     {
//         Instance = this;

//         // Подписываемся на подключение клиента
//         NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
//     }

//     private void OnDestroy()
//     {
//         if (NetworkManager.Singleton != null)
//             NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
//     }

//     private void OnClientConnected(ulong clientId)
//     {
//         // Только сервер спавнит игроков
//         if (!NetworkManager.Singleton.IsServer) return;

//         // Получаем индекс выбранного персонажа
//         // Для хоста (clientId = 0) читаем из PlayerPrefs
//         int selectedIndex = PlayerPrefs.GetInt($"SelectedCharacter_{clientId}", 0);

//         SpawnPlayer(clientId, selectedIndex);
//     }

//     public void SpawnPlayer(ulong clientId, int index)
//     {
//         if (index < 0 || index >= Characters.Length)
//         {
//             Debug.LogError("Неверный индекс персонажа: " + index);
//             return;
//         }

//         SOData selectedData = Characters[index];
//         GameObject prefab = selectedData.VisualPrefab;

//         if (prefab == null)
//         {
//             Debug.LogError("VisualPrefab не назначен в SOData: " + selectedData.name);
//             return;
//         }

//         // Спавним объект
//         GameObject playerObj = Instantiate(prefab, SpawnPoint.position, Quaternion.identity);

//         // Добавляем NetworkObject, если его нет
//         NetworkObject netObj = playerObj.GetComponent<NetworkObject>();
//         if (!netObj) netObj = playerObj.AddComponent<NetworkObject>();

//         // Спавним как объект игрока
//         netObj.SpawnAsPlayerObject(clientId, true);

//         // Инициализация контроллера и SOData
//         CharController ctrl = playerObj.GetComponent<CharController>();
//         if (ctrl != null)
//         {
//             ctrl.InitStats(selectedData);
//         }

//         Debug.Log($"Спавн игрока {clientId} с {selectedData.name}");
//     }
// }