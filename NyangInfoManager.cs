using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using UnityGameHelper;

[XmlRoot("NyangInfo")]
public struct XMLNyangInfo
{
    [XmlAttribute("ID")]
    public int id;
    [XmlAttribute("Name")]
    public string name;
    [XmlAttribute("Description")]
    public string desc;
    [XmlAttribute("Appear")]
    public string appear;
    [XmlIgnore]
    public ENyangRank rank;
    [XmlIgnore]
    public string imgPath;
    [XmlIgnore]
    public int visited;

    public XMLNyangInfo(XMLNyangInfo info)
    {
        id = info.id;
        name = info.name;
        desc = info.desc;
        appear = info.appear;
        rank = info.rank;
        imgPath = info.imgPath;
        visited = info.visited;
    }

    public void SetData(int _id, string _name, string _desc, string _appear)
    {
        id = _id;
        name = _name;
        desc = _desc;
        appear = _appear;
    }

    public void SetData(ENyangRank _rank, string _imgPath, int _visited)
    {
        rank = _rank;
        imgPath = _imgPath;
        visited = _visited;
    }

    public void Visited()
    {
        PlayerPrefs.SetInt(string.Format("NyangVisited_{0}", id), ++visited);
    }
}

public class NyangInfoManager : Singleton<NyangInfoManager>
{
    private List<XMLNyangInfo> nyangInfo;

    public int Size
    {
        get { return nyangInfo.Count; }
    }

    public IEnumerator Initialize()
    {
        nyangInfo = new List<XMLNyangInfo>();

        List<XMLNyangInfo> infoList = XMLManager<XMLNyangInfo>.Load(ResourcePath.xmlNyangInfo);

        NyangManager nyangManager = NyangManager.getInstance;
        
        List<XMLNyangInfo>.Enumerator eInfo = infoList.GetEnumerator();

        if (eInfo.MoveNext())
        {
            XMLNyangInfo info = eInfo.Current;

            info.SetData(ENyangRank.Normal, "cat_n_tipnyang", 1);
            
            nyangInfo.Add(new XMLNyangInfo(info));
        }

        while (eInfo.MoveNext())
        {
            XMLNyangInfo info = eInfo.Current;

            if (nyangManager.InvalidID(info.id))
            {
                XMLNyang data = nyangManager.GetNyang(info.id);

                info.SetData(data.rank, data.name, PlayerPrefs.GetInt(string.Format("NyangVisited_{0}", info.id), 0));

                nyangInfo.Add(new XMLNyangInfo(info));
            }
        }

        yield return null;
    }

    public XMLNyangInfo GetNyangInfo(int id)
    {
        return nyangInfo.Find(n => n.id == id);
    }

    public void Visited(int id)
    {
        XMLNyangInfo nyang = GetNyangInfo(id);
        XMLNyangInfo tempNyang = new XMLNyangInfo(nyang);

        nyangInfo.Remove(nyang);
        tempNyang.Visited();

        nyangInfo.Add(new XMLNyangInfo(tempNyang));
    }
}
