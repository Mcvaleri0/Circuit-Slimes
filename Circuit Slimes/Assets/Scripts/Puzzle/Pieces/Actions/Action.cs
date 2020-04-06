using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class Action
    {
        public virtual Action Available(Agent agent)
        {
            return null;
        }

        public virtual bool Execute(Agent agent)
        {
            return true;
        }
    }
}