using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;

namespace Puzzle
{
    public class Tile : MonoBehaviour
    {
        public LevelBoard Board { get; private set; } 

        public Vector2Int Coords { get; set; }

        public enum Types
        {
            None,
            Solder
        }

        public Types Type { get; protected set; }


        #region === Instantiate ===

        public static GameObject Instantiate(Transform parent, Types type, Vector2Int coords)
        {
            var prefabName = "";

            switch (type)
            {
                default:
                case Types.None:
                    break;

                case Types.Solder:
                    prefabName = "SolderTile";
                    break;
            }

            var position = LevelBoard.WorldCoords(coords);

            var rotation = Quaternion.identity;

            return GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Board Items/" + prefabName), position, rotation, parent);
        }

        #endregion

        #region === Ceeate Tile ===

        public static Tile CreateTile(Transform parent, LevelBoard board, Vector2Int coords, Types type)
        {
            GameObject obj = null;

            switch (type)
            {
                default:
                case Types.None:
                    return null;

                case Types.Solder:
                    obj = Tile.Instantiate(parent, type, coords);

                    var tile = obj.GetComponent<Tile>();

                    tile.Initialize(board, coords, type);

                    return tile;
            }
        }

        public static Tile CreateTile(Transform parent, LevelBoard board, Vector2Int coords, string prefabName)
        {
            Types type = GetType(prefabName);

            return CreateTile(parent, board, coords, type);
        }
        #endregion

        #region === Enum Methods ===

        public static Types GetType(string prefabName)
        {
            if (prefabName.Contains("Solder"))
            {
                return Types.Solder;
            }
            else
            {
                return Types.None;
            }
        }

        #endregion

        #region === Init ===

        public void Initialize(LevelBoard board, Vector2Int coords, Types type)
        {
            this.Board = board;

            this.Type = type;

            this.Coords = coords;
        }

        #endregion

        #region === Tile Methods ===

        // Checks if space on board has tile of a certain type or not
        public bool hasTile(Vector2Int coords, Types type)
        {
            return !this.Board.OutOfBounds(coords) && this.Board.GetTile(coords) != null && this.Board.GetTile(coords).Type == type;
        }

        // Checks the cross adjacent spaces for tiles of the specified type
        public ArrayList checkCrossAdjacentsTiles(Types type)
        {
            ArrayList adjacents = new ArrayList();

            for (var i = 0; i < 4; i++)
            {
                var ind = i * 2;

                var coords = this.Coords + LevelBoard.DirectionalVectors[ind];

                adjacents.Add(this.hasTile(coords, type));
            }

            return adjacents;
        }

        #endregion

        #region === Unity Methods ===

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion
    }
}
