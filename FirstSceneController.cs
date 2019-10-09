using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSceneControl : MonoBehaviour, ISceneControl, IUserAction
{
    public CCActionManager actionManager { set; get; }
    public ScoreRecorder scoreRecorder { set; get; }
    public Queue<GameObject> diskQueue = new Queue<GameObject>();
    private int diskNumber = 0;
    private int currentRound = -1;
    private float time = 0;
    private GameState gameState = GameState.START;

    void Awake()
    {
        Director director = Director.getInstance();
        director.current = this;
        diskNumber = 10;
        this.gameObject.AddComponent<ScoreRecorder>();
        this.gameObject.AddComponent<DiskFactory>();
        scoreRecorder = Singleton<ScoreRecorder>.Instance;
        director.current.loadResources();
    }

    public void loadResources()
    {

    }

    private void Update()
    {
        if (actionManager.getDiskNumber() == 0 && gameState == GameState.RUNNING)
        {
            gameState = GameState.ROUND_FINISH;
            if (currentRound == 2)
            {
                gameState = GameState.FUNISH;
                return;
            }
        }
        if (actionManager.getDiskNumber() == 0 && gameState == GameState.ROUND_START)
        {
            currentRound++;
            nextRound();
            actionManager.setDiskNumber(10);
            gameState = GameState.RUNNING;
        }
        if (time > 1 && gameState != GameState.PAUSE)
        {
            throwDisk();
            time = 0;
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    private void nextRound()
    {
        DiskFactory diskFactory = Singleton<DiskFactory>.Instance;
        for (int i = 0; i < diskNumber; i++)
        {
            diskQueue.Enqueue(diskFactory.getDisk(currentRound));
        }
        actionManager.startThrow(diskQueue);
    }

    void throwDisk()
    {
        if (diskQueue.Count != 0)
        {
            GameObject disk = diskQueue.Dequeue();
            Vector3 pos = new Vector3(-disk.GetComponent<DiskData>().getDirection().x * 10, Random.Range(0f, 4f), 0);
            disk.transform.position = pos;
            disk.SetActive(true);
        }
    }

    public int getScore()
    {
        return scoreRecorder.score;
    }

    public GameState getGameState()
    {
        return gameState;
    }

    public void setGameState(GameState gameState)
    {
        this.gameState = gameState;
    }

    public void hit(Vector3 pos)
    {
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(pos));
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider.gameObject.GetComponent<DiskData>() != null)
            {
                scoreRecorder.record(hit.collider.gameObject);
                hit.collider.gameObject.transform.position = new Vector3(0, -5, 0);
            }
        }
    }
}