using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolingBase<T> : MonoBehaviour where T : IPooling
{
    private Stack<GameObject> stack = new Stack<GameObject>();

	public int count { get; private set; }

    public void UnUse(GameObject t)
    {
        if (GlobalData.getInstance.isPause)
            return;

		count--;

        stack.Push(t);
    }

    // DESC :> 제너릭 캐스팅 제너릭
    //public void UnUse<TValue>(TValue tValue) where TValue : IPooling
    //{
    //    T resultValue = tValue as T;
    //    stack.Push(resultValue);
    //}

    public GameObject Push()
    {
        if (!EmptyStack())
            return Get();

        GameObject newObj = new GameObject();
		newObj.transform.SetParent(transform);
        T t = newObj.AddComponent<T>();

        //T t = gameObject.AddComponent<T>();
        //t.transform.SetParent(transform);

        // DESC :> where T 의 T가 들어갈 곳이 일치할 경우 제너릭 생략 가능
        t.ExeUnUse(UnUse);

        stack.Push(newObj);

        return Get();
    }

    public GameObject Get()
    {
        if (EmptyStack())
            return Push();

        GameObject go = stack.Pop();
        go.SetActive(true);

		count++;

        return go;
    }

    // DESC :> Empty is True
    private bool EmptyStack()
    {
        return stack.Count <= 0;
    }
}


/// <summary>
/// DESC :> MonoBehaviour 포함
/// </summary>
public abstract class IPooling : InputObject
{
    public delegate void PoolingUnUse(GameObject pooling);
    public PoolingUnUse poolingUnUse = null;

    public void ExeUnUse(PoolingUnUse _poolingUnUse)
    {
        poolingUnUse = _poolingUnUse;
    }

    public void OnDisable()
    {
        if (poolingUnUse != null)
            poolingUnUse(gameObject);
    }
}

public class ImageObject : IPooling
{

}