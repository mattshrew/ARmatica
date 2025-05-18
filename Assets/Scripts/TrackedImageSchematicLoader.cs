using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageSchematicLoader : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    public Transform spawnOffset;
    private GameObject currentSchematic;
    public GameObject instructionText;


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

        GameObject prefab = Resources.Load<GameObject>($"Schematics/scriptfab");
        if (prefab == null)
        {
            Debug.LogError($"Schematic 'scriptfab' not found in Resources/Schematics");
            return;
        }

        instructionText.GetComponent<TMP_Text>().text = $"Building {prefab.name}...";

        // Instantiate the prefab as a direct child of the tracked image
        currentSchematic = Instantiate(prefab, trackedImage.transform.position, trackedImage.transform.rotation, trackedImage.transform);

        // Match scale (optional: ensure scale is relative to the image size)
        currentSchematic.transform.localScale = Vector3.one;

        instructionText.SetActive(false);
    }
}
