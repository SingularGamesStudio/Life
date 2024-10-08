using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utils
{
    //TODO:split into categories?
    public static List<Vector2> rectPoints(Rect rect, Vector2 rotCenter, float rotation = 0)
    {
        List<Vector2> points = new List<Vector2>();
        points.Add(new Vector2(rect.xMin-rotCenter.x, rect.yMin - rotCenter.y));
		points.Add(new Vector2(rect.xMax - rotCenter.x, rect.yMin - rotCenter.y));
		points.Add(new Vector2(rect.xMin - rotCenter.x, rect.yMax - rotCenter.y));
		points.Add(new Vector2(rect.xMax - rotCenter.x, rect.yMax - rotCenter.y));
        for(int i = 0; i<4; i++) {
            points[i] = new Vector2(rotCenter.x + Mathf.Cos(rotation) * points[i].x - Mathf.Sin(rotation) * points[i].y,
									rotCenter.y + Mathf.Sin(rotation) * points[i].x + Mathf.Cos(rotation) * points[i].y);

		}
        return points;
	}
	public static Vector2 TransformPos(Vector3 WorldPos, SpriteRenderer Sprite) {
        Vector2 Pos = Sprite.transform.InverseTransformPoint(WorldPos);
        Pos *= 100;
        Pos+=new Vector2(Sprite.sprite.texture.width, Sprite.sprite.texture.height)/2;
        return Pos;
    }
    public static Vector2 TransformPos(Vector3 WorldPos, Transform SpriteCenter, int SpriteSize) {
        Vector2 Pos = SpriteCenter.InverseTransformPoint(WorldPos);
        Pos *= 100;
        Pos += new Vector2(SpriteSize, SpriteSize) / 2;
        return Pos;
    }
    public static Vector2 InverseTransformPos(Vector2 Point, Transform SpriteCenter, int SpriteSize) {
        Point -= new Vector2(SpriteSize, SpriteSize) / 2;
        Point /= 100;
        return SpriteCenter.TransformPoint(Point);
    }
    public static float Distance(Vector2 a, Vector2 b) {
        return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
    }
    public static float Distance2(Vector2 a, Vector2 b) {
        return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
    }
    public static float DistanceSquarePoint(Vector2 a, int size, Vector2 x) {
        if (PointInSquare(a, size, x))
            return 0;
        if(x.x >= a.x && x.x <= a.x+size)
            return Mathf.Min(Mathf.Abs(a.y-x.y), Mathf.Abs((a.y+size) - x.y));
        if (x.y >= a.y && x.y <= a.y + size)
            return Mathf.Min(Mathf.Abs(a.x - x.x), Mathf.Abs((a.x + size) - x.x));
        return Mathf.Min(Distance(a, x), Distance(a, new Vector2(x.x, x.y + size)), Distance(a, new Vector2(x.x + size, x.y)), Distance(a, new Vector2(x.x + size, x.y + size)));
    }
    public static float SinMultiply(Vector2 a, Vector2 b) {
        return a.x * b.y - a.y * b.x;
    }
    public static bool PointInAngle(Vector2 a, Vector2 b, Vector2 c, Vector2 x) {
        if (Mathf.Sign(SinMultiply(a - b, x - b)) == Mathf.Sign(SinMultiply(a - b, c - b)))
            if (Mathf.Sign(SinMultiply(c - b, x - b)) == Mathf.Sign(SinMultiply(c - b, a - b)))
                return true;
        return false;
    }
    public static bool SegmentCrossesSquare(Vector2 s1, Vector2 s2, Vector2 a, int size) {
        if (PointInSquare(a, size, s1))
            return true;
        if (PointInSquare(a, size, s2))
            return true;
        if (s1.x > s2.x) {
            Vector2 temp = s1;
            s1 = s2;
            s2 = temp;
        }
        if(s1.x <= a.x && s2.x >= a.x) {
            Vector2 mid = (s2 - s1) * (((float)a.x - s1.x) / (s2.x - s1.x)) + s1;
            if (mid.y >= a.y && mid.y <= a.y + size) {
                return true;
            }
        }
        if (s1.x <= a.x+size && s2.x >= a.x+size) {
            Vector2 mid = (s2 - s1) * (((float)a.x+size - s1.x) / (s2.x - s1.x)) + s1;
            if (mid.y >= a.y && mid.y <= a.y + size) {
                return true;
            }
        }
        if (s1.y > s2.y) {
            Vector2 temp = s1;
            s1 = s2;
            s2 = temp;
        }
        if (s1.y <= a.y && s2.y >= a.y) {
            Vector2 mid = (s2 - s1) * (((float)a.y - s1.y) / (s2.y - s1.y)) + s1;
            if (mid.x >= a.x && mid.x <= a.x + size) {
                return true;
            }
        }
        if (s1.y <= a.y + size && s2.y >= a.y + size) {
            Vector2 mid = (s2 - s1) * (((float)a.y + size - s1.y) / (s2.y - s1.y)) + s1;
            if (mid.x >= a.x && mid.x <= a.x + size) {
                return true;
            }
        }
        return false;
    }
    public static Tuple<Vector2, Vector2> SquaresIntersect(Vector2 a, int aSize, Vector2 b, int bSize) { 
        Vector2 p1 = new Vector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
        Vector2 p2 = new Vector2(Mathf.Min(a.x+aSize, b.x+bSize), Mathf.Min(a.y+aSize, b.y+bSize));
        if(p1.x >= p2.x || p1.y>=p2.y)
            return null;
        return new Tuple<Vector2, Vector2>(p1, p2);
    }
    public static bool PointInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 x) {
        if(PointInAngle(a, b, c, x) && PointInAngle(b, c, a, x))
            return true;
        return false;
    }
    public static bool PointInSquare(Vector2 a, int size, Vector2 x) {
        if (x.x >= a.x && x.x < a.x + size && x.y >= a.y && x.y < a.y + size)
            return true;
        return false;
    }

    public static float CrossProduct(Vector2 a, Vector2 b)
    {
        return a.x * b.y - b.x * a.y;
    }
}
