using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;

namespace Puzzle
{
    public class Tile : MonoBehaviour
    {
        private LevelBoard Board;

        public Vector2Int Coords { get; set; }

        public enum Types
        {
            None,
            Solder
        }

        public Types Type { get; protected set; }


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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
