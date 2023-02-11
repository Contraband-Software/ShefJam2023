using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
    class TileMapSplitCounter
    {
        readonly Tilemap tilemap;

        List<List<Vector3Int>> visited;

        List<int> count;

        public TileMapSplitCounter(Tilemap tilemap, Vector3Int tile1, Vector3Int tile2)
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

        private void GetNeighboringTiles(Vector3Int tile, int index)
        {
            visited[index].Add(tile);

            //Debug.Log(index + ": Traverse step");

            List<Vector3Int> surroundings = new List<Vector3Int>(4);

            Vector3Int up = tile + new Vector3Int(0, 1, 0);
            if (!CheckIfVisited(up, index))
            {
                if (tilemap.GetTile<Tile>(up) != null)
                {
                    surroundings.Add(up);
                    count[index]++;
                }
            }

            Vector3Int down = tile - new Vector3Int(0, 1, 0);
            if (!CheckIfVisited(down, index))
            {
                if (tilemap.GetTile<Tile>(down) != null)
                {
                    surroundings.Add(down);
                    count[index]++;
                }
            }

            Vector3Int left = tile - new Vector3Int(1, 0, 0);
            if (!CheckIfVisited(left, index))
            {
                if (tilemap.GetTile<Tile>(left) != null)
                {
                    surroundings.Add(left);
                    count[index]++;
                }
            }

            Vector3Int right = tile + new Vector3Int(1, 0, 0);
            if (!CheckIfVisited(right, index))
            {
                if (tilemap.GetTile<Tile>(right) != null)
                {
                    surroundings.Add(right);
                    count[index]++;
                }
            }

            foreach (Vector3Int adj in surroundings)
            {
                //Debug.Log(index + ": Adj");
                GetNeighboringTiles(adj, index);
            }
        }
    }
}