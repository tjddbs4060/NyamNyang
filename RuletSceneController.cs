using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameHelper;
using UnityEngine.UI;

public class RuletSceneController : Controller
{
    private bool isEnd = false;

    private GameObject ruletPan = null;
    private Text textGold = null;
    private int batingGold = 100;

    private readonly int ruletSize = 5;
    private Text timerLabel = null;
    private Text moneyLabel = null;
    private Text[] ruletText = null;

    private Text popupRateText = null;
    private Text popupGoldText = null;

    private void Awake()
	{
        List<int> Rate = new List<int> { 0, 1, 2, 10, 100 };

        UIButtonManager btnMgr = UIButtonManager.getInstance;

        ruletPan = GameObject.Find("RuletPanels");
        textGold = GameObject.Find("RateBoard").FindChild("Label (Rate)").GetComponent<Text>();
        textGold.text = string.Format("{0}", GoldConvert.GetGoldFormat(batingGold));

        int maxLen = Rate.Count;

        timerLabel = GameObject.Find("Timer").FindChild("Label (Timer)").GetComponent<Text>();

        int h = 20;
        int m = 0;

        bool isPM = (h / 12) == 1;
        timerLabel.text = string.Format("{0:00}:{1:00}{2}", isPM ? (h % 12) == 0 ? 12 : (h % 12) : (h % 12), m % 60, isPM ? "PM" : "AM");

        moneyLabel = GameObject.Find("Label (Money)").GetComponent<Text>();
        moneyLabel.text = string.Format("{0}", GoldConvert.GetGoldFormat(GlobalData.getInstance.curGold));

        while (Rate.Count > 0)
        {
            Text label = ruletPan.FindChild(string.Format("Image (Rulet{0})", maxLen - Rate.Count)).FindChild("Label (Rate)").GetComponent<Text>();
            
            int rand = Random.Range(0, Rate.Count);

            label.text = string.Format("X{0}", Rate[rand]);
            Rate.RemoveAt(rand);
        }

		GameObject pushBtnObj = GameObject.Find ("PushButton");
		btnMgr.SetButtonMsg (pushBtnObj, () => {
            if (isEnd)
                return;

            isEnd = true;

			GameObject readyBoxObj = GameObject.Find("ReadyBox");
            
			if (readyBoxObj)
				return;

			Image btnImg = pushBtnObj.FindChild("Image (Button)").GetComponent<Image>();
			btnImg.sprite = (Sprite)Resources.Load("Texture/MiniGame/btn_push_2", typeof(Sprite));

            StartCoroutine(UpdateRulet());
        });

        GameObject upBtnObj = GameObject.Find("UpButton");
        btnMgr.SetButtonMsg(upBtnObj, () => {
            if (!isEnd)
                IncreaseBating(10);
        });

        GameObject downBtnObj = GameObject.Find("DownButton");
        btnMgr.SetButtonMsg(downBtnObj, () => {
            if (!isEnd)
                IncreaseBating(-10);
        });

        GameObject popupInfo = GameObjectHelper.Find("DayEndPopup").FindChild("Info");

        popupRateText = popupInfo.FindChild("Label (Rate)").GetComponent<Text>();
        popupGoldText = popupInfo.FindChild("Label (Gold)").GetComponent<Text>();

        ruletText = new Text[ruletSize];
        ruletText[0] = ruletPan.FindChild("Image (Rulet0)").FindChild("Label (Rate)").GetComponent<Text>();
        ruletText[1] = ruletPan.FindChild("Image (Rulet4)").FindChild("Label (Rate)").GetComponent<Text>();
        ruletText[2] = ruletPan.FindChild("Image (Rulet3)").FindChild("Label (Rate)").GetComponent<Text>();
        ruletText[3] = ruletPan.FindChild("Image (Rulet2)").FindChild("Label (Rate)").GetComponent<Text>();
        ruletText[4] = ruletPan.FindChild("Image (Rulet1)").FindChild("Label (Rate)").GetComponent<Text>();
    }

    /* DESC :>
     * 배팅 금액 변경
     */ 
    public void IncreaseBating(int value)
    {
        batingGold += value;
        if (batingGold < 100 || batingGold > 900)
            batingGold -= value;

        textGold.text = string.Format("{0}", GoldConvert.GetGoldFormat(batingGold));
    }

    /* DESC :>
     * 룰렛 멈춘 금액 확인 후 금액 증가
     */
    public void StopRulet()
    {
        float degree = ruletPan.transform.localEulerAngles.z;

        if (degree < 0)
            degree = (degree + 360) % 360;

        int rate = 0;

		int curRulet = (int)(degree / 72);

        string parseText = ruletText[curRulet].text;
        parseText = parseText.Replace("X", "");
        rate = int.Parse(parseText);
        
        int increaseGold = rate * batingGold;

        GlobalData.getInstance.curGold += (increaseGold - batingGold);
        PlayerPrefs.SetInt ("Gold", GlobalData.getInstance.curGold);
        //PlayerPrefs.Save();

        PopupManager.getInstance.Show<DayEndPopup>();

        popupRateText.text = string.Format("x {0}", rate);
        popupGoldText.text = string.Format("{0}냥", GoldConvert.GetGoldFormat(increaseGold));

        int ruletCtn = PlayerPrefs.GetInt("RuletCount", 0);
        PlayerPrefs.SetInt("RuletCount", ruletCtn + 1);
    }

    /* DESC :>
     * 룰렛 판 중앙에 맞추어 룰렛 정지
     */
    IEnumerator UpdateRulet()
    {
		const float timerSpeed = 0.01f;
        Vector3 angleSpeed = new Vector3(0.0f, 0.0f, 20.0f);
        float rotateRate = 0.005f;

        float timer = Random.Range(1.0f, 2.0f);

        while (timer > 0)
        {
            ruletPan.transform.localEulerAngles += angleSpeed;

            if (timer < 0.3f)
            {
                rotateRate *= 1.1f;
            }

            timer -= timerSpeed;
            
            yield return new WaitForSeconds(rotateRate);
        }

		while ((ruletPan.transform.localEulerAngles.z + 360.0f) % 72 < 36)
		{
			ruletPan.transform.localEulerAngles += angleSpeed;

			yield return new WaitForSeconds(rotateRate);
		}

		ruletPan.transform.localEulerAngles -= new Vector3(0.0f, 0.0f,((ruletPan.transform.localEulerAngles.z + 360.0f) % 36));

        yield return new WaitForSeconds(3.0f);

        StopRulet();
    }

	public override void OnScreenTouchBegan (Vector2 _touchPos)
	{
		
	}

	public override void OnScreenDrag (Vector2 _deltaPos)
	{
		
	}

	public override void OnScreenSwipe (Vector2 _touchPos, float _angle)
	{
		
	}

	public override void OnScreenTouchEnded (Vector2 _touchPos)
	{
		
	}
}
