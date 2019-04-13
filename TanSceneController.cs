using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameHelper;
using UnityEngine.UI;

public enum ETanState
{
    RARE, 
    WELLDON, 
    OVERCOOK, 
}

public class TanSceneController : Controller
{
    private readonly float standard = 200.0f;
    private readonly float limitTemp = 50.0f;
    private RectTransform temperatureGaugeImg = null;

	private GameObject resultBox = null;
	private Image resultBoxImg = null;
	private Text resultLabel = null;

	private bool isEnd = false;

    private Text timerLabel = null;
    private Text moneyLabel = null;

    private Image characterImage = null;
    private Image faceImage = null;
    private Text readyText = null;

    private GameObject readyBoxObj = null;

    private void Awake()
	{
		temperatureGaugeImg = GameObject.Find ("Image (TemperatureGauge)").GetComponent<RectTransform> ();
        // DESC :> Image Size : 0 으로 초기화
        temperatureGaugeImg.sizeDelta = new Vector2(temperatureGaugeImg.sizeDelta.x, 0.0f);

		resultBox = GameObjectHelper.Find("ResultBox");
		resultBoxImg = resultBox.FindChild ("Image (Result Box)").GetComponent<Image> ();
		resultLabel = resultBox.FindChild ("Label (Result)").GetComponent<Text> ();

        UIButtonManager btnMgr = UIButtonManager.getInstance;

		GameObject pushBtnObj = GameObject.Find ("PushButton");
		btnMgr.SetButtonMsg (pushBtnObj, () => {
            // DESC :> 레디가 다 끝났는지
            GameObject readyBoxObj = GameObject.Find("ReadyBox");

            if (readyBoxObj)
                return;

            Image btnImg = pushBtnObj.FindChild("Image (Button)").GetComponent<Image>();
			btnImg.sprite = (Sprite)Resources.Load("Texture/MiniGame/btn_push_2", typeof(Sprite));

            // DESc :> 썬텐 종료
            isEnd = true;
		});

        timerLabel = GameObject.Find("Timer").FindChild("Label (Timer)").GetComponent<Text>();

        int h = 15;
        int m = 0;

        bool isPM = (h / 12) == 1;
        timerLabel.text = string.Format("{0:00}:{1:00}{2}", isPM ? (h % 12) == 0 ? 12 : (h % 12) : (h % 12), m % 60, isPM ? "PM" : "AM");

        moneyLabel = GameObject.Find("Label (Money)").GetComponent<Text>();
        moneyLabel.text = string.Format("{0}", GoldConvert.GetGoldFormat(GlobalData.getInstance.curGold));

        characterImage = GameObject.Find("Image (Character)").GetComponent<Image>();
        faceImage = GameObject.Find("Image (Face)").GetComponent<Image>();

        readyText = GameObject.Find("Label (Ready)").GetComponent<Text>();
        readyBoxObj = GameObject.Find("ReadyBox");

        StartCoroutine (FrameUpdate ());
	}

    /* DESC :>
     * 썬텐 온도 확인 후 버프 적용
     */ 
	private ETanState TanStateCheck()
    {
        characterImage.sprite = (Sprite)Resources.Load("Texture/MiniGame/tan_chr_1_front", typeof(Sprite));

        float temperature = temperatureGaugeImg.sizeDelta.y;

        float temperPercent = (temperature / standard) * limitTemp;

        ETanState tanState = ETanState.OVERCOOK;
        
        if (0 < temperPercent && temperPercent < 25)
        {
            tanState = ETanState.RARE;
        }
        else if (temperPercent < 33)
        {
            tanState = ETanState.WELLDON;
        }

        if (tanState == ETanState.WELLDON)
        {
            Client.AudioManager.Play((int)FX_SOUND_TYPE.HAPPY1);
            GlobalData.getInstance.goldIncreaseRate *= 1.5f;
        }
        else
        {
            Client.AudioManager.Play((int)FX_SOUND_TYPE.ANGRY);
            GlobalData.getInstance.goldIncreaseRate *= 0.5f;
        }

        return tanState;
    }

	private IEnumerator ReadySequence()
	{
		readyText.text = "READY";

		yield return new WaitForSeconds (3.0f);

		readyText.text = "GO";

		yield return new WaitForSeconds (2.0f);

		readyBoxObj.SetActive (false);
	}

    /* DESC :>
     * 썬텐 온도 상승
     * 썬텐 결과에 따른 UI 변경
     */
    private IEnumerator FrameUpdate()
	{
		yield return StartCoroutine (ReadySequence ());

		while (!isEnd)
		{
			Vector2 curSize = temperatureGaugeImg.sizeDelta;
            
            curSize.y += 20.0f / 9.0f;
			temperatureGaugeImg.sizeDelta = curSize;
            
            if (curSize.y >= 200.0f)
                isEnd = true;

			yield return null;
		}

        resultBox.SetActive(true);
        ETanState result = TanStateCheck ();
        resultBoxImg.color = (result == ETanState.WELLDON) ? 
            new Color(205.0f / 255.0f, 105.0f / 255.0f, 195.0f / 255.0f) : 
            new Color(255.0f / 255.0f, 195.0f / 255.0f, 190.0f / 255.0f);
		
        switch (result)
        {
            case ETanState.RARE:
                resultLabel.text = "RARE\n!";
                break;
            case ETanState.WELLDON:
                resultLabel.text = "WELL\nDON";
                break;
            case ETanState.OVERCOOK:
                resultLabel.text = "OVER\nCOOK\n!";
                break;
        }
		GameObjectHelper.Find ("Icon (Heart)").SetActive (result == ETanState.WELLDON);

        faceImage.sprite = (result == ETanState.WELLDON) ? 
            (Sprite)Resources.Load("Texture/MiniGame/tan_chr_1_face_1", typeof(Sprite)) : 
            (Sprite)Resources.Load("Texture/MiniGame/tan_chr_1_face_2", typeof(Sprite));
        faceImage.enabled = true;

		yield return new WaitForSeconds (3.0f);

        PlayerPrefs.SetFloat("Buf", (result == ETanState.WELLDON) ? 1.5f : 0.5f);
        //PlayerPrefs.Save();

        PopupManager.getInstance.Show<ToPMPopup>();
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
