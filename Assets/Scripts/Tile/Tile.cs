using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //tile type
    [SerializeField]
    private float _rotationAngle = 180f;
    protected virtual float rotationAngle
    {
        get { return _rotationAngle; }
        set { _rotationAngle = value; }
    }

    [SerializeField]
    private int _tileNum = 0;
    public virtual int tileNum
    {
        get { return _tileNum; }
        set { _tileNum = value; }
    }

    //from tileInfo
    public (int, int) loc;
    public bool cantRotate = false;
    public bool isRotate = false;

    //about interaction
    public bool rotateSelecting = false;
    public int moveTarget = -1;
    public int onTileObject = -1;
    public int onTileCandy = -1;

    public GameObject MovableEffect;

    //adj, cost
    public (int, int)[] adjacentIdx = new (int, int)[4];
    public Tile[] adjacentTiles = new Tile[4];
    public int[] costs = new int[4];
    public int rotateCost = 1; 

    //event
    public event Action<float, bool> isRotateChanged;

    //etc
    public Vector2 objectPosition;
    private Vector3 originalScale = new Vector3(1, 1, 1); // 오브젝트의 원래 크기 저장

    public void FirstRotate()
    {
        isRotate = true;
        objectPosition = -objectPosition;
        transform.eulerAngles = new Vector3(0, 0, tileNum!=1?180f:60f);
    }
    
    public virtual void Rotate((int,int) loc, bool isUndo)
    {
        if (loc != this.loc) return;

        isRotate ^= true;
        objectPosition = -objectPosition;
        RotateInvoke(360f - rotationAngle, rotationAngle, isUndo);
        UpdateCost();
    }

    protected void RotateInvoke(float firstAngle, float secondAngle, bool isUndo)
    {
        isRotateChanged?.Invoke(isRotate^isUndo ? firstAngle : secondAngle, isUndo);
    }

    private void Awake()
    {
        GameManager.Instance.TileRotate += Rotate;
        GameManager.Instance.TileClick += ClickJudge;
        GameManager.Instance.moveStart += UndoMove;

    }

    private void OnDestroy()
    {
        GameManager.Instance.TileRotate -= Rotate;
        GameManager.Instance.TileClick -= ClickJudge;
        GameManager.Instance.moveStart -= UndoMove;
    }

    private void Start()
    {
        UpdateCost();
    }

    private void Update()
    {

    }

    void UndoMove((int,int) startLoc, (int,int) loc, bool isUndo)
    {
        if (!isUndo) return;
        if (loc != this.loc) return;

        onTileObject = GameManager.Instance.tileList[startLoc.Item1, startLoc.Item2].GetComponent<Tile>().onTileObject;
        GameManager.Instance.tileList[startLoc.Item1, startLoc.Item2].GetComponent<Tile>().onTileObject = -1;

        //onTileObject = moveTarget;
        //GameManager.Instance.tileList[startLoc.Item1, startLoc.Item2].GetComponent<Tile>().AdjTileDisable();
        //GameManager.Instance.tileList[startLoc.Item1, startLoc.Item2].GetComponent<Tile>().onTileObject = -1;
        //moveTarget = -1;
    }

    void ClickJudge((int,int) loc)
    {
        if (loc != this.loc) // && Array.IndexOf(adjacentIdx,loc) == -1)
        {
            if (Array.IndexOf(adjacentIdx, loc) == -1)
                ResetSelect();

            return;
        }

        //타일 회전
        if (rotateSelecting)
        {
            ResetSelect();
            EventBus.RotateStart(loc, false);
        }
        //이 타일로 이동
        else if (moveTarget != -1)
        {
            onTileObject = moveTarget;
            (int, int) startLoc = GameManager.Instance.playerList[moveTarget].GetComponent<Player>().playerIndex;
            GameManager.Instance.tileList[startLoc.Item1, startLoc.Item2].GetComponent<Tile>().AdjTileDisable();
            GameManager.Instance.tileList[startLoc.Item1, startLoc.Item2].GetComponent<Tile>().onTileObject = -1;
            moveTarget = -1;
            EventBus.MoveStart(startLoc, loc, false);
        }
        else
        {
            //플레이어 클릭(이동 선택)
            if (onTileObject != -1)
            {
                AdjTileEnable(onTileObject);
                return;
            }
            //빈 타일 클릭(회전 선택)
            else if (onTileCandy == -1 || GameManager.Instance.candyList[onTileCandy].GetComponent<Candy>().isCatch)
            {
                RotatableSelect();
            }
        }

        AdjTileDisable();
    }

    public void MovableSelect(int idx)
    {
        ResetSelect();

        moveTarget = idx;
        //onTileObject = idx;
        MovableEffect.SetActive(true);
    }
    public void RotatableSelect()
    {
        ResetSelect();
        if (cantRotate)
        {
            if (GetComponent<ShakeEffect>() != null)
                GetComponent<ShakeEffect>().Shake();

            return;
        }

        originalScale = transform.localScale; // 원래 크기 저장
        transform.localScale = originalScale * 1.1f; // 크기 1.5배로 확대
        GetComponent<SpriteRenderer>().sortingOrder = 0;

        rotateSelecting = true;
    }
    public void ResetSelect()
    {
        moveTarget = -1;
        rotateSelecting = false;

        if (MovableEffect.activeSelf)
            MovableEffect.SetActive(false);

        transform.localScale = originalScale;
        GetComponent<SpriteRenderer>().sortingOrder = -1;
    }

    public void AdjTileEnable(int idx)
    {
        for (int i = 0; i < adjacentTiles.Length; i++)
        {
            if (adjacentTiles[i] == null) continue;
            if (adjacentTiles[i].onTileObject != -1) continue;
            if (costs[i] == -1) continue;
            //if (costs[i] > GameManager.Instance.actionPoint) continue;

            adjacentTiles[i].MovableSelect(idx);
        }
    }

    public void AdjTileDisable()
    {
        for (int i = 0; i < adjacentTiles.Length; i++)
        {
            if (adjacentTiles[i] == null) continue;

            adjacentTiles[i].ResetSelect();
        }
    }

    public void UpdateCost()
    {
        for (int i = 0; i < adjacentTiles.Length; i++)
        {
            if (adjacentTiles[i] == null) continue;

            costs[i] = Calculate(adjacentTiles[i], i);
            adjacentTiles[i].costs[3 - i] = adjacentTiles[i].Calculate(this, 3 - i);
        }
    }


    public virtual int Calculate(Tile adjTile, int index)
    {
        bool toUp = index < 2;
        //bool toLeft = index % 2 == 0;

        if (adjTile.tileNum == 0)
        {
            if (isRotate == adjTile.isRotate)
            {
                return 2;
            }
            else if (toUp != isRotate)
            {
                return 1;
            }
            else
            {
                return -1;
            }

        }

        else if (adjTile.tileNum == 1)
        {
            if (toUp == isRotate)
            {
                return -1;
            }
            else if (!adjTile.isRotate == (index == 1 || index == 2))
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (adjTile.tileNum == 2)
        {
            if (isRotate != adjTile.isRotate)
            {
                if (toUp == isRotate)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (adjTile.isRotate && index == 2)
            {
                return 1;
            }
            else if (!adjTile.isRotate && index == 1)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        else if (adjTile.tileNum == 3)
        {
            if (isRotate != adjTile.isRotate)
            {
                if (toUp == isRotate)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (adjTile.isRotate && index == 3)
            {
                return 1;
            }
            else if (!adjTile.isRotate && index == 0)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        //Debug.Log($"{tileNum}, {adjTile.tileNum} 간 정의 필요");
        return -1;
    }
}
