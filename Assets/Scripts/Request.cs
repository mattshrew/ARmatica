using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.Mime.MediaTypeNames;

public class Request : MonoBehaviour
{
    void Start()
    {
        string uri = "https://classic-collie-ultimately.ngrok-free.app/getfile";
        string fileName = "model.fbx";
        string filePath = Path.Combine(UnityEngine.Application.persistentDataPath, fileName);

        StartCoroutine(DownloadFBX(uri, filePath));
    }

    IEnumerator DownloadFBX(string uri, string path)
    {
        UnityWebRequest request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = new DownloadHandlerFile(path);
        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
        {
            UnityEngine.Debug.LogError("Download error: " + request.error);
        }
        else
        {
            UnityEngine.Debug.Log($"FBX file downloaded to: {path}");
        }
    }
}
