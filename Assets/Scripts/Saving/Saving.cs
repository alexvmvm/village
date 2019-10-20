using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System;

public class Saving : MonoBehaviour
{
    public Game Game;
    public string SavePath;

    public static bool IsInMacOS { get { return UnityEngine.SystemInfo.operatingSystem.IndexOf("MacOS") != -1; } }
    public static bool IsInWinOS { get { return UnityEngine.SystemInfo.operatingSystem.IndexOf("Windows") != -1; } }

    private MemoryStream _memoryStream;
    private XmlWriter _writer;

    [BitStrap.Button]
    public void Save()
    {
        _memoryStream = new MemoryStream();

        var settings = new XmlWriterSettings()
        {
            Indent = true,
            OmitXmlDeclaration = true
        };

        _writer = XmlWriter.Create(_memoryStream, settings);
        _writer.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
        _writer.WriteStartElement("entities");

        foreach(var thing in Game.Things)
        {
            var serializer = new XmlSerializer(typeof(ThingSave));
            var emptyNs = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            // write entity with id..
            _writer.WriteStartElement("entity");
            _writer.WriteAttributeString("id", thing.id);
            _writer.WriteAttributeString("type", thing.type.ToString());
            _writer.WriteAttributeString("x", thing.transform.position.x.ToString());
            _writer.WriteAttributeString("y", thing.transform.position.y.ToString());
            _writer.WriteAttributeString("z", thing.transform.position.z.ToString());
            serializer.Serialize(_writer, thing.ToSaveObj(), emptyNs);
            _writer.WriteEndElement();
        }

        _writer.WriteFullEndElement();
		_writer.Flush();
		_memoryStream.Flush();
		_memoryStream.Seek(0, SeekOrigin.Begin);
		using (var fileStream = File.Create(SavePath))
			_memoryStream.WriteTo(fileStream);

    }

    [BitStrap.Button]
    public void Load()
    {
        var document = new XmlDocument();
        document.Load(SavePath);

        foreach(var thing in Game.Things.ToArray())
        {
            Game.RemoveThing(thing);
        }

        var nodes = document.SelectNodes("/entities/entity");

        foreach(XmlNode node in nodes)
        {
            var serializer = new XmlSerializer(typeof(ThingSave));
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(node.InnerXml);
            sw.Flush();
            ms.Position = 0;

            var id = node.Attributes["id"].Value;
            var type = node.Attributes["type"].Value;
            var x = float.Parse(node.Attributes["x"].Value);
            var y = float.Parse(node.Attributes["y"].Value);
            var z = float.Parse(node.Attributes["z"].Value);

            var typeOfThing = (TypeOfThing)Enum.Parse(typeof(TypeOfThing), type);
            var thing = Game.Create(typeOfThing);
            thing.transform.position = new Vector3(x, y, z);

            var saveObj = (ThingSave)serializer.Deserialize(ms);
            thing.FromSaveObj(id, saveObj);

            Game.AddThing(thing);
            
        }


        // var nodes = document.SelectNodes("//entity[@id='" + id + "']");
        // if (nodes.Count > 1)
        //     throw new Exception(string.Format("Multiple elements found with the same id: {0}", id));
        // if (nodes.Count == 0)
        //     return default(T);

        // var node = nodes[0];
        // var serializer = new XmlSerializer(typeof(T));
        // var ms = new MemoryStream();
        // var sw = new StreamWriter(ms);
        // sw.Write(node.InnerXml);
        // sw.Flush();
        // ms.Position = 0;
        // return (T)serializer.Deserialize(ms);

    }

    [BitStrap.Button]
    public void OpenDirectory()
    {
        if (IsInWinOS)
            System.Diagnostics.Process.Start ("explorer.exe", Application.persistentDataPath.Replace (@"/", @"\"));
        else if (IsInMacOS) 
        {
            System.Diagnostics.Process.Start("open", Application.persistentDataPath);
            Debug.Log (Application.persistentDataPath);
        }
            
    }
}
