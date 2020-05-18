using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;
using UnityEngine.Rendering;

namespace Puzzle
{
    public class Piece : MonoBehaviour
    {
        protected Puzzle Puzzle { get; private set; }

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
            Solder,
            SElectric
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

        public struct Characteristics
        {
            public Piece.Categories Category { get; private set; }

            public Piece.SlimeTypes SlimeType { get; private set; }

            public Piece.ComponentTypes ComponentType { get; private set; }

            public Piece.CandyTypes CandyType { get; private set; }

            public Characteristics(Piece.Categories category, Piece.SlimeTypes sType)
            {
                this.Category = category;

                this.SlimeType = sType;

                this.ComponentType = Piece.ComponentTypes.None;

                this.CandyType = Piece.CandyTypes.None;
            }

            public Characteristics(Piece.Categories category, Piece.ComponentTypes cType)
            {
                this.Category = category;

                this.SlimeType = Piece.SlimeTypes.None;

                this.ComponentType = cType;

                this.CandyType = Piece.CandyTypes.None;
            }

            public Characteristics(Piece.Categories category, Piece.CandyTypes cdType)
            {
                this.Category = category;

                this.SlimeType = Piece.SlimeTypes.None;

                this.ComponentType = Piece.ComponentTypes.None;

                this.CandyType = cdType;
            }

            public Characteristics(string name)
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

            #region Parse Characteristics
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

                    case "SmartElectricSlime":
                        return SlimeTypes.SElectric;

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

                    case SlimeTypes.SElectric:
                        return "SmartElectric";
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
            public bool Matches(Characteristics Characteristics)
            {
                if (this.Category == Characteristics.Category)
                {
                    switch (this.Category)
                    {
                        default:
                            break;

                        case Piece.Categories.Slime:
                            return this.SlimeType == Characteristics.SlimeType;

                        case Piece.Categories.Component:
                            return this.ComponentType == Characteristics.ComponentType;

                        case Piece.Categories.Candy:
                            return this.CandyType == Characteristics.CandyType;
                    }
                }

                return false;
            }
            #endregion
        }

        public Characteristics Characterization;


        #region === Create Piece ===
        public static Piece CreatePiece(Puzzle puzzle, Characteristics Characterization, Vector2Int coords,
            LevelBoard.Directions ori = LevelBoard.Directions.South, int turn = 0)
        {
            return CreatePiece(puzzle, Characterization.ToString(), coords, ori, turn);
        }

        public static Piece CreatePiece(Puzzle puzzle, string prefabName, Vector2Int coords,
                                        LevelBoard.Directions ori = LevelBoard.Directions.South, int turn = 0)
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
                    slime.Initialize(puzzle, coords, new Characteristics(prefabName), ori, turn);
                }
                // Component Init
                else  if(agent is Pieces.Components.CircuitComponent component)
                {
                    component.Initialize(puzzle, coords, new Characteristics(prefabName), ori, turn);
                }
            }

            // Piece Init
            else
            {
                piece.Initialize(puzzle, coords, new Characteristics(prefabName));
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


        protected virtual void Initialize(Puzzle puzzle, Vector2Int coords, Characteristics Characterization)
        {
            this.Puzzle = puzzle;

            this.Coords = coords;

            this.Characterization = Characterization;
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
        #region Footprint
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
        #endregion

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
            return this.Characterization.Equals(other.Characterization);
        }


        public virtual void Reveal()
        {
            var position = transform.position;

            position.y = 1f;

            transform.position = position;
        }

        public virtual void Hide()
        {
            var position = transform.position;

            position.y = -2f;

            transform.position = position;
        }

        public virtual bool IsHidden()
        {
            return transform.position.y == -2f;
        }
        #endregion
    }
}
