using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;
using Puzzle.Pieces.Slimes;

namespace Puzzle.Actions
{
    public class Kamikaze : SeekTarget
    {
        new public ElectricSlime Target { get; private set; }

        private bool TargetDeactivated = false;

        public Kamikaze() : base(new Piece.Caracteristics("ElectricSlime")) { }

        public Kamikaze(Agent agent, ElectricSlime target)
            : base(agent, target) 
        {
            this.Target = target;
        }


        #region === Action Methods ===
        public override Action Available(Agent agent)
        {
            var baseAction = (SeekTarget) base.Available(agent);

            if (baseAction != null)
            {
                if((baseAction.Target.Coords - agent.Coords).magnitude < 2)
                {
                    return new Kamikaze(agent, (ElectricSlime) baseAction.Target);
                }
                else
                {
                    return baseAction;
                }
            }

            return null;
        }

        public override bool Execute(Agent agent)
        {
            if(base.Execute(agent))
            {
                var tile = agent.Board.GetTile(this.MoveCoords);

                if(tile != null && tile.Type == Tile.Types.Solder)
                {
                    agent.Puzzle.RemoveTile(tile);

                    GameObject.Destroy(tile.gameObject);
                }

                if (!this.TargetDeactivated)
                {
                    this.Target.Deactivate();

                    this.TargetDeactivated = true;
                }

                agent.Deactivate();

                return true;
            }

            return false;
        }

        public override bool Undo(Agent agent)
        {
            agent.Reactivate(agent.Coords);

            if (this.TargetDeactivated)
            {
                this.Target.Reactivate(this.Target.Coords);

                this.TargetDeactivated = false;
            }

            var tile = agent.Board.GetTile(this.MoveCoords);

            if (tile == null)
            {
                tile = this.RecreateSolderTile(agent.Puzzle);

                agent.Puzzle.AddTile(tile);
            }

            return base.Undo(agent);
        }
        #endregion


        #region === Aux Methods ===
        protected Tile RecreateSolderTile(Puzzle puzzle)
        {
            var target = Tile.CreateTile(puzzle, this.MoveCoords, Tile.Types.Solder);

            return target;
        }
        #endregion
    }
}