using UnityEngine;
using Minefactory.Common;
using Minefactory.Save;
using Minefactory.World;
using Minefactory.Factories.Mining;

namespace Minefactory.Game
{
    public class WorldManager : MonoBehaviour
    {
        [SerializeField] private GameObject topWorld;
        [SerializeField] private GameObject undergroundWorld;
        [SerializeField] private GameObject canvas;
        [SerializeField] private UIManager uIManager;


        // Static reference to the instance for global access
        private static WorldManager instance;
        public static WorldManager Instance
        {
            get
            {
                if (instance == null)
                    Debug.Log("WorldManager instance is null!");
                return instance;
            }
        }

        public static BaseWorldGeneration activeBaseWorld
        {
            get
            {
                if (instance == null)
                    return null;
                return instance.GetActiveWorld().GetComponent<BaseWorldGeneration>();
            }
        }

        private SaveManager saveManager;
        private bool isInputLocked = false;
        private float inputCooldown = 0.5f; // Prevents rapid switching
        private float lastSwitchTime;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (topWorld == null || undergroundWorld == null)
            {
                Debug.LogError("World references not set in WorldManager!");
                return;
            }

            saveManager = SaveManager.Instance;
            var miningManager = MiningProductionManager.Instance;
            saveManager.LoadGame(topWorld, undergroundWorld);
            lastSwitchTime = Time.time;
        }

        public GameObject GetActiveWorld()
        {
            if (topWorld == null || undergroundWorld == null)
                return null;
            if (topWorld.activeSelf)
                return topWorld;
            else
                return undergroundWorld;
        }

        public UIManager GetUIManager()
        {
            return uIManager;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N) && !isInputLocked && Time.time - lastSwitchTime >= inputCooldown)
            {
                SwitchWorlds();
            }
        }

        private void SwitchWorlds()
        {
            if (topWorld == null || undergroundWorld == null)
                return;

            isInputLocked = true;

            bool isTopWorld = topWorld.activeSelf;
            topWorld.SetActive(!isTopWorld);
            undergroundWorld.SetActive(isTopWorld);

            var canvasComp = canvas.GetComponent<Canvas>();
            if (canvasComp != null)
            {
                canvasComp.worldCamera = GetActiveWorld().GetComponentInChildren<Camera>();
            }
            

            lastSwitchTime = Time.time;

            isInputLocked = false;

            Debug.Log($"Switched to {(isTopWorld ? "underground" : "top")} world");
        }
    }
}