using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galbi : IIngredient
{
    private readonly int GalbiPrice = 300;
    private int bakeCount = 1;
    private bool sell = false;

    private InGameUIController uiCtrl = null;
    private InGameCookingController cookingCtrl = null;

    protected override void Initialize()
    {
        uiCtrl = GameObject.Find("Controller").GetComponent<InGameUIController>();
        cookingCtrl = FindObjectOfType<InGameCookingController>();
    }

    public void Reset()
    {
        bakeCount = 1;
        sell = false;
    }

    public void CookComplete()
    {
        sell = true;
    }

    public string GetFailGalbiPath()
    {
        string name = "";
        switch (itemID)
        {
            case 1:
                name = string.Format("obj_cook_meat_over");
                break;
            case 2:
                name = string.Format("obj_cook_tteok_over");
                break;
            case 3:
                name = string.Format("obj_cook_chicken_over");
                break;
            case 4:
                name = string.Format("obj_cook_apple_over");
                break;
            default:
                throw new System.Exception("Invalid Galbi Type");
        }

        return name;
    }

    /* DESC :> 
     * 갈비 완료 여부에 따라 갈비 판매 및 냥이 상태 결정
     */
    public override void OnClick(Vector2 _pos)
    {
        if (bakeCount < 7)
            return;
        
        if (sell)
        {
			GlobalData.getInstance.curGold += (int)(GalbiPrice * GlobalData.getInstance.goldIncreaseRate);
            uiCtrl.OnUpdateMoneyInfo();

            cookingCtrl.SeatedNyang.HappyNyang();
        }
        else
        {
            if (GlobalData.getInstance.isAdsBuff)
            {
                GlobalData.getInstance.curGold += GalbiPrice;
            }
            cookingCtrl.SeatedNyang.AngryNyang();
        }

		Reset ();
    }

	public void SwipeCall()
	{
		bakeCount++;

		cookingCtrl.GalbiBaking(bakeCount);
	}

    public string GetGalbiPath(int index, bool name)
    {
        string head = (name) ? ("") : (string.Format("{0}/", ResourcePath.cookObject));

        string path;

        switch (itemID)
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

        switch (itemID)
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
		string head = (name) ? string.Empty : (string.Format("{0}/", ResourcePath.cookObject));

        string path;

        switch (itemID)
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

    public void SetGameObject(GameObject _go)
    {
        go = _go;
    }
}