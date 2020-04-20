using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;

namespace Puzzle
{
    public class Tile : MonoBehaviour
    {
        public Puzzle Puzzle { get; private set; }

        public LevelBoard Board { get; private set; }

        public Vector2Int Coords { get; set; }

        public enum Types
        {
            None,
            Solder
        }

        public Types Type { get; protected set; }

        private bool NeedsUpdate = false;


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


        #region === Create Tile ===
        public static Tile CreateTile(Puzzle puzzle, Vector2Int coords, Types type)
        {
            Transform parent = puzzle.TilesObj.transform;

            GameObject obj;

            switch (type)
            {
                default:
                case Types.None:
                    return null;

                case Types.Solder:
                    obj = Tile.Instantiate(parent, type, coords);

                    var tile = obj.GetComponent<Tile>();

                    tile.Initialize(puzzle, coords, type);

                    return tile;
            }
        }

        public static Tile CreateTile(Puzzle puzzle, Vector2Int coords, string prefabName)
        {
            Types type = GetType(prefabName);

            return CreateTile(puzzle, coords, type);
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

        public void Initialize(Puzzle puzzle, Vector2Int coords, Types type)
        {
            this.Puzzle = puzzle;

            this.Board = puzzle.Board;

            this.Type = type;

            this.Coords = coords;

            this.UpdateTile();
            this.UpdateCrossTiles();
        }

        #endregion


        #region === Tile Methods ===
        // Checks if space on board has tile of a certain type or not
        public bool HasTile(Vector2Int coords, Types type)
        {
            if (this.Board == null || this.Board.OutOfBounds(coords)) return false;

            var tile = this.Board.GetTile(coords);

            return (tile != null) && tile.isActiveAndEnabled && tile.Type == type;
        }

        // Checks the cross adjacent spaces for tiles of the specified type
        public ArrayList CheckCrossAdjacentsTiles(Vector2Int coords, Types type)
        {
            ArrayList adjacents = new ArrayList();

            for (var i = 0; i < 4; i++)
            {
                var ind = i * 2;

                var xy = coords + LevelBoard.DirectionalVectors[ind];

                adjacents.Add(this.HasTile(xy, type));
            }

            return adjacents;
        }
        #endregion


        #region === TileUpdate Methods ===

        //update a tile's mesh
        public virtual void UpdateTile() {}


        //Mark the tiles arround this one, in 4 directions, has needing an update
        public virtual void UpdateCrossTiles()
        {
            for (var i = 0; i < 4; i++)
            {
                var ind = i * 2;

                var xy = this.Coords + LevelBoard.DirectionalVectors[ind];

                if(this.HasTile(xy, this.Type))
                {
                    this.Board.GetTile(xy).NeedsUpdate = true;
                }
            }
        }

        #endregion


        #region === Unity Methods ===
        //Update this Tile and Tiles around if created/enable
        protected virtual void OnEnable()
        {
            if (this.Board == null) return;

            this.UpdateTile();
            this.UpdateCrossTiles();
        }

        protected virtual void Start()
        {
            if (this.Board == null) return;

            this.UpdateTile();
            this.UpdateCrossTiles();
        }


        //Update Tiles arround this tile if destroyed/disabled
        protected virtual void OnDisable() {
            this.UpdateCrossTiles();
        }

        protected virtual void OnDestroy()
        {
            this.UpdateCrossTiles();
        }


        // Update is called once per frame
        protected virtual void Update()
        {
            if (this.NeedsUpdate) { this.UpdateTile(); NeedsUpdate = false; }
        }
        #endregion

    }
}
