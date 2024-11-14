using UnityEngine;
using Cinemachine;
using Newtonsoft.Json.Linq;
using ThunderWire.Attributes;
using System;

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


            foreach (ItemProperty item in ItemsFromStart)
            {
                Inventory.Instance.AddItem(item.GUID, 1, null);
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

        private void Update()
        {
            if (IsInSwamp) TimeInSwamp += Time.deltaTime;
        }
    }
}