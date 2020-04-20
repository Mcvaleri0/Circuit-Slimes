﻿using System.Collections;
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

        public Kamikaze() : base(new ElectricSlime()) 
        {
            this.Target = new ElectricSlime();
        }

        public Kamikaze(ElectricSlime target, Vector2Int targetCoords, LevelBoard.Directions direction)
            : base(target, targetCoords, direction) 
        {
            this.Target = target;
        }


        #region Action Methods
        public override Action Available(Agent agent)
        {
            var baseAction = (SeekTarget) base.Available(agent);

            if (baseAction != null)
            {
                if((baseAction.Target.Coords - agent.Coords).magnitude < 2)
                {
                    return new Kamikaze((ElectricSlime) baseAction.Target, baseAction.TargetCoords, baseAction.Direction);
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
                var tile = agent.Board.GetTile(this.TargetCoords);

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
        #endregion
    }
}