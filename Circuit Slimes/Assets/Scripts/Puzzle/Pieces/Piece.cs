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
        public virtual Vector2Int[] GetFootprint()
        {
            Vector2Int[] footprint = new Vector2Int[1]
            {
                this.Coords
            };

            return footprint;
        }

        public bool TypeMatches(Piece other)
        {
            return this.Caracterization.Equals(other.Caracterization);
        }
        #endregion
    }
}
