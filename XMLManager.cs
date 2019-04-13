using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


public static class XMLManager<T>
{
    public static List<T> Load(string path)
    {
        List<T> xml = new List<T>();

        if (path.Equals(""))
        {
            throw new Exception("Is not initialized Path!");
        }

        using (var reader = new StreamReader(string.Format("{0}/{1}", ResourcePath.xml, path)))
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<T>));
            xml = (List<T>)xs.Deserialize(reader);
        }

        return xml;
    }
}