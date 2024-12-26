using UnityEngine;
using Cinemachine;
using Newtonsoft.Json.Linq;
using ThunderWire.Attributes;
using System;
using Unity.Mathematics;

namespace UHFPS.Runtime
{
    [InspectorHeader("Player Manager", space = false)]
    public class PlayerManager : MonoBehaviour, ISaveableCustom
    {
        [Header("Player References")]
        public Transform CameraHolder;
        public Camera MainCamera;
        public CinemachineVirtualCamera MainVirtualCamera;
        public bool IsInSwamp => SwampBlockManager.IsInSwamp;
        public SwampBlockManager SwampBlockManager;
        public string[] ObjectivesFromStart;
        public ItemProperty[] ItemsFromStart;
        [HideInInspector] public float TimeInSwamp;

        [SerializeField] private Transform RayHelpPoint;
        [SerializeField] private Transform RayStartPoint;
        [SerializeField] private Transform RayHit;
        [SerializeField] private float RayDistance;
        [SerializeField] private float MaxViewDistance;
        [SerializeField] private float CloseDistance;
        [SerializeField] private Layer Layer;
        [SerializeField] private bool UseFirstStartItem = true;

        private static PlayerManager _instance;

        public static PlayerManager Instance
        {
            get
            {
                if (_instance == null ) _instance = GameObject.Find("HEROPLAYER").GetComponent<PlayerManager>();
                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            foreach (string key in ObjectivesFromStart)
            {
                ObjectiveManager.Instance.AddObjective(key.Split(";")[0], new string[] { key.Split(";")[1] });
            }

            InventoryItem newItem;
            foreach (ItemProperty item in ItemsFromStart)
            {
                
                Inventory.Instance.AddItem(item.GUID, 1, null, out newItem);
                if (UseFirstStartItem)
                {
                    Inventory.Instance.UseItem(newItem);
                    UseFirstStartItem = false;
                }
            }
            //inventory.AddItem(interactable.PickupItem.GUID, interactable.Quantity, interactable.ItemCustomData, out var addedItem)
        }

        private PlayerHealth m_PlayerHealth;
        public PlayerHealth PlayerHealth
        {
            get
            {
                if (m_PlayerHealth == null)
                    m_PlayerHealth = GetComponent<PlayerHealth>();

                return m_PlayerHealth;
            }
        }

        private PlayerItemsManager m_playerItems;
        public PlayerItemsManager PlayerItems
        {
            get
            {
                if (m_playerItems == null)
                    m_playerItems = GetComponentInChildren<PlayerItemsManager>();

                return m_playerItems;
            }
        }

        private MotionController m_motionController;
        public MotionController MotionController
        {
            get
            {
                if (m_motionController == null)
                    m_motionController = GetComponentInChildren<MotionController>();

                return m_motionController;
            }
        }

        private InteractController m_interactController;
        public InteractController InteractController
        {
            get
            {
                if (m_interactController == null)
                    m_interactController = GetComponentInChildren<InteractController>();

                return m_interactController;
            }
        }

        /// <summary>
        /// This function is used to collect all local player data to be saved.
        /// </summary>
        public StorableCollection OnCustomSave()
        {
            StorableCollection data = new StorableCollection();
            data.Add("health", PlayerHealth.EntityHealth);

            StorableCollection playerItemsData = new StorableCollection();
            for (int i = 0; i < PlayerItems.PlayerItems.Count; i++)
            {
                var playerItem = PlayerItems.PlayerItems[i];
                var itemData = (playerItem as ISaveableCustom).OnCustomSave();
                playerItemsData.Add("playerItem_" + i, itemData);
            }

            data.Add("playerItems", playerItemsData);
            return data;
        }

        /// <summary>
        /// This function is used to load all stored local player data.
        /// </summary>
        public void OnCustomLoad(JToken data)
        {
            PlayerHealth.StartHealth = data["health"].ToObject<uint>();
            PlayerHealth.InitHealth();

            for (int i = 0; i < PlayerItems.PlayerItems.Count; i++)
            {
                var playerItem = PlayerItems.PlayerItems[i];
                var itemData = data["playerItems"]["playerItem_" + i];
                (playerItem as ISaveableCustom).OnCustomLoad(itemData);
            }
        }

        public bool CheckObjectInViewField(GameObject obj)
        {
            Vector3 viewPos = GetObjectPositionInViewField(obj);
            return viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0;
        }

        public Vector3 GetObjectPositionInViewField(GameObject obj)
        {
            return MainCamera.WorldToViewportPoint(obj.transform.position);
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        public float CalculateDistanceToObj(Transform obj)
        {
            Vector2 playerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 helpPointPosition = new Vector2(obj.position.x, obj.position.z);
            return Vector2.Distance(playerPosition, helpPointPosition);
        }

        public Vector2 CalculateDelta(float distance, Transform obj)
        {
            Vector3 delta = transform.position - obj.position;
            return (new Vector2(delta.x, delta.z)) / CalculateDistanceToObj(obj) * distance;
        }

        public Vector3 CalculateBackPoint(float distance, float yPos)
        {
            Vector2 targetDelta = CalculateDelta(distance, RayHelpPoint);
            return new Vector3(transform.position.x - targetDelta.x, yPos, transform.position.z - targetDelta.y);
        }

        public Vector3 CalculateRayPoint(Vector3 startPosition, Vector3 direction, float distance = 100)
        {
            RaycastHit[] hits = Physics.RaycastAll(startPosition, direction, distance);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer == Layer)
                {
                    RayHit.position = hit.point;
                    return hit.point;
                }
            }
            return Vector3.zero;
        }

        public Vector3 CalculateRayStartPoint()
        {
            Vector3 startPosition = CalculateBackPoint(RayDistance, RayStartPoint.position.y);
            return CalculateRayPoint(startPosition, Vector3.down);
        }

        public Vector3 CalculateRandomPointInArea(float minDistance, float maxDistance)
        {
            float distance = UnityEngine.Random.Range(minDistance, maxDistance);
            float angle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
            Vector2 planePosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            return CalculateRayPoint(new Vector3(transform.position.x + planePosition.x, RayStartPoint.position.y, transform.position.z + planePosition.y), Vector3.down);
        }

        public Vector3 CalculateRandomPointInSafeArea()
        {
            return CalculateRandomPointInArea(CloseDistance, MaxViewDistance);
        }

        public bool IsPointInSafeZone(Transform obj)
        {
            float distance = CalculateDistanceToObj(obj);
            return distance >= CloseDistance && distance <= MaxViewDistance;
        }

        private void Update()
        {
            if (IsInSwamp) TimeInSwamp += Time.deltaTime;       
        }
    }
}