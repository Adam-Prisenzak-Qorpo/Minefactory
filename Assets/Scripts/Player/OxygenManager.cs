using System.Collections;
using UnityEngine;
using UnityEngine.UI;  // For displaying the HUD
using Minefactory.World;

namespace Minefactory.Player
{
    public class OxygenManager : MonoBehaviour
    {
        public static OxygenManager Instance;  // Singleton instance

        [Header("Oxygen Settings")]
        public int totalOxygenSegments = 10;  // Total oxygen

        public float timePerSegment = 5; //Time per oxygen segment in seconds
        private int currentOxygen;  // Current oxygen level in minutes

        [Header("UI Settings")]
        public GameObject oxygenBarContainer;  // Parent container with Grid Layout
        public GameObject oxygenSegmentPrefab;  // Prefab representing one oxygen segment
        private Image[] oxygenSegments;  // Array for storing segment Image components

        private bool isDepleting = false;
        private bool isInOxygenZone = false;  // Flag to check if player is in the zone
        private Coroutine depletionCoroutine;
        private void Awake()
        {
            // Ensure only one instance exists
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Debug.Log("Starting OxygenManager");
            currentOxygen = totalOxygenSegments;
            InitializeOxygenBar();
            depletionCoroutine = StartCoroutine(OxygenDepletionRoutine());
        }

        public void SetOxygenZoneState(bool state)
        {
            isInOxygenZone = state;

            if (isInOxygenZone)
            {
                // Stop oxygen depletion when in zone
                if (depletionCoroutine != null)
                {
                    StopCoroutine(depletionCoroutine);
                    depletionCoroutine = null;
                    isDepleting = false;
                }
            }
            else
            {
                // Restart depletion when exiting zone
                if (depletionCoroutine == null && !isDepleting)
                {
                    depletionCoroutine = StartCoroutine(OxygenDepletionRoutine());
                    isDepleting = true;
                }
            }
        }

        public int CurrentOxygen
        {
            get { return currentOxygen; }
            set
            {
                currentOxygen = value;
                UpdateOxygenBar();
            }
        }

        private void InitializeOxygenBar()
        {
            oxygenSegments = new Image[totalOxygenSegments];

            for (int i = 0; i < totalOxygenSegments; i++)
            {
                // Instantiate a new segment and set its parent to the container
                GameObject segment = Instantiate(oxygenSegmentPrefab, oxygenBarContainer.transform);
                Image segmentImage = segment.GetComponent<Image>();

                if (segmentImage != null)
                {
                    oxygenSegments[i] = segmentImage;
                }
            }
        }

        private IEnumerator OxygenDepletionRoutine()
        {
            isDepleting = true;

            while (CurrentOxygen > 0 && !isInOxygenZone)
            {
                yield return new WaitForSeconds(timePerSegment);
                if (!isInOxygenZone) // Double check we're still supposed to deplete
                {
                    currentOxygen--;
                    UpdateOxygenBar();

                    if (CurrentOxygen <= 0)
                    {
                        PlayerDeath();
                    }
                }
            }

            isDepleting = false;
            depletionCoroutine = null;
        }

        private void UpdateOxygenBar()
        {
            for (int i = 0; i < oxygenSegments.Length; i++)
            {
                if (i < currentOxygen)
                {
                    oxygenSegments[i].enabled = true;  // Show segment
                }
                else
                {
                    oxygenSegments[i].enabled = false; // Hide segment
                }
            }
        }

        private void PlayerDeath()
        {
            Debug.Log("Player has died due to lack of oxygen.");
            // Trigger game over or player death logic here
        }

        // Template method for future oxygen replenishment
        public void ReplenishOxygen(int amount)
        {
            currentOxygen = Mathf.Min(currentOxygen + amount, totalOxygenSegments);
            UpdateOxygenBar();
        }

        private void OnEnable()
        {
            Debug.Log("Starting OnEnable OxygenManager");
            StartCoroutine(WaitForSkillTreeManager());
        }

        private void OnDisable()
        {
            SkillTreeManager.Instance.OnOxygenSkillPurchased -= ApplyOxygenUpgrade;
        }

        private IEnumerator WaitForSkillTreeManager()
        {
            while (SkillTreeManager.Instance == null)
            {
                yield return null; // Wait until the next frame
            }
            SkillTreeManager.Instance.OnOxygenSkillPurchased += ApplyOxygenUpgrade;
        }

        private void ApplyOxygenUpgrade(float extraOxygen)
        {
            timePerSegment += extraOxygen;
            Debug.Log($"Additional oxygen added: {extraOxygen}. New time per segment: {timePerSegment}");
        }


    }
}