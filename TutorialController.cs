using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameHelper;
using UnityEngine.UI;

public class TutorialController : Controller
{
    private InGameUIController uiCtrl = null;
    private InGameCookingController cookingCtrl = null;
    private InGameNyangController nyangCtrl = null;
    private TutorialManager tutoManager = null;

    public void Awake()
    {
        uiCtrl = gameObject.AddComponent<InGameUIController>();
        uiCtrl.TutorialInit();

        cookingCtrl = gameObject.AddComponent<InGameCookingController>();
        cookingCtrl.TutorialInit();

        nyangCtrl = gameObject.AddComponent<InGameNyangController>();
        nyangCtrl.Initialize(cookingCtrl);

        tutoManager = TutorialManager.getInstance;
        tutoManager.StartTutorial(cookingCtrl, nyangCtrl);

        StartCoroutine("UpdateFrame");
    }

    private IEnumerator UpdateFrame()
    {
        while (true)
        {
            InputManager.getInstance.UpdateInput();

            yield return null;
        }
    }

    private void OnDestroy()
    {
        StopCoroutine("UpdateFrame");
    }

    public override void OnScreenTouchBegan(Vector2 _touchPos)
    {
        tutoManager.TouchStart(_touchPos);
        cookingCtrl.OnScreenTouchBegan(_touchPos);
    }

    public override void OnScreenDrag(Vector2 _deltaPos)
    {

    }

    public override void OnScreenSwipe(Vector2 _touchPos, float _angle)
    {
        tutoManager.TouchEnd(_touchPos);
        cookingCtrl.OnScreenSwipe(_touchPos, _angle);
    }

    public override void OnScreenTouchEnded(Vector2 _touchPos)
    {
        tutoManager.TouchEnd(_touchPos);
    }
}
