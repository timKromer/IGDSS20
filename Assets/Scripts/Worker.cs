using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public static Dictionary<GameManager.ResourceTypes, float> hungers = new Dictionary<GameManager.ResourceTypes, float>()
    {
        {GameManager.ResourceTypes.Fish, 0.3f},
        {GameManager.ResourceTypes.Clothes, 0.2f},
        {GameManager.ResourceTypes.Schnapps, 0.2f}
    };

    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    GameManager _gameManager;//Reference to the GameManager
    #endregion

    public float _age; // The age of this worker
    public float _happiness; // The happiness of this worker
    public float Happiness => _happiness;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Age();
        UpdateHappiness();
    }

    void UpdateHappiness()
    {
        float currentHappiness = 0;

        foreach (KeyValuePair<GameManager.ResourceTypes, float> hunger in hungers)
        {
            currentHappiness += hunger.Value * _gameManager.getNeedSatisfaction(hunger.Key);
        }

        if (_jobManager.IsEmployed(this)) {
            currentHappiness += 0.3f;
        }

        _happiness = _happiness * (1 - _gameManager.timeOfAYearPassed())
            + currentHappiness * (_gameManager.timeOfAYearPassed());
    }

    private void Age()
    {
        //TODO: Implement a life cycle, where a Worker ages by 1 year every 15 real seconds.
        //When becoming of age, the worker enters the job market, and leaves it when retiring.
        //Eventually, the worker dies and leaves an empty space in his home. His Job occupation is also freed up.
        _age = _age + _gameManager.timeOfAYearPassed();

        if (_age > 14)
        {
            BecomeOfAge();
        }

        if (_age > 64)
        {
            Retire();
        }

        if (_age > 100)
        {
            Die();
        }
    }


    public void BecomeOfAge()
    {
        _jobManager.RegisterWorker(this);
    }

    private void Retire()
    {
        _jobManager.RemoveWorker(this);
    }

    private void Die()
    {
        _jobManager.RemoveWorker(this);
        Destroy(this.gameObject, 1f);
    }
}
