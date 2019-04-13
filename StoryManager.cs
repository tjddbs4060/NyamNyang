using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[XmlRoot("XMLNyangStory")]
public struct XMLNyangStory
{
    [XmlAttribute("ID")]
    public int id;
    [XmlAttribute("Story1")]
    public string story1;
    [XmlAttribute("Story2")]
    public string story2;

    public XMLNyangStory(int _id, string _story1, string _story2)
    {
        id = _id;
        story1 = _story1;
        story2 = _story2;
    }
}

public class StoryManager : Singleton<StoryManager> {
    private List<XMLNyangStory> nyangStory;

    public int Size
    {
        get { return nyangStory.Count; }
    }

    public IEnumerator Initialize()
    {
        nyangStory = XMLManager<XMLNyangStory>.Load(ResourcePath.xmlStory);
        
        yield return null;
    }

    public XMLNyangStory GetNyangStory(int id)
    {
        return nyangStory.Find(n => n.id == id);
    }
}
