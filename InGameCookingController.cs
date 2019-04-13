using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameHelper;

public enum ECookingState
{
    WATING,
    SHOW_RECIPE,
    SWIPING,
    SWIPING_END,
    COMPLETE,
    FAIL,
}

public class InGameCookingController : MonoBehaviour
{
    private struct FParentsGameObject
    {
        public GameObject cooking { get; set; }
        public GameObject recipe { get; set; }
        public GameObject sauce { get; set; }
        public GameObject powder { get; set; }
        public GameObject galbi { get; set; }
    }
    private struct FIngredient
    {
        public Recipe recipe { get; set; }
        public Nyang nyang { get; set; }
        public Galbi galbi { get; set; }
        public Sauce sauce { get; set; }
        public Powder powder { get; set; }
    }
    private struct FImage
    {
        public Image galbi { get; set; }
        public Image sauce { get; set; }
        public Image powder { get; set; }
    }

    private readonly Color[] sauceColors = new Color[3] {
        //new Color(255.0f / 255.0f, 192.0f / 255.0f, 203.0f / 255.0f),
        new Color(249.0f / 255.0f, 138.0f / 255.0f, 183.0f / 255.0f),
        //new Color(0.0f / 255.0f, 124.0f / 255.0f, 204.0f / 255.0f),
        new Color(91.0f / 255.0f, 216.0f / 255.0f, 213.0f / 255.0f),
        //new Color(1.0f, 165.0f / 255.0f, 0.0f)
        new Color(255.0f, 158.0f / 255.0f, 62.0f / 255.0f)
    };
    private readonly Vector2[] saucePos = new Vector2[4] { new Vector2(-5, 55), new Vector2(5, 45), new Vector2(-10, 45), new Vector2(-5, 55) };
    private readonly Color[] powderReadyColors = new Color[4] {
        new Color(44.0f / 255.0f, 42.0f / 255.0f, 43.0f / 255.0f),
        new Color(176.0f / 255.0f, 182.0f / 255.0f, 187.0f / 255.0f), 
        new Color(214.0f / 255.0f, 166.0f / 255.0f, 204.0f / 255.0f), 
        new Color(214.0f / 255.0f, 166.0f / 255.0f, 204.0f / 255.0f)
    };
    private readonly Color[] powderColors = new Color[3] { Color.gray, Color.yellow, Color.green };
    private readonly Vector2[] powderPos = new Vector2[4] { Vector2.zero, new Vector2(10, 0), new Vector2(-10, 5), Vector2.zero };

    private ECookingState cookState = ECookingState.WATING;
    
    private BoxCollider galbiBox = null;
    private BoxCollider seatBox = null;
    private RectTransform galbiRectTrasnform = null;

    private GameObject angryGauge = null;
    private RectTransform angryGaugeImg = null;
    private float curGauge = 0.0f;
    private float maxGauge = 180.0f;

    private IngredientStorePopup storePopup = null;

    private ResourcesManager resourcesManager = null;

    private AudioClip clip = null;

    private FParentsGameObject goParents;
    private FIngredient ingredients;
    private FImage images;

    public GameObject galbiParents { get { return goParents.galbi; } }
    public Nyang SeatedNyang { get { return ingredients.nyang; } }

    private GameObject increaseMoney = null;

    private float cookingTime = 10;

    private void Awake()
    {
        clip = Resources.Load("Sound/FX/grill", typeof(AudioClip)) as AudioClip;
    }

    public void TutorialInit()
    {
        seatBox = GameObject.Find("SeatedRect").GetComponent<BoxCollider>();
        GameObject panelParents = GameObject.Find("Canvas").FindChild("Panel");

        goParents.cooking = new GameObject();
        goParents.cooking.name = "Cooking";
        panelParents.AttachChild(goParents.cooking);

        resourcesManager = ResourcesManager.getInstance;

        RecipeInitialize();
    }

    public void TutorialSetRecipe(int id)
    {
        ingredients.recipe.SetRecipe(RecipeManager.getInstance.GetRecipe(id));
    }

    public void InitCooking()
    {
        storePopup = GameObjectHelper.Find("IngredientStorePopup").GetComponent<IngredientStorePopup>();
        angryGauge = GameObjectHelper.Find("AngryGauge");
        angryGaugeImg = angryGauge.FindChild("Image (AngryGauge)").GetComponent<RectTransform>();

        increaseMoney = GameObjectHelper.Find("IncreaseMoney");

        seatBox = GameObject.Find("SeatedRect").GetComponent<BoxCollider>();
        GameObject panelParents = GameObject.Find("Canvas").FindChild("Panel");

        goParents.cooking= new GameObject();
        goParents.cooking.name = "Cooking";
        panelParents.AttachChild(goParents.cooking);

        resourcesManager = ResourcesManager.getInstance;

        RecipeInitialize();
    }

    private void RecipeInitialize()
    {
        goParents.recipe = new GameObject();
        goParents.recipe.name = "Recipe Object";
        ingredients.recipe = goParents.recipe.AddComponent<Recipe>();
        goParents.cooking.AttachChild(goParents.recipe);
        goParents.recipe.transform.localPosition = Vector3.zero;
        goParents.recipe.transform.localScale = Vector3.one;

        goParents.galbi = new GameObject();
        goParents.galbi.name = "Cook Galbi";
        goParents.galbi.transform.localPosition = new Vector2(16.0f, -455.0f);
        goParents.galbi.transform.localScale = Vector3.one;

        ingredients.galbi = goParents.galbi.AddComponent<Galbi>();
        ingredients.galbi.SetIngredient(EIngredientType.Galbi, ingredients.galbi);
        images.galbi = goParents.galbi.AddComponent<Image>();
        galbiBox = goParents.galbi.AddComponent<BoxCollider>();
        goParents.recipe.AttachChild(goParents.galbi);
        galbiRectTrasnform = goParents.galbi.GetComponent<RectTransform>();

        goParents.sauce = new GameObject();
        goParents.sauce.name = "Cook Sauce";
        ingredients.sauce = goParents.sauce.AddComponent<Sauce>();
        ingredients.sauce.SetIngredient(EIngredientType.Sauce, ingredients.sauce);
        images.sauce = goParents.sauce.AddComponent<Image>();
        images.sauce.color = Color.black;
        goParents.galbi.AttachChild(goParents.sauce);
        goParents.sauce.transform.localPosition = new Vector2(-5.0f, 50.0f);
        goParents.sauce.transform.localScale = Vector3.one;

        goParents.powder = new GameObject();
        goParents.powder.name = "Cook Powder";
        ingredients.powder = goParents.powder.AddComponent<Powder>();
        ingredients.powder.SetIngredient(EIngredientType.Powder, ingredients.powder);
        images.powder = goParents.powder.AddComponent<Image>();
        goParents.galbi.AttachChild(goParents.powder);
        goParents.powder.transform.localScale = Vector3.one;

        ingredients.galbi.SetGameObject(goParents.galbi);

        goParents.galbi.SetActive(false);
        goParents.sauce.SetActive(false);
        goParents.powder.SetActive(false);

        cookingTime = 10;
    }

    public void CookTimeUpdate()
    {
        if (ECookingState.SHOW_RECIPE < cookState && cookState < ECookingState.FAIL)
        {
            if (cookingTime <= 0)
            {
                images.galbi.sprite = ResourcesManager.getInstance.CreateSprite(ingredients.galbi.GetFailGalbiPath(), new Vector2(0.5f, 0.5f));

                cookingTime = 12;

                //cookState = ECookingState.FAIL;
                FailGalbi();

                goParents.powder.SetActive(false);
                goParents.sauce.SetActive(false);

                return;
            }

            cookingTime -= Time.deltaTime;
        }
    }

    /* DESC :>
     * 요리하는 과정 초기화
     * 갈비 판매 이후, 냥이를 앉히는 과정부터 시작
     */ 
    public void Reset()
    {
        ResetGalbi();

        goParents.recipe.SetActive(false);

        ingredients.nyang = null;
        cookState = ECookingState.WATING;
    }
    /* DESC :>
     * 갈비만 초기화
     * 요리하는 과정 및 쓰레기통으로 버릴 때
     */
    private void ResetGalbi()
    {
        ingredients.galbi.SetItem(0, "", 0);
        ingredients.galbi.Reset();

        images.galbi.sprite = null;
        images.galbi.color = Color.white;

        goParents.galbi.SetActive(false);

        ingredients.sauce.SetItem(0, "", 0);

        images.sauce.sprite = null;
        images.sauce.color = Color.black;

        ingredients.powder.SetItem(0, "", 0);

        images.powder.sprite = null;
        images.powder.color = Color.white;

        cookingTime = 10;
    }

    /* DESC :>
     * 쓰레기통에 갈비를 버림
     * 갈비를 올리는 과정부터 재시작
     */
    public void GarbageGalbi()
    {
        ResetGalbi();

        cookState = ECookingState.SHOW_RECIPE;
    }

    /* DESC :>
     * 갈비를 스토브에 올림
     */
    public void SetGalbi(int id)
    {
        InGameInputController.getInstance.AddTouchableObject(ingredients.galbi);

        goParents.galbi.SetActive(true);
        goParents.sauce.SetActive(false);
        goParents.powder.SetActive(false);

        ingredients.galbi.SetIngredient(IngredientManager.getInstance.GetIngredient(EIngredientType.Galbi, id));

        images.galbi.sprite = resourcesManager.CreateSprite(ingredients.galbi.GetGalbiPath(1, true), new Vector2(0.5f, 0.5f));
        images.galbi.SetNativeSize();

        images.sauce.sprite = resourcesManager.CreateSprite(ingredients.galbi.GetSaucePath(true), new Vector2(0.5f, 0.5f));
        images.sauce.SetNativeSize();

        images.powder.sprite = resourcesManager.CreateSprite(ingredients.galbi.GetPowderPath(true), new Vector2(0.5f, 0.5f));
        images.powder.SetNativeSize();

        Rect rect = galbiRectTrasnform.rect;
        galbiBox.size = new Vector3(rect.width, rect.height, 1.0f);
    }

    /* DESC :>
     * 사용한 재료 감소
     */
    public void SpreadIngredient(IIngredient ingredient, int id)
    {
        if (ingredient.itemID != 0)
            return;

        ingredient.SetItem(id, ingredient.itemName, ingredient.itemPrice);

        GlobalData.getInstance.GetIngredient(ingredient.Type, id).amount--;
        GlobalData.getInstance.SetIngredient(ingredient.Type, id);
    }

    /* DESC :>
     * 갈비, 소스, 파우더 사용
     * 갈비가 존재하지 않고, 냥이가 있을때 갈비 올림
     * 갈비가 존재하고, 소스 및 파우더가 사용되지 않았을 때 소스 및 파우더 사용
     */
    public void UseIngredient(EIngredientType type, int id)
    {
        if (GlobalData.getInstance.GetIngredientAmount(type, id) <= 0)
        {
            PopupManager.getInstance.Show(storePopup);
        }

        switch(cookState)
        {
            case ECookingState.SHOW_RECIPE:
                {
                    if (type == EIngredientType.Galbi)
                    {
                        SetGalbi(id);

                        cookState = ECookingState.SWIPING;

                        GlobalData.getInstance.GetIngredient(type, id).amount--;
                        GlobalData.getInstance.SetIngredient(type, id);

                        // DESC :> Powder 갈비에 따른 준비 컬러값 및 좌표
                        goParents.sauce.transform.localPosition = saucePos[id - 1];
                        goParents.powder.transform.localPosition = powderPos[id - 1];
                        images.powder.color = powderReadyColors[id - 1];
                    }

                    break;
                }
		case ECookingState.SWIPING_END:
                {
                    if (type == EIngredientType.Sauce)
                    {
                        if (ingredients.sauce.itemID < 1)
                        {
                            SpreadIngredient(ingredients.sauce, id);
                            images.sauce.color = sauceColors[id - 1];
                        }
                    }
                    if (type == EIngredientType.Powder)
                    {
                        if (ingredients.powder.itemID < 1)
                        {
                            goParents.powder.SetActive(true);
                            SpreadIngredient(ingredients.powder, id);
                            images.powder.color = powderColors[id - 1];
                        }
                    }
                    if (ingredients.recipe.Equals(ingredients.galbi, ingredients.sauce, ingredients.powder))
                    {
                        ingredients.galbi.CookComplete();
                        cookState = ECookingState.COMPLETE;
                    }

                    break;
                }
        default:
            break;
        }
    }

    /* DESC :>
     * 냥이를 주문석에 앉힘
     */
    public void EnteredNyang(Nyang nyang)
    {
        goParents.recipe.SetActive(true);

        RecipeManager recipeManager = RecipeManager.getInstance;

        // ingredients.recipe.SetRecipe(recipeManager.GetRecipe(Random.Range(1, recipeManager.Size - 1)));
        int limGalbi = 5;
        int limSauce = 4;
        int limPowder = 4;

        if (GlobalData.getInstance.curDay >= 15)
        {
            limGalbi = IngredientManager.getInstance.GetIngredients(EIngredientType.Galbi).Count;
            limSauce = IngredientManager.getInstance.GetIngredients(EIngredientType.Sauce).Count;
            limPowder = IngredientManager.getInstance.GetIngredients(EIngredientType.Powder).Count;
        }

        // ingredients.recipe.SetRecipe(recipeManager.GetRecipe(Random.Range(1, recipeManager.Size - 1)));
        int idxGalbi = Random.Range(1, limGalbi);
        int idxSauce = Random.Range(1, limSauce);
        int idxPowder = Random.Range(1, limPowder);

        ingredients.recipe.SetRecipe(recipeManager.GetRecipe(idxGalbi, idxSauce, idxPowder));

        ingredients.nyang = nyang;

        cookState = ECookingState.SHOW_RECIPE;

        seatBox.size = new Vector2(600.0f, 800.0f);

        if (angryGauge)
        {
            angryGauge.SetActive(true);
            StartCoroutine("AngryGauge");
        }
    }

    private IEnumerator AngryGauge()
    {
        while (!GlobalData.getInstance.isPause)
        {
            if (!ingredients.nyang)
                break;

            float curNyangWaitTime = ((ingredients.nyang.waitingTime > 30.0f) ? 30.0f : ingredients.nyang.waitingTime);

            angryGaugeImg.sizeDelta = new Vector2(angryGaugeImg.sizeDelta.x, curNyangWaitTime * 6);

            yield return null;
        }

        angryGauge.SetActive(false);

        yield return null;
    }

    /* DESC :>
     * 갈비를 스와이프 할 때마다, 구워짐
     */
    public void GalbiBaking(int count)
    {
        if (cookState != ECookingState.SWIPING)
            return;

        if (count <= 7)
        {
            Client.AudioManager.Play((int)FX_SOUND_TYPE.GRILL);
            images.galbi.sprite = resourcesManager.CreateSprite(ingredients.galbi.GetGalbiPath(count, true), new Vector2(0.5f, 0.5f));
        }

        if (count == 7)
        {
            goParents.sauce.SetActive(true);
            goParents.powder.SetActive(true);
            cookState = ECookingState.SWIPING_END;
        }
    }

    public void ArrivedNyang()
    {
        increaseMoney.SetActive(false);
        seatBox.size = new Vector2(600.0f, 600.0f);
        Reset();
    }

    public void FailGalbi()
    {
        cookState = ECookingState.FAIL;
    }

    public ECookingState GetState()
    {
        return cookState;
    }

    ///////

    bool bGalbiSwipe = false;
    public void OnScreenTouchBegan(Vector2 _touchPos)
    {
        BoxCollider seatBox = GameObject.Find("SeatedRect").GetComponent<BoxCollider>();
        seatBox.enabled = true;
        Ray ray = GameObject.Find("Default Camera").GetComponent<Camera>().ScreenPointToRay(_touchPos);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == seatBox)
                bGalbiSwipe = true;
        }
        seatBox.enabled = false;
    }
    
    public void OnScreenSwipe(Vector2 _touchPos, float _angle)
    {
        if (!bGalbiSwipe)
            return;

        bGalbiSwipe = false;

        if (_angle < 15 || _angle > 165)
            return;

        //BoxCollider seatBox = GameObject.Find("SeatedRect").GetComponent<BoxCollider>();
        //seatBox.enabled = true;
        //Ray ray = GameObject.Find("Default Camera").GetComponent<Camera>().ScreenPointToRay(_touchPos);

        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))
        //{
        //    if (hit.collider == seatBox)
        //    {
        //        if (galbiParents)
        //            galbiParents.GetComponent<Galbi>().SwipeCall();
        //    }
        //}
        //seatBox.enabled = false;
        if (galbiParents)
            galbiParents.GetComponent<Galbi>().SwipeCall();
    }
}
