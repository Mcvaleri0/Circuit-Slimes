using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;

namespace Puzzle
{
    public class Piece : MonoBehaviour
    {
        public LevelBoard Board { get; private set; }

        public Vector2Int Coords { get; set; }

        public enum Categories
        {
            None,
            Slime,
            Component,
            Candy
        }

        public enum SlimeTypes
        {
            None,
            Electric,
            Water,
            Solder
        }

        public enum ComponentTypes
        {
            None,
            LED,
            Battery
        }

        public Categories Category;
        public SlimeTypes SlimeType;
        public ComponentTypes ComponentType;


        public static GameObject Instantiate(Transform parent, SlimeTypes type, Vector2 coords)
        {
            var prefabName = "";

            switch (type)
            {
                default:
                case SlimeTypes.None:
                    break;

                case SlimeTypes.Electric:
                    prefabName = "ElectricSlime";
                    break;

                case SlimeTypes.Water:
                    prefabName = "WaterSlime";
                    break;

                case SlimeTypes.Solder:
                    prefabName = "SolderSlime";
                    break;
            }

            var position = LevelBoard.WorldCoords(coords);

            var rotation = Quaternion.identity;

            return GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Board Items/" + prefabName), position, rotation, parent);
        }

        public static GameObject Instantiate(Transform parent, ComponentTypes type, Vector2 coords)
        {
            var prefabName = "";

            switch (type)
            {
                default:
                case ComponentTypes.None:
                    break;

                case ComponentTypes.LED:
                    break;

                case ComponentTypes.Battery:
                    prefabName = "Battery";
                    break;
            }

            var position = new Vector3(1f, 1f, 1f);
            position.x = coords.x + 1f;
            position.z = coords.y + 1f;

            var rotation = Quaternion.identity;

            return GameObject.Instantiate((GameObject)Resources.Load("Prefabs/" + prefabName), position, rotation, parent);
        }


        #region === Init ===
        protected virtual void Initialize(LevelBoard board, Vector2Int coords, Categories category)
        {
            this.Board = board;

            this.Coords = coords;

            this.Category = category;
        }

        public virtual void Initialize(LevelBoard board, Vector2Int coords, SlimeTypes type = SlimeTypes.None)
        {
            this.Initialize(board, coords, Categories.Slime);

            this.SlimeType = type;

            this.ComponentType = ComponentTypes.None;
        }

        public virtual void Initialize(LevelBoard board, Vector2Int coords, ComponentTypes type = ComponentTypes.None)
        {
            this.Initialize(board, coords, Categories.Component);

            this.ComponentType = type;

            this.SlimeType = SlimeTypes.None;
        }
        #endregion

        #region === Unity Methods ===
        // Start is called before the first frame update
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {
            
        }
        #endregion
    }
}
