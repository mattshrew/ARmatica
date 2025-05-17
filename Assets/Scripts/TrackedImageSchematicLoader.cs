using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageSchematicLoader : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    public Transform spawnOffset; // optional empty GameObject to tweak relative placement
    private GameObject currentSchematic;
    public GameObject instructionText;

    [Header("Set from UI or code")]
    public string selectedSchematicName = "";

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var image in args.added)
        {
            TryPlaceSchematic(image);
        }

        foreach (var image in args.updated)
        {
            if (image.trackingState == TrackingState.Tracking && currentSchematic == null)
            {
                TryPlaceSchematic(image);
            }
        }
    }

    void TryPlaceSchematic(ARTrackedImage trackedImage)
    {
        if (string.IsNullOrEmpty(selectedSchematicName)) return;

        GameObject prefab = Resources.Load<GameObject>($"Schematics/{selectedSchematicName}");
        if (prefab == null)
        {
            Debug.LogError($"Schematic '{selectedSchematicName}' not found in Resources/Schematics");
            return;
        }

        instructionText.GetComponent<TMP_Text>().text = $"Building {prefab.name}...";

        currentSchematic = Instantiate(prefab, trackedImage.transform);
        currentSchematic.transform.localPosition = spawnOffset != null ? spawnOffset.localPosition : Vector3.zero;
        currentSchematic.transform.localRotation = spawnOffset != null ? spawnOffset.localRotation : Quaternion.identity;
        currentSchematic.transform.localScale = spawnOffset != null ? spawnOffset.localScale : Vector3.one;

        instructionText.SetActive(false);
    }
}
