using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Map generation
    public Texture2D heightmap;
    public MouseManager mouseManager;
    public GameObject[] _tiles;
    private Tile[,] _tileMap; //2D array of all spawned tiles
    //MoneyManagement
    public int _money = 0;
    int _sumUpkeep = 0;

    // Sets the HeightScaling for the Interpretation of the heightmaps (max Value of Heightmap = 1)
    // Is choosen according to our zooming-function
    private float h_mult = 20;


    void GenerateMap()
    {
        _tileMap = new Tile[heightmap.width, heightmap.height];
        // Where to set the Tiles
        float multiplier = 10;
        float xmult = (5 * multiplier / 6f);
        // Border of the field (Center of BorderTiles)
        float xLim = ((heightmap.height - 1) * (5f / 6f) * multiplier) / 2f;
        float zLim = ((heightmap.height - 0.5f) * multiplier) / 2f;
        //Set Tiles
        for (int x = 0; x < heightmap.height; x++)
        {
            for (int z = 0; z < heightmap.width; z++)
            {
                float height = heightmap.GetPixel(z, x).r;
                float xpos = -xLim + (x) * xmult;
                float zpos = -zLim + z * multiplier + ((multiplier / 2f) * (x % 2));
                Tile tile = Instantiate(HeightToTile(height), new Vector3(xpos, h_mult * height, zpos), Quaternion.identity).GetComponent<Tile>();
                tile._coordinateHeight = x;
                tile._coordinateWidth = z;
                _tileMap[x, z] = tile;
            }
        }
        //This sets the size of the terrain
        mouseManager.camLimit = new Vector2(xLim, zLim);
        mouseManager.SetGameManager(this);
        SetTileNeighbours();
    }

    void SetTileNeighbours()
    {
        //TODO Vielleicht gibt es noch eine elegantere Variante, bei der nicht so viele Abfragen nötig sind.
        for (int i = 0; i < _tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < _tileMap.GetLength(1); j++)
            {
                Tile tile = _tileMap[i, j];
                tile._neighborTiles = FindNeighborsOfTile(tile);
            }
        }
    }

    // Returns a Prefab depending on give height
    GameObject HeightToTile(float height)
    {
        if (height == 0)
        {
            return _tiles[0];
        }
        if (height <= 0.2)
        {
            return _tiles[1];
        }
        if (height <= 0.4)
        {
            return _tiles[2];
        }
        if (height <= 0.6)
        {
            return _tiles[3];
        }
        if (height <= 0.8)
        {
            return _tiles[4];
        }
        return _tiles[5];
    }
    #endregion

    #region Buildings
    public GameObject[] _buildingPrefabs; //References to the building prefabs
    public int _selectedBuildingPrefabIndex = 0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list
    #endregion


    #region Resources
    private Dictionary<ResourceTypes, float> _resourcesInWarehouse = new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType

    //A representation of _resourcesInWarehouse, broken into individual floats. Only for display in inspector, will be removed and replaced with UI later
    [SerializeField]
    private float _ResourcesInWarehouse_Fish;
    [SerializeField]
    private float _ResourcesInWarehouse_Wood;
    [SerializeField]
    private float _ResourcesInWarehouse_Planks;
    [SerializeField]
    private float _ResourcesInWarehouse_Wool;
    [SerializeField]
    private float _ResourcesInWarehouse_Clothes;
    [SerializeField]
    private float _ResourcesInWarehouse_Potato;
    [SerializeField]
    private float _ResourcesInWarehouse_Schnapps;
    #endregion

    #region Enumerations
    public enum ResourceTypes { None, Fish, Wood, Planks, Wool, Clothes, Potato, Schnapps }; //Enumeration of all available resource types. Can be addressed from other scripts by calling GameManager.ResourceTypes
    #endregion

    private float _productionCycle;

    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {
        _productionCycle = Time.time;
        GenerateMap();
        PopulateResourceDictionary();
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
        UpdateInspectorNumbersForResources();
        UpdateMoney();
    }
    #endregion

    #region Methods
    void UpdateMoney()
    {
        int tick = 5;
        _productionCycle += Time.deltaTime;
        //When money-cycle ends
        if(_productionCycle >= tick)
        {
            _productionCycle = _productionCycle % tick;
            _money += 100 - _sumUpkeep;
        }
    }

    //Makes the resource dictionary usable by populating the values and keys
    void PopulateResourceDictionary()
    {
        _resourcesInWarehouse.Add(ResourceTypes.None, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Fish, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wood, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Planks, 5);
        _resourcesInWarehouse.Add(ResourceTypes.Wool, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Clothes, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Potato, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Schnapps, 0);
    }

    //Sets the index for the currently selected building prefab by checking key presses on the numbers 1 to 0
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedBuildingPrefabIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedBuildingPrefabIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedBuildingPrefabIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedBuildingPrefabIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _selectedBuildingPrefabIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _selectedBuildingPrefabIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _selectedBuildingPrefabIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _selectedBuildingPrefabIndex = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _selectedBuildingPrefabIndex = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _selectedBuildingPrefabIndex = 9;
        }
    }

    //Updates the visual representation of the resource dictionary in the inspector. Only for debugging
    void UpdateInspectorNumbersForResources()
    {
        _ResourcesInWarehouse_Fish = _resourcesInWarehouse[ResourceTypes.Fish];
        _ResourcesInWarehouse_Wood = _resourcesInWarehouse[ResourceTypes.Wood];
        _ResourcesInWarehouse_Planks = _resourcesInWarehouse[ResourceTypes.Planks];
        _ResourcesInWarehouse_Wool = _resourcesInWarehouse[ResourceTypes.Wool];
        _ResourcesInWarehouse_Clothes = _resourcesInWarehouse[ResourceTypes.Clothes];
        _ResourcesInWarehouse_Potato = _resourcesInWarehouse[ResourceTypes.Potato];
        _ResourcesInWarehouse_Schnapps = _resourcesInWarehouse[ResourceTypes.Schnapps];
    }

    //Checks if there is at least one material for the queried resource type in the warehouse
    public bool HasResourceInWarehoues(ResourceTypes resource)
    {
        return _resourcesInWarehouse[resource] >= 1;
    }

    //Is called by MouseManager when a tile was clicked
    //Forwards the tile to the method for spawning buildings
    public void TileClicked(int height, int width)
    {
        Tile t = _tileMap[height, width];
        Debug.Log(t._type + " " + height + " " + width);
        if (t._building == null)
        {
            PlaceBuildingOnTile(t);
        }
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void PlaceBuildingOnTile(Tile t)
    {
        //if there is building prefab for the number input
        if (_selectedBuildingPrefabIndex < _buildingPrefabs.Length)
        {
            //TODO: check if building can be placed and then istantiate it
            Building building = _buildingPrefabs[_selectedBuildingPrefabIndex].GetComponent<Building>();
            bool enoughMats = _money >= building._buildCostMoney && _resourcesInWarehouse[ResourceTypes.Planks] >= building._buildCostPlanks;
            if (building._canBeBuiltOnTileTypes.Contains(t._type) && enoughMats)
            {
                Building instanciated = Instantiate(_buildingPrefabs[_selectedBuildingPrefabIndex], t.transform.position, Quaternion.identity).GetComponent<Building>();
                instanciated.SetWareHouse(_resourcesInWarehouse);
                instanciated._tile = t;
                t._building = instanciated;
                _sumUpkeep += instanciated._upkeep;
                _money -= building._buildCostMoney;
                _resourcesInWarehouse[ResourceTypes.Planks] -= building._buildCostPlanks;
            }

        }
    }

    //Returns a list of all neighbors of a given tile
    private List<Tile> FindNeighborsOfTile(Tile t)
    {
        List<Tile> result = new List<Tile>();
        int j = t._coordinateHeight;
        int i = t._coordinateWidth;
        //TODO: put all neighbors in the result list
        //grundsätzliches Muster
        if (j > 0)
        {
            //Above Neighbours
            if (i + j % 2 > 0)
            {
                result.Add(_tileMap[j - 1, i - 1 + (j % 2)]);
            }
            if (i + j % 2 < _tileMap.GetLength(0))
            {
                result.Add(_tileMap[j - 1, i + (j % 2)]);
            }
        }
        //BesideNeighbours
        if (i > 0)
        {
            result.Add(_tileMap[j, i - 1]);
        }
        if (i < _tileMap.GetLength(0) - 1)
        {
            result.Add(_tileMap[j, i + 1]);
        }

        if (j < _tileMap.GetLength(1) - 1)
        {
            //Below Neighbours
            if (i + j % 2 > 0)
            {
                result.Add(_tileMap[j + 1, i - 1 + (j % 2)]);
            }
            if (i + j % 2 < _tileMap.GetLength(0))
            {
                result.Add(_tileMap[j + 1, i + (j % 2)]);
            }
        }
        return result;
    }
    #endregion
}
