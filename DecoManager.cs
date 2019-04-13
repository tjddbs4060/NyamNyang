using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameHelper;
using System.Xml.Serialization;

public enum EDecoType
{
    ROOF,
    STOVE,
    ACCESSORY1,
    SIGN,
    ACCESSORY2,
}

[XmlRoot("Deco")]
public struct XMLDeco
{
    [XmlAttribute("Type")]
    public EDecoType type;
    [XmlAttribute("ID")]
    public int id;
    [XmlAttribute("Name")]
    public string name;
    [XmlAttribute("Price")]
    public int price;

    public void SetData(EDecoType type, int id, string name, int price)
    {
        this.type = type;
        this.id = id;
        this.name = name;
        this.price = price;
    }

    public string GetResourcePath(EDecoType type)
    {
        return string.Format("{0}/{1}", ResourcePath.decoObject, name);
    }
}


public class DecoManager : MonoSingleton<DecoManager>
{
    private List<Image> decoImgList = new List<Image>();
    
    public void Init()
    {
        decoImgList.Add(GameObject.Find("Image (Roof)").GetComponent<Image>());
    }

    public void ChangeDecoImage(int _index, string _defaultImgName)
    {
        string imgName = PlayerPrefs.GetString(string.Format("DECO_USE_{0}", _index), _defaultImgName);
        decoImgList[_index].sprite = ResourcesManager.getInstance.CreateSprite(imgName, new Vector2(0.5f, 0.5f));
    }

    // Manager
    public List<XMLDeco> decos { get; private set; }

    public int Size
    {
        get { return decos.Count; }
    }

    public IEnumerator Initialize()
    {
        //galbis = XMLManager<XMLGalbi, Galbi>.Load (gameObject, "Galbi.xml");
        decos = XMLManager<XMLDeco>.Load(ResourcePath.xmlDeco);

        ResourcesManager resourceManager = ResourcesManager.getInstance;

        List<XMLDeco>.Enumerator eDeco = decos.GetEnumerator();

        while (eDeco.MoveNext())
        {
            XMLDeco deco = eDeco.Current;

            List<string> paths = new List<string>();
            
            paths.Add(string.Format("{0}/{1}_icon", ResourcePath.decoObject, deco.name));

            List<string>.Enumerator path = paths.GetEnumerator();

            while (path.MoveNext())
            {
                string resourcePath = path.Current;

                resourceManager.LoadResource(RESOURCE_TYPE.TEXTURE, resourcePath);
            }
        }

        yield return null;
    }

    public XMLDeco GetDeco(int id)
    {
        if (decos == null)
        {
            //Initialize();
        }

        List<XMLDeco>.Enumerator eDeco = decos.GetEnumerator();

        while (eDeco.MoveNext())
        {
            XMLDeco deco = eDeco.Current;

            if (deco.id.Equals(id))
            {
                return deco;
            }
        }

        throw new Exception("Invalid Galbi as ID");
    }

    public List<XMLDeco> GetDecos(Predicate<XMLDeco> predicate)
    {
        return decos.FindAll(predicate);
    }

    public List<XMLDeco> GetDecos(EDecoType _type)
    {
        return decos.FindAll(i => i.type == _type);
    }
}
