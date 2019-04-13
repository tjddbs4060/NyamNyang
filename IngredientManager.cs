using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;


[XmlRoot("Ingredient")]
public struct XMLIngredient
{
    [XmlAttribute("Type")]
    public EIngredientType type;
    [XmlAttribute("ID")]
    public int id;
    [XmlAttribute("Name")]
    public string name;
    [XmlAttribute("Price")]
    public int price;

    public void SetData(EIngredientType type, int id, string name, int price)
    {
        this.type = type;
        this.id = id;
        this.name = name;
        this.price = price;
    }

    public string GetResourcePath(EIngredientType type)
    {
        return string.Format("{0}/{1}", (type == EIngredientType.Deco) ? ResourcePath.decoObject : ResourcePath.cookObject, name);
    }

    public string GetFailGalbiPath(bool name)
    {
        string head = (name) ? ("") : (string.Format("{0}/", ResourcePath.cookObject));

        string path = "";
        switch (id)
        {
            case 1:
                path = string.Format("{0}obj_cook_meat_over", head);
                break;
            case 2:
                path = string.Format("{0}obj_cook_tteok_over", head);
                break;
            case 3:
                path = string.Format("{0}obj_cook_chicken_over", head);
                break;
            case 4:
                path = string.Format("{0}obj_cook_apple_over", head);
                break;
            default:
                throw new System.Exception("Invalid Galbi Type");
        }

        return path;
    }

    public string GetGalbiPath(int index, bool name)
    {
        string head = (name) ? ("") : (string.Format("{0}/", ResourcePath.cookObject));

        string path;

        switch (id)
        {
            case 1:
                path = string.Format("{0}obj_cook_meat_{1}", head, index);
                break;
            case 2:
                path = string.Format("{0}obj_cook_tteok_{1}", head, index);
                break;
            case 3:
                path = string.Format("{0}obj_cook_chicken_{1}", head, index);
                break;
            case 4:
                path = string.Format("{0}obj_cook_apple_{1}", head, index);
                break;
            default:
                throw new System.Exception("Invalid Galbi Type");
        }

        return path;
    }

    public string GetPowderPath(bool name)
    {
        string head = (name) ? ("") : (string.Format("{0}/", ResourcePath.cookObject));

        string path;

        switch (id)
        {
            case 1:
                path = string.Format("{0}obj_cook_meat_coca", head);
                break;
            case 2:
                path = string.Format("{0}obj_cook_chicken_coca", head);
                break;
            case 3:
                path = string.Format("{0}obj_cook_chicken_tteok_coca", head);
                break;
            case 4:
                path = string.Format("{0}obj_cook_apple_coca", head);
                break;
            default:
                throw new System.Exception("Invalid Galbi Type");
        }

        return path;
    }

    public string GetSaucePath(bool name)
    {
        string head = (name) ? ("") : (string.Format("{0}/", ResourcePath.cookObject));

        string path;

        switch (id)
        {
            case 1:
                path = string.Format("{0}obj_cook_meat_lsd", head);
                break;
            case 2:
                path = string.Format("{0}obj_cook_chicken_lsd", head);
                break;
            case 3:
                path = string.Format("{0}obj_cook_chicken_tteok_lsd", head);
                break;
            case 4:
                path = string.Format("{0}obj_cook_apple_lsd", head);
                break;
            default:
                throw new System.Exception("Invalid Galbi Type");
        }

        return path;
    }

}

public class IngredientManager : Singleton<IngredientManager>
{
    public List<XMLIngredient> ingredients { get; private set; }
    
    public int Size
    {
        get { return ingredients.Count; }
    }

	public IEnumerator Initialize()
    {
        //galbis = XMLManager<XMLGalbi, Galbi>.Load (gameObject, "Galbi.xml");
        ingredients = XMLManager<XMLIngredient>.Load(ResourcePath.xmlIngredient);

        ResourcesManager resourceManager = ResourcesManager.getInstance;

        List<XMLIngredient>.Enumerator eIngredient = ingredients.GetEnumerator();

        while (eIngredient.MoveNext())
        {
            XMLIngredient ingredient = eIngredient.Current;

            List<string> paths = new List<string>();
            paths.Add(ingredient.GetResourcePath(ingredient.type));

            if (ingredient.type == EIngredientType.Galbi)
            {
                paths.Add(ingredient.GetGalbiPath(1, false));
                paths.Add(ingredient.GetGalbiPath(2, false));
                paths.Add(ingredient.GetGalbiPath(3, false));
                paths.Add(ingredient.GetGalbiPath(4, false));
                paths.Add(ingredient.GetGalbiPath(5, false));
                paths.Add(ingredient.GetGalbiPath(6, false));
                paths.Add(ingredient.GetGalbiPath(7, false));

                paths.Add(ingredient.GetFailGalbiPath(false));

                paths.Add(ingredient.GetSaucePath(false));
                paths.Add(ingredient.GetPowderPath(false));
            }

            if (ingredient.type == EIngredientType.Deco)
            {
                paths.Add(string.Format("{0}/{1}_icon", ResourcePath.decoObject, ingredient.name));
            }

            List<string>.Enumerator path = paths.GetEnumerator();

            while (path.MoveNext())
            {
                string resourcePath = path.Current;

                resourceManager.LoadResource(RESOURCE_TYPE.TEXTURE, resourcePath);
            }
        }

		yield return null;
    }

    public XMLIngredient GetIngredient(EIngredientType type, int id)
    {
        if (ingredients == null)
        {
            //Initialize();
        }

        List<XMLIngredient>.Enumerator eIngredient = ingredients.GetEnumerator();

        while (eIngredient.MoveNext())
        {
            XMLIngredient ingredient = eIngredient.Current;

            if (ingredient.type.Equals(type) && ingredient.id.Equals(id))
            {
                return ingredient;
            }
        }

        throw new Exception("Invalid Galbi as ID");
    }

	public List<XMLIngredient> GetIngredientAll()
	{
		return ingredients;
	}

	public List<XMLIngredient> GetIngredients(Predicate<XMLIngredient> predicate)
	{
		return ingredients.FindAll (predicate);
	}

    public List<XMLIngredient> GetIngredients(EIngredientType _type)
    {
        return ingredients.FindAll(i => i.type == _type);
    }
}