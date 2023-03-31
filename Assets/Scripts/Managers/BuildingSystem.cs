using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cyan;

namespace Managers
{
    /// <summary>
    /// This class handles: all player building, the SINGLE block the cannon ball destroys and calling into TileMapSplitCounter
    /// to determine the other resultant destroyed blocks.
    /// </summary>
    [
        SelectionBase, 
        RequireComponent(typeof(Grid))
    ]
    public class BuildingSystem : MonoBehaviour
    {
        /// <summary>
        /// Returns a reference to the class if it has been instantiated somewhere in the scene.
        /// </summary>
        /// <returns></returns>
        public static BuildingSystem GetReference()
        {
            return GameObject.FindGameObjectWithTag("BlockWorldManager").GetComponent<BuildingSystem>();
        }

        public enum BlockType {
            METAL,
            WOOD
        }

        // Tilemap of the solid, destructible blocks (includes player placed blocks)
        [SerializeField, RequireNotNull] Tilemap buildingTileMap;

        // A list of tilemaps to consider when deciding if a tile is actually occupied, allowing non-solid
        // (blocks the player can walk through) to block placement on top of themselves
        [SerializeField] List<Tilemap> proximityTileMaps;

        // A tilemap solely for the block placement ghost
        [SerializeField, RequireNotNull] Tilemap ghostMap;

        // The actual tile to use as the block placement ghost
        [SerializeField, RequireNotNull] Tile ghostBlock;

        [Serializable]
        public struct TileEntry
        {
            public BlockType type;
            [Min(0)] public int strength;
            public List<Tile> decorative;
            public List<Tile> placeable;
        }
        // A comprehensive database mapping EVERY different tile to a category, or type (as in the BlockType enum),
        // So there would be ONE entry for EACH BlockType, and then every tile would be sorted into it's relavent sub-list
        // depending on whether it is decorative or placeable, the only difference being whether it is in the pool of randomly chosen
        // tiles when a player is placing a block (to allow for variations). Look at how this is used in the inspector for a better idea.
        [SerializeField] List<TileEntry> tiles;

        // The SINGLE Grid object which ALL TileMaps use.
        public Grid tileMapGrid { private set; get; }

        #region UNITY

        private void Awake()
        {
            tileMapGrid = GetComponent<Grid>();
        }

        private void Update()
        {
            // The ghost tile block is deleted, the Player object then puts it BACK after this Update function in their OWN Update function. (Script execution ordering)
            ghostMap.ClearAllTiles();

            #region DEBUG

            // This code allows you place or break blocks with your mouse by clicking

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

            #endregion // DEBUG
        }

        #endregion // UNITY

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
                // This loop uses .Concat to actually combine the placeable and decorative lists together for a TileEntry object
                // so we ensure the correct tiletype is returned.
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

        /// <summary>
        /// Returns the tile reference at a specific position
        /// </summary>
        /// <param name="area">The position</param>
        /// <returns></returns>
        private Tile CheckMultipleOccupancy(Vector3Int area)
        {
            // This checks the position in each "Not-allowed-to-build-on-top-of-this" TileMap,
            // as it being empty in one does not mean the position is empty in the other.
            foreach (Tilemap tmap in proximityTileMaps)
            {
                Tile t = tmap.GetTile<Tile>(area);
                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }

        /// <summary>
        /// The soul of this class, here is where we decide what neighboring blocks are destroyed AS A RESULT of a block being destroyed.
        /// We do this by calculating if the destruction has caused a SEPARATION, that means it has disconnected some blocks in the 3x3 area around it.
        /// 
        /// If a separation or split has occurred, we use ExecuteDestroy() with the two suspected "halfs" to determine whether we have actually
        /// split the ship in half and then destroy the smaller half.
        /// </summary>
        /// <param name="tile">The center tile that was destroyed</param>
        private void DestroyLogic(Vector3Int tile)
        {
            // Nothing much to these if statements, just a bunch of hard-coded rules to determine if a 3x3 structure has been split (meaning two unconnected tile blobs).
            // The important bit is that this function calls ExecuteDestroy() at splits.

            if (CheckMultipleOccupancy(tile + new Vector3Int(0, 1, 0)) &&
                CheckMultipleOccupancy(tile - new Vector3Int(0, 1, 0))) 
            {
                ExecuteDestroy(tile + new Vector3Int(0, 1, 0), tile - new Vector3Int(0, 1, 0));
            }
            else if (CheckMultipleOccupancy(tile + new Vector3Int(1, 0, 0)) &&
                CheckMultipleOccupancy(tile - new Vector3Int(1, 0, 0)))
            {
                ExecuteDestroy(tile + new Vector3Int(1, 0, 0), tile - new Vector3Int(1, 0, 0));
            }
            else if (CheckMultipleOccupancy(tile + new Vector3Int(0, 1, 0)) &&
                     CheckMultipleOccupancy(tile - new Vector3Int(1, 0, 0))
            )
            {
                ExecuteDestroy(tile + new Vector3Int(0, 1, 0), tile - new Vector3Int(1, 0, 0));
            }
            else if (CheckMultipleOccupancy(tile + new Vector3Int(0, 1, 0)) &&
                     CheckMultipleOccupancy(tile + new Vector3Int(1, 0, 0))
            )
            {
                ExecuteDestroy(tile + new Vector3Int(0, 1, 0), tile + new Vector3Int(1, 0, 0));
            }
            else if (CheckMultipleOccupancy(tile - new Vector3Int(0, 1, 0)) &&
                     CheckMultipleOccupancy(tile + new Vector3Int(1, 0, 0))
            )
            {
                ExecuteDestroy(tile - new Vector3Int(0, 1, 0), tile + new Vector3Int(1, 0, 0));
            }
            else if (CheckMultipleOccupancy(tile - new Vector3Int(0, 1, 0)) &&
                     CheckMultipleOccupancy(tile - new Vector3Int(1, 0, 0))
            )
            {
                ExecuteDestroy(tile - new Vector3Int(0, 1, 0), tile - new Vector3Int(1, 0, 0));
            }
        }

        /// <summary>
        /// Takes in the positions of two blocks located within two suspected halfs of the structure, determines if we have actually cut the
        /// structure in half (no connections between the blobs) and destroyes the smaller blob.
        /// </summary>
        /// <param name="tile1"></param>
        /// <param name="tile2"></param>
        private void ExecuteDestroy(Vector3Int tile1, Vector3Int tile2)
        {
            // TO REALLY UNDERSTAND HOW THIS WORKS, LOOK AT THE TileMapSplitCounter CLASS, A LOT OF IMPORTANT ANNOTATED LOGIC IS THERE.
            TileMapSplitCounter splitCounter = new TileMapSplitCounter(proximityTileMaps, tile1, tile2);
            List<Vector3Int> tilesToDestroy = splitCounter.Resolve();

            // Destory the tiles, if any, in the smaller half.
            if (tilesToDestroy != null)
            {
                foreach (Vector3Int tile3 in tilesToDestroy)
                {
                    foreach (Tilemap tmap in proximityTileMaps)
                    {
                        tmap.SetTile(tile3, null);
                    }
                }
            }
        }

        #endregion // PRIVATE_INTERFACE

        #region PUBLIC_INTERFACE

        /// <summary>
        /// Updates the position of the ghost building block in the grid
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
            Tile tile = CheckMultipleOccupancy(tilePos);
            if (tile != null)
            {
                int strength = GetTileStrength(GetTileType(tile));

                foreach (Tilemap tmap in proximityTileMaps)
                {
                    tmap.SetTile(tilePos, null);
                }

                DestroyLogic(tilePos);
                return strength;
            }

            return 0;
        }

        /// <summary>
        /// Checks if a given WORLD POSITION is occupied by a tile in ANY of the TileMaps.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public bool CheckOccupancy(Vector2 worldPosition)
        {
            return IsTileOccupied(tileMapGrid.WorldToCell(worldPosition));
        }

        #endregion // PUBLIC_INTERFACE
    }
}