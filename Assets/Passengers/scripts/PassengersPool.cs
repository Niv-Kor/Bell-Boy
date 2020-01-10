using System;
using System.Collections.Generic;
using UnityEngine;

public class PassengersPool : MonoBehaviour, IPoolable<GameObject>
{
    [Serializable]
    public struct PassengerData
    {
        [Tooltip("The prefab of the passenger.")]
        [SerializeField] public GameObject prefab;

        [Tooltip("The exact passenger with which the prefab corresponds.")]
        public PassengerPersona persona;
    }

    [Tooltip("A list of all spawnable passengers.")]
    [SerializeField] private List<PassengerData> passengers;

    private IDictionary<PassengerPersona, List<GameObject>> freePassengers, occupiedPassengers;
    private PassengerPersona[] availablePersonas;

    private void Start() {
        this.freePassengers = new Dictionary<PassengerPersona, List<GameObject>>();
        this.occupiedPassengers = new Dictionary<PassengerPersona, List<GameObject>>();
        this.availablePersonas = new PassengerPersona[passengers.Count];

        //retrieve all available personas
        for (int i = 0; i < passengers.Count; i++)
            availablePersonas[i] = passengers[i].persona;

        //init
        InitDictionary(freePassengers);
        InitDictionary(occupiedPassengers);
    }

    /// <summary>
    /// Initialize a dictionary's entries with empty lists.
    /// </summary>
    /// <param name="dictionary">The dictionary to initialize.</param>
    private void InitDictionary(IDictionary<PassengerPersona, List<GameObject>> dictionary) {
        foreach (PassengerPersona persona in availablePersonas)
            dictionary.Add(persona, new List<GameObject>());
    }

    /// <param name="persona">The corresponds persona</param>
    /// <returns>The original prefab of the persona.</returns>
    private GameObject GetPrefab(PassengerPersona persona) {
        foreach (PassengerData data in passengers)
            if (data.persona == persona) return data.prefab;

        return null;
    }

    /// <returns>A random persona out of the available prefabs.</returns>
    private PassengerPersona GeneratePersona() {
        int length = availablePersonas.Length;
        int selected = UnityEngine.Random.Range(0, length);
        return availablePersonas[selected];
    }

    public GameObject Lease() {
        return Lease(GeneratePersona());
    }

    /// <param name="persona">The corresponds persona</param>
    /// <seealso cref="Lease"/>
    public GameObject Lease(PassengerPersona persona) {
        List<GameObject> cacheList = freePassengers[persona];
        GameObject selectedObj;

        //instantiate new
        if (cacheList.Count == 0) selectedObj = Instantiate(GetPrefab(persona));
        //retrieve from cache
        else selectedObj = CollectionsUtil.SelectRandom(cacheList);

        //transfer between chaches
        selectedObj.SetActive(true);
        occupiedPassengers[persona].Add(selectedObj);
        cacheList.Remove(selectedObj);

        return selectedObj;
    }

    public GameObject Clone() {
        return Clone(GeneratePersona());
    }

    /// <param name="persona">The corresponds persona</param>
    /// <seealso cref="Clone"/>
    public GameObject Clone(PassengerPersona persona) {
        return Instantiate(GetPrefab(persona));
    }

    public void Free(GameObject obj) {
        PassengerPersona foundPersona = default;
        bool found = false;
        
        foreach (PassengerPersona persona in occupiedPassengers.Keys) {
            List<GameObject> list = occupiedPassengers[persona];
            
            if (list.Contains(obj)) {
                foundPersona = persona;
                list.Remove(obj);
                found = true;
                break;
            }
        }

        //add the object back to the free chache
        if (found) {
            obj.GetComponent<Passenger>().Reset();
            obj.transform.position = Vector3.zero;
            freePassengers[foundPersona].Add(obj);
            obj.SetActive(false);
        }
    }
}