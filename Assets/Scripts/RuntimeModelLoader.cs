using System.Diagnostics;
using TriLibCore;
using TriLibCore.General;
using UnityEngine;

public class RuntimeModelLoader : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
        {
            print("You clicked the button!");
            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            var assetLoaderFilePicker = AssetLoaderFilePicker.Create();
            assetLoaderFilePicker.LoadModelFromFilePickerAsync("Select a 3D Model", OnLoad, OnMaterialsLoad, OnProgress, OnBeginLoad, OnError, null, assetLoaderOptions);
        }
    }

    private void OnBeginLoad(bool anyModelSelected)
    {
        if (!anyModelSelected)
        {
            UnityEngine.Debug.Log("No model selected.");
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

        // Iterate through children in reverse to safely delete
        for (int i = root.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = root.transform.GetChild(i);

            foreach (string name in namesToDelete)
            {
                if (child.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
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
            UnityEngine.Debug.LogError("URP Lit Shader not found. Make sure URP is installed and set up.");
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

                // Attempt to preserve textures and colors
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
