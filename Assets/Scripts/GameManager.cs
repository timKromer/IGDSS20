using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Texture2D heightmap;

    // The Prefabs used to create the Field
    public GameObject water;
    public GameObject sand;
    public GameObject grass;
    public GameObject forest;
    public GameObject stone;
    public GameObject mountain;
    public MouseManager mouseManager;

    // Sets the HeightScaling for the Interpretation of the heightmaps (max Value of Heightmap = 1)
    // Is choosen according to our zooming-function
    private float h_mult = 20;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // Where to set the Tiles
        float multiplier = 10;
        float xmult = (5 * multiplier / 6f);
        // Border of the field (Center of BorderTiles)
        float xLim = ((heightmap.height - 1) * (5f / 6f) * multiplier) / 2f;
        float zLim = ((heightmap.height - 0.5f) * multiplier) / 2f;
        //Set Tiles
        for (int x = 0; x < heightmap.width; x++)
        {
            for (int z = 0; z < heightmap.height; z++)
            {
                float height = heightmap.GetPixel(z, x).r;
                float xpos = -xLim + (x) * xmult;
                float zpos = -zLim + z * multiplier + ((multiplier / 2f) * (x % 2));
                Instantiate(HeightToTile(height), new Vector3(xpos, h_mult * height, zpos), Quaternion.identity);
            }
        }
        //This sets the size of the terrain
        mouseManager.camLimit = new Vector2(xLim, zLim);
    }

    // Returns a Prefab depending on give height
    GameObject HeightToTile(float height)
    {
        if (height == 0)
        {
            return water;
        }
        if (height <= 0.2)
        {
            return sand;
        }
        if (height <= 0.4)
        {
            return grass;
        }
        if (height <= 0.6)
        {
            return forest;
        }
        if (height <= 0.8)
        {
            return stone;
        }
        return mountain;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
