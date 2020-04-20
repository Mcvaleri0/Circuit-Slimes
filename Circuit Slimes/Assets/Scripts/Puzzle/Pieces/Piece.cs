using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;



namespace Puzzle
{
    public class Piece : MonoBehaviour
    {
        public Puzzle Puzzle { get; private set; }

        public LevelBoard Board { get; private set; }

        public Vector2Int Coords { get; set; }

        #region /* Enums */

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
            GreenLED,
            RedLED,
            CellBattery,
            AABattery,
            V9Battery,
            SingleChip,
            DoubleChip,
            SquareChip,
            SolderTile
        }

        public enum CandyTypes
        {
            None,
            Water,
            Solder
        }

        #endregion

        public Categories Category;
        public SlimeTypes SlimeType;
        public ComponentTypes ComponentType;
        public CandyTypes CandyType;

        public struct Caracteristics
        {
            public Piece.Categories Category { get; private set; }

            public Piece.SlimeTypes SlimeType { get; private set; }

            public Piece.ComponentTypes ComponentType { get; private set; }

            public Piece.CandyTypes CandyType { get; private set; }

            public Caracteristics(Piece.Categories category, Piece.SlimeTypes sType)
            {
                this.Category = category;

                this.SlimeType = sType;

                this.ComponentType = Piece.ComponentTypes.None;

                this.CandyType = Piece.CandyTypes.None;
            }

            public Caracteristics(Piece.Categories category, Piece.ComponentTypes cType)
            {
                this.Category = category;

                this.SlimeType = Piece.SlimeTypes.None;

                this.ComponentType = cType;

                this.CandyType = Piece.CandyTypes.None;
            }

            public Caracteristics(Piece.Categories category, Piece.CandyTypes cdType)
            {
                this.Category = category;

                this.SlimeType = Piece.SlimeTypes.None;

                this.ComponentType = Piece.ComponentTypes.None;

                this.CandyType = cdType;
            }

            public Caracteristics(Piece piece)
            {
                this.Category = piece.Category;

                this.SlimeType = piece.SlimeType;

                this.ComponentType = piece.ComponentType;

                this.CandyType = piece.CandyType;
            }

            public Caracteristics(string name)
            {
                this.Category = ParseCategory(name);

                switch(this.Category)
                {
                    default:
                        this.SlimeType = SlimeTypes.None;
                        this.ComponentType = ComponentTypes.None;
                        this.CandyType = CandyTypes.None;
                        break;

                    case Categories.Slime:
                        this.SlimeType = ParseSlimeType(name);
                        this.ComponentType = ComponentTypes.None;
                        this.CandyType = CandyTypes.None;
                        break;

                    case Categories.Component:
                        this.SlimeType = SlimeTypes.None;
                        this.ComponentType = ParseComponentType(name);
                        this.CandyType = CandyTypes.None;
                        break;

                    case Categories.Candy:
                        this.SlimeType = SlimeTypes.None;
                        this.ComponentType = ComponentTypes.None;
                        this.CandyType = ParseCandyType(name);
                        break;
                }
            }

            #region Parse Caracteristics
            public static Piece.Categories ParseCategory(string name)
            {
                if (name.Contains("Slime"))
                {
                    return Categories.Slime;
                }
                else if (name.Contains("Candy"))
                {
                    return Categories.Candy;
                }
                else if (name.Contains("Component"))
                {
                    return Categories.Component;
                }
                else
                {
                    return Categories.None;
                }
            }

            public static Piece.SlimeTypes ParseSlimeType(string name)
            {
                switch (name)
                {
                    case "ElectricSlime":
                        return SlimeTypes.Electric;

                    case "WaterSlime":
                        return SlimeTypes.Water;

                    case "SolderSlime":
                        return SlimeTypes.Solder;

                    default:
                        return SlimeTypes.None;
                }
            }

            public static Piece.ComponentTypes ParseComponentType(string name)
            {
                switch (name)
                {
                    case "GreenLED":
                        return ComponentTypes.GreenLED;

                    case "RedLED":
                        return ComponentTypes.RedLED;

                    case "CellBattery":
                        return ComponentTypes.CellBattery;

                    case "AABattery":
                        return ComponentTypes.AABattery;

                    case "9VBattery":
                        return ComponentTypes.V9Battery;

                    case "SingleChip":
                        return ComponentTypes.SingleChip;

                    case "DoubleChip":
                        return ComponentTypes.DoubleChip;

                    case "SquareChip":
                        return ComponentTypes.SquareChip;

                    case "SolderTile":
                        return ComponentTypes.SolderTile;

                    default:
                        return ComponentTypes.None;
                }
            }

            public static Piece.CandyTypes ParseCandyType(string name)
            {
                switch (name)
                {
                    case "WaterCandy":
                        return CandyTypes.Water;

                    case "SolderCandy":
                        return CandyTypes.Solder;

                    default:
                        return CandyTypes.None;
                }
            }
            #endregion

            #region ToString
            override public string ToString()
            {
                var name = CategoryToString();

                switch(this.Category)
                {
                    default:
                        break;

                    case Categories.Slime:
                        name += SlimeTypeToString();
                        break;

                    case Categories.Component:
                        name = ComponentTypeToString();
                        break;

                    case Categories.Candy:
                        name = CandyTypeToString() + name;
                        break;
                }

                return name;
            }

            private string CategoryToString()
            {
                switch(this.Category)
                {
                    default:
                    case Categories.Component:
                        return "";

                    case Categories.Slime:
                        return "Slime";

                    case Categories.Candy:
                        return "Candy";
                }
            }

            private string SlimeTypeToString()
            {
                switch(this.SlimeType)
                {
                    default:
                        return "";

                    case SlimeTypes.Electric:
                        return "Electric";

                    case SlimeTypes.Solder:
                        return "Solder";

                    case SlimeTypes.Water:
                        return "Water";
                }
            }

            private string ComponentTypeToString()
            {
                switch (this.ComponentType)
                {
                    case ComponentTypes.GreenLED:
                        return "GreenLED";

                    case ComponentTypes.RedLED:
                        return "RedLED";

                    case ComponentTypes.CellBattery:
                        return "CellBattery";

                    case ComponentTypes.AABattery:
                        return "AABattery";

                    case ComponentTypes.V9Battery:
                        return "9VBattery";

                    case ComponentTypes.SingleChip:
                        return "SingleChip";

                    case ComponentTypes.DoubleChip:
                        return "DoubleChip";

                    case ComponentTypes.SquareChip:
                        return "SquareChip";

                    case ComponentTypes.SolderTile:
                        return "SolderTile";

                    default:
                        return "";
                }
            }

            private string CandyTypeToString()
            {
                switch(this.CandyType)
                {
                    default:
                        return "";

                    case CandyTypes.Solder:
                        return "Solder";

                    case CandyTypes.Water:
                        return "Water";
                }
            }
            #endregion

            #region Equals
            public bool Equals(Caracteristics caracteristics)
            {
                if (this.Category == caracteristics.Category)
                {
                    switch (this.Category)
                    {
                        default:
                            break;

                        case Piece.Categories.Slime:
                            return this.SlimeType == caracteristics.SlimeType;

                        case Piece.Categories.Component:
                            return this.ComponentType == caracteristics.ComponentType;

                        case Piece.Categories.Candy:
                            return this.CandyType == caracteristics.CandyType;
                    }
                }

                return false;
            }

            public bool Equals(Piece piece)
            {
                if (this.Category == piece.Category)
                {
                    switch (this.Category)
                    {
                        default:
                            break;

                        case Piece.Categories.Slime:
                            return this.SlimeType == piece.SlimeType;

                        case Piece.Categories.Component:
                            return this.ComponentType == piece.ComponentType;

                        case Piece.Categories.Candy:
                            return this.CandyType == piece.CandyType;
                    }
                }

                return false;
            }
            #endregion
        }
        

        #region === Instantiate ===

        private static GameObject Instantiate(Transform parent, SlimeTypes type, Vector2 coords)
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

        private static GameObject Instantiate(Transform parent, ComponentTypes type, Vector2 coords)
        {
            var prefabName = "";

            switch (type)
            {
                default:
                case ComponentTypes.None:
                    break;

                case ComponentTypes.GreenLED:
                    prefabName = "GreenLED";
                    break;

                case ComponentTypes.RedLED:
                    prefabName = "RedLED";
                    break;

                case ComponentTypes.CellBattery:
                    prefabName = "CellBattery";
                    break;

                case ComponentTypes.AABattery:
                    prefabName = "AABattery";
                    break;

                case ComponentTypes.V9Battery:
                    prefabName = "9VBattery";
                    break;
                
                case ComponentTypes.SingleChip:
                    prefabName = "SingleChip";
                    break;

                case ComponentTypes.DoubleChip:
                    prefabName = "DoubleChip";
                    break;

                case ComponentTypes.SquareChip:
                    prefabName = "SquareChip";
                    break;

                case ComponentTypes.SolderTile:
                    prefabName = "SolderTile";
                    break;
            }

            var position = LevelBoard.WorldCoords(coords);

            var rotation = Quaternion.identity;

            return GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Board Items/" + prefabName), position, rotation, parent);
        }

        private static GameObject Instantiate(Transform parent, CandyTypes type, Vector2 coords)
        {
            var prefabName = "";

            switch (type)
            {
                default:
                case CandyTypes.None:
                    break;

                case CandyTypes.Water:
                    prefabName = "WaterCandy";
                    break;

                case CandyTypes.Solder:
                    prefabName = "SolderCandy";
                    break;
            }

            var position = LevelBoard.WorldCoords(coords);

            var rotation = Quaternion.identity;

            return GameObject.Instantiate((GameObject) Resources.Load("Prefabs/Board Items/" + prefabName), position, rotation, parent);
        }

        #endregion


        #region === Create Piece ===

        public static Piece CreatePiece(Puzzle puzzle, Vector2Int coords, string prefabName,
            LevelBoard.Directions ori = LevelBoard.Directions.East, int turn = 0)
        {
            Categories     cat       = GetCategory(prefabName);
            SlimeTypes     slimeType = GetSlimeType(prefabName);
            ComponentTypes compType  = GetComponentType(prefabName);
            CandyTypes     candyType = GetCandyType(prefabName);

            return CreatePiece(puzzle, coords, cat, slimeType, compType, candyType, ori, turn);
        }

        public static Piece CreatePiece(Puzzle puzzle, Vector2Int coords, 
                                        Categories category, SlimeTypes slimeType, 
                                        ComponentTypes compType, CandyTypes candyType,
                                        LevelBoard.Directions ori, int turn)
        {
            Transform parent = puzzle.PiecesObj.transform;

            GameObject obj;

            switch (category)
            {
                default:
                case Categories.None:
                    return null;

                case Categories.Slime:
                    obj = Instantiate(parent, slimeType, coords);

                    var slime = obj.GetComponent<Pieces.Slimes.Slime>();

                    if (slime != null)
                    {
                        slime.Initialize(puzzle, coords, slimeType, ori, turn);
                    }

                    return slime;

                case Categories.Component:
                    obj = Instantiate(parent, compType, coords);

                    var component = obj.GetComponent<Pieces.Components.CircuitComponent>();

                    if (component != null)
                    {
                        component.Initialize(puzzle, coords, compType, ori, turn);
                    }

                    return component;

                case Categories.Candy:
                    obj = Instantiate(parent, candyType, coords);

                    var candy = obj.GetComponent<Pieces.Candy>();

                    if (candy != null)
                    {
                        candy.Initialize(puzzle, coords, candyType);
                    }

                    return candy;
            }

        }

        
        #endregion


        #region === Enums Methods ===

        public static Categories GetCategory(string prefabName)
        {
            if (prefabName.Contains("Slime"))
            {
                return Categories.Slime;
            }
            else if (prefabName.Contains("Candy"))
            {
                return Categories.Candy;
            }
            else
            {
                return Categories.Component;
            }
        }

        public static SlimeTypes GetSlimeType(string prefabName)
        {
            switch(prefabName)
            {
                case "ElectricSlime":
                    return SlimeTypes.Electric;

                case "WaterSlime":
                    return SlimeTypes.Water;

                case "SolderSlime":
                    return SlimeTypes.Solder;

                default:
                    return SlimeTypes.None;
            }
        }

        public static ComponentTypes GetComponentType(string prefabName)
        {
            switch(prefabName)
            {
                case "GreenLED":
                    return ComponentTypes.GreenLED;

                case "RedLED":
                    return ComponentTypes.RedLED;

                case "CellBattery":
                    return ComponentTypes.CellBattery;

                case "AABattery":
                    return ComponentTypes.AABattery;

                case "9VBattery":
                    return ComponentTypes.V9Battery;

                case "SingleChip":
                    return ComponentTypes.SingleChip;

                case "DoubleChip":
                    return ComponentTypes.DoubleChip;

                case "SquareChip":
                    return ComponentTypes.SquareChip;

                case "SolderTile":
                    return ComponentTypes.SolderTile;

                default:
                    return ComponentTypes.None;
            }
        }

        public static CandyTypes GetCandyType(string prefabName)
        {
            switch(prefabName)
            {
                case "WaterCandy":
                    return CandyTypes.Water;

                case "SolderCandy":
                    return CandyTypes.Solder;

                default:
                    return CandyTypes.None;
            }
        }

        #endregion


        #region === Init ===
        protected virtual void Initialize(Puzzle puzzle, Vector2Int coords, Categories category)
        {
            this.Puzzle = puzzle;

            this.Board = puzzle.Board;

            this.Coords = coords;

            this.Category = category;
        }

        public virtual void Initialize(Puzzle puzzle, Vector2Int coords, SlimeTypes type = SlimeTypes.None)
        {
            this.Initialize(puzzle, coords, Categories.Slime);

            this.SlimeType = type;

            this.ComponentType = ComponentTypes.None;
        }

        public virtual void Initialize(Puzzle puzzle, Vector2Int coords, ComponentTypes type = ComponentTypes.None)
        {
            this.Initialize(puzzle, coords, Categories.Component);

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
        

        #region === Piece Methods ===
        public bool TypeMatches(Piece other)
        {
            if(other.Category == this.Category)
            {
                switch(this.Category)
                {
                    default:
                        return true;

                    case Categories.Slime:
                    case Categories.Candy:
                        return other.SlimeType == this.SlimeType;

                    case Categories.Component:
                        return other.ComponentType == this.ComponentType;
                }
            }

            return false;
        }
        #endregion
    }
}
