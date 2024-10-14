using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int ID;

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
            if (pos.x > game.Size / 2 && pos.y > game.Size / 2) {
                move.x = rnd.Next(100) - 80;
                move.y = rnd.Next(100) - 80;
                move.x = (move.x < 0) ? -1 : 1;
                move.y = (move.y < 0) ? -1 : 1;
            } else {
                move.x = rnd.Next(3) - 1;
				move.y = rnd.Next(3) - 1;
			}
            if (SUPER>=Data.Main.ExplodeCost) {
                game.ExplodeSpell(ID);
            }
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
