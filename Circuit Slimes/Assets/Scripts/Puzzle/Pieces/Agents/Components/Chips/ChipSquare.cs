﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Puzzle.Actions;
using Puzzle.Board;

namespace Puzzle.Pieces.Components
{
    public class ChipSquare : Chip
    {
        //footprint
        public override Vector2Int[] Footprint { get; set; } = {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        };

        //square chips don't really need to rotate
        new public bool Rotate(LevelBoard.Directions targetDir)
        {
            return false;   
        }
    }
}
