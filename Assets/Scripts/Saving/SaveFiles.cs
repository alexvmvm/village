using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveFile
{
    public string FileNameWithoutExt { get { return Path.GetFileNameWithoutExtension(FileName); } }
    public string FileName;
    public string FilePath;
    public DateTime Created;
    public DateTime Modified;
}

public class SaveFiles
{
    public static string SaveDirectory 
    {
        get 
        {
            var path = $"{Application.persistentDataPath}/village";
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }

    public static IEnumerable<SaveFile> GetSaves()
    {
        return Directory
            .GetFiles(SaveDirectory)
            .Where(f => Path.GetExtension(f) == ".xml")
            .Select(f => new SaveFile
            {
                FileName = Path.GetFileName(f),
                Created = File.GetCreationTime(f),
                Modified = File.GetLastWriteTime(f),
                FilePath = Path.Combine(SaveDirectory, Path.GetFileName(f))
            })
            .OrderByDescending(s => s.Created)
            .ToArray();
    }

    public static void DeleteSave(SaveFile save)
    {
        File.Delete(save.FilePath);
    }

}
