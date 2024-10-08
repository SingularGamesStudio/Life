using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
using System.Linq;

public class Planet : MonoBehaviour {
    public Tree Root;
    public int Size = 256;
    public bool DebugMode;
    public PlanetRenderer Renderer;
    public int[] BiomeByPos;
    public int PlanetRadius = 128;
    public int seed = 42;

    
    void Start() {
        Root = new Tree(Size, this, new PixelState(0, true));
		InitPlanet(seed);
        Root.BuildBiome();
    }
    private void Update() {
        if (Input.GetMouseButtonUp(0)) {//TODO: doesnt work
            Vector2 Pos = Utils.TransformPos(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform, Size);
            List<EdgePoint> Edge = GetEdge(new Vector2Int((int)Pos.x, (int)Pos.y), 300, 8);
            if (Edge != null) {
                List<float> Curve;
                Curve = Curves.Curve_Square(100, 300);
                //Curve = Curves.Curve_Perlin(25, 100, 300);
                List<Vector2> NewEdge = Curves.Shift(Edge, Curve);
                
                Transform(Edge, NewEdge);
            }
        }
    }

    public Rect GetRect()
    {
        return Root.GetRect();
    }

    public void InitPlanet(int seed)
	{
        System.Random rnd = new System.Random(seed);
        int Length = (int)(PlanetRadius * Mathf.PI * 2f);
        BiomeByPos = new int[Length+1];
        int iter = 0;
        while (Length > 0) {
            int id = rnd.Next(Data.Main.Biomes.Count);
            while (Data.Main.Biomes[id].MinSize > Length) {
                id = rnd.Next(Data.Main.Biomes.Count);
            }
            Biome biome = new Biome(Data.Main.Biomes[id]);
            if (Data.Main.MinBiomeSize + biome.MinSize > Length)
                biome.Init(rnd.Next(100000), Length, PlanetRadius);
            else biome.Init(rnd.Next(100000), rnd.Next(biome.MinSize, Mathf.Min(Length-Data.Main.MinBiomeSize, biome.MaxSize)), PlanetRadius);
            biome.LeftEdge = iter;
            Biomes.Add(biome);
            Length -= biome.Size;
            for (int i = iter; i < iter + biome.Size; i++) {
                BiomeByPos[i] = Biomes.Count - 1;
            }
            iter += biome.Size;
            
        }
        BiomeByPos[Length] = Biomes.Count-1;
    }
}
