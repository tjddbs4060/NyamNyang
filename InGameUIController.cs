using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameHelper;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour
{
	private List<SubUI> subUIList = new List<SubUI> ();

	private IngredientStorePopup IngredientStorePopup;
    private DecoStorePopup decoStorePopup;

    private Text moneyLabel = null;
    private Text timerLabel = null;
    private Text timerTypeLabel = null;
    private Text dayLabel = null;

    private InGameCookingController gameCtrl = null;

    private GameObject canvasUI = null;

    public void TutorialInit()
    {
        gameCtrl = FindObjectOfType<InGameCookingController>();
        canvasUI = GameObject.Find("Canvas (UI)").FindChild("Panel");

        UIButtonManager btnMgr = UIButtonManager.getInstance;

        btnMgr.SetButtonMsg(GameObject.Find("MeatButton"), () => { OnShowSelectSubUI(subUIList[(int)SUB_UI_TYPE.MEAT]); });
        btnMgr.SetButtonMsg(GameObject.Find("SauceButton"), () => { OnShowSelectSubUI(subUIList[(int)SUB_UI_TYPE.SAUCE]); });

        GameObject selectCookUIObj = GameObjectHelper.Find("SelectMeatUI");
        GameObject selectSauceUIObj = GameObjectHelper.Find("SelectSauceUI");

        subUIList.Clear();
        subUIList.Add(selectCookUIObj.GetComponent<SelectMeatUI>());
        subUIList.Add(selectSauceUIObj.GetComponent<SelectSauceUI>());

        subUIList[0].Init(IngredientManager.getInstance.GetIngredients(EIngredientType.Galbi), OnSetFoodMakeItem);
        subUIList[1].Init(IngredientManager.getInstance.GetIngredients(i => i.type == EIngredientType.Powder || i.type == EIngredientType.Sauce), OnSetFoodMakeItem);

        timerLabel = GameObject.Find("Timer").FindChild("Label (Timer)").GetComponent<Text>();
        timerTypeLabel = GameObject.Find("Timer").FindChild("Label (Timer Type)").GetComponent<Text>();

        timerLabel.text = string.Format("{0:00}:{1:00}", 8, 0);
        timerTypeLabel.text = "AM";

        moneyLabel = GameObject.Find("Label (Money)").GetComponent<Text>();
        moneyLabel.text = string.Format("{0}", GoldConvert.GetGoldFormat(GlobalData.getInstance.curGold));
    }

	public void InitUI()
	{
        gameCtrl = FindObjectOfType<InGameCookingController>();
        canvasUI = GameObject.Find("Canvas (UI)").FindChild("Panel");

        InitSubUIList ();

        DecoManager.getInstance.Init();
        DecoManager.getInstance.ChangeDecoImage(0, "main_roof");

        IngredientStorePopup = GameObjectHelper.Find ("IngredientStorePopup").GetComponent<IngredientStorePopup> ();
        decoStorePopup = GameObjectHelper.Find("DecoStorePopup").GetComponent<DecoStorePopup>();

        UIButtonManager btnMgr = UIButtonManager.getInstance;
		btnMgr.SetButtonMsg (GameObject.Find ("MatShopButton"), OnMatShopButtonClick);
		btnMgr.SetButtonMsg (GameObject.Find ("HouseShopButton"), OnHouseShopButtonClick);

		btnMgr.SetButtonMsg (GameObject.Find ("BuffButton"), OnBuffButtonClick);
		btnMgr.SetButtonMsg (GameObject.Find ("TrashButton"), OnTrashButtonClick);

		btnMgr.SetButtonMsg (GameObject.Find ("OptionButton"), OnOptionButtonClick);
		btnMgr.SetButtonMsg (GameObject.Find ("CatListButton"), OnCatListButtonClick);

		btnMgr.SetButtonMsg (GameObject.Find ("MeatButton"), ()=>{ OnShowSelectSubUI(subUIList[(int)SUB_UI_TYPE.MEAT]); } );
		btnMgr.SetButtonMsg (GameObject.Find ("SauceButton"), ()=>{ OnShowSelectSubUI(subUIList[(int)SUB_UI_TYPE.SAUCE]); } );

        timerLabel = GameObject.Find("Timer").FindChild("Label (Timer)").GetComponent<Text>();
        timerTypeLabel = GameObject.Find("Timer").FindChild("Label (Timer Type)").GetComponent<Text>();

        moneyLabel = GameObject.Find("Label (Money)").GetComponent<Text>();
        moneyLabel.text = string.Format("{0}", GoldConvert.GetGoldFormat(GlobalData.getInstance.curGold));

        dayLabel = GameObject.Find("Label(Day)").GetComponent<Text>();
        dayLabel.text = GlobalData.getInstance.curDay.ToString();

        if (GlobalData.getInstance.isAdsBuff)
        {
            GameObjectHelper.Find("Bone").FindChild("Image").GetComponent<Image>().sprite =
                        ResourcesManager.getInstance.CreateSprite("main_bone_buff", new Vector2(0.5f, 0.5f), 1.0f);
            GameObjectHelper.Find("BuffButton").FindChild("Image").GetComponent<Image>().sprite =
                ResourcesManager.getInstance.CreateSprite("main_btn_buff_push", new Vector2(0.5f, 0), 1.0f);
            GameObjectHelper.Find("BuffNyang").FindChild("Image").SetActive(true);
        }
    }

	private void InitSubUIList()
	{
		GameObject selectCookUIObj = GameObjectHelper.Find ("SelectMeatUI");
		GameObject selectSauceUIObj = GameObjectHelper.Find ("SelectSauceUI");

		subUIList.Clear ();
		subUIList.Add(selectCookUIObj.GetComponent<SelectMeatUI>());
		subUIList.Add(selectSauceUIObj.GetComponent<SelectSauceUI>());

        subUIList [0].Init (IngredientManager.getInstance.GetIngredients(EIngredientType.Galbi), OnSetFoodMakeItem);
		subUIList [1].Init (IngredientManager.getInstance.GetIngredients(i=>i.type == EIngredientType.Powder || i.type == EIngredientType.Sauce), OnSetFoodMakeItem);

        // DESC :> 방명록 초기화
        GameObject guest = GameObjectHelper.Find("GuestBookPopup");
        guest.GetComponent<GuestBookPopup>().Initialize();
        guest.SetActive(false);
        GameObject list = guest.FindChild("List");
        
        for (int i = 0; i < 6; i++)
        {
            GameObject nyang = guest.FindChild(string.Format("Guest Nyang {0}", i));
            list.AttachChild(nyang);
        }

        list.GetComponent<ListUI>().UpdateList();
    }

    public void OnUpdateMoneyInfo()
    {
        int curGold = GlobalData.getInstance.curGold;
#if DEBUG_LOG
        Debug.Log("CUR GOLD : " + curGold);
#endif
        moneyLabel.text = string.Format("{0}", GoldConvert.GetGoldFormat(curGold));
        PlayerPrefs.SetInt("GOLD", curGold);
    }

	public void OnUpdateTimeInfo(int _sec)
	{
		int h = _sec / (60 * 60);
		int m = _sec / 60;

		bool isPM = (h / 12) == 1;
		timerLabel.text = string.Format ("{0:00}:{1:00}", isPM ? (h%12) == 0 ? 12 : (h%12) : (h % 12), m % 60);
        timerTypeLabel.text = (isPM) ? "PM" : "AM";

        PlayerPrefs.SetInt("CUR_TIME", _sec);
        //PlayerPrefs.Save();

		if (h == 15)
		{
            GlobalData.getInstance.isTanTime = true;
            PlayerPrefs.SetInt("IS_TAN", 1);
		}

		if (h == 20)
		{
            GlobalData.getInstance.isRuletTime = true;
            PlayerPrefs.SetInt("IS_RULET", 1);
		}
	}

	private void OnMatShopButtonClick()
	{
        if (PopupManager.getInstance.IsShow())
        {
            if (PopupManager.getInstance.GetPopup() is DecoStorePopup)
                PopupManager.getInstance.Close();
            else
                return;
        }

        Debug.Log("음식 재료 상점");
		PopupManager.getInstance.Show (IngredientStorePopup);
	}

	private void OnHouseShopButtonClick()
	{
        if (PopupManager.getInstance.IsShow())
        {
            if (PopupManager.getInstance.GetPopup() is IngredientStorePopup)
                PopupManager.getInstance.Close();
            else
                return;
        }

        Debug.Log("집 꾸미기용 상점");
        PopupManager.getInstance.Show(decoStorePopup);
	}

	private void OnBuffButtonClick()
	{
        if (PopupManager.getInstance.IsShow())
            return;

        Debug.Log ("광고 출력");
        PopupManager.getInstance.Show<AdsPopup>();
    }

	private void OnTrashButtonClick()
	{
        if (PopupManager.getInstance.IsShow())
            return;

        Debug.Log ("쓰레기통");

        gameCtrl.GarbageGalbi();
    }

    private void OnOptionButtonClick()
    {
        if (PopupManager.getInstance.IsShow())
            return;

        //Client.AudioManager.Play((int)FX_SOUND_TYPE.SHOW_POPUP);

        OptionPopup.Show(canvasUI);
    }

	private void OnCatListButtonClick()
	{
        if (PopupManager.getInstance.IsShow())
            return;

        PopupManager.getInstance.Show<GuestBookPopup>();
	}

	private void OnShowSelectSubUI(SubUI _subUI)
	{
        if (PopupManager.getInstance.IsShow())
            return;

        _subUI.Show ();
	}

    /// <summary>
    /// 음식 만들 재료 세팅 함수
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_type">고기 : 0번 소스 : 1번 가루 : 2번</param>
	private void OnSetFoodMakeItem(int _id, EIngredientType _type)
    {
        InGameCookingController cookingController = FindObjectOfType<InGameCookingController>();

        if (cookingController)
        {
            cookingController.UseIngredient(_type, _id);
        }
#if DEBUG_LOG
        Debug.Log("ITEM ID : " + _id + " TYPE : " + _type);
#endif
    }
}
