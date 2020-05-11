using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;


namespace Puzzle
{
    public class Piece : MonoBehaviour
    {
        public Puzzle Puzzle { get; private set; }

        public LevelBoard Board { get; private set; }

        public Vector2Int Coords { get; set; }

        public LevelBoard.Directions Orientation { get; set; }

        public virtual Vector2Int[] Footprint { get; set; } = { new Vector2Int(0, 0) };

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

            public Caracteristics(string name)
            {
                this.Category = ParseCategory(name);

                switch (this.Category)
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
                else
                {
                    var componentType = ParseComponentType(name);

                    if (componentType != ComponentTypes.None) return Categories.Component;

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

                switch (this.Category)
                {
                    default:
                        break;

                    case Categories.Slime:
                        name = SlimeTypeToString() + name;
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
                switch (this.Category)
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
                switch (this.SlimeType)
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
                switch (this.CandyType)
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
            public bool Matches(Caracteristics caracteristics)
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
            #endregion
        }

        public Caracteristics Caracterization;


        #region === Create Piece ===
        public static Piece CreatePiece(Puzzle puzzle, Vector2Int coords, Caracteristics caracterization,
            LevelBoard.Directions ori = LevelBoard.Directions.East, int turn = 0)
        {
            return CreatePiece(puzzle, coords, caracterization.ToString(), ori, turn);
        }

        public static Piece CreatePiece(Puzzle puzzle, Vector2Int coords, string prefabName,
                                        LevelBoard.Directions ori = LevelBoard.Directions.East, int turn = 0)
        {
            GameObject obj = Instantiate(puzzle, prefabName, coords);

            var piece = obj.GetComponent<Piece>();

            piece.Rotate(ori);

            // Agent Init
            if(piece is Agent agent)
            {
                // Slime Init
                if (agent is Pieces.Slimes.Slime slime)
                {
                    slime.Initialize(puzzle, coords, new Caracteristics(prefabName), ori, turn);
                }
                // Component Init
                else  if(agent is Pieces.Components.CircuitComponent component)
                {
                    component.Initialize(puzzle, coords, new Caracteristics(prefabName), ori, turn);
                }
            }

            // Piece Init
            else
            {
                piece.Initialize(puzzle, coords, new Caracteristics(prefabName));
            }

            return piece;
        }

        
        private static GameObject Instantiate(Puzzle puzzle, string prefabName, Vector2Int coords)
        {
            var position = LevelBoard.WorldCoords(coords);

            var rotation = Quaternion.identity;

            var parent = puzzle.transform.GetChild(0);

            return GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Board Items/" + prefabName), position, rotation, parent);
        }


        protected virtual void Initialize(Puzzle puzzle, Vector2Int coords, Caracteristics caracterization)
        {
            this.Puzzle = puzzle;

            this.Board = puzzle.Board;

            this.Coords = coords;

            this.Caracterization = caracterization;
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

        //Calculate footprint in world coords
        private Vector2Int[] CalculateFootprint(Vector2Int coords, LevelBoard.Directions orientation) {
            
            //SINGLE TILE
            if (this.Footprint.Length == 1) return new Vector2Int[] { coords };

            //MULTI-TILE
            var footprint = new Vector2Int[this.Footprint.Length];

            for (var i = 0; i < footprint.Length; i++)
            {
                //rotate footprint to match orientation
                var xx = this.Footprint[i].x;
                var yy = this.Footprint[i].y;

                switch (orientation)
                {
                    // 0 degree rotation 
                    case LevelBoard.Directions.East:
                    default:
                        footprint[i].x = xx;
                        footprint[i].y = yy;
                        break;

                    // +90 degree rotation
                    case LevelBoard.Directions.North:
                        footprint[i].x = yy;
                        footprint[i].y = -xx;
                        break;

                    // +180 degree rotation
                    case LevelBoard.Directions.West:
                        footprint[i].x = -xx;
                        footprint[i].y = -yy;
                        break;

                    // -90 degree rotation
                    case LevelBoard.Directions.South:
                        footprint[i].x = -yy;
                        footprint[i].y = xx;
                        break;
                }

                //apply footprint to world coord point
                footprint[i] = coords + footprint[i];
            }

            return footprint;
        }


        //get the shape of footprint (aka footprint centered at 0,0)
        public virtual Vector2Int[] GetFootprintShape()
        {
            return this.CalculateFootprint(new Vector2Int(0,0), this.Orientation);
        }


        //Methods for getting footprint with diferent orientation and position
        public virtual Vector2Int[] GetFootprint()
        {
            return this.CalculateFootprint(this.Coords, this.Orientation);
        }

        public virtual Vector2Int[] GetFootprintAt(Vector2Int coords)
        {
            return this.CalculateFootprint(coords, this.Orientation);
        }

        public virtual Vector2Int[] GetFootprintAt(LevelBoard.Directions orientation)
        {
            return this.CalculateFootprint(this.Coords, orientation);
        }

        public virtual Vector2Int[] GetFootprintAt(Vector2Int coords, LevelBoard.Directions orientation)
        {
            return this.CalculateFootprint(coords, orientation);
        }


        //Gets the Offset between point coords and the footprint's origin
        public Vector2Int GetFootPrintOffset(Vector2Int coords)
        {
            var footprint = this.GetFootprint();
            var orig = footprint[0];

            return coords - orig;
        }


        //rotate piece
        public virtual bool Rotate(LevelBoard.Directions targetDir)
        {
            float targetAngle = 360 - ((float) targetDir) * 45f;
            if (targetAngle == 360) targetAngle = 0;

            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, targetAngle, this.transform.eulerAngles.z);

            this.Orientation = targetDir;

            return true;
        }


        public virtual bool TypeMatches(Piece other)
        {
            return this.Caracterization.Equals(other.Caracterization);
        }
        #endregion
    }
}
