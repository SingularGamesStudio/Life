using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Game game;
    public Vector2Int pos;
    [HideInInspector]
    public Vector2Int move;
	public Vector2Int respawn;
	public int hp;
    public int hpMax;
    public int SUPER;

    public int points;

    public int frozen;
    [System.Serializable]
    public enum Controller
    {
        Player,
        Bot
    }
    public Controller controller;
    System.Random rnd = new System.Random(42);
    
    public void TakeTurn()
    {
        if (SUPER<Data.Main.SUPERmax) {
			SUPER++;
		}
        if (frozen>0 ) {
            frozen--;
            return;
        }
        if (controller == Controller.Bot) {
            move.x = rnd.Next(100) - 80;
			move.y = rnd.Next(100) - 80;
			move.x = (int)Mathf.Sign(move.x);
			move.y = (int)Mathf.Sign(move.y);
            return;
		}
        Vector2Int newPos = new Vector2Int(pos.x+move.x, pos.y+move.y);
        if (newPos.x >= 0 && newPos.y >=0 && newPos.x<game.Size && newPos.y<game.Size) { 
            pos = newPos;
        }
        move = new Vector2Int(0, 0);
    }

    public void TakeDamage()
    {
        points -= 5000;
        hp--;
        if (hp==0) {
            hp = hpMax;
            pos = respawn;
        }
    }
}
