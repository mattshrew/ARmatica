using System;
using System.Diagnostics;
using System.IO;
using TriLibCore;
using TriLibCore.General;
using UnityEngine;

public class RuntimeModelLoader : MonoBehaviour
{
    // Set this to the absolute path of your model file
    private string modelFilePath = "C:\\Users\\amitw\\Downloads\\breadboar.fbx"; // Example path on Android

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Load Model"))
        {
            if (!File.Exists(modelFilePath))
            {
                UnityEngine.Debug.LogError($"Model file not found at path: {modelFilePath}");
                return;
            }

            UnityEngine.Debug.Log($"Attempting to load model from path: {modelFilePath}");

            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();

            AssetLoader.LoadModelFromFile(modelFilePath, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions);
        }
    }

    private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
    {
        UnityEngine.Debug.Log($"Loading progress: {progress * 100f}%");
    }

    private void OnError(IContextualizedError contextualizedError)
    {
        UnityEngine.Debug.LogError($"Error loading model: {contextualizedError.GetInnerException()}");
    }

    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        var loadedGameObject = assetLoaderContext.RootGameObject;
        loadedGameObject.SetActive(false);
    }

    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        var loadedGameObject = assetLoaderContext.RootGameObject;
        CleanUpImportedModel(loadedGameObject);
        ConvertMaterialsToURPLit(loadedGameObject);
        loadedGameObject.SetActive(true);
        loadedGameObject.transform.position = Vector3.zero;
    }

    private void CleanUpImportedModel(GameObject root)
    {
        string[] namesToDelete = { "cube", "camera", "light" };

        for (int i = root.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = root.transform.GetChild(i);
            foreach (string name in namesToDelete)
            {
                if (child.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    DestroyImmediate(child.gameObject);
                    break;
                }
            }
        }
    }

    private void ConvertMaterialsToURPLit(GameObject root)
    {
        Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLitShader == null)
        {
            UnityEngine.Debug.LogError("URP Lit Shader not found. Ensure URP is installed and set up correctly.");
            return;
        }

        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            Material[] newMaterials = new Material[renderer.sharedMaterials.Length];

            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                Material originalMat = renderer.sharedMaterials[i];
                if (originalMat == null) continue;

                Material newMat = new Material(urpLitShader);

                if (originalMat.HasProperty("_MainTex"))
                    newMat.SetTexture("_BaseMap", originalMat.GetTexture("_MainTex"));

                if (originalMat.HasProperty("_Color"))
                    newMat.SetColor("_BaseColor", originalMat.GetColor("_Color"));

                newMaterials[i] = newMat;
            }

            renderer.sharedMaterials = newMaterials;
        }
    }
}
