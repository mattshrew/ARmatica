using System.Collections;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class SchematicUIManager : MonoBehaviour
{
    public static SchematicUIManager Instance;
    public GameObject menuButton;
    public GameObject exitButton;
    public GameObject schematicMenu;
    public GameObject buttonTemplate;
    public Transform buttonParent;
    public TrackedImageSchematicLoader schematicLoader;
    public GameObject instructionText;
    public GameObject loadingText;
    public string fileNom;

    public bool menuOpen = false;
    public bool start = false;

    private string path;

    public string GetFileName()
    {
        return fileNom;
    }
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        // Hide all UI except loadingText
        loadingText.SetActive(true);
        menuButton.SetActive(false);
        exitButton.SetActive(false);
        schematicMenu.SetActive(false);
        instructionText.SetActive(false);

        StartCoroutine(WaitForStart());
    }

    IEnumerator WaitForStart()
    {
        // Wait until start flag is set true
        yield return new WaitUntil(() => start);

        loadingText.SetActive(false);

        // Proceed with UI setup
        path = Path.Combine(UnityEngine.Application.persistentDataPath, "ARmatica");

        menuButton.GetComponent<Button>().onClick.AddListener(ToggleMenu);
        exitButton.GetComponent<Button>().onClick.AddListener(ToggleMenu);

        if (!Directory.Exists(path))
        {
            UnityEngine.Debug.LogError($"Folder not found: {path}");
            yield break;
        }

        string[] fbxFiles = Directory.GetFiles(path, "*.fbx");
        UnityEngine.Debug.Log(fbxFiles.Length);

        foreach (string filePath in fbxFiles)
        {
            UnityEngine.Debug.Log(filePath);
            string fileName = Path.GetFileName(filePath);
            GameObject buttonObj = Instantiate(buttonTemplate, buttonParent);
            buttonObj.SetActive(true);

            TMP_Text label = buttonObj.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = fileName;

            string capturedName = fileName;
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                fileNom = capturedName;
                ToggleMenu();
                menuButton.SetActive(false);
                instructionText.SetActive(true);
                instructionText.GetComponent<TMP_Text>().text = "Scanning...";
                UnityEngine.Debug.Log("Selected schematic: " + capturedName);
            });
        }

        menuButton.SetActive(true); // finally show the menu button
    }


    private void ToggleMenu()
    {
        menuButton.SetActive(menuOpen);
        schematicMenu.SetActive(!menuOpen);
        exitButton.SetActive(!menuOpen);
        menuOpen = !menuOpen;
        UnityEngine.Debug.Log($"Menu toggled. turned on: {fileNom}");
    }
}
