using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cyan;

namespace Managers
{
    [SelectionBase, RequireComponent(typeof(Grid))]
    public class BuildingSystem : MonoBehaviour
    {
        public enum BlockType {
            METAL,
            WOOD
        }

        [SerializeField, RequireNotNull] Tilemap blockMap;
        [SerializeField, RequireNotNull] Tilemap ghostMap;

        [SerializeField, RequireNotNull] Tile ghostBlock;

        [Serializable]
        public struct TileEntry
        {
            public BlockType type;
            public Tile tile;
        }
        [SerializeField] List<TileEntry> tiles;

        private Grid tileMapGrid;

        #region UNITY
        private void Awake()
        {
            tileMapGrid = GetComponent<Grid>();
        }

        private void Update()
        {
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            UpdateGhostBlock(mouseWorldPos);

            if (Input.GetMouseButtonDown(0))
            {
                PlaceBlock(BlockType.WOOD, mouseWorldPos);
            } else if (Input.GetMouseButtonDown(1))
            {
                PlaceBlock(BlockType.METAL, mouseWorldPos);
            } else if (Input.GetMouseButtonDown(2))
            {
                DestroyBlock(mouseWorldPos);
            }
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
            if (blockMap.GetTile<TileBase>(tileMapPosition) != null) { return true; }
            return false;
        }

        /// <summary>
        /// Takes a block type and returns the relavent tile reference
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private TileBase GetTileReference(BlockType type)
        {
            foreach (TileEntry tile in tiles)
            {
                if (tile.type == type)
                {
                    return tile.tile;
                }
            }

            throw new ArgumentException("Tile type does not have an associated reference");
        }
        #endregion

        #region PUBLIC_INTERFACE
        /// <summary>
        /// Updates the position of the ghost block in the grid
        /// </summary>
        /// <param name="worldPosition"></param>
        public void UpdateGhostBlock(Vector2 worldPosition)
        {
            Vector3Int tilePos = tileMapGrid.WorldToCell(worldPosition);
            ghostMap.ClearAllTiles();
            ghostMap.SetTile(tilePos, ghostBlock);
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

            if (!IsTileOccupied(tilePos))
            {
                blockMap.SetTile(tilePos, GetTileReference(block));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Destroys a block at a coordinate if there is one present
        /// </summary>
        /// <param name="worldPosition"></param>
        public bool DestroyBlock(Vector2 worldPosition)
        {
            Vector3Int tilePos = tileMapGrid.WorldToCell(worldPosition);
            bool wasPresent = IsTileOccupied(tilePos);
            blockMap.SetTile(tilePos, null);

            //flood fill breaking here



            return wasPresent;
        }
        #endregion
    }
}