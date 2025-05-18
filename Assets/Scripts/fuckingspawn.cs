using System;
using System.Collections.Specialized;
using System.IO;
using TriLibCore;
using TriLibCore.General;
using UnityEngine;

public class fuckingspawn : MonoBehaviour
{
    private string modelFilePath;

    private void Start()
    {
        if (SchematicUIManager.Instance == null)
        {
            UnityEngine.Debug.LogError("SchematicUIManager instance is null.");
            return;
        }

        string selectedSchematicName = SchematicUIManager.Instance.GetFileName();
        UnityEngine.Debug.Log($"Selected schematic: {selectedSchematicName}");

        if (string.IsNullOrEmpty(selectedSchematicName))
        {
            UnityEngine.Debug.LogError("Selected schematic name is empty or null.");
            return;
        }

        modelFilePath = Path.Combine(UnityEngine.Application.persistentDataPath, "ARmatica", selectedSchematicName);
        UnityEngine.Debug.Log($"Attempting to load model from path: {modelFilePath}");

        if (!File.Exists(modelFilePath))
        {
            UnityEngine.Debug.LogError("Model file does not exist: " + modelFilePath);
            return;
        }

        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        AssetLoader.LoadModelFromFile(
            modelFilePath,
            OnLoad,
            OnMaterialsLoad,
            OnProgress,
            OnError,
            null,
            assetLoaderOptions
        );
    }

    private void OnProgress(AssetLoaderContext context, float progress)
    {
        UnityEngine.Debug.Log($"Loading progress: {progress * 100f}%");
    }

    private void OnError(IContextualizedError error)
    {
        UnityEngine.Debug.LogError($"Error loading model: {error.GetInnerException()}");
    }

    private void OnLoad(AssetLoaderContext context)
    {
        if (context?.RootGameObject != null)
        {
            // Don't show until materials are loaded
            context.RootGameObject.SetActive(false);
        }
    }

    private void OnMaterialsLoad(AssetLoaderContext context)
    {
        if (context?.RootGameObject == null) return;

        GameObject loadedObject = context.RootGameObject;

        CleanUpImportedModel(loadedObject);
        ConvertMaterialsToURPLit(loadedObject);

        // Attach to the prefab (which is already under the tracked image)
        loadedObject.transform.SetParent(transform, false);
        loadedObject.transform.localPosition = Vector3.zero;
        loadedObject.transform.localRotation = Quaternion.identity;
        loadedObject.transform.localScale = Vector3.one;

        loadedObject.transform.position += new Vector3(125f, 50f, 0);
        loadedObject.SetActive(true);

        NormalizeModel(loadedObject);
    }

    private void NormalizeModel(GameObject model)
    {
        Bounds bounds = GetBounds(model);
        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = 0.1f / maxSize; // Shrink large models to fit within 0.1 units


        model.transform.localScale = Vector3.one * scaleFactor;
    }

    private Bounds GetBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(Vector3.zero, Vector3.one);
        Bounds bounds = renderers[0].bounds;
        foreach (var r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }
        return bounds;
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
