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

        private bool Consuming = true;

        private bool TargetDeactivated = false;

        private Vector2Int AgentCoords { get; set; }

        public Kamikaze() : base(new Piece.Characteristics("ElectricSlime")) { }

        public Kamikaze(SeekTarget seek)
            : base(seek.MoveCoords, seek.Direction, seek.Target) 
        {
            this.Target = (ElectricSlime) seek.Target;
        }


        #region === Action Methods ===
        public override Action Available(Agent agent)
        {
            var baseAction = (SeekTarget) base.Available(agent);

            if (baseAction != null)
            {
                if((baseAction.Target.Coords - agent.Coords).magnitude < 2)
                {
                    return new Kamikaze(baseAction);
                }
                else
                {
                    return baseAction;
                }
            }

            return null;
        }

        public override bool Confirm(Agent agent)
        {
            if(agent.PieceExists(this.Target))
            {
                if(agent.PieceAt(this.TargetCoords) == this.Target)
                {
                    agent.RemovePiece(this.TargetCoords);

                    this.AgentCoords = agent.Coords;
                    agent.RemovePiece(agent.Coords);

                    return true;
                }
            }

            return false;
        }

        public override bool Execute(Agent agent)
        {
            if(base.Execute(agent))
            {
                if (this.Consuming)
                {
                    var tile = agent.TileAt(this.MoveCoords);

                    if (tile != null && tile.Type == Tile.Types.Solder)
                    {
                        agent.RemoveTile(this.MoveCoords);

                        GameObject.Destroy(tile.gameObject);

                        GameObject.Find("AudioManager").GetComponent<AudioManager>().Play("SlimeElectricExplosion");
                        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayRandom("SlimeElectric2", "SlimeElectric3");
                    }

                    if (!this.TargetDeactivated)
                    {
                        this.Target.Deactivate();

                        this.TargetDeactivated = true;
                    }

                    agent.Deactivate();
                }

                return true;
            }

            return false;
        }

        public override bool Undo(Agent agent)
        {
            agent.Reactivate(this.AgentCoords);

            if (this.TargetDeactivated)
            {
                this.Target.Reactivate(this.TargetCoords);

                this.TargetDeactivated = false;
            }

            var tile = agent.TileAt(this.MoveCoords);

            if (tile == null)
            {
                agent.CreateTile(Tile.Types.Solder, this.MoveCoords);
            }

            return base.Undo(agent);
        }
        #endregion
    }
}