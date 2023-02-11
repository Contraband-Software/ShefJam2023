using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cyan;

namespace Managers
{
    [
        SelectionBase, 
        RequireComponent(typeof(Grid))
    ]
    public class BuildingSystem : MonoBehaviour
    {
        public static BuildingSystem GetReference()
        {
            return GameObject.FindGameObjectWithTag("BlockWorldManager").GetComponent<BuildingSystem>();
        }

        public enum BlockType {
            METAL,
            WOOD
        }

        [SerializeField, RequireNotNull] Tilemap buildingTileMap;
        [SerializeField] List<Tilemap> proximityTileMaps;
        [SerializeField, RequireNotNull] Tilemap ghostMap;

        [SerializeField, RequireNotNull] Tile ghostBlock;

        [Serializable]
        public struct TileEntry
        {
            public BlockType type;
            [Min(0)] public int strength;
            public List<Tile> decorative;
            public List<Tile> placeable;
        }
        [SerializeField] List<TileEntry> tiles;

        public Grid tileMapGrid { private set; get; }

        #region UNITY
        private void Awake()
        {
            tileMapGrid = GetComponent<Grid>();
        }

        private void Update()
        {
            ghostMap.ClearAllTiles();
            #region DEBUG
            //Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //mouseWorldPos.z = 0f;

            //if (Input.GetMouseButtonDown(0))
            //{
            //    PlaceBlock(BlockType.WOOD, mouseWorldPos);
            //} else if (Input.GetMouseButtonDown(1))
            //{
            //    PlaceBlock(BlockType.METAL, mouseWorldPos);
            //} else if (Input.GetMouseButtonDown(2))
            //{
            //    DestroyBlock(mouseWorldPos);
            //    //Vector3Int tile = tileMapGrid.WorldToCell(mouseWorldPos);
            //    //Vector3Int up = tile + new Vector3Int(0, 1, 0);
            //    //Vector3Int down = tile - new Vector3Int(0, 1, 0);
            //    //Vector3Int left = tile - new Vector3Int(1, 0, 0);
            //    //Vector3Int right = tile + new Vector3Int(1, 0, 0);
            //    //Debug.Log(
            //    //    "up: " + blockMap.GetTile<TileBase>(up)?.name + " " +
            //    //    "down: " + blockMap.GetTile<TileBase>(down)?.name + " " +
            //    //    "left: " + blockMap.GetTile<TileBase>(left)?.name + " " +
            //    //    "right: " + blockMap.GetTile<TileBase>(right)?.name
            //    //);
            //}
            #endregion
        }
        #endregion

        #region PRIVATE_INTERFACE
        /// <summary>
        /// Returns if a block exists at a position
        /// </summary>
        /// <param name="tileMapPosition"></param>
        /// <returns></returns>
        private bool IsTileOccupied(Vector3Int tileMapPosition)
        {
            foreach (Tilemap tmap in proximityTileMaps)
            {
                if (tmap.GetTile<TileBase>(tileMapPosition) != null) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Takes a block type and returns the relavent tile reference
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private TileBase GetRandomPlaceableVariant(BlockType type)
        {
            foreach (TileEntry tile in tiles)
            {
                if (tile.type == type)
                {
                    return tile.placeable[UnityEngine.Random.Range(0, tile.placeable.Count)];
                }
            }

            throw new ArgumentException("Tile type does not have an associated reference");
        }

        /// <summary>
        /// Returns the block type for a tile
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private BlockType GetTileType(Tile tile)
        {
            foreach (TileEntry tileEntry in tiles)
            {
                foreach (Tile tileVariant in tileEntry.placeable.Concat(tileEntry.decorative))
                {
                    if (tileVariant == tile)
                    {
                        return tileEntry.type;
                    }
                }
            }

            throw new ArgumentException("Unknown block tile: " + tile.name);
        }

        /// <summary>
        /// Returns the block type for a tile
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private int GetTileStrength(BlockType type)
        {
            foreach (TileEntry tile in tiles)
            {
                if (tile.type == type)
                {
                    return tile.strength;
                }
            }

            throw new ArgumentException("Tile entry error");
        }

        private void DestroyLogic(Vector3Int tile)
        {
            if (buildingTileMap.GetTile<TileBase>(tile + new Vector3Int(0, 1, 0)) &&
                buildingTileMap.GetTile<TileBase>(tile - new Vector3Int(0, 1, 0))) 
            {
                ExecuteDestroy(tile + new Vector3Int(0, 1, 0), tile - new Vector3Int(0, 1, 0));
            }
            else if (buildingTileMap.GetTile<TileBase>(tile + new Vector3Int(1, 0, 0)) &&
                buildingTileMap.GetTile<TileBase>(tile - new Vector3Int(1, 0, 0)))
            {
                ExecuteDestroy(tile + new Vector3Int(1, 0, 0), tile - new Vector3Int(1, 0, 0));
            }
            else if (buildingTileMap.GetTile<TileBase>(tile + new Vector3Int(0, 1, 0)) &&
                     buildingTileMap.GetTile<TileBase>(tile - new Vector3Int(1, 0, 0))
            )
            {
                ExecuteDestroy(tile + new Vector3Int(0, 1, 0), tile - new Vector3Int(1, 0, 0));
            }
            else if (buildingTileMap.GetTile<TileBase>(tile + new Vector3Int(0, 1, 0)) &&
                     buildingTileMap.GetTile<TileBase>(tile + new Vector3Int(1, 0, 0))
            )
            {
                ExecuteDestroy(tile + new Vector3Int(0, 1, 0), tile + new Vector3Int(1, 0, 0));
            }
            else if (buildingTileMap.GetTile<TileBase>(tile - new Vector3Int(0, 1, 0)) &&
                     buildingTileMap.GetTile<TileBase>(tile + new Vector3Int(1, 0, 0))
            )
            {
                ExecuteDestroy(tile - new Vector3Int(0, 1, 0), tile + new Vector3Int(1, 0, 0));
            }
            else if (buildingTileMap.GetTile<TileBase>(tile - new Vector3Int(0, 1, 0)) &&
                     buildingTileMap.GetTile<TileBase>(tile - new Vector3Int(1, 0, 0))
            )
            {
                ExecuteDestroy(tile - new Vector3Int(0, 1, 0), tile - new Vector3Int(1, 0, 0));
            }
        }

        private void ExecuteDestroy(Vector3Int tile1, Vector3Int tile2)
        {
            TileMapSplitCounter splitCounter = new TileMapSplitCounter(buildingTileMap, tile1, tile2);
            List<Vector3Int> tilesToDestroy = splitCounter.Resolve();
            if (tilesToDestroy != null)
            {
                foreach (Vector3Int tile3 in tilesToDestroy)
                {
                    buildingTileMap.SetTile(tile3, null);
                }
            }
        }
        #endregion

        #region PUBLIC_INTERFACE
        /// <summary>
        /// Updates the position of the ghost block in the grid
        /// </summary>
        /// <param name="worldPosition"></param>
        public void UpdateGhostBlock(Vector2 worldPosition, int xOffset, int yOffset)
        {
            Vector3Int tilePos = tileMapGrid.WorldToCell(worldPosition);
            ghostMap.SetTile(tilePos + new Vector3Int(xOffset, yOffset, 0), ghostBlock);
        }

        /// <summary>
        /// Places a block in the world
        /// </summary>
        /// <param name="block">What type of block</param>
        /// <param name="location">Where in the world</param>
        /// <returns>Whether the block was able to be placed</returns>
        public bool PlaceBlock(BlockType block, Vector2 worldPosition)
        {
            Vector3Int tilePos = tileMapGrid.WorldToCell(worldPosition);

            int surroundings = 0;
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (y == -1 || y == 1)
                    {
                        if (x == -1 || x == 1)
                        {
                            continue;
                        }
                    }

                    foreach (Tilemap tmap in proximityTileMaps)
                    {
                        if (tmap.GetTile<TileBase>(tilePos + new Vector3Int(x, y, 0)) != null)
                        {
                            surroundings++;
                            break;
                        }
                    }
                }
            }

            if (!IsTileOccupied(tilePos) && surroundings >= 1)
            {
                buildingTileMap.SetTile(tilePos, GetRandomPlaceableVariant(block));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Destroys a block at a coordinate if there is one present
        /// </summary>
        /// <param name="worldPosition"></param>
        public int DestroyBlock(Vector2 worldPosition)
        {
            Vector3Int tilePos = tileMapGrid.WorldToCell(worldPosition);
            Tile tile = buildingTileMap.GetTile<Tile>(tilePos);
            if (tile != null)
            {
                int strength = GetTileStrength(GetTileType(tile));
                buildingTileMap.SetTile(tilePos, null);
                DestroyLogic(tilePos);
                return strength;
            }

            return 0;
        }

        public bool CheckOccupancy(Vector2 worldPosition)
        {
            return IsTileOccupied(tileMapGrid.WorldToCell(worldPosition));
        }
        #endregion
    }
}