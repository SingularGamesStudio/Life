using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Player player1;
    public Player player2;

    void Update()
    {
		if (player1.controller == Player.Controller.Player) {
			Vector2Int move = new Vector2Int(0, 0);
			if (Input.GetKey(KeyCode.W)) {
				move.y++;
			}
			if (Input.GetKey(KeyCode.S)) {
				move.y--;
			}

			if (Input.GetKey(KeyCode.A)) {
				move.x--;
			}
			if (Input.GetKey(KeyCode.D)) {
				move.x++;
			}
			if (Input.GetKeyDown(KeyCode.Z) && player1.SUPER >= Data.Main.FreezeCost) {
				player1.game.FreezeSpell(0);
			}
			if (Input.GetKeyDown(KeyCode.X) && player1.SUPER >= Data.Main.ExplodeCost) {
				player1.game.ExplodeSpell(0);
			}
			if (!(move.x == 0 && move.y == 0)) {
				player1.move = move;
			}
		}

		if (player2.controller == Player.Controller.Player) {
			Vector2Int move = new Vector2Int(0, 0);
			if (Input.GetKey(KeyCode.UpArrow)) {
				move.y++;
			}
			if (Input.GetKey(KeyCode.DownArrow)) {
				move.y--;
			}

			if (Input.GetKey(KeyCode.LeftArrow)) {
				move.x--;
			}
			if (Input.GetKey(KeyCode.RightArrow)) {
				move.x++;
			}
			if (Input.GetKeyDown(KeyCode.O) && player2.SUPER >= Data.Main.FreezeCost) {
				player2.game.FreezeSpell(1);
			}
			if (Input.GetKeyDown(KeyCode.P) && player2.SUPER >= Data.Main.ExplodeCost) {
				player2.game.ExplodeSpell(1);
			}
			if (!(move.x == 0 && move.y == 0)) {
				player2.move = move;
			}
		}
		Camera.main.orthographicSize = Camera.main.orthographicSize * (1f - Input.mouseScrollDelta.y*0.1f);

		if (player1.game.running==0) {
			if (Input.GetMouseButton(0)) {
				Vector2 pos = Utils.TransformPos(Camera.main.ScreenToWorldPoint(Input.mousePosition), player1.game.transform, player1.game.Size);
				player1.game.SetPixel(new Vector2Int((int)pos.x, (int)pos.y), 1);
			}
			if (Input.GetMouseButton(1)) {
				Vector2 pos = Utils.TransformPos(Camera.main.ScreenToWorldPoint(Input.mousePosition), player1.game.transform, player1.game.Size);
				player1.game.SetPixel(new Vector2Int((int)pos.x, (int)pos.y), 2);
			}
		}


		if (Input.GetKeyDown(KeyCode.Escape)) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		if (Input.GetKeyDown(KeyCode.Space) && player1.game.running==0) {
			player1.game.running = 1;
		}
		if (Input.GetKeyDown(KeyCode.B) && player1.game.running == 0) {
			player1.game.running = 1;
			player2.controller = Player.Controller.Bot;
		}
	}
}
