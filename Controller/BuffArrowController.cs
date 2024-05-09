using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffArrowController : MonoBehaviour
{
    private float curAliveTime = 0f;
    private float aliveTime = 10f;

    void Update()
    {
        TimeElapse();
        SetRotation();
    }
    private void TimeElapse()
    {
        curAliveTime += Time.deltaTime;
        if(curAliveTime >= aliveTime)
            gameObject.SetActive(false);
    }
    private void SetRotation()
    {
        transform.LookAt(GameManager.instance.enemyTreasure.transform.position);
    }
}
