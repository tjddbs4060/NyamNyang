using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameHelper;

public class ArrivedManager
{
    private Nyang nyang = null;
    private int arrivedTick = 0;

    private InGameCookingController cookingCtrl = null;
    private SpawnManager spawnManager = null;

    public void Initialize(InGameCookingController _cookingCtrl, SpawnManager _spawnManager)
    {
        cookingCtrl = _cookingCtrl;
        spawnManager = _spawnManager;
    }

    public void UpdateArrived()
    {
        if (!nyang)
            return;

        arrivedTick++;

        if (arrivedTick < 36)
            return;

        cookingCtrl.ArrivedNyang();
        spawnManager.SpawnUnNotify(nyang.position);
        nyang.gameObject.SetActive(false);
        nyang = null;
    }

    public void Arrived(Nyang _nyang)
    {
        arrivedTick = 0;
        cookingCtrl.FailGalbi();
        nyang = _nyang;
    }
}
