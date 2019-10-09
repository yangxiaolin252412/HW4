using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserGUI : MonoBehaviour
{
    private IUserAction action;
    public IUserAction action1;
    bool isFirst = true;
    GUIStyle red;
    GUIStyle black;

    // Use this for initialization
    void Start()
    {
        action = Director.getInstance().current as IUserAction;
        black = new GUIStyle("button");
        black.fontSize = 20;
        red = new GUIStyle();
        red.fontSize = 30;
        red.fontStyle = FontStyle.Bold;
        red.normal.textColor = Color.red;
        red.alignment = TextAnchor.UpperCenter;
    }

    private void OnGUI()
    {
        if (action != null)
        {
            if (action.getGameState() == GameState.FUNISH)
            {
                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 150, 200, 100), action.getScore() >= 30 ? "You win" : "You fail", red);
                if (GUI.Button(new Rect(Screen.width / 2 - 60, Screen.height / 2 - 50, 120, 40), "Restart", black))
                {
                    SceneManager.LoadScene("DiskAttack");
                }
                return;
            }
        }
        
        Rect rect = new Rect(Screen.width / 2 - 100, 0, 200, 40);
        Rect rect2 = new Rect(Screen.width / 2 - 45, 60, 120, 40);
        if (action != null)
        {
            if (Input.GetButtonDown("Fire1") && action.getGameState() != GameState.PAUSE)
            {
                Vector3 pos = Input.mousePosition;
                action.hit(pos);
            }
        }

        if (!isFirst)
        {
            if(action!=null)
            GUI.Label(rect, "Your score: " + action.getScore().ToString(), red);
        }
        else
        {
            GUIStyle blackLabel = new GUIStyle();
            blackLabel.fontSize = 16;
            blackLabel.normal.textColor = Color.black;
            GUI.Label(new Rect(Screen.width / 2 - 250, 120, 500, 200), "There are 3 rounds, every round has 10 disk " +
                "whose color is different.\nIf you attack the white one, you will get 1 score. And you will get 2 score\n" +
                "if you attack the gray one. Finally, if you can attack the black and most\nfast one, you will get 4 " +
                "score. Once you get 30 scores, you win!", blackLabel);
        }
        if (action != null)
        {
            if (action.getGameState() == GameState.RUNNING && GUI.Button(rect2, "Paused", black))
            {
                action.setGameState(GameState.PAUSE);
            }
            else if (action.getGameState() == GameState.PAUSE && GUI.Button(rect2, "Run", black))
            {
                action.setGameState(GameState.RUNNING);
            }
        }
        if (isFirst && GUI.Button(rect2, "Start", black))
        {
            isFirst = false;
            if(action != null)
            action.setGameState(GameState.ROUND_START);
        }
        if (action != null)
        {
            if (!isFirst && action.getGameState() == GameState.ROUND_FINISH && GUI.Button(rect2, "Next Round", black))
            {
                if (action != null)
                    action.setGameState(GameState.ROUND_START);
            }
        }
    }
}