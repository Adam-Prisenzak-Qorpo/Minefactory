using System.Collections;
using UnityEngine;
using UnityEngine.UI;  // For displaying the HUD

namespace Minefactory.Player
{
    public class OxygenManager : MonoBehaviour
    {
        [Header("Oxygen Settings")]
        public int totalOxygenSegments = 10;  // Total oxygen

        public float timePerSegment = 5; //Time per oxygen segment in seconds
        private int currentOxygen;  // Current oxygen level in minutes

        [Header("UI Settings")]
        public GameObject oxygenBarContainer;  // Parent container with Grid Layout
        public GameObject oxygenSegmentPrefab;  // Prefab representing one oxygen segment
        private Image[] oxygenSegments;  // Array for storing segment Image components


        private void Start()
        {
            currentOxygen = totalOxygenSegments;
            InitializeOxygenBar();
            StartCoroutine(OxygenDepletionRoutine());
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
            while (CurrentOxygen > 0)
            {
                yield return new WaitForSeconds(timePerSegment);  
                currentOxygen--;
                UpdateOxygenBar();

                if (CurrentOxygen <= 0)
                {
                    PlayerDeath();
                }
            }
        }

        private void UpdateOxygenBar()
        {
            // Destroy excess segments if current oxygen is less than the number of active segments
            for (int i = oxygenBarContainer.transform.childCount - 1; i >= currentOxygen; i--)
            {
                Destroy(oxygenBarContainer.transform.GetChild(i).gameObject);
            }

            // Add new segments if current oxygen is greater than the number of active segments
            for (int i = oxygenBarContainer.transform.childCount; i < currentOxygen; i++)
            {
                Instantiate(oxygenSegmentPrefab, oxygenBarContainer.transform);
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
        
        public void IncreaseOxygenSegmentDuration(float additionalSeconds)
        {
            timePerSegment += additionalSeconds;
            Debug.Log("Oxygen segment duration increased to " + timePerSegment + " seconds!");
        }
    }
}
