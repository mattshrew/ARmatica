using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SchematicUIManager : MonoBehaviour
{
    public GameObject menuButton;
    public GameObject exitButton;
    public GameObject schematicMenu; 
    public GameObject buttonTemplate;
    public Transform buttonParent;
    public TrackedImageSchematicLoader schematicLoader;
    public GameObject instructionText;
    public bool menuOpen = false;

    void Start()
    {
        void toggleMenu()
        {
            menuButton.SetActive(menuOpen);
            schematicMenu.SetActive(!menuOpen);
            exitButton.SetActive(!menuOpen);
            menuOpen = !menuOpen;
            Debug.Log("Menu toggled.");
        }

        menuButton.GetComponent<Button>().onClick.AddListener(toggleMenu);
        exitButton.GetComponent<Button>().onClick.AddListener(toggleMenu);

        GameObject[] prefabs = Resources.LoadAll<GameObject>("Schematics");

        foreach (GameObject prefab in prefabs)
        {
            GameObject buttonObj = Instantiate(buttonTemplate, buttonParent);
            buttonObj.SetActive(true);
            buttonObj.GetComponentInChildren<TMP_Text>().text = prefab.name;

            string prefabName = prefab.name;
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                schematicLoader.selectedSchematicName = prefabName;
                toggleMenu();
                menuButton.SetActive(false);
                instructionText.SetActive(true);
                instructionText.GetComponent<TMP_Text>().text = "Scanning...";
                Debug.Log("Selected schematic: " + prefabName);
            });
        }
    }
}
