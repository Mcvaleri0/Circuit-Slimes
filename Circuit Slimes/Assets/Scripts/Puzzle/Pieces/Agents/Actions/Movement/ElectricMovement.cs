using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;
using Puzzle.Pieces.Components;

namespace Puzzle.Actions
{
    public class ElectricMovement : Move
    {
        public ElectricMovement() : base() { }

        public ElectricMovement(LevelBoard.Directions dir, Vector2Int coords) :
            base(dir, coords) { }


        #region Action Methods
        override public Action Available(Agent agent)
        {
            var adjacents = CheckAdjacentSolderTiles(agent);
                       
            var start = ((int) agent.Orientation) / 2;

            for (var i = 0; i < 4; i++)
            {
                var dir = (LevelBoard.Directions) (((start + i + 3) % 4) * 2);

                if(adjacents.TryGetValue(dir, out var component))
                {
                    var tcoords = LevelBoard.GetAdjacentCoords(agent.Coords, dir);

                    if (tcoords.sqrMagnitude > 1000f) continue;

                    if (component == null)
                    {
                        return new ElectricMovement(dir, tcoords);
                    }
                    else
                    {
                        return new Charge(component);
                    }
                }
            }

            return null;
        }

        public override bool Confirm(Agent agent)
        {
            var tile = agent.TileAt(this.MoveCoords);

            if (tile != null && tile.Type == Tile.Types.Solder)
            {
                return base.Confirm(agent);
            }

            return false;
        }
        #endregion


        #region Aux
        protected Dictionary<LevelBoard.Directions, CircuitComponent> CheckAdjacentSolderTiles(Agent agent)
        {
            Dictionary<LevelBoard.Directions, CircuitComponent> adjacents = new Dictionary<LevelBoard.Directions, CircuitComponent>();
            
            for(var i = 0; i < 4; i++)
            {
                var dir = (LevelBoard.Directions) (i * 2);

                var coords = LevelBoard.GetAdjacentCoords(agent.Coords, dir);

                var tile  = agent.TileAt(coords);

                // If there's a Solder Tile there
                if(tile != null && tile.Type == Tile.Types.Solder)
                {
                    var piece = agent.PieceAt(coords);

                    if (piece == null)
                    {
                        adjacents.Add(dir, null);
                    }
                    else if(piece is CircuitComponent component &&
                        component.Stats.Food < component.Stats.MaxFood)
                    {
                        adjacents.Add(dir, component);
                    }
                }
            }

            return adjacents;
        }
        #endregion
    }
}