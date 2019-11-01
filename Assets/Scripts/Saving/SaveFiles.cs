using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;

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
                var path = $"{Application.persistentDataPath}/Saves";
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
            var saves = GetSaves().ToArray();
            return saves.Where(s => s.FileNameWithoutExt == name).FirstOrDefault();
        }

        public static bool SaveExists(string saveName)
        {
            return GetSaveFile(saveName) != null;
        }

        public static void DeleteSave(SaveFile save)
        {
            File.Delete(save.FilePath);
        }

        public static void SaveGameWithName(Game game, string name)
        {
            var savePath = Path.Combine(SaveDirectory, $"{name}.xml");
            var saveFile = SaveFileFromFilePath(savePath);
            SaveGame(game, saveFile.FilePath);
        }

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

        public static GameSave LoadGameFromName(string name)
        {
            var saveFile = GetSaveFile(name);
            return LoadGame(saveFile.FilePath);
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
