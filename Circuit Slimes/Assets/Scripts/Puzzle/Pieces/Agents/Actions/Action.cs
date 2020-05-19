using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Pieces;
using Puzzle.Board;

namespace Puzzle.Actions
{
    public class Action
    {
        public virtual Action Available(Agent agent)
        {
            return null;
        }

        public virtual void Begin(Agent agent)
        {
            return;
        }

        public virtual bool Confirm(Agent agent)
        {
            return true;
        }

        public virtual bool Execute(Agent agent)
        {
            return true;
        }

        public virtual bool End(Agent agent)
        {
            return true;
        }

        public virtual bool Undo(Agent agent)
        {
            return true;
        }


    }
}