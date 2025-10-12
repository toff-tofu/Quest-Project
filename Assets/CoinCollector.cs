using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    public static int Coins = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CollectCoin()
    {
        gameObject.GetComponent<AudioSource>().Play();
        Coins ++;
    }
}
