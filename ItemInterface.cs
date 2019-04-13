using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

/// <summary>
/// DESC :> MonoBehaviour 포함
/// </summary>
public abstract class IItem : IPooling
{
    public int itemID { get; private set; }
    public string itemName { get; private set; }
    public int itemPrice { get; private set; }

    public void SetItem(int id, string name, int price)
    {
        itemID = id;
        itemName = name;
        itemPrice = price;

        Initialize();
    }

    protected virtual void Initialize()
    {

    }
}

public abstract class IShop<T> where T : IItem
{
    private List<T> items;

    public T GetItem(int id)
    {
        List<T>.Enumerator eItem = items.GetEnumerator();

        while (eItem.MoveNext())
        {
            T item = eItem.Current as T;

            if (item.itemID.Equals(id))
                return item;
        }

        throw new Exception("Invaild Item in index!");
    }

    T GetItem(string name)
    {
        List<T>.Enumerator eItem = items.GetEnumerator();

        while (eItem.MoveNext())
        {
            T item = eItem.Current as T;

            if (item.itemName.Equals(name))
                return item;
        }

        throw new Exception("Invaild Item in index!");
    }

    List<T> GetAllItems()
    {
        return items;
    }
}

public enum EIngredientType
{
    Galbi,
    Sauce,
    Powder,

    // TODO :> Deco Category
    Deco, 
}

public abstract class IIngredient : IItem
{
    public EIngredientType Type { get; protected set; }

    public void SetIngredient(EIngredientType type, int id, string name, int price)
    {
        Type = type;
        SetItem(id, name, price);
    }

    public void SetIngredient(EIngredientType type, IIngredient ingredient)
    {
        Type = type;
        SetItem(ingredient.itemID, ingredient.itemName, ingredient.itemPrice);
    }

    public void SetIngredient(XMLIngredient ingredient)
    {
        SetIngredient(ingredient.type, ingredient.id, ingredient.name, ingredient.price);
    }

    public XMLIngredient GetXmlData()
    {
        XMLIngredient xml;

        xml.type = Type;
        xml.id = itemID;
        xml.name = itemName;
        xml.price = itemPrice;

        return xml;
    }

    public string GetResourcePath()
    {
        return string.Format("{0}/{1}", ResourcePath.cookObject, itemName);
    }
}
