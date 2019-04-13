using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameHelper;

public class NyangPosition
{
    public bool isEmpty { get; private set; }
    public ENyangPosition position { get; private set; }

    public NyangPosition(bool _isEmpty, ENyangPosition _position)
    {
        isEmpty = _isEmpty;
        position = _position;
    }

    public void Notified()
    {
        isEmpty = false;
    }

    public void UnNotified()
    {
        isEmpty = true;
    }
}

public class SpawnManager
{
    private List<NyangPosition> nyangPositions;
    private List<Nyang> nyangList;
    private int spawnCompleteTick = 0;
    private int spawnTick = 0;

    private InGameNyangController nyangCtrl = null;
    private NyangPoolingManager pooling = null;
    private GameObject nyangParents = null;

    private Dictionary<ENyangPosition, Vector2> setNyangPositions;

    public void Initialize(InGameNyangController _nyangCtrl)
    {
        nyangPositions = new List<NyangPosition>();
        nyangList = new List<Nyang>();
        setNyangPositions = new Dictionary<ENyangPosition, Vector2>();

        nyangPositions.Add(new NyangPosition(true, ENyangPosition.A));
        nyangPositions.Add(new NyangPosition(true, ENyangPosition.B));
        nyangPositions.Add(new NyangPosition(true, ENyangPosition.C));
        nyangPositions.Add(new NyangPosition(true, ENyangPosition.D));
        nyangPositions.Add(new NyangPosition(true, ENyangPosition.E));
        nyangPositions.Add(new NyangPosition(true, ENyangPosition.F));
        
        setNyangPositions.Add(ENyangPosition.A, new Vector2(0, 760));
        setNyangPositions.Add(ENyangPosition.B, new Vector2(-368, 280));
        setNyangPositions.Add(ENyangPosition.C, new Vector2(-115, 350));
        setNyangPositions.Add(ENyangPosition.D, new Vector2(110, 530));
        setNyangPositions.Add(ENyangPosition.E, new Vector2(335, 394));
        setNyangPositions.Add(ENyangPosition.F, new Vector2(138, 174));

        nyangCtrl = _nyangCtrl;

        GameObject poolObj = new GameObject();
        poolObj.transform.SetParent(nyangCtrl.transform);
        pooling = poolObj.AddComponent<NyangPoolingManager>();
        poolObj.name = "Nyang Pooling Manager";

        GameObject panelParents = GameObject.Find("Canvas").FindChild("Panel");
        nyangParents = new GameObject();
        nyangParents.name = "Nyang";
        panelParents.AttachChild(nyangParents);

        Spawn(Random.Range(3, 5));
    }

    public void UpdateNyangTimer()
    {
        List<Nyang>.Enumerator eNyang = nyangList.GetEnumerator();

        while (eNyang.MoveNext())
        {
            Nyang nyang = eNyang.Current as Nyang;

            nyang.TimerUpdate();
        }
    }

    /* DESC :> 
     * 3시 또는 8시가 아닐 경우
     * 시간 흐름에 따라 냥이 지속적 생성 루프
     * 3마리 이하 : 3~5초, 4마리 이상 : 10~20초
     */ 
    public void UpdateSpawn()
    {
        if (!IsAvailableSpawn())
            return;

        SpawnEventNyang();

        spawnTick++;
        if (spawnTick < spawnCompleteTick)
            return;

        int randPosition = Random.Range(0, AvailableSpawnSize() - 1);
        ENyangPosition position = AvailableSpawn()[randPosition].position;
        SpawnNyang(position);

        spawnCompleteTick = 0;

        if (UnavailableSpawnSize() < 4)
        {
            Spawn(Random.Range(3, 5));
        }
        else
        {
            Spawn(Random.Range(10, 20));
        }
    }

    public void SpawnNotify(ENyangPosition position)
    {
        NyangPosition nyang = nyangPositions.Find(n => n.position.Equals(position));

        if (nyang != null)
        {
            nyang.Notified();
        }
    }

    public void SpawnUnNotify(ENyangPosition position)
    {
        NyangPosition nyang = nyangPositions.Find(n => n.position.Equals(position));

        if (nyang != null)
        {
            nyang.UnNotified();
        }
    }

    public List<NyangPosition> AvailableSpawn()
    {
        return nyangPositions.FindAll(n => n.isEmpty);
    }

    public int AvailableSpawnSize()
    {
        return AvailableSpawn().Count;
    }

    public List<NyangPosition> UnavailableSpawn()
    {
        return nyangPositions.FindAll(n => n.isEmpty == false);
    }

    public int UnavailableSpawnSize()
    {
        return UnavailableSpawn().Count;
    }

    public bool IsAvailableSpawn()
    {
        return AvailableSpawnSize() > 0;
    }

    public void Spawn(float time)
    {
        spawnCompleteTick = (int)(60 * time);
        spawnTick = 0;
    }

    public void SpawnEventNyang()
    {
        List<XMLNyang> nyangs = NyangManager.getInstance.GetNyangs(EConditionType.Event);

        List<XMLNyang>.Enumerator eNyang = nyangs.GetEnumerator();

        while (eNyang.MoveNext())
        {
            bool appearFlag = true;

            XMLNyang nyang = eNyang.Current;

            int size = nyang.appearType.Length;

            for (int i = 0; i < size; i++)
            {
                if (!AppearConditionManager.getInstance.CheckCondition(nyang.appearType[i], nyang.appear[i]))
                    appearFlag = false;
            }

            if (appearFlag && AvailableSpawn().Exists(n => n.position.Equals(nyang.position)))
            {
                SpawnNyang(nyang);
            }
        }
    }

    /* DESC :>
     * 생성 조건에 맞는 냥이들 리스트 중, 확률적으로 노멀/랜덤/히든 냥이 생성
     */
    public void SpawnNyang(ENyangPosition position)
    {
        XMLNyang nyangData = new XMLNyang();

        List<XMLNyang> normals;
        List<XMLNyang> rares;
        List<XMLNyang> hidden;

        if (NyangManager.getInstance.GetNyang(position, out normals, out rares, out hidden) == false)
            return;

        bool bSelected = false;
        float rankRate = Random.Range(0, 100);

        if (rankRate / 70 >= 1)
        {
            if (rares.Count > 0)
            {
                nyangData = rares[Random.Range(0, rares.Count)];
                bSelected = true;
            }
        }
        else if (rankRate / 40 >= 1)
        {
            if (hidden.Count > 0)
            {
                nyangData = hidden[Random.Range(0, hidden.Count)];
                bSelected = true;
            }
        }

        if (!bSelected)
        {
            if (normals.Count > 0)
            {
                nyangData = normals[Random.Range(0, normals.Count)];
            }
            else
            {
                if (rares.Count > 0)
                {
                    nyangData = rares[Random.Range(0, rares.Count)];
                }
                else if (hidden.Count > 0)
                {
                    nyangData = hidden[Random.Range(0, hidden.Count)];
                }
            }
        }

        SpawnNyang(nyangData);
    }

    public void TutoSpawnNyang(int id)
    {
        SpawnNyang(NyangManager.getInstance.GetNyang(id));
    }

    private void SpawnNyang(XMLNyang nyangData)
    {
        GameObject nyangObj = pooling.Get();
        Vector3 position = setNyangPositions[nyangData.position];
        nyangObj.name = nyangData.name;

        Nyang nyang = nyangObj.GetComponent<Nyang>();
        Image image = nyangObj.GetComponent<Image>();
        if (!image)
        {
            image = nyangObj.AddComponent<Image>();
        }

        nyangParents.AttachChild(nyangObj);
        nyang.SetGameObject(nyangObj);

        nyang.SetNyang(nyangData);

        nyangObj.transform.localPosition = position;

        BoxCollider box = nyangObj.GetComponent<BoxCollider>();
        Rect rect = nyangObj.GetComponent<RectTransform>().rect;
        box.size = new Vector3(rect.width, rect.height, 1.0f);

        InGameInputController.getInstance.AddTouchableObject(nyang);

        SpawnNotify(nyangData.position);
        NyangInfoManager.getInstance.Visited(nyang.id);

        if (!nyangList.Contains(nyang))
            nyangList.Add(nyang);
    }
}
