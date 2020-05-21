﻿using UnityEngine;
using System.Collections.Generic;
using Puzzle.Pieces.Slimes;
using Puzzle.Board;

namespace Puzzle {
    public class ElectricHivemind : MonoBehaviour
    {
        public Puzzle Puzzle { get; private set; }
    
        public List<SmartElectricSlime> Society { get; private set; }

        public Dictionary<Vector2Int, Crossing> Crossings { get; private set; }


        #region === Unity Methods ===
        // Use this for initialization
        void Start()
        {
            this.Puzzle = transform.parent.GetComponent<Puzzle>();

            this.Society = new List<SmartElectricSlime>();

            this.Crossings = new Dictionary<Vector2Int, Crossing>();
        }

        // Update is called once per frame
        void Update()
        {
            this.UpdateSociety();
        }
        #endregion


        #region === Society Methods ===
        public void UpdateSociety()
        {
            this.Society.Clear();

            foreach(var agent in this.Puzzle.Agents)
            {
                if (agent is SmartElectricSlime slime) this.Society.Add(slime);
            }
        }
        #endregion


        #region === Exploration Methods ===
        public void UpdateCrossing(Vector2Int crossingCoords, List<LevelBoard.Directions> available)
        {
            bool _new;

            if (!this.Crossings.ContainsKey(crossingCoords)) this.Crossings.Add(crossingCoords, new Crossing(available)); _new = true;

            var crossing = this.Crossings[crossingCoords];

            if(!_new) crossing.Update(available);
        }

        public void RegisterExplored(Vector2Int crossingCoords, LevelBoard.Directions exploredPath)
        {
            this.Crossings.TryGetValue(crossingCoords, out var crossing);

            // Should never happen
            if (crossing == null) return;

            crossing.MarkExplored(exploredPath);

            if (crossing.FullyExplored()) crossing.ClearExplored();
        }

        public List<LevelBoard.Directions> GetUnexplored(Vector2Int crossingCoords)
        {
            this.Crossings.TryGetValue(crossingCoords, out var crossing);

            if (crossing == null) return null;

            return crossing.GetUnexplored();
        }

        public void UnregisterExplored(Vector2Int crossingCoords, LevelBoard.Directions exploredPath)
        {
            this.Crossings.TryGetValue(crossingCoords, out var crossing);

            //Should never happen
            if (crossing == null) return;

            crossing.UnmarkExplored(exploredPath);
        }
        #endregion
    }
}