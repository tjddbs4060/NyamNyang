using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;


[XmlRoot("Nyang")]
public struct XMLNyang
{
    [XmlAttribute("ID")]
    public int id;
    [XmlAttribute("Name")]
    public string name;
    [XmlAttribute("Rank")]
    public ENyangRank rank;
    [XmlAttribute("AppearType")]
    public EConditionType[] appearType;
    [XmlAttribute("Appear")]
    public int[] appear;
    [XmlAttribute("Position")]
    public ENyangPosition position;

    public void SetData(int id, string name, ENyangRank rank, EConditionType[] appearType, int[] appear, ENyangPosition pos)
    {
        this.id = id;
        this.name = name;
        this.rank = rank;
        this.appearType = appearType;
        this.appear = appear;
        this.position = pos;
    }
    public List<string> GetResourcePath()
    {
        List<string> paths = new List<string>();

        if (name.Contains("_n_"))
        {
            paths.Add(string.Format("{0}/{1}", ResourcePath.normalCharacter, name));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.normalCharacter, name, ENyangState.wait.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.normalCharacter, name, ENyangState.order.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.normalCharacter, name, ENyangState.pick.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.normalCharacter, name, ENyangState.angry.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.normalCharacter, name, ENyangState.happy.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.normalCharacter, name, ENyangState.buff.ToString()));
        }

        if(name.Contains("_r_"))
        {
            paths.Add(string.Format("{0}/{1}", ResourcePath.rareCharacter, name));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.rareCharacter, name, ENyangState.wait.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.rareCharacter, name, ENyangState.order.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.rareCharacter, name, ENyangState.pick.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.rareCharacter, name, ENyangState.angry.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.rareCharacter, name, ENyangState.happy.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.rareCharacter, name, ENyangState.buff.ToString()));
        }

        if(name.Contains("_h_"))
        {
            paths.Add(string.Format("{0}/{1}", ResourcePath.hiddenCharacter, name));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.hiddenCharacter, name, ENyangState.wait.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.hiddenCharacter, name, ENyangState.order.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.hiddenCharacter, name, ENyangState.pick.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.hiddenCharacter, name, ENyangState.angry.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.hiddenCharacter, name, ENyangState.happy.ToString()));
            paths.Add(string.Format("{0}/{1}_{2}", ResourcePath.hiddenCharacter, name, ENyangState.buff.ToString()));
        }

        string story1 = string.Format("{0}/{1}_1", ResourcePath.story, name);
        string story2 = string.Format("{0}/{1}_2", ResourcePath.story, name);

        story1 = story1.Replace("cat", "story");
        story2 = story2.Replace("cat", "story");

        paths.Add(story1);
        paths.Add(story2);

        return paths;
    }
}

public class NyangManager : Singleton<NyangManager>
{
    private List<XMLNyang> nyangs { get; set; }

    public int Size
    {
        get { return nyangs.Count; }
    }

	public IEnumerator Initialize()
    {
        nyangs = XMLManager<XMLNyang>.Load(ResourcePath.xmlNyang);

        ResourcesManager resourceManager = ResourcesManager.getInstance;

        List<XMLNyang>.Enumerator eNyang = nyangs.GetEnumerator();

        while (eNyang.MoveNext())
        {
            XMLNyang nyang = eNyang.Current;

            List<string> paths = nyang.GetResourcePath();

            List<string>.Enumerator path = paths.GetEnumerator();

            while (path.MoveNext())
            {
                string resourcePath = path.Current;

                resourceManager.LoadResource(RESOURCE_TYPE.TEXTURE, resourcePath);
            }
        }

		yield return null;
    }

    public bool InvalidID(int id)
    {
        return nyangs.FindAll(n => n.id == id).Count > 0;
    }

    public XMLNyang GetNyang(int id)
    {
        List<XMLNyang>.Enumerator eNyang = nyangs.GetEnumerator();

        while (eNyang.MoveNext())
        {
            XMLNyang nyang = eNyang.Current;

            if (nyang.id.Equals(id))
            {
                return nyang;
            }
        }

        throw new Exception("Invaild Nyang as ID : " + id);
    }

    public List<XMLNyang> GetNyangs(EConditionType conditionType)
    {
        return nyangs.FindAll((n) => {
            if (n.appear == null || n.appearType == null)
                return false;

            int size = n.appearType.Length;

            for (int i = 0; i < size; i++)
            {
                if (n.appearType[i] == conditionType)
                    return true;
            }

            return false;
        });
    }

    private Predicate<XMLNyang> GetPredicate(ENyangRank _rank)
    {
        Predicate<XMLNyang> predicate = new Predicate<XMLNyang>((n)=> {
            if (n.rank != _rank)
                return false;

            bool nyangCondition = true;

            if (n.appearType != null && n.appear != null)
            {
                for (int i = 0; i < n.appearType.Length; i++)
                {
                    if (!AppearConditionManager.getInstance.CheckCondition(n.appearType[i], n.appear[i]))
                    {
                        nyangCondition = false;
                        break;
                    }
                }
            }

            return nyangCondition;
        });
        
        return predicate;
    }

    public bool GetNyang(ENyangPosition position, out List<XMLNyang> normals, out List<XMLNyang> rares, out List<XMLNyang> hidden)
    {
        List<XMLNyang> equalPositions = nyangs.FindAll(n => n.position.Equals(position));

        normals = new List<XMLNyang>();
        rares = new List<XMLNyang>();
        hidden = new List<XMLNyang>();

        normals = equalPositions.FindAll(GetPredicate(ENyangRank.Normal));
        rares = equalPositions.FindAll(GetPredicate(ENyangRank.Rare));
        hidden = equalPositions.FindAll(GetPredicate(ENyangRank.Hidden));

        return normals.Count + rares.Count + hidden.Count > 0;
    }
}
