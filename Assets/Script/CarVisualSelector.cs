// using UnityEngine;
// using UnityEngine.UI;

// public class CarVisualSelector : MonoBehaviour
// {
//     [SerializeField] private Button _NextButton;
//     [SerializeField] private Button _PrevButton;

//     [SerializeField] private SOData[] _CarData;

//     [SerializeField] private PreviewSpawner _PreviewSpawner;

//     [SerializeField] private bool _AutoRotate = true;
//     [SerializeField] private float _RotateSpeed = 35f;

//     private GameObject _shellRoot;          // root заспавненной пустышки
//     private GameObject _currentVisual;
//     private int _currentIndex = 0;

//     private int Count => _CarData?.Length ?? 0;

//     private void Awake()
//     {
//         if (Count == 0)
//         {
//             Debug.LogWarning("_CarData пустой", this);
//             return;
//         }

//         if (_PreviewSpawner == null)
//         {
//             Debug.LogError("_PreviewSpawner не привязан", this);
//             return;
//         }

//         _shellRoot = _PreviewSpawner.GetSpawnedShellRoot();
//         if (_shellRoot == null)
//         {
//             Debug.LogError("Не удалось получить root пустышки", this);
//             return;
//         }

//         _NextButton?.onClick.AddListener(Next);
//         _PrevButton?.onClick.AddListener(Prev);

//         ShowVisual(_currentIndex);
//     }



//     private void Update()
//     {
//         if (!_AutoRotate || _shellRoot == null) return;
//         _shellRoot.transform.Rotate(0, _RotateSpeed * Time.deltaTime, 0, Space.Self);
//     }

//     private void Next()
// {
//     Debug.Log("Кнопка NEXT нажата!");
//     Change(1);
// }
//     private void Prev() => Change(-1);

//     private void Change(int delta)
// {
//     Debug.Log($"Change вызван с delta = {delta}, текущий индекс был {_currentIndex}");
//     _currentIndex = (_currentIndex + delta + Count) % Count;
//     Debug.Log($"Новый индекс: {_currentIndex}");
//     ShowVisual(_currentIndex);
// }

// private void ShowVisual(int index)
// {
//     Debug.Log($"ShowVisual вызван для индекса {index}");
    
//     if (_currentVisual != null)
//     {
//         Debug.Log("Удаляем старый визуал");
//         Destroy(_currentVisual);
//         _currentVisual = null;
//     }

//     if (index < 0 || index >= Count)
//     {
//         Debug.LogWarning("Индекс вне диапазона");
//         return;
//     }

//     var data = _CarData[index];
//     if (data == null)
//     {
//         Debug.LogWarning($"_CarData[{index}] = null");
//         return;
//     }

//     if (data.VisualPrefab == null)
//     {
//         Debug.LogWarning($"VisualPrefab null у {data.name}");
//         return;
//     }

//     Debug.Log($"Спавним визуал {data.name}");
//     _currentVisual = Instantiate(data.VisualPrefab, _shellRoot.transform);
//     _currentVisual.transform.localPosition = Vector3.zero;
//     _currentVisual.transform.localRotation = Quaternion.identity;
//     _currentVisual.transform.localScale = Vector3.one;
// }

//     private void OnDestroy()
//     {
//         _NextButton?.onClick.RemoveAllListeners();
//         _PrevButton?.onClick.RemoveAllListeners();
//     }
// }