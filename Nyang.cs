using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameHelper;

public enum ENyangRank : byte
{
    Normal,
    Rare,
    Hidden,
}

public enum ENyangState
{
    none,
    wait,
    pick,
    order,
    happy,
    angry,
    buff, 
}

public enum ENyangPosition
{
    A, B, C, D, E, F, 
}

/* Nyang Data */
public class Nyang : IPooling
{
    private string nyangName;
    public string NyangName
    {
        get { return string.Format("{0}", nyangName); }
    }
    public string NyangFullName
    {
        get { return string.Format("{0}_{1}", nyangName, state.ToString()); }
    }
    
	private Image image = null;
	public Vector3 originPosition { get; private set; }

    public int id { get; private set; }
    public ENyangPosition position { get; private set; }
    public EConditionType[] appearType { get; private set; }
    public int[] appearCondition { get; private set; }
	public ENyangRank rank { get; private set; }

    private InGameNyangController nyangCtrl = null;
    private InGameCookingController cookingCtrl = null;

    public float waitingTime { get; private set; }

    private Camera defaultCam = null;

    private BoxCollider collider = null;
    private BoxCollider seatBox = null;
    
    private GameObject customer = null;
    private GameObject dragedObj = null;

    private GameObject increaseMoney = null;
    private Text increaseMoneyText = null;

    private RectTransform rectTransform = null;

    public ENyangState state { get; private set; }

    public void TutorialSetState(ENyangState _state)
    {
        SetState(_state);
    }

    public void Initialize()
    {
        waitingTime = 0.0f;

        nyangCtrl = FindObjectOfType<InGameNyangController>();
        cookingCtrl = FindObjectOfType<InGameCookingController>();

        seatBox = GameObject.Find("SeatedRect").GetComponent<BoxCollider>();
        defaultCam = GameObject.Find("Default Camera").GetComponent<Camera>();

        customer = GameObject.Find("SetCostomer");
        dragedObj = GameObject.Find("DraggedObject");

        increaseMoney = GameObjectHelper.Find("IncreaseMoney");
        increaseMoneyText = increaseMoney.FindChild("Money").FindChild("Label (Money)").GetComponent<Text>();
    }

    public void SetNyang(int id, string nyangName, ENyangPosition position, ENyangRank rank, EConditionType[] appearType, int[] appearCondition)
    {
        this.id = id;
        this.nyangName = nyangName;
        this.position = position;
        this.rank = rank;
        this.appearType = appearType;
        this.appearCondition = appearCondition;

        SetState(ENyangState.wait);

        Initialize();
    }
    public void SetNyang(XMLNyang nyang)
    {
        SetNyang(nyang.id, nyang.name, nyang.position, nyang.rank, nyang.appearType, nyang.appear);
    }

    public void SetGameObject(GameObject _go)
    {
        go = _go;
		image = go.GetComponent<Image> ();
        collider = go.GetComponent<BoxCollider>();
        if (collider == null)
            collider = go.AddComponent<BoxCollider>();

        collider.enabled = true;
    }
    
    public override void OnPress(Vector2 _pos)
    {
		originPosition = transform.localPosition;
    }

    /* DESC :>
     * 냥이가 대기 상태일 때, 드래그
     */ 
    public override void OnDragStart()
    {
        if (state == ENyangState.wait || state == ENyangState.buff)
        {
            dragedObj.AttachChild(go);
        }
    }
    /* DESC :>
     * 드래그시 냥이 이미지 변경
     */
    public override void OnDrag(Vector3 _diff)
    {
        if (state == ENyangState.wait || state == ENyangState.buff)
        {
            SetState(ENyangState.pick);
        }

        if (state == ENyangState.pick)
        {
            transform.localPosition -= _diff;
        }
    }

    /* DESC :>
     * 주문대에 오른 뒤 30초 후에 냥이 떠남
     */
    public void TimerUpdate()
    {
        if (state == ENyangState.order)
        {
            if (waitingTime >= 30.0f)
            {
                SetState(ENyangState.angry);
                nyangCtrl.arrivedManager.Arrived(this);
            }

            waitingTime += Time.deltaTime;
        }
    }

    /* DESC :>
     * 냥이를 드래그 한 위치가 스토브이면 주문 상태로 변경
     */
    public override void OnSwipe(Vector2 _pos, float angle)
    {
        if (state == ENyangState.pick)
        {
            collider.enabled = false;

            GameObject parents = transform.parent.gameObject;
			Vector3 setPosition = originPosition;
            ENyangState setState = ENyangState.wait;

            seatBox.enabled = true;
            Ray ray = defaultCam.ScreenPointToRay(_pos);

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if (hit.collider == seatBox)
                {
                    if (cookingCtrl.SeatedNyang == null)
                    {
                        setState = ENyangState.order;

                        customer.AttachChild(go);
                        parents = customer;
                        setPosition = new Vector3(0.0f, 4.0f, 0.0f);    // Vector3.zero;

                        cookingCtrl.EnteredNyang(this);
                    }

                    seatBox.enabled = false;
                }
            }

            parents.AttachChild(gameObject);
            transform.localPosition = setPosition;
            SetState(setState);

			if (setState == ENyangState.order)
                InGameInputController.getInstance.RemoveTouchableObject(this);
            else
                collider.enabled = true;
        }
    }

    private void SetState(ENyangState _state)
    {
        state = _state;

        if (GlobalData.getInstance.isAdsBuff)
        {
            if (state == ENyangState.wait)
            {
                state = ENyangState.buff;
            }
        }

        image.sprite = ResourcesManager.getInstance.CreateSprite (NyangFullName, new Vector2 (0.5f, 0.5f), 1.0f);
		image.SetNativeSize ();
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform)
        {
            rectTransform.sizeDelta = image.sprite.bounds.size * 2.0f;
        }
    }

    public void AngryNyang()
    {
        Client.AudioManager.Play((int)FX_SOUND_TYPE.ANGRY);
        waitingTime += 30.0f;
    }

    public void HappyNyang()
    {
        int sndIdx = UnityEngine.Random.Range((int)FX_SOUND_TYPE.HAPPY1, (int)FX_SOUND_TYPE.HAPPY2);
        
        Client.AudioManager.Play(sndIdx);

        SetState(ENyangState.happy);
        nyangCtrl.arrivedManager.Arrived(this);

        increaseMoneyText.text = string.Format("+{0}", GoldConvert.GetGoldFormat((int)(300 * GlobalData.getInstance.goldIncreaseRate)));
        increaseMoney.SetActive(true);
    }

    public void BuffStateInitialize()
    {
        SetState(ENyangState.wait);
    }
}