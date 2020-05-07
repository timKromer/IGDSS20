using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Texture2D heightmap;

    public GameObject water;
    public GameObject sand;
    public GameObject grass;
    public GameObject forest;
    public GameObject stone;
    public GameObject mountain;

    private float multiplier = 5;
    private float h_mult = 20;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: For each Pixel instantiate a tile
        // Tiles have to be placed in xz-plane
        // Tiles have to be placed in y-axis

        for (int x = 1; x <= heightmap.width; x++)
        {
            for (int z = 1; z <= heightmap.height; z++)
            {
                float height = heightmap.GetPixel(x - 1, z - 1).grayscale;
                Instantiate(HeightToTile(height), new Vector3((heightmap.width/2 -x)*10 + (10/6f)*(x-1), h_mult*height, (z-heightmap.height/2)* 10 + 5 * (x % 2)), Quaternion.identity);
            }
        }
        
    }

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
