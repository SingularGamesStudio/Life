using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Game: MonoBehaviour {
	public Tree Root;
	public int[,] field;
	public int Size = 256;
	public GameRenderer Renderer;
	public int seed = 42;
	public float UpdateTime = 0.5f;
	private float curTime = 0;
	System.Random rnd = new System.Random(42);
	public bool PvP;
	public Player[] players;
	void Start() {
		Root = new Tree(Size, this, new PixelState(0, true));
		Renderer.Init();
		InitPlanet();

		players[0].pos = new Vector2Int(0, 0);
		players[1].pos = new Vector2Int(Size - 1, Size - 1);
		players[0].respawn = new Vector2Int(0, 0);
		players[1].respawn = new Vector2Int(Size - 1, Size - 1);
		players[0].hpMax = 3;
		players[1].hpMax = 3;
		players[0].hp = 3;
		players[1].hp = 3;
	}
	private void Update() {
		curTime += Time.deltaTime;
		if (curTime < UpdateTime) {
			return;
		}
		curTime = 0;

		for (int i = 0; i < players.Length; i++) {
			players[i].TakeTurn();
			int color = field[players[i].pos.x, players[i].pos.y];
			if (color != 0 && color != i + 1) {
				players[i].TakeDamage();
			}
		}

		if (players[0].frozen == 0 && players[1].frozen == 0) {
			PlayerDraw(0, Data.Main.PlayerSize, 1);
			PlayerDraw(1, Data.Main.PlayerSize, 1);
			TickField();
			return;
		}
		if (players[0].frozen == 0) {
			PlayerDraw(0, Data.Main.PlayerSize, 0.3f);
		}
		if (players[1].frozen == 0) {
			PlayerDraw(1, Data.Main.PlayerSize, 0.3f);
		}

	}

	void PlayerDraw(int id, int R, float chanceMultiplier)
	{
		for(int dx = -R; dx<=R; dx++) {
			for(int dy = -R; dy<=R; dy++) {
				if (rnd.Next(1000)<=Data.Main.DrawChance* chanceMultiplier * 1000 &&
					players[id].pos.x + dx >= 0 &&
					players[id].pos.x + dx < Size &&
					players[id].pos.y + dy >= 0 &&
					players[id].pos.y + dy < Size) {
					SetPixel(new Vector2Int(players[id].pos.x+dx, players[id].pos.y+dy), id + 1);
				}
			}
		}
	}

	class Counter
	{
		public int[] players = { 0, 0};
		public void Count(int val)
		{
			if (val != 0) {
				players[val-1]++;
			}
		}
		public int Sum() { return players[0]+players[1]; }
		public int Max() { 
			if (players[0] > players[1]) {
				return 1;
			}
			if (players[1] > players[0]) {
				return 2;
			}
			return 0;
		}
	}

	void TickField()
	{
		int[] prevLine = new int[Size];
		for (int i = 0; i < Size; i++) {
			prevLine[i] = 0;
		}
		for (int i = 0; i < Size; i++) {
			int prev = 0;
			for (int j = 0; j < Size; j++) {
				Counter cnt = new Counter();
				cnt.Count(prevLine[j]);
				if (j != Size - 1) {
					cnt.Count(prevLine[j + 1]);
					cnt.Count(field[i, j + 1]);
					if (i != Size - 1)
						cnt.Count(field[i + 1, j + 1]);
				}
				if (i != Size - 1) {
					cnt.Count(field[i + 1, j]);
					if (j != 0)
						cnt.Count(field[i + 1, j - 1]);
				}
				if (j != 0) {
					cnt.Count(prevLine[j - 1]);
					prevLine[j - 1] = prev;
				}
				prev = field[i, j];
				if (field[i, j] == 0 && cnt.Sum() == 3) {
					SetPixel(new Vector2Int(i, j), cnt.Max());
				} else if (field[i, j] != 0 && cnt.Sum() != 2 && cnt.Sum() != 3) {
					SetPixel(new Vector2Int(i, j), 0);
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
                if (rnd.Next(1000)<Data.Main.InitDrawChance*1000) {
					if(i+j<Size / 5) {
						SetPixel(new Vector2Int(i, j), 1);
					} else if ((Size-i) + (Size-j) < Size / 5) {
						SetPixel(new Vector2Int(i, j), 2);
					} else {
						SetPixel(new Vector2Int(i, j), rnd.Next(2) + 1);
					}
                }
			}
		}
	}

	public void SetPixel(Vector2Int Point, int color)
	{
		if (color!=0) {
			players[color - 1].points++;
		}
		
		Tree Lowest = Root.Locate(Point);
		while (Lowest.Size>1) {
			Lowest.InitChildren(false);
            Lowest = Lowest.Locate(Point);
		}
		field[Point.x, Point.y] = color;
		Lowest.Update(new PixelState(field[Point.x, Point.y], true));
	}

	public void FreezeSpell(int player)
	{
		players[player].SUPER -= Data.Main.FreezeCost;
		players[-player + 1].frozen = Data.Main.FreezeDuration;
	}

	public void ExplodeSpell(int player)
	{
		players[player].SUPER -= Data.Main.ExplodeCost;
		PlayerDraw(player, Data.Main.ExplodeSize, 1.5f);
	}
}
