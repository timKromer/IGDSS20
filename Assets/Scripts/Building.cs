using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{

    public enum BuildingType { None, Fishery, Lumberjack, Sawmill, SheepFarm, FrameworkKnitters, PotatoFarm, SchnappsDistillery };
    public BuildingType _type;
    public int _upkeep;
    public int _buildCostMoney;
    public int _buildCostPlanks;
    public Tile _tile;

    public float _efficiencyValue;
    public float _resourceGenerationInterval;
    public int _outputCount;

    public List<Tile.TileTypes> _canBeBuiltOnTileTypes;
    public Tile.TileTypes _efficiencyScalesWithNeighbouringTiles;
    public int _minimumNeighbours;
    public int _maximumNeighbours;

    public List<GameManager.ResourceTypes> _inputResources;
    public GameManager.ResourceTypes _outputResource;

    private Dictionary<GameManager.ResourceTypes, float> _warehouse;
    private float _timeSinceLastProduction = 0;
    private bool _producing = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ProductionCycle();
    }

    public void SetWareHouse(Dictionary<GameManager.ResourceTypes, float> wh)
    {
        _warehouse = wh;
    }

    bool CheckNeededResources()
    {
        bool gotResources = true;
        foreach (GameManager.ResourceTypes resource in _inputResources)
        {
            gotResources = gotResources && (_warehouse[resource] > 0);
        }
        return gotResources;
    }

    void GetNeededResources()
    {
        foreach (GameManager.ResourceTypes resource in _inputResources)
        {
            _warehouse[resource] -= 1;
        }
    }

    void ProductionCycle()
    {
        DetermineProductionScaling();
        if (!_producing)
        {
            if (CheckNeededResources())
            {
                GetNeededResources();
                _producing = true;
                Debug.Log("Begun Production Cycle");
            }
            else
            {
                _timeSinceLastProduction = 0;
            }
        }
        if (_producing)
        {
            _timeSinceLastProduction += Time.deltaTime * _efficiencyValue;
            if (_timeSinceLastProduction >= _resourceGenerationInterval)
            {
                //End ProductionCycle
                _warehouse[_outputResource] += 1;
                Debug.Log("Ended Production Cycle:"+ _warehouse[_outputResource]);
                _producing = false;
                _timeSinceLastProduction -= _resourceGenerationInterval;
            }
        }
    }

    void DetermineProductionScaling()
    {
        //Building doesnt scale
        if (_maximumNeighbours == 0)
        {
            _efficiencyValue = 1f;
            return;
        }
        //Determine number of scaling-Tiles
        int scalingTiles = 0;
        foreach (Tile tile in _tile._neighborTiles)
        {
            if (tile._type == _efficiencyScalesWithNeighbouringTiles)
            {
                scalingTiles += 1;
            }
        }
        //Set efficiencyValue
        if (scalingTiles < _minimumNeighbours)
        {
            _efficiencyValue = 0;
        }
        else
        {
            _efficiencyValue = (((float)_maximumNeighbours) / Mathf.Min(scalingTiles, _maximumNeighbours));
        }
    }
}
