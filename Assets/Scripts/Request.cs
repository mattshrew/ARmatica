using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.Mime.MediaTypeNames;

public class Request : MonoBehaviour
{
    public SchematicUIManager schematicUIManager;

    void Start()
    {
        schematicUIManager.start = true;
        return;
        string uri = "https://classic-collie-ultimately.ngrok-free.app/getfile";
        string fileName = "files.zip";

        string zipPath = Path.Combine(UnityEngine.Application.persistentDataPath, fileName);
        string extractPath = Path.Combine(UnityEngine.Application.persistentDataPath, "ARmatica");

        StartCoroutine(DownloadAndExtract(uri, zipPath, extractPath));
    }

    IEnumerator DownloadAndExtract(string uri, string zipPath, string extractPath)
    {
        UnityWebRequest request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = new DownloadHandlerFile(zipPath);
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
            UnityEngine.Debug.Log($"ZIP file downloaded to: {zipPath}");

            if (!Directory.Exists(extractPath))
                Directory.CreateDirectory(extractPath);

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // Skip directories
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;

                    // Force extraction into the ARmatica folder (flatten structure)
                    string flatFileName = Path.GetFileName(entry.FullName);
                    string destinationPath = Path.Combine(extractPath, flatFileName);

                    // Skip if file already exists
                    if (File.Exists(destinationPath))
                    {
                        UnityEngine.Debug.Log($"Skipped duplicate file: {flatFileName}");
                        continue;
                    }

                    entry.ExtractToFile(destinationPath);
                    UnityEngine.Debug.Log($"Extracted: {flatFileName}");
                }
            }

            UnityEngine.Debug.Log($"ZIP file extracted to: {extractPath} (without overwriting existing files)");
            schematicUIManager.start = true;

        }
    }
}
