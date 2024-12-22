using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Minefactory.World
{
    public class SkillTreeManager : MonoBehaviour
    {
        public static SkillTreeManager Instance { get; private set; }

        public event Action<float> OnOxygenSkillPurchased;
        public event Action<float> OnMovementSpeedPurchased;
        public event Action<float> OnMiningRatePurchased;

        [Header("UI Elements")]
        public GameObject skillTreePanel;
        public Button[] skillButtons;
        public Text populationText; // UI text displaying the player's population

        [Header("Game Variables")]
        public int[] skillCosts;          // Costs for each skill, in order
        private List<List<int>> requiredSkills;  // Array of arrays for prerequisites

        private bool[] skillsPurchased;

        private void Awake()
        {
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

            // Initialize the requiredSkills list
            requiredSkills = new List<List<int>>
        {
            new List<int>(),          // Skill 1A (no prerequisites)
            new List<int>(),          // Skill 1B (no prerequisites)
            new List<int> { 0, 1 },   // Skill 2A (requires Skill 1A or 1B)
            new List<int> { 0, 1 },   // Skill 2B (requires Skill 1A or 1B)
            new List<int> { 2, 3 },   // Skill 3A (requires Skill 2A or 2B)
            new List<int> { 2, 3 }    // Skill 3B (requires Skill 2A or 2B)
        };

            skillsPurchased = new bool[requiredSkills.Count]; // Initialize purchase state for all skills
            GameStateManager.Instance.OnPopulationChanged += UpdateUI;
            UpdateUI(); // Update the skill tree to reflect the initial state
        }



        public void PurchaseSkill(int skillIndex)
        {
            if (skillsPurchased[skillIndex]) return; // Check if the skill is already purchased
            if (GameStateManager.Instance.Population < skillCosts[skillIndex]) return; // Check if there is enough population

            // Check if any skill in the same row has already been purchased
            int rowStartIndex = (skillIndex / 2) * 2; // Calculate the start index of the row
            int rowEndIndex = rowStartIndex + 1;      // End index is start + 1 (2 skills per row)
            for (int i = rowStartIndex; i <= rowEndIndex; i++)
            {
                if (skillsPurchased[i])
                {
                    return; // Block the purchase
                }
            }

            // Check if the skill has prerequisites
            if (requiredSkills[skillIndex] == null || requiredSkills[skillIndex].Count == 0)
            {
                // No prerequisites, allow purchase
            }
            else
            {
                // Check if at least one required skill has been purchased
                bool prerequisiteMet = false;

                foreach (int requiredSkill in requiredSkills[skillIndex])
                {
                    if (skillsPurchased[requiredSkill])
                    {
                        prerequisiteMet = true;  // At least one prerequisite is met
                        break;
                    }
                }

                if (!prerequisiteMet)
                {
                    return;
                }
            }

            // Purchase the skill
            //GameStateManager.Instance.Population -= skillCosts[skillIndex];
            skillsPurchased[skillIndex] = true;

            // Disable the other skill in the same row
            DisableOtherSkill(skillIndex);

            // Apply the skill effect
            ApplySkillEffect(skillIndex);

            // Update the UI
            UpdateUI();
        }





        private void DisableOtherSkill(int purchasedSkillIndex)
        {
            int rowStartIndex = (purchasedSkillIndex / 2) * 2; // Start of the row
            int rowEndIndex = rowStartIndex + 1;               // End of the row

            for (int i = rowStartIndex; i <= rowEndIndex; i++)
            {
                if (i != purchasedSkillIndex && i < skillButtons.Length)
                {
                    skillButtons[i].interactable = false; // Make the button unclickable
                    skillButtons[i].GetComponent<Image>().color = Color.gray; // Gray out the button
                }
            }
        }



        private bool IsSkillUnlocked(int skillIndex)
        {
            // If no prerequisites, the skill is unlocked by default
            if (requiredSkills[skillIndex] == null || requiredSkills[skillIndex].Count == 0)
            {
                return true;
            }

            // Check if any prerequisite skill is purchased
            foreach (int prerequisite in requiredSkills[skillIndex])
            {
                if (skillsPurchased[prerequisite])
                {
                    return true;
                }
            }
            return false;  // None of the prerequisites are purchased
        }

        private bool IsRowDisabled(int skillIndex)
        {
            int rowStartIndex = (skillIndex / 2) * 2; // Start of the row
            int rowEndIndex = rowStartIndex + 1;      // End of the row

            // Check if any skill in the row is purchased
            for (int i = rowStartIndex; i <= rowEndIndex; i++)
            {
                if (skillsPurchased[i])
                {
                    return true; // Row is disabled if any skill in it is purchased
                }
            }
            return false; // Row is not disabled
        }


        private void UpdateUI()
        {

            for (int i = 0; i < skillButtons.Length; i++)
            {
                // If the skill is purchased
                if (skillsPurchased[i])
                {
                    skillButtons[i].interactable = false;
                    skillButtons[i].GetComponent<Image>().color = Color.green;  // Purchased skill
                }
                // If the skill is in a row where another skill has been purchased
                else if (IsRowDisabled(i))
                {
                    skillButtons[i].interactable = false;
                    skillButtons[i].GetComponent<Image>().color = Color.gray;  // Disabled skill
                }
                // If the skill is locked (prerequisites or population)
                else if (GameStateManager.Instance.Population < skillCosts[i] || !IsSkillUnlocked(i))
                {
                    skillButtons[i].interactable = false;
                    skillButtons[i].GetComponent<Image>().color = Color.gray;  // Locked skill
                }
                // If the skill is available
                else
                {
                    skillButtons[i].interactable = true;
                    skillButtons[i].GetComponent<Image>().color = Color.white;  // Available skill
                }
            }
        }





        private void ApplySkillEffect(int skillIndex)
        {
            // Call specific methods for each skill
            switch (skillIndex)
            {
                case 0:  // Skill 1A
                    float newOxygenDuration = 10f;
                    OnOxygenSkillPurchased?.Invoke(newOxygenDuration);
                    break;
                case 1:  // Skill 1B
                    float newFlashlightRadius = 10f;
                    GameStateManager.Instance.SetSharedState("Flashlight", newFlashlightRadius);
                    break;
                case 2:  // Skill 2A
                    float newMovementSpeed = 7f;
                    GameStateManager.Instance.SetSharedState("Speed", newMovementSpeed);
                    OnMovementSpeedPurchased?.Invoke(newMovementSpeed);
                    break;
                case 3:  // Skill 2B
                    float newJumpForce = 10f;
                    GameStateManager.Instance.SetSharedState("JumpForce", newJumpForce);
                    break;
                case 4:  // Skill 3A
                    float newMiningRate = 2f;
                    OnMiningRatePurchased?.Invoke(newMiningRate);
                    GameStateManager.Instance.SetSharedState("Mining", newMiningRate);
                    Debug.Log("invoke mining rate from skill tree");
                    break;
                case 5:  // Skill 3B
                         // Example for another skill
                    break;
                default:
                    Debug.LogWarning("Skill effect not implemented!");
                    break;
            }
        }
    }
}