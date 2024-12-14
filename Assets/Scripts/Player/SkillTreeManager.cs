using System.Collections;
using System.Collections.Generic;
using Minefactory.Player;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillTreeManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject skillTreePanel;
    public Button[] skillButtons;
    public Text populationText; // UI text displaying the player's population

    [Header("Game Variables")]
    public int population = 100;
    public int[] skillCosts;          // Costs for each skill, in order
    private List<List<int>> requiredSkills;  // Array of arrays for prerequisites

    private bool[] skillsPurchased;

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
        UpdateUI(); // Update the skill tree to reflect the initial state
    }
    
    private void Update()
    {
        // Toggle the skill tree panel with the P key
        if (Input.GetKeyDown(KeyCode.P))
        {
            skillTreePanel.SetActive(!skillTreePanel.activeSelf);
        }
    }

    public void PurchaseSkill(int skillIndex)
    {
        if (skillsPurchased[skillIndex]) return; // Check if the skill is already purchased
        if (population < skillCosts[skillIndex]) return; // Check if there is enough population

        // Check if any skill in the same row has already been purchased
        int rowStartIndex = (skillIndex / 2) * 2; // Calculate the start index of the row
        int rowEndIndex = rowStartIndex + 1;      // End index is start + 1 (2 skills per row)
        for (int i = rowStartIndex; i <= rowEndIndex; i++)
        {
            if (skillsPurchased[i])
            {
                Debug.Log($"You can only purchase one skill per row. Skill {i} is already purchased.");
                return; // Block the purchase
            }
        }

        // Check if the skill has prerequisites
        if (requiredSkills[skillIndex] == null || requiredSkills[skillIndex].Count == 0)
        {
            // No prerequisites, allow purchase
            Debug.Log($"Skill {skillIndex} has no prerequisites and can be purchased.");
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
                Debug.Log("You need to purchase a prerequisite skill first!");
                return;
            }
        }

        // Purchase the skill
        population -= skillCosts[skillIndex];
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
            Debug.Log($"Skill {skillIndex} has no prerequisites and is unlocked.");
            return true;
        }

        // Check if any prerequisite skill is purchased
        foreach (int prerequisite in requiredSkills[skillIndex])
        {
            if (skillsPurchased[prerequisite])
            {
                Debug.Log($"Skill {skillIndex} is unlocked because prerequisite Skill {prerequisite} is purchased.");
                return true;
            }
        }
        Debug.Log($"Skill {skillIndex} is locked. No prerequisites are purchased.");
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
        populationText.text = "Population: " + population;

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
            else if (population < skillCosts[i] || !IsSkillUnlocked(i))
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
                FindObjectOfType<OxygenManager>().IncreaseOxygenSegmentDuration(10);
                break;
            case 1:  // Skill 1B
                FindObjectOfType<PlayerController>().IncreaseJumpHeight(1.5f);
                // Add effect for Skill 1B if necessary
                break;
            case 2:  // Skill 2A
                // Example for another skill
                break;
            case 3:  // Skill 2B
                break;
            case 4:  // Skill 3A
                // Example for another skill
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
