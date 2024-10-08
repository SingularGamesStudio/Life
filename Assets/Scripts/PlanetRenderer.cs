using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRenderer : MonoBehaviour
{
    public Planet What;
    public List<GameObject> Tracking;
    public int VisionRange;
    public int CellSize;// must be a big enough power of 2, more than VisionRange*2
	private const int RenderBatchSize = 1023;
    private Vector3 RendererShift = new Vector3(100, 100, 100);

	public List<RenderArea> Cells = new List<RenderArea>();
    public int TicksPerClear = 30;
    private int Ticks = 0;

    private List<Tree> DelayedDraw = new List<Tree>();

    //Shader params for squares being rendered
    private List<List<List<Matrix4x4>>> Matrices = new List<List<List<Matrix4x4>>>();
    private List<List<List<Vector4>>> ShaderCuts = new List<List<List<Vector4>>>();
    private Queue<int> DrawOrder = new Queue<int>();
    private List<bool> ToBeRendered = new List<bool>();
    public class RenderArea {
        private RenderTexture Tex;
        private Camera Rec;
        private Material Mat;
        public Vector2Int Pos;
        private MeshRenderer Instance;
        public int Size;
        public RenderArea(Vector2Int Pos, int Size, PlanetRenderer Renderer) {
            this.Pos = Pos;
            this.Size = Size;
            Rec = Instantiate(Data.Main.CameraRenderer).GetComponent<Camera>();
            Rec.orthographicSize = Size / 2f / 100f;
            Tex = new RenderTexture(Size, Size, 0);
            Rec.targetTexture = Tex;
            Instance = Instantiate(Data.Main.PlanetRenderer).GetComponent<MeshRenderer>();
            Mat = new Material(Shader.Find("Unlit/Texture"));
            Mat.mainTexture = Tex;
            Instance.material = Mat;
            Rec.gameObject.transform.position = Utils.InverseTransformPos(Pos + new Vector2(Size / 2f, Size / 2f), Renderer.transform, Renderer.What.Size)+new Vector2(100, 100);
            Instance.transform.position = Utils.InverseTransformPos(Pos + new Vector2(Size / 2f, Size / 2f), Renderer.transform, Renderer.What.Size);
            Instance.transform.localScale = Vector3.one/1000f*((float)Size);
        }
        public void Destroy()
        {
            UnityEngine.Object.Destroy(Instance.gameObject);
			UnityEngine.Object.Destroy(Rec.gameObject);
        }
    }
    Mesh BasicPlane;
    private void Awake() {
        for(int i = 0; i<Data.Main.Textures.Count; i++) {
            Matrices.Add(new List<List<Matrix4x4>>());
            ShaderCuts.Add(new List<List<Vector4>>());
            ToBeRendered.Add(false);
        }
		BasicPlane = Data.Main.EmptySprite.GetComponent<MeshFilter>().mesh;
	}

    private void AddToRenderQueue(Tree Square) {
        int id = Square.Color.TextureID;
        if (Square.Color.Active == false)
            id = 1;
        if (!ToBeRendered[id]) {
            ToBeRendered[id] = true;
            DrawOrder.Enqueue(id);
            Matrices[id].Add(new List<Matrix4x4>());
            ShaderCuts[id].Add(new List<Vector4>());
        }
        if (ShaderCuts[id][ShaderCuts[id].Count - 1].Count == RenderBatchSize) {
            Matrices[id].Add(new List<Matrix4x4>());
            ShaderCuts[id].Add(new List<Vector4>());
        }
        ShaderCuts[id][ShaderCuts[id].Count - 1].Add(new Vector4(Square.Pos.x, Square.Pos.y, Square.Size, Square.Size));
        Matrix4x4 tf = Matrix4x4.identity;
        tf.SetTRS((Vector3)Utils.InverseTransformPos(Square.Pos + new Vector2(Square.Size / 2f, Square.Size / 2f), What.transform, What.Size) + RendererShift, Quaternion.Euler(90f, 90f, -90f), new Vector3(Square.Size / 1000f, Square.Size / 1000f, Square.Size / 1000f));
        Matrices[id][Matrices[id].Count - 1].Add(tf);
    }
    
    private void FixedUpdate() {
        Ticks++;
        foreach(Tree Square in DelayedDraw) {
            if (!Square.finishedRender) {
                Square.finishedRender = true;
                if (Square.Color == null)
                    continue;
                foreach (RenderArea Cell in Cells) {
                    Tuple<Vector2, Vector2> Box = Utils.SquaresIntersect(Square.Pos, Square.Size, Cell.Pos, Cell.Size);
                    if (Box == null) continue;
                    AddToRenderQueue(Square);
                    break;//all render areas capture simultaneously, so it is never needed to render twice
                }
            }
        }
        foreach (Tree Square in DelayedDraw) {
            Square.finishedRender = false;
            Square.inRenderQueue= false;
        }
        DelayedDraw.Clear();
        Render();
        
        //update viewed area
        List<RenderArea> added = new List<RenderArea>();
        foreach (GameObject g in Tracking) {//TODO: poor performance, when renderareas are created
            for (int dx = -VisionRange; dx < VisionRange + CellSize; dx += CellSize) {
                for (int dy = -VisionRange; dy < VisionRange + CellSize; dy += CellSize) {
                    Vector2 d = new Vector2(dx, dy);
                    Vector2 Point = Utils.TransformPos((Vector2)g.transform.position, What.transform, What.Size) + d;
                    bool ok = false;
                    foreach (RenderArea Square in Cells) {
                        if (Utils.PointInSquare(Square.Pos, Square.Size, Point)) {
                            ok = true;
                            break;
                        }
                    }
                    if (!ok) {
                        Vector2Int ToSpawn = new Vector2Int((int)Mathf.Floor(Point.x / CellSize) * CellSize, (int)Mathf.Floor(Point.y / CellSize) * CellSize);
                        Cells.Add(new RenderArea(ToSpawn, CellSize, this));
                        added.Add(Cells[Cells.Count - 1]);
                    }
                }
            }
        }
        if (added.Count>0) {
            DrawRecursive(What.Root, added);//TODO:optimize
        }
        
        if(Ticks%TicksPerClear==0) {
			List<RenderArea> newCells = new List<RenderArea>();
			foreach (RenderArea area in Cells) {
                bool needed = false;
                foreach(GameObject g in Tracking) {
                    if(Utils.Distance(Utils.TransformPos((Vector2)g.transform.position, What.transform, What.Size), area.Pos)<(VisionRange+CellSize)*1.5f) { needed = true; break; }
                }
                if(needed) {
                    newCells.Add(area);
                } else {
                    area.Destroy();
                }
            }
            Cells = newCells;
        }
	}
    
    private void Render() {
        foreach (int id in DrawOrder) {
            for (int id2 = 0; id2 < ShaderCuts[id].Count; id2++) {
                ToBeRendered[id] = false;
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetVectorArray("_Rect", ShaderCuts[id][id2]);
                Graphics.DrawMeshInstanced(BasicPlane, 0, Data.Main.Materials[id], Matrices[id][id2], block, UnityEngine.Rendering.ShadowCastingMode.Off, false);
            }
            Matrices[id].Clear();
            ShaderCuts[id].Clear();
        }
        DrawOrder.Clear();
    }
    
    Tree LastRes = null;
    public PixelState GetPixel(int x, int y) {
        
        Vector2 Pos = new Vector2(x, y);
        if (LastRes == null) {
            LastRes = What.Root.Locate(Pos);
            return LastRes.Color;
        } else {
            LastRes = LastRes.LocateUp(Pos);
            return LastRes.Color;
        }
    }
    public PixelState GetPixel(Vector2Int Pos) {
        return What.Root.Locate(Pos).Color;
    }
    /// <summary>
    /// Draws a node of a tree (if the node is not a leaf, does nothing)
    /// Delayed until start of the next frame
    /// </summary>
    /// <param name="Square"></param>
    /// <param name="areas"></param>
    public void Draw(Tree Square) {
        if(!Square.inRenderQueue)
            DelayedDraw.Add(Square);
        Square.inRenderQueue = true;
    }
    /// <summary>
    /// Draws a node of a tree (and children if the node is not filled)
    /// Draws Instantly, could affect performance
    /// </summary>
    /// <param name="Square"></param>
    /// <param name="areas"></param>
    private void DrawRecursive(Tree Square, List<RenderArea> areas) {
        foreach (RenderArea Cell in areas) {
            Tuple<Vector2, Vector2> Box = Utils.SquaresIntersect(Square.Pos, Square.Size, Cell.Pos, Cell.Size);
            if (Box == null) continue;
            if (Square.Color != null) {
                AddToRenderQueue(Square);
                return;
            }
            else {
                foreach (Tree Child in Square.Children) {
                    DrawRecursive(Child, areas);
                }
                return;
            }
        }
    }
}
