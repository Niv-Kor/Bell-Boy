using System.Collections.Generic;
using UnityEngine;

public class CoinsPool : MonoBehaviour, IPoolable<Coin>
{
    [Tooltip("The prefab of the tossed coin.")]
    [SerializeField] private GameObject coin;

    private static readonly string COINS_PARENT_NAME = "Coins";

    private List<Coin> freeCoins;
    private GameObject coinsParent;

    private void Start() {
        this.freeCoins = new List<Coin>();
        this.coinsParent = new GameObject(COINS_PARENT_NAME);
        coinsParent.transform.SetParent(transform);
    }

    public Coin Lease() {
        Coin coin = Clone();
        freeCoins.Remove(coin);
        return coin;
    }

    public Coin Clone() {
        Coin coin = (freeCoins.Count > 0) ? CollectionsUtil.SelectRandom(freeCoins) : CreateNew();
        coin.gameObject.SetActive(true);
        return coin;
    }

    public void Free(Coin obj) {
        if (!freeCoins.Contains(obj)) freeCoins.Add(obj);
    }

    /// <summary>
    /// Instantiate a new instance of a coin.
    /// </summary>
    /// <returns>The new created coin instance.</returns>
    private Coin CreateNew() {
        GameObject coinInstance = Instantiate(coin);
        Coin coinComponent = coinInstance.GetComponent<Coin>();
        coinInstance.transform.SetParent(coinsParent.transform);
        freeCoins.Add(coinComponent);
        return coinComponent;
    }
}
