using DG.Tweening.Plugins.Core.PathCore;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class JsonTool
{
    public static void Save<T>(T obj, string folderPath, string fileName)
    {
        try
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            var path = $"{folderPath}/{fileName}";

            File.WriteAllText(path, json);
            Debug.Log($"JSON 저장 완료: {path}");
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public static bool TryLoad<T>(string folderPath, string fileName, out T output)
    {
        try
        {
            var path = $"{folderPath}/{fileName}";

            if (!File.Exists(path))
            {
                output = default;
                return false;
            }
            
            string json = File.ReadAllText(path);
            output = JsonConvert.DeserializeObject<T>(json);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);

            output = default;
            return false;
        }
    }
}
