using UnityEngine;

public class UIManager: MonoBehaviour {

    public GameObject furnaceUI;
    public GameObject crafterUI;

    public GameObject inventoryUI;

    public GameObject skillTreeUI;


    public void OpenFurnaceUI()
    {
        CloseAllUI();
        furnaceUI.SetActive(true);
    }

    public void OpenCrafterUI()
    {
        CloseAllUI();
        crafterUI.SetActive(true);
    }

    public void OpenInventoryUI()
    {
        CloseAllUI();
        inventoryUI.SetActive(true);
    }

    public void OpenSkillTreeUI(){
        CloseAllUI();
        skillTreeUI.SetActive(true);
    }



    public void CloseAllUI()
    {
        inventoryUI.SetActive(false);
        furnaceUI.SetActive(false);
        crafterUI.SetActive(false);
        skillTreeUI.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           CloseAllUI();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenInventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            OpenSkillTreeUI();
        }
    }

}

