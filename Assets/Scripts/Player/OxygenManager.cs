using System.Collections;
using UnityEngine;
using UnityEngine.UI;  // For displaying the HUD

namespace Minefactory.Player
{
    public class OxygenManager : MonoBehaviour
    {
        [Header("Oxygen Settings")]
        public int totalOxygenMinutes = 10;  // Total oxygen in minutes
        private int currentOxygen;  // Current oxygen level in minutes
        
        [Header("UI Settings")]
        public GameObject oxygenBarContainer;  // Parent container with Grid Layout
        public GameObject oxygenSegmentPrefab;  // Prefab representing one oxygen segment
        private Image[] oxygenSegments;  // Array for storing segment Image components
        
        private void Start()
        {
            currentOxygen = totalOxygenMinutes;
            InitializeOxygenBar();
            StartCoroutine(OxygenDepletionRoutine());
        }

        private void InitializeOxygenBar()
        {
            oxygenSegments = new Image[totalOxygenMinutes];

            for (int i = 0; i < totalOxygenMinutes; i++)
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
            while (currentOxygen > 0)
            {
                yield return new WaitForSeconds(5);  
                currentOxygen--;
                UpdateOxygenBar();

                if (currentOxygen <= 0)
                {
                    PlayerDeath();
                }
            }
        }

        private void UpdateOxygenBar()
        {
            // Update the visual state of the oxygen bar
            for (int i = 0; i < oxygenSegments.Length; i++)
            {
                // Set active or inactive based on current oxygen level
                oxygenSegments[i].color = i < currentOxygen ? Color.white : new Color(1, 1, 1, 0.3f);  // Adjust color to show depletion
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
            currentOxygen = Mathf.Min(currentOxygen + amount, totalOxygenMinutes);
            UpdateOxygenBar();
        }
    }
}
