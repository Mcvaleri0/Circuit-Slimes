using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;
using Puzzle.Board;

namespace Puzzle.Pieces.Components
{
    public class Battery : CircuitComponent
    {
        #region Unity Events

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.KnownActions.Add(new Discharge());

            this.Stats = new Statistics(10, 0, -1)
            {
                Food = this.StartingCharges
            };
        }
        #endregion

        #region Component Methods
        protected override void UpdateConnections()
        {
            var footprint = this.GetFootprint();

            var origCoords = footprint[footprint.Length - 1];

            var tile = this.Puzzle.GetTile(origCoords);

            if (tile == null || tile.Type != Tile.Types.Solder) return;

            this.Connections.Clear();

            for (var i = 0; i < 4; i++)
            {
                int dirId = i * 2;

                tile = this.Puzzle.GetTile(origCoords + LevelBoard.DirectionalVectors[dirId]);

                if (tile != null && tile.Type == Tile.Types.Solder)
                {
                    this.Connections.Add((LevelBoard.Directions)dirId, tile.Coords);
                }
            }
        }


        override public Action Think()
        {
            return base.Think();
        }
        #endregion
    }
}