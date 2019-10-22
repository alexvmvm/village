using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

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

    public static SaveFile GetSaveFile(string name)
    {
        return GetSaves().Where(s => s.FileNameWithoutExt == name).FirstOrDefault();
    }

    public static bool SaveExists(string saveName)
    {
        return GetSaveFile(saveName) != null;
    }

    public static void CreateSave(string saveName)
    {
        File.Create($"{SaveDirectory}/{saveName}.xml");
    }

    public static void DeleteSave(SaveFile save)
    {
        File.Delete(save.FilePath);
    }

    public static IEnumerable<ThingSave> LoadThingSaves(SaveFile save)
    {
        var document = new XmlDocument();
        document.Load(save.FilePath);

        var objs = new List<ThingSave>();
        var nodes = document.SelectNodes("/entities/entity");

        foreach(XmlNode node in nodes)
        {
            var serializer = new XmlSerializer(typeof(ThingSave));
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(node.InnerXml);
            sw.Flush();
            ms.Position = 0;
            objs.Add((ThingSave)serializer.Deserialize(ms));            
        }

        return objs;
    }

    public static void SaveThings(IEnumerable<Thing> things, SaveFile saveFile)
    {
        var memoryStream = new MemoryStream();

        var settings = new XmlWriterSettings()
        {
            Indent = true,
            OmitXmlDeclaration = true
        };

        var writer = XmlWriter.Create(memoryStream, settings);
        writer.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
        writer.WriteStartElement("entities");

        foreach(var thing in things)
        {
            var serializer = new XmlSerializer(typeof(ThingSave));
            var emptyNs = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            // write entity with id..
            writer.WriteStartElement("entity");
            serializer.Serialize(writer, thing.ToSaveObj(), emptyNs);
            writer.WriteEndElement();
        }

        writer.WriteFullEndElement();
		writer.Flush();
		memoryStream.Flush();
		memoryStream.Seek(0, SeekOrigin.Begin);
		using (var fileStream = File.Create(saveFile.FilePath))
			memoryStream.WriteTo(fileStream);
    }

    public static IEnumerator LoadGame(Game game, SaveFile save)
    {
        // clear game
        foreach(var thing in game.Things.ToArray())
        {
            game.RemoveThing(thing);
        }

        // load game`
        var objs = SaveFiles.LoadThingSaves(save);
        foreach(var obj in objs)
        {
            var thing = game.Create(obj.type);
            thing.FromSaveObj(obj);
            game.AddThing(thing);
            yield return null;
        }
    }

    public static IEnumerator SaveGame(Game game, SaveFile save)
    {
        SaveThings(game.Things, save);
        yield return null;
    }

}
