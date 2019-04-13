using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Recipe : IPooling
{
    public int id { get; private set; }
    public int galbi { get; private set; }
    public int sauce { get; private set; }
    public int powder { get; private set; }

    private GameObject galbiObj = null;
    private GameObject sauceObj = null;
    private GameObject powderObj = null;

    private GameObject plus1 = null;
    private GameObject plus2 = null;

    public bool Equals(Galbi _galbi, Sauce _sauce, Powder _powder)
    {
        return galbi == _galbi.itemID && sauce == _sauce.itemID && powder == _powder.itemID;
    }

    public void SetRecipe(int id, int galbi, int sauce, int powder)
    {
        this.id = id;
        this.galbi = galbi;
        this.sauce = sauce;
        this.powder = powder;

        if (galbiObj == null)
        {
            galbiObj = new GameObject();
            galbiObj.name = "Galbi";
            galbiObj.transform.SetParent(transform);
            galbiObj.AddComponent<Galbi>();
            galbiObj.AddComponent<Image>();

            galbiObj.transform.localPosition = new Vector3(-135.0f, 55.0f);
            galbiObj.transform.localScale = Vector3.one;
        }
        if (plus1 == null)
        {
            plus1 = new GameObject();
            plus1.name = "Plus1";
            plus1.transform.SetParent(transform);
            Text plusText1 = plus1.AddComponent<Text>();
            plusText1.font = Resources.Load(ResourcePath.font, typeof(Font)) as Font;
            plusText1.text = "+";
            plusText1.fontSize = 32;
            plusText1.alignment = TextAnchor.MiddleCenter;
            plusText1.color = Color.black;

            plus1.transform.localPosition = new Vector3(-25.0f, 55.0f);
            plus1.transform.localScale = Vector3.one;
        }
        if (powderObj == null)
        {
            powderObj = new GameObject();
            powderObj.name = "Powder";
            powderObj.transform.SetParent(transform);
            powderObj.AddComponent<Powder>();
            powderObj.AddComponent<Image>();

            powderObj.transform.localPosition = new Vector3(45.0f, 55.0f);
            powderObj.transform.localScale = Vector3.one;
        }
        if (plus2 == null)
        {
            plus2 = new GameObject();
            plus2.name = "Plus2";
            plus2.transform.SetParent(transform);
            Text plusText2 = plus2.AddComponent<Text>();
            plusText2.font = Resources.Load(ResourcePath.font, typeof(Font)) as Font;
            
            plusText2.text = "+";
            plusText2.fontSize = 32;
            plusText2.alignment = TextAnchor.MiddleCenter;
            plusText2.color = Color.black;

            plus2.transform.localPosition = new Vector3(120.0f, 55.0f);
            plus2.transform.localScale = Vector3.one;
        }
        if (sauceObj == null)
        {
            sauceObj = new GameObject();
            sauceObj.name = "Sauce";
            sauceObj.transform.SetParent(transform);
            sauceObj.AddComponent<Sauce>();
            sauceObj.AddComponent<Image>();

            sauceObj.transform.localPosition = new Vector3(195.0f, 55.0f);
            sauceObj.transform.localScale = Vector3.one;
        }

        ResourcesManager resourcesManager = ResourcesManager.getInstance;

        // DESC :> Create Galbi Image
        Galbi galbiComp = galbiObj.GetComponent<Galbi>();
        galbiComp.SetIngredient(IngredientManager.getInstance.GetIngredient(EIngredientType.Galbi, galbi));

        Image galbiImage = galbiObj.GetComponent<Image>();
        if (!resourcesManager.IsExistKey(RESOURCE_TYPE.TEXTURE, galbiComp.itemName))
            resourcesManager.LoadResource(RESOURCE_TYPE.TEXTURE, galbiComp.GetResourcePath());

		Sprite galbiSprite = resourcesManager.CreateSprite (galbiComp.itemName, new Vector2 (0.5f, 0.5f));
		galbiImage.sprite = galbiSprite;
        galbiImage.SetNativeSize();

        RectTransform galbiRectTransform = galbiObj.GetComponent<RectTransform> ();
		galbiRectTransform.sizeDelta = galbiSprite.bounds.size;

        // DESC :> Create Ingredient Image
        Powder powderComp = powderObj.GetComponent<Powder>();
        powderComp.SetIngredient(IngredientManager.getInstance.GetIngredient(EIngredientType.Powder, powder));

        Image powderImage = powderObj.GetComponent<Image>();
		Sprite powderSprite = resourcesManager.CreateSprite (powderComp.itemName, new Vector2 (0.5f, 0.5f));
		powderImage.sprite = powderSprite;
		RectTransform powderRectTransform = powderObj.GetComponent<RectTransform> ();
		powderRectTransform.sizeDelta = powderSprite.bounds.size;

        // DESC :> Create Sauce Image
        Sauce sauceComp = sauceObj.GetComponent<Sauce>();
        sauceComp.SetIngredient(IngredientManager.getInstance.GetIngredient(EIngredientType.Sauce, sauce));

        Image sauceImage = sauceObj.GetComponent<Image>();
		Sprite sauceSprite = resourcesManager.CreateSprite (sauceComp.itemName, new Vector2 (0.5f, 0.5f));
		sauceImage.sprite = sauceSprite;
		RectTransform sauceRectTransform = sauceObj.GetComponent<RectTransform> ();
		sauceRectTransform.sizeDelta = sauceSprite.bounds.size;
    }

    public void SetRecipe(XMLRecipe recipe)
    {
        SetRecipe(recipe.id, recipe.galbi, recipe.sauce, recipe.powder);
    }
}