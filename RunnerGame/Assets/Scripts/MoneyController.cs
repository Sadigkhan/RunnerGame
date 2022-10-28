using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MoneyController : MonoBehaviour
{

    
    void Start()
    {
        transform.DORotate(new Vector3(0,90,0),1f).SetLoops(-1,LoopType.Incremental);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
