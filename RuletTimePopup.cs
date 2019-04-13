using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameHelper;

public class RuletTimePopup : MonoBehaviour, IPopup
{
	public void Show()
	{
        gameObject.SetActive(true);

        GlobalData.getInstance.goldIncreaseRate = 1.0f;


        UIButtonManager.getInstance.SetButtonMsg (gameObject.FindChild ("ConfirmButton"), () => {
			PopupManager.getInstance.Close();
            PlayerPrefs.SetFloat("GOLD_INCREASE_BUFF", GlobalData.getInstance.goldIncreaseRate);

            GlobalData.getInstance.curDay++;
            AdsManager.getInstance.remainShowAdsDay--;
            PlayerPrefs.SetInt("REMAIN_SHOW_ADS_DAY", AdsManager.getInstance.remainShowAdsDay);
            PlayerPrefs.SetInt("CUR_DAY", GlobalData.getInstance.curDay);

            SceneHelper.getInstance.ChangeScene(typeof(RuletScene));
		});

		UIButtonManager.getInstance.SetButtonMsg (gameObject.FindChild ("CloseButton"), () => {
			PopupManager.getInstance.Close();

            GlobalData.getInstance.curDay++;
            AdsManager.getInstance.remainShowAdsDay--;
            PlayerPrefs.SetInt("REMAIN_SHOW_ADS_DAY", AdsManager.getInstance.remainShowAdsDay);
            PlayerPrefs.SetInt("CUR_DAY", GlobalData.getInstance.curDay);

            GlobalData.getInstance.isPause = false;

            GlobalData.getInstance.curTime = 60 * 60 * 11;
            PlayerPrefs.SetInt("CUR_TIME", GlobalData.getInstance.curTime);

            PlayerPrefs.SetFloat("GOLD_INCREASE_BUFF", GlobalData.getInstance.goldIncreaseRate);

            GlobalData.getInstance.isRuletTime = false;
            PlayerPrefs.SetInt("IS_RULET", 0);

            //PlayerPrefs.Save();

            GlobalData.getInstance.cameraColor = new Color(234.0f / 255.0f, 239.0f / 255.0f, 242.0f / 255.0f, 0);
            GlobalData.getInstance.backgroundColor = new Color(138.0f / 255.0f, 212.0f / 255.0f, 223.0f / 255.0f, 1);

            SceneHelper.getInstance.ChangeScene(typeof(InGameScene));
		});
	}

	public void Close()
	{
		gameObject.SetActive (false);
	}
}
