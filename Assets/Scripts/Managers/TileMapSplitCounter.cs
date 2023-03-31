using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
    /// <summary>
    /// This class is solely responsible for calculating if a breakage has cut the player base in half,
    /// if so, the Resolve() function returns a list of blocks in the smaller half, the ones to destroy.
    /// </summary>
    class TileMapSplitCounter
    {
        List<Tilemap> tilemap;

        // The list of visited tiles for each of the two halves
        List<List<Vector3Int>> visited;

        // Keeps track of the number of tiles on each side of the split
        List<int> count;

        /// <summary>
        /// Initialisation of the logic
        /// </summary>
        /// <param name="tilemap">A list of tilemaps (even though they are different tilemaps, they make up the same game structure, 
        ///                     such as the blocks map and the decorations map) to consider in the destroy logic (to count from and to possibly destroy from)</param>
        /// <param name="tile1">The tile on one edge of the breakage</param>
        /// <param name="tile2">The tile on the other suspected "half" of the breakage</param>
        public TileMapSplitCounter(List<Tilemap> tilemap, Vector3Int tile1, Vector3Int tile2)
        {
            this.tilemap = tilemap;

            count = new List<int>();
            for (int i = 0; i < 2; i++)
            {
                count.Add(0);
            }
            visited = new List<List<Vector3Int>>();
            for (int i = 0; i < 2; i++)
            {
                visited.Add(new List<Vector3Int>());
            }

            GetNeighboringTiles(tile1, 0);
            //Debug.Log("1: " + count[0]);
            GetNeighboringTiles(tile2, 1);
            //Debug.Log("2: " + count[1]);
        }

        /// <summary>
        /// Just returns the list of objects with the lowest amount (the lesser half),
        /// If the counts are the same, then that MUST mean the SHIP WAS NOT CUT IN HALF (as both the DFS's covered the exact same set of nodes).
        /// 
        /// You may wonder why this is even a separete function: easier to keep in my head + couldnt return the result in the constructor.
        /// </summary>
        /// <returns></returns>
        public List<Vector3Int> Resolve()
        {
            if (count[0] == count[1] )
            {
                return null;
            }
            else if (count[0] > count[1])
            {
                return visited[1];
            }
            else
            {
                return visited[0];
            }
        }

        /// <summary>
        /// Checks if we already came across this tile using it's position as an identifier
        /// </summary>
        /// <param name="tile">The position in question</param>
        /// <param name="index">Which "half" the algorithm is currently considering</param>
        /// <returns></returns>
        private bool CheckIfVisited(Vector3Int tile, int index)
        {
            for (int i = 0; i < visited[index].Count; i++)
            {
                Vector3Int pos = visited[index][i];
                if ((pos.x == tile.x) && (pos.y == tile.y) && (pos.z == tile.z))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a position is occupied with a tile
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        private bool CheckMultipleOccupancy(Vector3Int area)
        {
            foreach (Tilemap tmap in this.tilemap)
            {
                if (tmap.GetTile<Tile>(area) != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Recursive depth-first search that travels through the graph and counts every node it visits
        /// </summary>
        /// <param name="tile">What tile we are visiting</param>
        /// <param name="index">Which "half" of the breakage this algorithm is counting for</param>
        private void GetNeighboringTiles(Vector3Int tile, int index)
        {
            visited[index].Add(tile);

            //Debug.Log(index + ": Traverse step");

            // do not confuse this list as a queue for BFS, this is a non-standard method, this search is technically DFS.
            List<Vector3Int> surroundings = new List<Vector3Int>(4);

            // Repeat this for all possible neighbors
            // 1. calculate the position of a neighbor (up,down,left,right)
            // 2. check if we already vistied it
            // 3. if not, check if a tile is actually there
            // 4. if so, increment the count for the "half" we are considering, and add that node to the list of children to visit.

            Vector3Int up = tile + new Vector3Int(0, 1, 0);
            if (!CheckIfVisited(up, index))
            {
                if (CheckMultipleOccupancy(up))
                {
                    surroundings.Add(up);
                    count[index]++;
                }
            }

            Vector3Int down = tile - new Vector3Int(0, 1, 0);
            if (!CheckIfVisited(down, index))
            {
                if (CheckMultipleOccupancy(down))
                {
                    surroundings.Add(down);
                    count[index]++;
                }
            }

            Vector3Int left = tile - new Vector3Int(1, 0, 0);
            if (!CheckIfVisited(left, index))
            {
                if (CheckMultipleOccupancy(left))
                {
                    surroundings.Add(left);
                    count[index]++;
                }
            }

            Vector3Int right = tile + new Vector3Int(1, 0, 0);
            if (!CheckIfVisited(right, index))
            {
                if (CheckMultipleOccupancy(right))
                {
                    surroundings.Add(right);
                    count[index]++;
                }
            }

            // visit every unvisited neighbor we found
            foreach (Vector3Int adj in surroundings)
            {
                //Debug.Log(index + ": Adj");
                GetNeighboringTiles(adj, index);
            }
        }
    }
}