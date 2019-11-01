using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;
using Village.Things;
using Village.AI;

namespace Village.Saving
{

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
        public static SaveFile SelectedSave { get { return _saveFile; } }
        private static SaveFile _saveFile;

        public static string SaveDirectory
        {
            get
            {
                var path = $"{Application.persistentDataPath}/village";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        public static IEnumerable<SaveFile> GetSaves()
        {
            return Directory
                .GetFiles(SaveDirectory)
                .Where(f => Path.GetExtension(f) == ".xml")
                .Select(SaveFileFromFilePath)
                .OrderByDescending(s => s.Created)
                .ToArray();
        }

        static SaveFile SaveFileFromFilePath(string filePath)
        {
            return new SaveFile
            {
                FileName = Path.GetFileName(filePath),
                Created = File.GetCreationTime(filePath),
                Modified = File.GetLastWriteTime(filePath),
                FilePath = Path.Combine(SaveDirectory, Path.GetFileName(filePath))
            };
        }

        public static SaveFile GetSaveFile(string name)
        {
            return GetSaves().Where(s => s.FileNameWithoutExt == name).FirstOrDefault();
        }

        public static bool SaveExists(string saveName)
        {
            return GetSaveFile(saveName) != null;
        }

        // public static SaveFile CreateNewSave(string saveName)
        // {
        //     var savePath = $"{SaveDirectory}/{saveName}.xml";
        //     var saveFile = SaveFileFromFilePath(savePath);
        //     SaveThings(new List<Thing>(), saveFile);
        //     _saveFile = saveFile;
        //     return saveFile;
        // }

        public static void DeleteSave(SaveFile save)
        {
            File.Delete(save.FilePath);
        }

        // public static IEnumerable<ThingSave> LoadThingSaves(SaveFile save)
        // {
        //     var document = new XmlDocument();
        //     document.Load(save.FilePath);

        //     var objs = new List<ThingSave>();
        //     var nodes = document.SelectNodes("/entities/entity");

        //     foreach (XmlNode node in nodes)
        //     {
        //         var serializer = new XmlSerializer(typeof(ThingSave));
        //         var ms = new MemoryStream();
        //         var sw = new StreamWriter(ms);
        //         sw.Write(node.InnerXml);
        //         sw.Flush();
        //         ms.Position = 0;
        //         objs.Add((ThingSave)serializer.Deserialize(ms));
        //     }

        //     return objs;
        // }

        // public static void SaveThings(IEnumerable<Thing> things, SaveFile saveFile)
        // {
        //     var memoryStream = new MemoryStream();

        //     var settings = new XmlWriterSettings()
        //     {
        //         Indent = true,
        //         OmitXmlDeclaration = true
        //     };

        //     var writer = XmlWriter.Create(memoryStream, settings);
        //     writer.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
        //     writer.WriteStartElement("entities");

        //     foreach (var thing in things)
        //     {
        //         var serializer = new XmlSerializer(typeof(ThingSave));
        //         var emptyNs = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

        //         // write entity with id..
        //         writer.WriteStartElement("entity");
        //         serializer.Serialize(writer, thing.ToSaveObj(), emptyNs);
        //         writer.WriteEndElement();
        //     }

        //     writer.WriteFullEndElement();
        //     writer.Flush();
        //     memoryStream.Flush();
        //     memoryStream.Seek(0, SeekOrigin.Begin);
        //     using (var fileStream = File.Create(saveFile.FilePath))
        //         memoryStream.WriteTo(fileStream);

        //     writer.Close();
        //     memoryStream.Close();
        // }

        // public static IEnumerator LoadCurrentSaveGame(Game game)
        // {
        //     if (_saveFile == null)
        //     {
        //         throw new Exception("Save file not set");
        //     }

        //     // clear game
        //     game.Clear();

        //     // load game
        //     var objs = SaveFiles.LoadThingSaves(_saveFile);
        //     foreach (var obj in objs)
        //     {
        //         var thing = game.Create(obj.type);
        //         thing.FromSaveObj(obj);
        //         game.AddThing(thing);
        //         yield return null;
        //     }
        // }

        public static void SaveGame(Game game, string path)
        {
            var memoryStream = new MemoryStream();
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            var writer = XmlWriter.Create(memoryStream, settings);
            var serializer = new XmlSerializer(typeof(GameSave));
            var emptyNs = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            serializer.Serialize(writer, game.ToSaveObj(), emptyNs);
            writer.Flush();
            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            using (var fileStream = File.Create(path))
                memoryStream.WriteTo(fileStream);

            writer.Close();
            memoryStream.Close();
        }

        public static GameSave LoadGame(string path)
        {
            var document = new XmlDocument();
            document.Load(path);

            var serializer = new XmlSerializer(typeof(GameSave));
            var reader = new StreamReader(path);
            var obj = (GameSave)serializer.Deserialize(reader);
            reader.Close();

            return obj;
        }

        public static void SetSaveAndLoad(string name, bool newGame)
        {
            PlayerPrefs.SetString(Constants.PLAYER_PREFS_SAVE_NAME, name);
            PlayerPrefs.SetInt(Constants.PLAYER_PREFS_EXISTING_SAVE, newGame ? 0 : 1);
            SceneManager.LoadScene(Constants.SCENE_MAIN);
        }

        public static void ClearSetSave()
        {
            PlayerPrefs.DeleteKey(Constants.PLAYER_PREFS_SAVE_NAME);
            PlayerPrefs.DeleteKey(Constants.PLAYER_PREFS_EXISTING_SAVE);
        }

        public static bool IsNewGame()
        {
            return PlayerPrefs.GetInt(Constants.PLAYER_PREFS_EXISTING_SAVE) == 0;
        }

        public static GameSave GetCurrentSetGameSave()
        {
            var saveFile = GetSaveFile(PlayerPrefs.GetString(Constants.PLAYER_PREFS_SAVE_NAME));
            return LoadGame(saveFile.FilePath);
        }
    }

}
