using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Tree {
    //structure
    public Tree Parent = null;
    public Game Root = null;
	public Tree[] Children = new Tree[4];//numbered as basic quarters of the plane (right-up, left-up, left-down, right-down)
    //shape
	public Vector2Int Pos = new Vector2Int(0, 0);
    public int Size = 0;
    //contents
    public PixelState Color = null;
	[HideInInspector]
    public bool finishedRender = false; 
	[HideInInspector]
	public bool inRenderQueue = false;

    public Tree(Tree Parent, Vector2Int Pos, bool render = true) {
        this.Parent = Parent;
        this.Pos = Pos;
        this.Size = Parent.Size / 2;
        this.Root = Parent.Root;
        this.Color = Parent.Color;
        if(render)
            TryUpdate(Parent.Color);
	}
    /// <summary>
    /// Initialize root of a tree
    /// </summary>
    public Tree(int Size, Game Root, PixelState Color, bool render = true) {
        this.Parent = null;
        this.Pos = new Vector2Int(0, 0);
        this.Size = Size;
        this.Root = Root;
		this.Color = Color;
		if (render)
			TryUpdate(Color);
	}

    public Rect GetRect()
    {
        return new Rect(Utils.InverseTransformPos(Pos, Root.transform, Root.Size), Utils.InverseTransformPos(Pos + new Vector2(Size, Size), Root.transform, Root.Size) - Utils.InverseTransformPos(Pos, Root.transform, Root.Size));
    }

    /// <summary>
    /// creates children in the tree and propagates the color to them
    /// </summary>
	public void InitChildren(bool render = true) {
        if (Color == null) {
            Debug.LogError("Color not defined");
            return;
        }
        for (int i = 0; i < Children.Length; i++) {
            Children[i] = new Tree(this, Pos + Data.Main.Shifts01[i] * (Size / 2), render);
        }
        Color = null;
    }

    /// <summary>
    /// find a leaf containing given point
    /// </summary>
    public Tree Locate(Vector2 Point) {
        if (Color != null)
            return this;
        if (Point.x >= Pos.x + Size / 2) {
            if (Point.y >= Pos.y + Size / 2) {
                return Children[3].Locate(Point);
            }
            else {
                return Children[0].Locate(Point);
            }
        }
        else {
            if (Point.y >= Pos.y + Size / 2) {
                return Children[2].Locate(Point);
            }
            else {
                return Children[1].Locate(Point);
            }
        }
    }
    /// <summary>
    /// find closest parent containing a point
    /// </summary>
    public Tree LocateUp(Vector2 Point)
    {
        if (Utils.PointInSquare(Pos, Size, Point)) {
            return Locate(Point);
        }
        else return Parent.LocateUp(Point);
    }
    /// <summary>
    /// Fill square with a color (deletes children, if any)
    /// </summary>
    public void Update(PixelState NewColor) {
        if (NewColor == null) {
            Debug.LogError("Color not defined");
            return;
        }
		Color = NewColor;
        Children = new Tree[4];
		Root.Renderer.Draw(this);
    }
	/// <summary>
	/// Fill square with a color (deletes children, if any)
	/// </summary>
	private void TryUpdate(PixelState NewColor)
	{
		if (NewColor == null) {
			return;
		}
		Update(NewColor);
	}
}

