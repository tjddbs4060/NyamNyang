using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameHelper;

class GuestBookPopup : MonoBehaviour, IPopup
{
    private struct FGuestBook
    {
        public Image nyangImage { get; set; }

        public Text labelName { get; set; }
        public Text labelRank { get; set; }
        public Text labelVisit { get; set; }
        public Text labelCondition { get; set; }
    }

    private readonly int guestBookSize = 6;
    private GameObject[] nyangs = null;
    private FGuestBook guestBook;
    
    private GameObject[] nonGuestBookObjs;
    
    private int page = 0;

    public void Initialize()
    {
        nyangs = new GameObject[guestBookSize];
        for (int i = 0; i < guestBookSize; i++)
        {
            nyangs[i] = gameObject.FindChild(string.Format("Guest Nyang {0}", i));
        }

        GameObject canvas = GameObject.Find("Canvas").FindChild("Panel");
        GameObject canvasUI = GameObject.Find("Canvas (UI)").FindChild("Panel");

        nonGuestBookObjs = new GameObject[8];
        nonGuestBookObjs[0] = canvas.FindChild("AnchorBottom");
        nonGuestBookObjs[1] = canvas.FindChild("Cooking");
        nonGuestBookObjs[2] = canvas.FindChild("Nyang");
        nonGuestBookObjs[3] = canvasUI.FindChild("AnchorCenter").FindChild("DraggedObject");
        nonGuestBookObjs[4] = canvasUI.FindChild("AnchorBottomLeft").FindChild("SauceButton");
        nonGuestBookObjs[5] = canvasUI.FindChild("AnchorBottomRight").FindChild("MeatButton");
        nonGuestBookObjs[6] = canvasUI.FindChild("AnchorBottom").FindChild("Buttons");
        nonGuestBookObjs[7] = canvasUI.FindChild("SubUI");

        // DESC :> 방명록 좌, 우, 종료 버튼
        UIButtonManager btnMgr = UIButtonManager.getInstance;

        btnMgr.SetButtonMsg(GameObjectHelper.Find("CloseButton (GuestBook)"), () => { PopupManager.getInstance.Close(); });
        btnMgr.SetButtonMsg(GameObjectHelper.Find("Previous (GuestBook)"), () => { ChangePage(-1); });
        btnMgr.SetButtonMsg(GameObjectHelper.Find("Next (GuestBook)"), () => { ChangePage(1); });
    }

    public void SetVisible(bool isShow)
    {
        if (isShow)
            page = 0;

        gameObject.SetActive(isShow);
        for (int i = 0; i < nonGuestBookObjs.Length; i++)
        {
            nonGuestBookObjs[i].SetActive(!isShow);
        }
    }

    public void Show()
    {
        // TODO :> Show GuestBook
        SetVisible(true);
        DrawGuestBook();
    }

    public void Close()
    {
        // TODO :> Hide GuestBook
        SetVisible(false);
    }

    /* DESC :>
     * 방명록 Page 변경
     */
    public void ChangePage(int value)
    {
        int lastPage = NyangManager.getInstance.Size / 6;

        page += value;

        if (page < 0)
        {
            page = lastPage;
        }
        if (page > lastPage)
        {
            page = 0;
        }

        DrawGuestBook();
    }

    /* DESC :>
     * 방명록 리스트 업데이트
     * 도감 순서에 따라 리스트 출력
     */
    public void DrawGuestBook()
    {
        for (int i = page * 6; i < page * 6 + 6; i++)
        {
            GameObject nyang = nyangs[i % 6];

            #region GuestBook Initialize
            guestBook.nyangImage = nyang.FindChild("Image (Nyang)").GetComponent<Image>();

            guestBook.labelName = nyang.FindChild("Label (Name)").GetComponent<Text>();
            guestBook.labelRank = nyang.FindChild("Label (Rank)").GetComponent<Text>();
            guestBook.labelVisit = nyang.FindChild("Label (Visited)").GetComponent<Text>();
            guestBook.labelCondition = nyang.FindChild("Label (Condition)").GetComponent<Text>();
            #endregion

            if (NyangInfoManager.getInstance.Size <= i)
            {
                nyang.SetActive(false);

                continue;
            }

            if (!nyang.activeSelf)
                nyang.SetActive(true);

            XMLNyangInfo info = NyangInfoManager.getInstance.GetNyangInfo(i);

            GameObject plusButton = nyang.FindChild("Plus");

            plusButton.GetComponent<SimpleValueContainer>().SetID(info.id);
            UIButtonManager.getInstance.ClearButtonMsg(plusButton);
            UIButtonManager.getInstance.SetButtonMsg(plusButton, () => { plusButton.GetComponent<SimpleValueContainer>().OnButtonPlus(); });
            //plusButton.

            #region GuestBook Setting
            guestBook.nyangImage.sprite = ResourcesManager.getInstance.CreateSprite(info.imgPath, new Vector2(0.5f, 0.5f));
            guestBook.nyangImage.color = Color.black;
            guestBook.labelName.text = "-";
            guestBook.labelRank.text = "-";
            guestBook.labelVisit.text = "-";
            guestBook.labelCondition.text = "";
            #endregion

            int visitedCount = info.visited;

            if (visitedCount > 0)
            {
                guestBook.nyangImage.color = Color.white;

                guestBook.labelName.text = NyangInfoManager.getInstance.GetNyangInfo(info.id).name;
                guestBook.labelRank.text = NyangInfoManager.getInstance.GetNyangInfo(info.id).desc;

                guestBook.labelVisit.text = (i == 0) ? "" : string.Format("{0}회방문", visitedCount);
            }
            else
            {
                switch (info.rank)
                {
                    case ENyangRank.Rare:
                    case ENyangRank.Normal:
                        guestBook.labelName.text = NyangInfoManager.getInstance.GetNyangInfo(info.id).name;
                        break;
                    default:
                        break;
                }
            }

            guestBook.nyangImage.SetNativeSize();
        }
    }
}
