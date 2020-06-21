using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBuilding : Building
{
    GameObject _workerPrefab;
    public GameManager _gameManager;
    float _lastBirth = 0;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _gameManager = GameManager.Instance;
        _workerPrefab = GameObject.Find("Farmer Residence");
        SpawnWorker();
        SpawnWorker();
    }

    // Update is called once per frame
    void Update()
    {
      CheckForNewborn();   
    }

    private void CheckForNewborn()
    {
        float periodBetweenBirths = GetAverageSatisfaction() * Time.deltaTime / 30;
        
        if (_gameManager.Year - _lastBirth >= periodBetweenBirths)
        {
            SpawnWorker();
        }
    }

    void SpawnWorker()
    {
        if (_workers.Count <= 9)
        {
            GameObject workerGameObject = Instantiate(_workerPrefab, this.transform.position, Quaternion.identity);
            _workers.Add(workerGameObject.GetComponent<Worker>());
        }
    }

    private float GetAverageSatisfaction()
    {
        float summedSatisfaction = 0;
        foreach(Worker worker in _workers)
        {
            summedSatisfaction += worker.Happiness;
        }

        return summedSatisfaction / _workers.Count;
    }
}
