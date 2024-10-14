using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Planet : MonoBehaviour {
	public Text TPSMeter;
    public Tree Root;
    public int[,] field;
    public int Size = 256;
    public PlanetRenderer Renderer;
    public int seed = 42;
    public float UpdateTime = 0.5f;
    private float curTime = 0;
    System.Random rnd = new System.Random(42);
	int Ticks = 0;
	float TimeSinceStart = 0;
    void Start() {
        Root = new Tree(Size, this, new PixelState(0, true));
		Renderer.Init();
		InitPlanet();
    }
    private void Update() {
        curTime += Time.deltaTime;
        if (curTime<UpdateTime) {
            return;
        }
		Ticks++;
		TimeSinceStart += curTime;
		TPSMeter.text = "TPS: "+(Ticks/TimeSinceStart).ToString();
        curTime = 0;

		int[] prevLine = new int[Size];
		for (int i = 0; i < Size; i++) {
			prevLine[i] = 0;
		}
		for (int i = 0; i < Size; i++) {
			int prev = 0;
			for (int j = 0; j < Size; j++) {
				int cnt = prev;
				cnt += prevLine[j];
				if (j != Size - 1) {
					cnt += prevLine[j + 1];
					cnt += field[i,j + 1];
					if (i!=Size-1) 
						cnt += field[i+1, j + 1];
				}
				if (i != Size - 1) {
					cnt += field[i + 1, j];
					if (j != 0)
						cnt += field[i+1, j - 1];
				}
				if (j != 0) {
					cnt += prevLine[j - 1];
					prevLine[j - 1] = prev;
				}
				prev = field[i, j];
				if (field[i,j]==0 && cnt==3) {
					field[i, j] = 1;
					SetPixel(new Vector2Int(i, j));
				} else if (field[i, j] == 1 && cnt!=2 && cnt!=3) {
					field[i, j] = 0;
					SetPixel(new Vector2Int(i, j));
				}
			}
		}
	}

    public void InitPlanet()
	{
		field = new int[Size, Size];
		for (int i = 0; i < Size; i++) {
			for (int j = 0; j < Size; j++) {
				field[i, j] = 0;
                if (rnd.Next(Size)<Size/2) {
					field[i, j] = 1;
					SetPixel(new Vector2Int(i, j));
                }
			}
		}
	}

	public void SetPixel(Vector2Int Point)
	{
		Tree Lowest = Root.Locate(Point);
		while (Lowest.Size>1) {
			Lowest.InitChildren(false);
            Lowest = Lowest.Locate(Point);
		}
        Lowest.Update(new PixelState(field[Point.x, Point.y], true));
	}
}
