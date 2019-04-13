using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourcePath
{
	#if UNITY_EDITOR
    public static readonly string root = Application.dataPath;
    public static readonly string resources = string.Format("{0}/Resources", root);
    public static readonly string xml = string.Format("{0}/Xml", resources);
#else
	public static readonly string root = Application.persistentDataPath;
	public static readonly string resources = string.Format("{0}/Resources", root);
    public static readonly string xml = root;//string.Format ("{0}/Xml", resources);
#endif


    public static readonly string texture = "Texture";
    public static readonly string normalCharacter = string.Format("{0}/NormalCharacter", texture);
    public static readonly string rareCharacter = string.Format("{0}/RareCharacter", texture);
    public static readonly string hiddenCharacter = string.Format("{0}/HiddenCharacter", texture);
    public static readonly string cookObject = string.Format("{0}/Object", texture);
    public static readonly string decoObject = string.Format("{0}/Deco", texture);
    public static readonly string story = string.Format("{0}/Story", texture);

    public static readonly string font = "Font/Bameulihaebyun";

    public static readonly string xmlNyang = "Nyang.xml";
    public static readonly string xmlNyangInfo = "NyangInfo.xml";
    public static readonly string xmlIngredient = "Ingredient.xml";
    public static readonly string xmlRecipe = "Recipe.xml";
    public static readonly string xmlStory = "NyangStory.xml";
    public static readonly string xmlDeco = "Deco.xml";
}
