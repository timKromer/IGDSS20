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

    // Update is called once per frame
    void Update()
    {
        ProductionCycle();
    }

    // Is used in the Gamemanager to connect the warehouse system to each building
    public void SetWareHouse(Dictionary<GameManager.ResourceTypes, float> wh)
    {
        _warehouse = wh;
    }

    // Returns true if all needed Resources (for beginning production) are avaible in the warehouse
    bool CheckNeededResources()
    {
        bool gotResources = true;
        foreach (GameManager.ResourceTypes resource in _inputResources)
        {
            gotResources = gotResources && (_warehouse[resource] > 0);
        }
        return gotResources;
    }

    // Decrements all Resources needed for producing
    void GetNeededResources()
    {
        foreach (GameManager.ResourceTypes resource in _inputResources)
        {
            _warehouse[resource] -= 1;
        }
    }

    void ProductionCycle()
    {
        if (!_producing)
        {
            if (CheckNeededResources())
            {
                GetNeededResources();
                _producing = true;
            }
            else
            {
                _timeSinceLastProduction = 0;
            }
        }
        if (_producing)
        {
            _timeSinceLastProduction += Time.deltaTime * _efficiencyValue;
            //End ProductionCycle
            if (_timeSinceLastProduction >= _resourceGenerationInterval)
            {
                _warehouse[_outputResource] += _outputCount;
                _timeSinceLastProduction -= _resourceGenerationInterval;
                _producing = false;
            }
        }
    }

    // Returns if a given tile increases the scaling
    //  -> needed tile type and nothing build on it
    bool ScaleTest(Tile tile)
    {
        return (tile._type == _efficiencyScalesWithNeighbouringTiles && tile._building == null);
    }

    // Determines and sets the _efficiencyValue
    public void DetermineProductionScaling()
    {
        // if this building has no scaling property
        if (_maximumNeighbours == 0)
        {
            _efficiencyValue = 1f;
            return;
        }
        // Determine number of scaling-Tiles
        int nr_scaling = _tile._neighborTiles.FindAll(ScaleTest).Count;
        // Set _efficiencyValue depending on nr_scaling and scaling properties
        if (nr_scaling < _minimumNeighbours)
        {
            _efficiencyValue = 0;
        }
        else
        {
            _efficiencyValue = ((Mathf.Min(nr_scaling, _maximumNeighbours)) / (float)_maximumNeighbours);
        }
    }
}
