using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

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
    public string path = "/storage/emulated/0/Download/ARmatica";

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

        if (!Directory.Exists(path))
        {
            Debug.LogError($"Folder not found: ${path}");
            return;
        }

        string[] fbxFiles = Directory.GetFiles(path, "*.fbx");

        foreach(string _path in fbxFiles)
        {
            string name = Path.GetFileName(_path);
            GameObject buttonObj = Instantiate(buttonTemplate, buttonParent);
            buttonObj.SetActive(true);
            buttonObj.GetComponentInChildren<TMP_Text>().text = name;

            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                schematicLoader.selectedSchematicName = name;
                toggleMenu();
                menuButton.SetActive(false);
                instructionText.SetActive(true);
                instructionText.GetComponent<TMP_Text>().text = "Scanning...";
                Debug.Log("Selected schematic: " + name);
            });
        }
    }
}
