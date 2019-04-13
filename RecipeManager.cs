using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


[XmlRoot("Recipe")]
public struct XMLRecipe
{
    [XmlAttribute("ID")]
    public int id;
    [XmlAttribute("Galbi")]
    public int galbi;
    [XmlAttribute("Sauce")]
    public int sauce;
    [XmlAttribute("Powder")]
    public int powder;

    public void SetData(int id, int galbi, int sauce, int powder)
    {
        this.id = id;
        this.galbi = galbi;
        this.sauce = sauce;
        this.powder = powder;
    }
}

public class RecipeManager : Singleton<RecipeManager>
{
    //private GameObject gameObject = new GameObject();
    public List<XMLRecipe> recipes { get; private set; }

    public int Size
    {
        get { return recipes.Count; }
    }

	public IEnumerator Initialize()
    {
        //recipes = XMLManager<XMLRecipe, Recipe>.Load(gameObject, "Recipe.xml");
        recipes = XMLManager<XMLRecipe>.Load(ResourcePath.xmlRecipe);

		yield return null;
    }

    public XMLRecipe GetRecipe(int id)
    {
        List<XMLRecipe>.Enumerator eRecipe = recipes.GetEnumerator();

        while (eRecipe.MoveNext())
        {
            XMLRecipe recipe = eRecipe.Current;

            if (recipe.id.Equals(id))
            {
                return recipe;
            }
        }

        throw new Exception("Invaild Recipe as ID");
    }

    public XMLRecipe GetRecipe(int galbi, int sauce, int powder)
    {
        XMLRecipe recipe = new XMLRecipe();

        recipe.SetData(-1, galbi, sauce, powder);

        return recipe;
    }
}
