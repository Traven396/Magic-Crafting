using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inspector;
using DG.Tweening;

public class TestingNestedConditions : MonoBehaviour
{

    Tween myTween;

    float timer = 5;

    private void Start()
    {
        myTween = transform.DOMove(Vector3.zero, 2).OnComplete(TestingCompleteMethod);
        myTween.SetAutoKill(false);
    }

    private void Update()
    {
        if(timer != 69)
        {
            timer -= Time.deltaTime;
            Debug.Log(timer);
        }

        if (timer <= 0)
        {
            myTween.Rewind(true);
            timer = 69;
            Debug.Log("Rewind");
        }
    }
    void TestingCompleteMethod()
    {
        Debug.Log("The tween was completed");
    }

}