using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static Data Main;
    public List<Material> Materials;
    public GameObject PlanetRenderer;
    public GameObject CameraRenderer;
    public GameObject EmptySprite;
    public GameObject PlayerMarker;
    public readonly Vector2Int[] Shifts01 = { new Vector2Int(1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) };
    public readonly Vector2Int[] ShiftsLR = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };

	public int PlayerSize = 4;

	public int FreezeCost = 150;
	public int FreezeDuration = 20;

	public int ExplodeCost = 70;
    public int ExplodeSize = 10;

    public float DrawChance = 0.5f;

    public float InitDrawChance = 0f;

    public int SUPERmax = 600;

    public int winCon = 5000;
	private void Awake() {
        Main = this;
    }
}
