using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    #region Basic Attributes
    public string _type; //The name of the building
    public float _upkeep; //The money cost per minute
    public float _buildCostMoney; //placement money cost
    public float _buildCostPlanks; //placement planks cost
    public Tile _tile; //Reference to the tile it is built on
    protected List<Tile> _neighborTiles; //List of all neighboring tiles, derived from _tile
    #endregion

    #region Tile Restrictions
    public List<Tile.TileTypes> _canBeBuiltOnTileTypes; //A list that defines all types of tiles it can be placed on. Increase the number in the inspector and then choose from the drop-down menu
    #endregion

    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    #endregion
    
    #region Workers
    public List<Worker> _workers; //List of all workers associated with this building, either for work or living
    #endregion

    #region Jobs
    public List<Job> _jobs; // List of all available Jobs. Is populated in Start()
    #endregion
    

    #region Methods   
    protected void PublishJobOffers()
    {
        foreach (Job job in _jobs)
        {
            _jobManager.InsertJobOffer(job);
        }
    }
    public void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
    }

    public void WorkerRemovedFromBuilding(Worker w)
    {
        _workers.Remove(w);
    }
    #endregion

    #region MonoBehaviour
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _neighborTiles = _tile._neighborTiles;
        PublishJobOffers();
    }
    #endregion
}
