using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Creator.UI.Drawer;



namespace Puzzle
{
    public class Resource 
    {
        #region /* Attributes */
        
        public string Name { get; private set; }
        public int Amount { get; private set; }

        private Text Text { get; set; }
        private Draggable Draggable { get; set; }
        private bool CanEdit { get; set; }

        #endregion



        #region === Init Methods ===
        
        public Resource(string prefab)
        {
            this.Name = prefab;
            this.Amount = 0;
            this.Text   = null;
        }
        
        
        public Resource(string prefab, int amount)
        {
            this.Name = prefab;
            this.Amount = amount;
            this.Text   = null;
        }

        #endregion


        #region === UI Methods ===

        public void DefineUI(Text text, Draggable draggable, bool canEdit)
        {
            this.Text = text;
            this.Draggable = draggable;
            this.CanEdit = canEdit;

            this.UpdateUI();
        }


        private void UpdateUI()
        {
            if (this.Text != null)
            {
                this.Text.text = this.Amount.ToString();
            }
            
            if (this.Draggable != null)
            {
                this.Draggable.enabled = (this.CanEdit || this.Available());
            }
        }

        #endregion


        #region === Info Methods ===

        public bool Available()
        {
            return this.Amount > 0;
        }


        public bool isTile()
        {
            return this.Name.Contains("Tile");
        }

        #endregion


        #region === Amount Methods ===

        public void Increase()
        {
            this.Amount++;
            this.UpdateUI();
        }


        public void Decrease()
        {
            this.Amount = Mathf.Max(0, this.Amount - 1);
            this.UpdateUI();
        }

        #endregion
    }
}
