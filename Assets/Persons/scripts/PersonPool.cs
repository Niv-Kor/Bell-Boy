using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersonPool : MonoBehaviour, IPoolable<GameObject>
{
    [Serializable]
    public struct PassengerData
    {
        [Tooltip("The prefab of the passenger.")]
        [SerializeField] public GameObject Prefab;

        [Tooltip("The exact passenger with which the prefab corresponds.")]
        [SerializeField] public Persona Persona;

        [Tooltip("The percentage at which the passenger will be spontaneously spawned.")]
        [SerializeField] [Range(0f, 1f)] public float SpawnChance;
    }

    [Tooltip("A list of all spawnable passengers.")]
    [SerializeField] private List<PassengerData> passengers;

    private IDictionary<Persona, List<GameObject>> freePersons, occupiedPersons;
    private List<Persona> availablePersonas;

    private void Start() {
        this.freePersons = new Dictionary<Persona, List<GameObject>>();
        this.occupiedPersons = new Dictionary<Persona, List<GameObject>>();
        this.availablePersonas = new List<Persona>();

        //create a personas pool
        foreach (PassengerData data in passengers) {
            int personaSize = (int) (data.SpawnChance * 100);

            for (int i = 0; i < personaSize; i++)
                availablePersonas.Add(data.Persona);
        }

        //init
        InitDictionary(freePersons);
        InitDictionary(occupiedPersons);
    }

    /// <summary>
    /// Initialize a dictionary's entries with empty lists.
    /// </summary>
    /// <param name="dictionary">The dictionary to initialize.</param>
    private void InitDictionary(IDictionary<Persona, List<GameObject>> dictionary) {
        foreach (Persona persona in availablePersonas.Distinct())
            dictionary.Add(persona, new List<GameObject>());
    }

    /// <param name="persona">The corresponds persona</param>
    /// <returns>The original prefab of the persona.</returns>
    private GameObject GetPrefab(Persona persona) {
        foreach (PassengerData data in passengers)
            if (data.Persona == persona) return data.Prefab;

        return null;
    }

    /// <summary>
    /// Get a random available persona to spawn.
    /// If no persona can be spawned at the moment, return a default value.
    /// </summary>
    /// <returns>A random persona out of the available prefabs.</returns>
    private Persona GeneratePersona() {
        HashSet<Persona> checkedPersonas = new HashSet<Persona>();
        bool nonAvailable = false;
        bool canBeSpawned;
        Persona persona;

        //find a persona the can be spawned
        do {
            persona = CollectionsUtil.SelectRandom(availablePersonas);
            canBeSpawned = GetPrefab(persona).GetComponent<Passenger>().CanBeSpawned();
            checkedPersonas.Add(persona);

            if (checkedPersonas.Count == passengers.Count) {
                nonAvailable = true;
                break;
            }
        }
        while (!canBeSpawned);

        if (!canBeSpawned && nonAvailable) return default;
        else return persona;
    }

    public GameObject Lease() {
        return Lease(GeneratePersona());
    }

    /// <param name="persona">The corresponds persona</param>
    /// <seealso cref="Lease"/>
    public GameObject Lease(Persona persona) {
        List<GameObject> cacheList = freePersons[persona];
        GameObject selectedObj;

        //instantiate new
        if (cacheList.Count == 0) selectedObj = Instantiate(GetPrefab(persona));
        //retrieve from cache
        else selectedObj = CollectionsUtil.SelectRandom(cacheList);

        //transfer between chaches
        selectedObj.SetActive(true);
        occupiedPersons[persona].Add(selectedObj);
        cacheList.Remove(selectedObj);

        return selectedObj;
    }

    public GameObject Clone() {
        return Clone(GeneratePersona());
    }

    /// <param name="persona">The corresponds persona</param>
    /// <seealso cref="Clone"/>
    public GameObject Clone(Persona persona) {
        return Instantiate(GetPrefab(persona));
    }

    public void Free(GameObject obj) {
        Persona foundPersona = default;
        bool found = false;
        
        foreach (Persona persona in occupiedPersons.Keys) {
            List<GameObject> list = occupiedPersons[persona];
            
            if (list.Contains(obj)) {
                foundPersona = persona;
                list.Remove(obj);
                found = true;
                break;
            }
        }

        //add the object back to the free chache
        if (found) {
            PersonRole role = obj.GetComponent<PersonInlayer>().Role;

            //reset person
            switch (role) {
                case PersonRole.Passenger: obj.GetComponent<Passenger>().Reset(); break;
                case PersonRole.Pedestrian: obj.GetComponent<Pedestrian>().Reset(); break;
            }

            obj.transform.position = SpawnController.Instance.NeutralPoint;
            freePersons[foundPersona].Add(obj);
        }
    }
}