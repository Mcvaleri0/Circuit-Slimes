using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Puzzle
{
    public class Resource 
    {
        #region /* Attributes */
        
        public string Prefab { get; private set; }
        public int Amount { get; private set; }

        #endregion



        #region === Init Methods ===
        
        public Resource(string prefab)
        {
            this.Prefab = prefab;
            this.Amount = 0;
        }
        
        
        public Resource(string prefab, int amount)
        {
            this.Prefab = prefab;
            this.Amount = amount;
        }

        #endregion


        #region === Info Methods ===

        public bool WorthSaving()
        {
            return this.Amount != 0;
        }

        #endregion


        #region === Amount Methods ===

        public void Increase()
        {
            this.Amount++;
        }


        public void Decrease()
        {
            this.Amount = Mathf.Max(0, this.Amount - 1);
        }

        #endregion
    }
}
