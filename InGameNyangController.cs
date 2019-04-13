using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameHelper;


public class InGameNyangController : MonoBehaviour
{
    public SpawnManager spawnManager { get; private set;}
    public ArrivedManager arrivedManager { get; private set; }

    public int SpawnTick { get; set; }
    public int ArrivedTick { get; set; }

    public void Initialize(InGameCookingController cookingCtrl)
    {
        spawnManager = new SpawnManager();
        arrivedManager = new ArrivedManager();

        spawnManager.Initialize(this);
        arrivedManager.Initialize(cookingCtrl, spawnManager);
    }

	public void SpawnUpdate()
	{
        spawnManager.UpdateSpawn();
	}

    public void ArriveUpdate()
    {
        spawnManager.UpdateNyangTimer();
        arrivedManager.UpdateArrived();
    }
}
