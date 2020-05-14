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


        #region === Create Tile ===
        public static Tile CreateTile(Puzzle puzzle, Types type, Vector2Int coords)
        {
            return CreateTile(puzzle, Tile.GetName(type), coords);
        }

        public static Tile CreateTile(Puzzle puzzle, string prefabName, Vector2Int coords)
        {
            Transform parent = puzzle.TilesObj.transform;

            GameObject obj = Tile.Instantiate(parent, prefabName, coords);

            Tile tile = obj.GetComponent<Tile>();

            tile.Initialize(puzzle, coords, Tile.GetType(prefabName));

            return tile;
        }


        public static GameObject Instantiate(Transform parent, string prefabName, Vector2Int coords)
        {
            var position = LevelBoard.WorldCoords(coords);

            var rotation = Quaternion.identity;

            return GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Board Items/" + prefabName), position, rotation, parent);
        }


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


        #region === Type and Name conversion ===

        public static string GetName(Types type)
        {
            switch(type)
            {
                default:
                    return "";

                case Types.Solder:
                    return "SolderTile";
            }
        }

        public static Types GetType(string prefabName)
        {
            string typeName = prefabName.Substring(0, prefabName.Length - 4);

            switch(typeName)
            {
                default:
                    return Types.None;

                case "Solder":
                    return Types.Solder;
            }
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

        //FIXME: we should extract tile behaviour from solder slime and abstract it to "TilesetTile" class
        public virtual void UpdateTile(int tilesetNum) { } 


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
            this.UpdateTile(0);
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
