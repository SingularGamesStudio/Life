using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
using System.Linq;

public class Planet : MonoBehaviour {
    public Tree Root;
    public int Size = 256;
    public PlanetRenderer Renderer;
    public int seed = 42;

    
    void Start() {
        Root = new Tree(Size, this, new PixelState(0, true));
		Renderer.Init();
		InitPlanet();
    }
    private void FixedUpdate() {
        
    }

    public Rect GetRect()
    {
        return Root.GetRect();
    }

    public void InitPlanet()
	{
        
    }
}
