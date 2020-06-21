using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JobManager : MonoBehaviour
{

    private List<Job> _availableJobs = new List<Job>();
    private List<Job> _occupiedJobs = new List<Job>();
    public List<Worker> _unoccupiedWorkers = new List<Worker>();



    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleUnoccupiedWorkers();
    }
    #endregion


    #region Methods
    public void InsertJobOffer(Job job)
    {
        if (_availableJobs.Contains(job)) {
            throw new System.Exception($"{job} was already inserted!");
        }

        _availableJobs.Add(job);
    }
    private void HandleUnoccupiedWorkers()
    {
        // assign workers to jobs following FIFO
        foreach (Worker worker in _unoccupiedWorkers)
        {
            Job job = _availableJobs[0];
            job.AssignWorker(worker);
            _occupiedJobs.Add(job);
            _availableJobs.RemoveAt(0);
        }
    }

    public void RegisterWorker(Worker w)
    {
        _unoccupiedWorkers.Add(w);
    }

    public bool IsEmployed(Worker worker)
    {
        return !_unoccupiedWorkers.Contains(worker);
    }

    public void RemoveWorker(Worker w)
    {
        bool workerWasUnemployed = _unoccupiedWorkers.Remove(w);
        
        if (!workerWasUnemployed)
        {
            Job jobToReoccupy = _occupiedJobs.Where(job => job._worker.Equals(w)).First();
            _occupiedJobs.Remove(jobToReoccupy);
            jobToReoccupy._worker = null;
            _availableJobs.Add(jobToReoccupy);
        }
    }

    #endregion
}
