using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAnimationEvent : MonoBehaviour
{
    public GameObject trueCoin;

    public void RevealCoin() {
        trueCoin.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
