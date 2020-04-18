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

        private bool NeedsUpdate = false;


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

            return GameObject.Instantiate((GameObject)Resources.Load("Prefabs/" + prefabName), position, rotation, parent);
        }


        public void Initialize(LevelBoard board, Vector2Int coords, Types type)
        {
            this.Board = board;

            this.Type = type;

            this.Coords = coords;
        }


        #region === Aux Methods ===

        // Checks if space on board has tile of a certain type or not
        public bool hasTile(Vector2Int coords, Types type)
        {
            if (this.Board.OutOfBounds(coords)) return false;

            var tile = this.Board.GetTile(coords);

            return (tile != null) && tile.isActiveAndEnabled && tile.Type == type;
        }

        // Checks the cross adjacent spaces for tiles of the specified type
        public ArrayList checkCrossAdjacentsTiles(Vector2Int coords, Types type)
        {
            ArrayList adjacents = new ArrayList();

            for (var i = 0; i < 4; i++)
            {
                var ind = i * 2;

                var xy = coords + LevelBoard.DirectionalVectors[ind];

                adjacents.Add(this.hasTile(xy, type));
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
            ArrayList adjacents = new ArrayList();

            for (var i = 0; i < 4; i++)
            {
                var ind = i * 2;

                var xy = this.Coords + LevelBoard.DirectionalVectors[ind];

                if(this.hasTile(xy, this.Type))
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
            this.UpdateTile();
            this.UpdateCrossTiles();
        }
        protected virtual void Start()
        {
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
            if (NeedsUpdate) { this.UpdateTile(); NeedsUpdate = false; }
        }

        #endregion

    }
}
