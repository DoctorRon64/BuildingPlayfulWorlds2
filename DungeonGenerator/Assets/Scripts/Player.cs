﻿using System.Collections.Generic;
using UnityEngine;
using static DungeonGenerator;

public class Player : MonoBehaviour
{
	public Vector2 MovePoint;
	public Animator AnimatorController;
	public float MoveSpeed = 5f;
    public int StepsAmount;

    public int Health = 20;
    public GameObject EnemieHitHud;
    private TurnManager turnManager;
    private GameOver gameOver;
    private DungeonGenerator dungeonGenerator;
    private InventoryManager inventory;
    public DungeonData dungeonData;

    private Vector2Int[] WhichSideToMove = new Vector2Int[4];

    private void Awake()
	{
        WhichSideToMove[0] = new Vector2Int(0, 1); //up
        WhichSideToMove[1] = new Vector2Int(0, -1); //down
        WhichSideToMove[2] = new Vector2Int(1, 0); //right
        WhichSideToMove[3] = new Vector2Int(-1, 0); //left

        MovePoint = transform.position;
        EnemieHitHud.SetActive(false);
        dungeonGenerator = FindObjectOfType<DungeonGenerator>();
        turnManager = FindObjectOfType<TurnManager>();
        inventory = FindObjectOfType<InventoryManager>();
        gameOver = turnManager.GetComponent<GameOver>();
        AnimatorController = GetComponent<Animator>();
    }

	private void Update()
	{
        if (StepsAmount > 0 && turnManager.IsPlayerTurn == true)
        {
            PlayerMovement();
        }
    }

    private TileType GetTileTypeWithKey(Vector2Int _Vector2)
    {
        dungeonGenerator.Dungeon.TryGetValue(_Vector2, out TileType tiletip);
        return tiletip;
    }

    private bool isFloorTile(Vector3 _vector3)
    {
        Vector2Int vector2 = new Vector2Int((int)_vector3.x, (int)_vector3.y);
        return GetTileTypeWithKey(vector2) == TileType.Floor || GetTileTypeWithKey(vector2) == TileType.StartFloor || GetTileTypeWithKey(vector2) == TileType.BossFloor;
    }

    private void PlayerMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, MovePoint, MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, MovePoint) <= .01f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                Vector2Int moveDir = new Vector2Int((int)Input.GetAxisRaw("Horizontal"), 0);
                Vector2Int newTilePos = new Vector2Int((int)MovePoint.x, (int)MovePoint.y) + moveDir;
                Vector3 newTilePosVector3 = new Vector3(newTilePos.x, newTilePos.y, 1);

                if (isFloorTile(newTilePosVector3))
                {
                    MovePoint = newTilePos;
                    StepsAmount--;
                    turnManager.GetIfPlayerWalked();
                    CheckIfPlayerSteppedOnEnemy();
                }
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                Vector2Int moveDir = new Vector2Int(0, (int)Input.GetAxisRaw("Vertical"));
                Vector2Int newTilePos = new Vector2Int((int)MovePoint.x, (int)MovePoint.y) + moveDir;
                Vector3 newTilePosVector3 = new Vector3(newTilePos.x, newTilePos.y, 1);

                if (isFloorTile(newTilePosVector3))
                {
                    MovePoint = newTilePos;
                    StepsAmount--;
                    turnManager.GetIfPlayerWalked();
                    CheckIfPlayerSteppedOnEnemy();
                }
            }

            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 0f && Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 0f)
            {
                CheckIfPlayerDies();
                GetItemWhenWalkedOn();
            }

            AnimatorController.SetBool("Moving", false);
        }

        else
        {
            AnimatorController.SetBool("Moving", true);
        }
    }

    public void GetItemWhenWalkedOn()
    {
        for (int i = 0; i < dungeonData.ItemList.Count; i++)
        {
            if (gameObject.transform.position == dungeonData.ItemList[i].transform.position)
            {
                inventory.PickupItem(dungeonData.ItemList[i]);
            }
        }
    }

    public void CheckIfPlayerSteppedOnEnemy()
	{
        for (int i = 0; i < dungeonData.EnemyList.Count; i++)
		{
            if (gameObject.transform.position == dungeonData.EnemyList[i].transform.position || (Vector3)MovePoint == dungeonData.EnemyList[i].transform.position)
			{
                TakeDamage(dungeonData.EnemyList[i].AttackDamage);
			}
		}
	}

    public void TakeDamage(int _Damage)
    {
        Health -= _Damage;
    }

    public void ApplyHealth(int _Health)
    {
        Health += _Health;
    }

    public void DoDamage(int _Damage)
	{
        for (int i = 0; i < dungeonData.EnemyList.Count; i++)
		{
            for (int j = 0; j < WhichSideToMove.Length; j++)
            {
                if (dungeonData.EnemyList[i].transform.position == transform.position + new Vector3(WhichSideToMove[j].x, WhichSideToMove[j].y, 0f))
                {
                    dungeonData.EnemyList[i].TakeDamage(_Damage);
                }
            }
        }
	}

    public void CheckIfPlayerDies()
    {
        if (Health <= 0)
        {
            gameOver.GameLose();
        }
    }
}