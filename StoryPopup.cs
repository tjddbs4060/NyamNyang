using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameHelper;

public class StoryPopup : MonoBehaviour, IPopup
{
    /*
    private Image storyImage1 = null;
    private Image storyImage2 = null;
    private Text story1 = null;
    private Text story2 = null;
    */
    private void Awake()
    {
        GameObject info = gameObject.FindChild("Info");

        /*
        storyImage1 = info.FindChild("Image (StoryIcon1)").GetComponent<Image>();
        storyImage2 = info.FindChild("Image (StoryIcon2)").GetComponent<Image>();
        story1 = info.FindChild("Label1").GetComponent<Text>();
        story2 = info.FindChild("Label2").GetComponent<Text>();
        */
        UIButtonManager uIButtonMgr = UIButtonManager.getInstance;
        uIButtonMgr.SetButtonMsg(gameObject.FindChild("CloseButton"), () => { PopupManager.getInstance.Close(); });
    }

    public void Show()
    {
        gameObject.SetActive(true);

        /*
        int nyangID = GlobalData.getInstance.curStory;
        
        if (nyangID == 34 || nyangID == 29)// nyangID == 6 || nyangID == 3 || nyangID == 16 || nyangID == 28 || nyangID == 31)
        {
            PopupManager.getInstance.Close();

            return;
        }

        XMLNyang nyang = NyangManager.getInstance.GetNyang(nyangID);
        XMLNyangStory story = StoryManager.getInstance.GetNyangStory(nyangID);

        string storyPath = nyang.name;
        storyPath = storyPath.Replace("cat", "story");
        
        Sprite image1 = ResourcesManager.getInstance.CreateSprite(string.Format("{0}_1", storyPath), new Vector2(0.5f, 0.5f));
        Sprite image2 = ResourcesManager.getInstance.CreateSprite(string.Format("{0}_2", storyPath), new Vector2(0.5f, 0.5f));

        storyImage1.sprite = image1;
        storyImage2.sprite = image2;

        storyImage1.SetNativeSize();
        storyImage2.SetNativeSize();

        story1.text = story.story1;
        story2.text = story.story2;
        */      
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
