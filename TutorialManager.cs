using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameHelper;

public enum ETutorialState
{
    Start, 
    Process, 
    End, 
}

public abstract class Tutorial
{
    public abstract void TutorialStart();
    public abstract void TutorialProcess();
    public abstract void TutorialEnd();
    public abstract void TutoTouchStart();
    public abstract void TutoTouchEnd();
    public abstract ETutorialState GetState();
}

public class TutorialNode : Tutorial
{
    private ETutorialState state = ETutorialState.Start;

    public override void TutorialStart()
    {
        state = ETutorialState.Process;
    }

    public override void TutorialProcess()
    {
        
    }

    public override void TutorialEnd()
    {
        state = ETutorialState.End;
    }

    public override ETutorialState GetState()
    {
        return state;
    }

    public override void TutoTouchStart()
    {

    }

    public override void TutoTouchEnd()
    {

    }
}

public class TutorialScript : TutorialNode
{
    public string script = "";
    public Text text { get; set; }

    public TutorialScript(Text _text, string _script)
    {
        text = _text;
        script = _script;
    }

    public override void TutorialStart()
    {
        base.TutorialStart();

        text.text = script;
    }

    public override void TutoTouchEnd()
    {
        TutorialEnd();
    }
}

public class TutorialSkipScript : TutorialScript
{
    public TutorialSkipScript(Text _text, string _script) : base(_text, _script)
    {
        
    }

    public override void TutorialStart()
    {
        base.TutorialStart();

        TutorialEnd();
    }
}

public class TutorialSpawnNyang : TutorialNode
{
    private SpawnManager spawnManager = null;
    private int id;

    public TutorialSpawnNyang(SpawnManager spawn, int _id)
    {
        spawnManager = spawn;

        id = _id;
    }

    public override void TutorialStart()
    {
        base.TutorialStart();

        spawnManager.TutoSpawnNyang(id);

        TutorialEnd();
    }
}

public class TutorialSeatNyang : TutorialNode
{
    private InGameCookingController cookingCtrl = null;

    public TutorialSeatNyang(InGameCookingController cooking)
    {
        cookingCtrl = cooking;
    }

    public override void TutorialProcess()
    {
        base.TutorialProcess();
        
        if (cookingCtrl.SeatedNyang)
            TutorialEnd();
    }
}

public class TutorialSpawnImage : TutorialNode
{
    private ImagePoolingManager imagePooling = null;
    private Sprite sprite = null;
    private Vector2 pos;
    private Vector3 scale = Vector3.one;
    private float angle;
    private TutorialClearImage imageClear = null;
    private GameObject go = null;
    private GameObject parents = null;
    protected Image image = null;

    public TutorialSpawnImage(GameObject _parents, TutorialClearImage _imageClear, ImagePoolingManager pooling, Sprite _sprite, Vector2 _pos, float _angle = 0.0f)
    {
        parents = _parents;

        imagePooling = pooling;
        sprite = _sprite;
        pos = _pos;
        angle = _angle;
        imageClear = _imageClear;
    }

    public TutorialSpawnImage(GameObject _parents, TutorialClearImage _imageClear, ImagePoolingManager pooling, Sprite _sprite, Vector2 _pos, Vector3 _scale, float _angle = 0.0f)
    {
        parents = _parents;

        imagePooling = pooling;
        sprite = _sprite;
        pos = _pos;
        scale = _scale;
        angle = _angle;
        imageClear = _imageClear;
    }

    public TutorialSpawnImage(TutorialClearImage _imageClear, ImagePoolingManager pooling, Sprite _sprite, Vector2 _pos, float _angle = 0.0f)
    {
        imagePooling = pooling;
        sprite = _sprite;
        pos = _pos;
        angle = _angle;
        imageClear = _imageClear;
    }

    public TutorialSpawnImage(TutorialClearImage _imageClear, ImagePoolingManager pooling, Sprite _sprite, Vector2 _pos, Vector3 _scale, float _angle = 0.0f)
    {
        imagePooling = pooling;
        sprite = _sprite;
        pos = _pos;
        scale = _scale;
        angle = _angle;
        imageClear = _imageClear;
    }

    public override void TutorialStart()
    {
        base.TutorialStart();

        GameObject canvas = GameObject.Find("Canvas").FindChild("Panel");

        go = imagePooling.Get();
        image = go.GetComponent<Image>();

        if (!go.transform.parent.Equals(canvas.transform))
            go.transform.SetParent(canvas.transform);
        if (image == null)
            image = go.AddComponent<Image>();

        Vector2 screenPos = new Vector2(pos.x - 540, 960 - pos.y);
        go.transform.localPosition = screenPos;
        go.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
        go.transform.localScale = scale;

        image.sprite = sprite;
        image.color = Color.white;
        image.SetNativeSize();

        if (parents)
        {
            go.transform.SetParent(parents.transform);
        }

        TutorialEnd();
    }

    public override void TutorialEnd()
    {
        base.TutorialEnd();

        imageClear.AddImage(go);
    }
}

public class TutorialNyangStateChange : TutorialNode
{
    private GameObject parents = null;
    private ENyangState state;

    public TutorialNyangStateChange(GameObject _parents, ENyangState _state)
    {
        parents = _parents;
        state = _state;
    }

    public override void TutorialStart()
    {
        base.TutorialStart();

        Nyang nyang = parents.GetComponentInChildren<Nyang>();

        if (nyang)
        {
            nyang.TutorialSetState(state);
        }

        TutorialEnd();
    }
}

public class TutorialSpawnBlackImage : TutorialSpawnImage
{
    public TutorialSpawnBlackImage(GameObject _parents, TutorialClearImage _imageClear, ImagePoolingManager pooling, Sprite _sprite, Vector2 _pos, float _angle = 0.0f) : base(_parents, _imageClear, pooling, _sprite, _pos, _angle)
    {
    }

    public TutorialSpawnBlackImage(GameObject _parents, TutorialClearImage _imageClear, ImagePoolingManager pooling, Sprite _sprite, Vector2 _pos, Vector3 _scale, float _angle = 0.0f) : base(_parents, _imageClear, pooling, _sprite, _pos, _scale, _angle)
    {
    }

    public TutorialSpawnBlackImage(TutorialClearImage _imageClear, ImagePoolingManager pooling, Sprite _sprite, Vector2 _pos, float _angle = 0.0f) : base(_imageClear, pooling, _sprite, _pos, _angle)
    {
    }

    public TutorialSpawnBlackImage(TutorialClearImage _imageClear, ImagePoolingManager pooling, Sprite _sprite, Vector2 _pos, Vector3 _scale, float _angle = 0.0f) : base(_imageClear, pooling, _sprite, _pos, _scale, _angle)
    {
    }

    public override void TutorialStart()
    {
        base.TutorialStart();
        
        image.color = Color.black;
    }
}

public class TutorialClearImage : TutorialNode
{
    private ImagePoolingManager imagePooling = null;
    private List<GameObject> goList = new List<GameObject>();

    public TutorialClearImage(ImagePoolingManager pooling)
    {
        imagePooling = pooling;
    }

    public void AddImage(GameObject go)
    {
        goList.Add(go);
    }

    public override void TutorialStart()
    {
        base.TutorialStart();

        List<GameObject>.Enumerator eGo = goList.GetEnumerator();

        while (eGo.MoveNext())
        {
            GameObject go = eGo.Current as GameObject;
            
            if (go && go.activeSelf)
                go.SetActive(false);
        }

        TutorialEnd();
    }
}

public class TutorialWaitStateCompleted : TutorialNode
{
    private InGameCookingController cookingCtrl = null;
    private ECookingState state;

    public TutorialWaitStateCompleted(InGameCookingController cooking, ECookingState _state)
    {
        cookingCtrl = cooking;
        state = _state;
    }

    public override void TutorialProcess()
    {
        base.TutorialProcess();

        if (cookingCtrl.GetState().Equals(state))
            TutorialEnd();
    }
}

public class TutorialSetRecipe : TutorialNode
{
    private InGameCookingController cookingCtrl = null;
    private int id;

    public TutorialSetRecipe(InGameCookingController cooking, int _id)
    {
        cookingCtrl = cooking;
        id = _id;
    }

    public override void TutorialProcess()
    {
        base.TutorialProcess();

        cookingCtrl.TutorialSetRecipe(id);
        
        TutorialEnd();
    }
}

public class TutorialSetActive : TutorialNode
{
    private GameObject go = null;
    private bool isActive;

    public TutorialSetActive(GameObject _go, bool _isActive)
    {
        go = _go;
        isActive = _isActive;
    }

    public override void TutorialStart()
    {
        base.TutorialStart();
        
        go.SetActive(isActive); 

        TutorialEnd();
    }
}

public class TutorialWaitNyangStateCompleted : TutorialNode
{
    private InGameCookingController cookingCtrl = null;
    private ENyangState state;

    public TutorialWaitNyangStateCompleted(InGameCookingController cooking, ENyangState _state)
    {
        cookingCtrl = cooking;
        state = _state;
    }

    public override void TutorialProcess()
    {
        base.TutorialProcess();

        if (cookingCtrl.SeatedNyang.state.Equals(state))
            TutorialEnd();
    }
}

public class TutorialChangeScene : TutorialNode
{
    public override void TutorialStart()
    {
        base.TutorialStart();

        PlayerPrefs.SetInt("Tutorial", 1);

        SceneHelper.getInstance.ChangeScene(typeof(InGameScene));
    }
}

public class TutorialDecreaseGold : TutorialNode
{
    private int gold;

    public TutorialDecreaseGold(int _gold)
    {
        gold = _gold;
    }

    public override void TutorialStart()
    {
        base.TutorialStart();

        GlobalData.getInstance.curGold -= gold;

        TutorialEnd();
    }
}

public class TutorialWaitGameObjectActiveState : TutorialNode
{
    private GameObject go = null;
    private bool state;

    public TutorialWaitGameObjectActiveState(GameObject _go, bool _state)
    {
        go = _go;
        state = _state;
    }

    public override void TutorialProcess()
    {
        base.TutorialProcess();

        if (go.activeSelf == state)
            TutorialEnd();
    }
}

public class TutorialNyangSetActive : TutorialNode
{
    private bool isActive;

    public TutorialNyangSetActive(bool _isActive)
    {
        isActive = _isActive;
    }

    public override void TutorialStart()
    {
        base.TutorialStart();

        GameObject go = GameObjectHelper.Find("Nyang");

        if (go.transform.childCount > 0)
        {
            BoxCollider nyang = go.transform.GetChild(0).gameObject.GetComponent<BoxCollider>();

            if (nyang)
            {
                nyang.enabled = isActive;
            }
        }

        TutorialEnd();
    }
}

public class TutorialGameObjectBlockingSetActive : TutorialNode
{
    private string goName = null;
    private bool isActive;

    public TutorialGameObjectBlockingSetActive(string _goName, bool _isActive)
    {
        goName = _goName;
        isActive = _isActive;
    }

    public override void TutorialStart()
    {
        base.TutorialStart();

        GameObject go = GameObjectHelper.Find(goName);

        if (go)
        {
            BoxCollider collider = go.gameObject.GetComponent<BoxCollider>();

            if (collider)
            {
                collider.enabled = isActive;
            }
        }

        TutorialEnd();
    }
}

public class TutorialNeededIngredient : TutorialNode
{
    private EIngredientType type;
    private int id = 0;
    private int amount = 0;

    public TutorialNeededIngredient(EIngredientType _type, int _id, int _amount)
    {
        type = _type;
        id = _id;
        amount = _amount;
    }

    public override void TutorialStart()
    {
        base.TutorialStart();

        GlobalData.getInstance.GetIngredient(type, id).amount += amount;
        GlobalData.getInstance.SetIngredient(type, id);

        TutorialEnd();
    }
}

public class TutorialManager : MonoSingleton<TutorialManager>
{
    private List<TutorialNode> nodes = new List<TutorialNode>();
    private ImagePoolingManager imagePooling = null;

    public void StartTutorial(InGameCookingController cookingCtrl, InGameNyangController nyangCtrl)
    {
        GameObject customerGO = GameObjectHelper.Find("SetCostomer");
        GameObject cookingGO = GameObjectHelper.Find("Cooking");

        GameObject increaseMoney = GameObjectHelper.Find("IncreaseMoney");

        GameObject galbiUI = GameObjectHelper.Find("SelectMeatUI");
        GameObject ingredientUI = GameObjectHelper.Find("SelectSauceUI");

        GameObject allBlock = GameObjectHelper.Find("AllBlocking");
        GameObject optionBlock = GameObjectHelper.Find("OptionBlocking");
        GameObject subButtonBlock = GameObjectHelper.Find("SubButtonBlocking");
        GameObject storeBlock = GameObjectHelper.Find("StoreBlocking");
        GameObject ingredientBlock = GameObjectHelper.Find("IngredientBlocking");
        GameObject galbiBlock = GameObjectHelper.Find("GalbiBlocking");
        GameObject cookingBlock = GameObjectHelper.Find("CookingBlocking");
        GameObject choiceGalbiBlock = GameObjectHelper.Find("ChoiceGalbiBlocking");
        GameObject choiceIngredientBlock = GameObjectHelper.Find("ChoiceIngredientBlocking");

        imagePooling = gameObject.AddComponent<ImagePoolingManager>();

        GameObject talk = GameObjectHelper.Find("Image (Talk)");
        talk.SetActive(true);

        ResourcesManager resourceManager = ResourcesManager.getInstance;
        Vector2 centerPivot = new Vector2(0.5f, 0.5f);

        TutorialNeededIngredient galbiSetting = new TutorialNeededIngredient(EIngredientType.Galbi, 1, 1);
        TutorialNeededIngredient sauceSetting = new TutorialNeededIngredient(EIngredientType.Sauce, 2, 1);
        TutorialNeededIngredient powderSetting = new TutorialNeededIngredient(EIngredientType.Powder, 2, 1);

        TutorialSpawnNyang spawnNyang = new TutorialSpawnNyang(nyangCtrl.spawnManager, 1);
        TutorialNyangSetActive nyangStateFalse = new TutorialNyangSetActive(false);
        TutorialScript tuto1 = new TutorialScript(talk.FindChild("Label (Talk)").GetComponent<Text>(), "안녕? 오늘은 지각이네?\n손님냥들이 기다리겠어");

        TutorialSetActive blocking2_1 = new TutorialSetActive(allBlock, false);
        TutorialClearImage imageClear2 = new TutorialClearImage(imagePooling);
        TutorialSpawnImage spawnImage2_1 = new TutorialSpawnImage(imageClear2, imagePooling, resourceManager.CreateSprite("tuto_hand", centerPivot), new Vector2(315, 540));
        TutorialSpawnImage spawnImage2_2 = new TutorialSpawnImage(imageClear2, imagePooling, resourceManager.CreateSprite("tuto_arrow", centerPivot), new Vector2(390, 540));
        TutorialSpawnImage spawnImage2_3 = new TutorialSpawnImage(imageClear2, imagePooling, resourceManager.CreateSprite("tuto_shadow", centerPivot), new Vector2(470, 540));
        TutorialSpawnImage spawnImage2_4 = new TutorialSpawnImage(imageClear2, imagePooling, resourceManager.CreateSprite("tuto_arrow", centerPivot), new Vector2(800, 500), -90);
        TutorialSpawnBlackImage spawnImage2_5 = new TutorialSpawnBlackImage(customerGO, imageClear2, imagePooling, resourceManager.CreateSprite(string.Format("{0}_order", NyangManager.getInstance.GetNyang(1).name, centerPivot), centerPivot), new Vector2(540, 1226), new Vector3(2.0f, 2.0f, 2.0f));
        TutorialNyangSetActive nyangStateTrue = new TutorialNyangSetActive(true);
        TutorialSkipScript tuto2 = new TutorialSkipScript(talk.FindChild("Label (Talk)").GetComponent<Text>(), "\n\n손냥이를 이동시켜\n손님석에 앉혀봐");
        TutorialSeatNyang seatedNyang = new TutorialSeatNyang(cookingCtrl);
        
        TutorialClearImage imageClear3 = new TutorialClearImage(imagePooling);
        TutorialSpawnImage spawnImage3_1 = new TutorialSpawnImage(imageClear3, imagePooling, resourceManager.CreateSprite("tuto_arrow", centerPivot), new Vector2(540, 810), -90);
        TutorialSetRecipe setRecipe = new TutorialSetRecipe(cookingCtrl, 5);
        TutorialScript tuto3 = new TutorialScript(talk.FindChild("Label (Talk)").GetComponent<Text>(), "지붕위의 레시피를 잘봐");

        TutorialSetActive blocking4_1 = new TutorialSetActive(galbiBlock, false);
        TutorialClearImage imageClear4 = new TutorialClearImage(imagePooling);
        TutorialSpawnImage spawnImage4_1 = new TutorialSpawnImage(imageClear4, imagePooling, resourceManager.CreateSprite("tuto_icon_1", centerPivot), new Vector2(270, 540));
        TutorialSpawnImage spawnImage4_2 = new TutorialSpawnImage(imageClear4, imagePooling, resourceManager.CreateSprite("tuto_arrow", centerPivot), new Vector2(368, 540));
        TutorialSpawnImage spawnImage4_3 = new TutorialSpawnImage(imageClear4, imagePooling, resourceManager.CreateSprite("obj_food_1", centerPivot), new Vector2(490, 540));
        TutorialSpawnImage spawnImage4_4 = new TutorialSpawnImage(imageClear4, imagePooling, resourceManager.CreateSprite("tuto_arrow", centerPivot), new Vector2(960, 1720), -90);
        TutorialSkipScript tuto4 = new TutorialSkipScript(talk.FindChild("Label (Talk)").GetComponent<Text>(), "\n\n먼저 분홍색 박스에서\n갈비를 꺼내보자");
        TutorialWaitGameObjectActiveState galbiButtonWait = new TutorialWaitGameObjectActiveState(galbiUI, true);
        TutorialSetActive blocking4_2 = new TutorialSetActive(choiceGalbiBlock, true);
        TutorialSetActive blocking4_3 = new TutorialSetActive(ingredientBlock, false);
        TutorialWaitStateCompleted waitState4 = new TutorialWaitStateCompleted(cookingCtrl, ECookingState.SWIPING);
        
        TutorialSetActive blocking5_1 = new TutorialSetActive(choiceGalbiBlock, false);
        TutorialSetActive blocking5_2 = new TutorialSetActive(galbiBlock, true);
        TutorialSetActive blocking5_3 = new TutorialSetActive(ingredientBlock, true);
        TutorialClearImage imageClear5 = new TutorialClearImage(imagePooling);
        TutorialSpawnImage spawnImage5_2 = new TutorialSpawnImage(imageClear5, imagePooling, resourceManager.CreateSprite("obj_food_1", centerPivot), new Vector2(445, 540));
        TutorialSpawnImage spawnImage5_1 = new TutorialSpawnImage(imageClear5, imagePooling, resourceManager.CreateSprite("tuto_arrow", centerPivot), new Vector2(320, 540), -90);
        TutorialSpawnImage spawnImage5_3 = new TutorialSpawnImage(imageClear5, imagePooling, resourceManager.CreateSprite("tuto_hand", centerPivot), new Vector2(475, 570));
        TutorialSpawnImage spawnImage5_4 = new TutorialSpawnImage(imageClear5, imagePooling, resourceManager.CreateSprite("tuto_arrow", centerPivot), new Vector2(558, 1400), -90);
        TutorialSpawnImage spawnImage5_5 = new TutorialSpawnImage(imageClear5, imagePooling, resourceManager.CreateSprite("tuto_hand", centerPivot), new Vector2(578, 1460));
        TutorialSkipScript tuto5 = new TutorialSkipScript(talk.FindChild("Label (Talk)").GetComponent<Text>(), "\n\n\n갈비가 잘 익도록 구워봐\n타지 않도록 얼른~");
        TutorialWaitStateCompleted waitState5 = new TutorialWaitStateCompleted(cookingCtrl, ECookingState.SWIPING_END);

        TutorialGameObjectBlockingSetActive cookingSetFalse = new TutorialGameObjectBlockingSetActive("Cook Galbi", false);
        TutorialSetActive blocking6_1 = new TutorialSetActive(cookingBlock, true);
        TutorialSetActive blocking6_2 = new TutorialSetActive(ingredientBlock, false);
        TutorialClearImage imageClear6 = new TutorialClearImage(imagePooling);
        TutorialSpawnImage spawnImage6_1 = new TutorialSpawnImage(imageClear6, imagePooling, resourceManager.CreateSprite("tuto_icon_2", centerPivot), new Vector2(270, 540));
        TutorialSpawnImage spawnImage6_2 = new TutorialSpawnImage(imageClear6, imagePooling, resourceManager.CreateSprite("tuto_arrow", centerPivot), new Vector2(372, 540));
        TutorialSpawnImage spawnImage6_3 = new TutorialSpawnImage(imageClear6, imagePooling, resourceManager.CreateSprite("obj_coca_2", centerPivot), new Vector2(453, 540));
        TutorialSpawnImage spawnImage6_4 = new TutorialSpawnImage(imageClear6, imagePooling, resourceManager.CreateSprite("obj_lsd_2", centerPivot), new Vector2(543, 540));
        TutorialSkipScript tuto6 = new TutorialSkipScript(talk.FindChild("Label (Talk)").GetComponent<Text>(), "\n\n\n다음은 노란색 박스에 있는\n조미료를 뿌려보자");
        TutorialWaitGameObjectActiveState ingredientButtonWait = new TutorialWaitGameObjectActiveState(ingredientUI, true);
        TutorialSetActive blocking6_3 = new TutorialSetActive(choiceIngredientBlock, true);
        TutorialSpawnImage spawnImage6_5 = new TutorialSpawnImage(imageClear6, imagePooling, resourceManager.CreateSprite("tuto_arrow", centerPivot), new Vector2(375, 1640), -90);
        TutorialSpawnImage spawnImage6_6 = new TutorialSpawnImage(imageClear6, imagePooling, resourceManager.CreateSprite("tuto_arrow", centerPivot), new Vector2(705, 1640), -90);
        TutorialWaitStateCompleted waitState6 = new TutorialWaitStateCompleted(cookingCtrl, ECookingState.COMPLETE);
        
        TutorialGameObjectBlockingSetActive cookingSetTrue = new TutorialGameObjectBlockingSetActive("Cook Galbi", true);
        TutorialSetActive blocking7_1 = new TutorialSetActive(choiceIngredientBlock, false);
        TutorialSetActive blocking7_2 = new TutorialSetActive(ingredientBlock, true);
        TutorialSetActive blocking7_3 = new TutorialSetActive(cookingBlock, false);
        TutorialClearImage imageClear7 = new TutorialClearImage(imagePooling);
        TutorialSpawnImage spawnImage7_1 = new TutorialSpawnImage(imageClear7, imagePooling, resourceManager.CreateSprite("tuto_arrow", centerPivot), new Vector2(558, 1350), -90);
        TutorialSkipScript tuto7 = new TutorialSkipScript(talk.FindChild("Label (Talk)").GetComponent<Text>(), "갈비가 완성됐어\n이제 손냥이에게 갈비를 줘");
        TutorialWaitNyangStateCompleted waitState7 = new TutorialWaitNyangStateCompleted(cookingCtrl, ENyangState.happy);
        TutorialDecreaseGold decreaseGold = new TutorialDecreaseGold(300);

        TutorialSetActive blocking8_1 = new TutorialSetActive(allBlock, true);
        TutorialClearImage imageClear8 = new TutorialClearImage(imagePooling);
        TutorialSpawnImage spawnImage8_1 = new TutorialSpawnImage(imageClear8, imagePooling, resourceManager.CreateSprite("tuto_arrow", centerPivot), new Vector2(980, 150), 90);
        TutorialScript tuto8 = new TutorialScript(talk.FindChild("Label (Talk)").GetComponent<Text>(), "잘했어! 이제 혼자 할 수 있겠지?\n돈은 내가 받고 있으니 걱정마");

        TutorialClearImage imageClear9 = new TutorialClearImage(imagePooling);
        TutorialSpawnImage spawnImage9_1 = new TutorialSpawnImage(imageClear9, imagePooling, resourceManager.CreateSprite("tuto_icon_3", centerPivot), new Vector2(390, 540));
        TutorialScript tuto9 = new TutorialScript(talk.FindChild("Label (Talk)").GetComponent<Text>(), "\n\n\n갈비를 너무 오래구우면\n타버린다는거 명심해");

        TutorialSetActive increaseMoneySetActive = new TutorialSetActive(increaseMoney, false);
        TutorialClearImage imageClear10 = new TutorialClearImage(imagePooling);
        TutorialSetActive setActive10 = new TutorialSetActive(cookingGO, false);
        TutorialNyangStateChange nyangStateChange = new TutorialNyangStateChange(customerGO, ENyangState.angry);
        TutorialScript tuto10 = new TutorialScript(talk.FindChild("Label (Talk)").GetComponent<Text>(), "탄갈비를 주거나 레시피대로\n굽지않으면 손냥이가 화낼거야");

        TutorialSetActive setActive11 = new TutorialSetActive(customerGO, false);
        TutorialScript tuto11 = new TutorialScript(talk.FindChild("Label (Talk)").GetComponent<Text>(), "그럼 우리 같이 졸부가 되자!");

        TutorialChangeScene tuto12 = new TutorialChangeScene();
        
        nodes.Add(galbiSetting);
        nodes.Add(sauceSetting);
        nodes.Add(powderSetting);

        nodes.Add(spawnNyang);
        nodes.Add(nyangStateFalse);
        nodes.Add(tuto1);

        nodes.Add(blocking2_1);
        nodes.Add(spawnImage2_1);
        nodes.Add(spawnImage2_2);
        nodes.Add(spawnImage2_3);
        nodes.Add(spawnImage2_4);
        nodes.Add(spawnImage2_5);
        nodes.Add(nyangStateTrue);
        nodes.Add(tuto2);
        nodes.Add(seatedNyang);
        nodes.Add(imageClear2);

        nodes.Add(spawnImage3_1);
        nodes.Add(setRecipe);
        nodes.Add(tuto3);
        nodes.Add(imageClear3);

        nodes.Add(blocking4_1);
        nodes.Add(spawnImage4_1);
        nodes.Add(spawnImage4_2);
        nodes.Add(spawnImage4_3);
        nodes.Add(spawnImage4_4);
        nodes.Add(tuto4);
        nodes.Add(galbiButtonWait);
        nodes.Add(blocking4_2);
        nodes.Add(blocking4_3);
        nodes.Add(waitState4);
        nodes.Add(imageClear4);

        nodes.Add(blocking5_1);
        nodes.Add(blocking5_2);
        nodes.Add(blocking5_3);
        nodes.Add(spawnImage5_1);
        nodes.Add(spawnImage5_2);
        nodes.Add(spawnImage5_3);
        nodes.Add(spawnImage5_4);
        nodes.Add(spawnImage5_5);
        nodes.Add(tuto5);
        nodes.Add(waitState5);
        nodes.Add(imageClear5);

        nodes.Add(cookingSetFalse);
        nodes.Add(blocking6_1);
        nodes.Add(blocking6_2);
        nodes.Add(spawnImage6_1);
        nodes.Add(spawnImage6_2);
        nodes.Add(spawnImage6_3);
        nodes.Add(spawnImage6_4);
        nodes.Add(tuto6);
        nodes.Add(ingredientButtonWait);
        nodes.Add(blocking6_3);
        nodes.Add(spawnImage6_5);
        nodes.Add(spawnImage6_6);
        nodes.Add(waitState6);
        nodes.Add(imageClear6);

        nodes.Add(cookingSetTrue);
        nodes.Add(blocking7_1);
        nodes.Add(blocking7_2);
        nodes.Add(blocking7_3);
        nodes.Add(spawnImage7_1);
        nodes.Add(tuto7);
        nodes.Add(waitState7);
        nodes.Add(imageClear7);
        nodes.Add(decreaseGold);

        nodes.Add(blocking8_1);
        nodes.Add(spawnImage8_1);
        nodes.Add(tuto8);
        nodes.Add(imageClear8);

        nodes.Add(spawnImage9_1);
        nodes.Add(tuto9);
        nodes.Add(imageClear9);
        
        nodes.Add(increaseMoneySetActive);
        nodes.Add(nyangStateChange);
        nodes.Add(setActive10);
        nodes.Add(tuto10);
        nodes.Add(imageClear10);

        nodes.Add(setActive11);
        nodes.Add(tuto11);

        nodes.Add(tuto12);
    }

    public void Update()
    {
        if (nodes.Count <= 0)
            return;
        
        if (nodes[0].GetState() == ETutorialState.Start)
        {
            nodes[0].TutorialStart();
        }
        if (nodes[0].GetState() == ETutorialState.Process)
        {
            nodes[0].TutorialProcess();
        }
        if (nodes[0].GetState() == ETutorialState.End)
        {
            nodes.RemoveAt(0);
        }
    }

    public void TouchStart(Vector2 _touchPos)
    {
        if (nodes.Count <= 0)
            return;

        nodes[0].TutoTouchStart();
    }

    public void TouchEnd(Vector2 _touchPos)
    {
        if (nodes.Count <= 0)
            return;

        nodes[0].TutoTouchEnd();
    }
}
