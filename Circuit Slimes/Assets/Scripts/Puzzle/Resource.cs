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

        private List<Text> Texts { get; set; }
        public List<Draggable> Draggables { get; private set; }
        private bool CanEdit { get; set; }

        #endregion



        #region === Init Methods ===
        
        public Resource(string prefab)
        {
            this.Name = prefab;
            this.Amount = 0;
            this.Texts  = new List<Text>();
            this.Draggables = new List<Draggable>();
        }
        
        
        public Resource(string prefab, int amount)
        {
            this.Name = prefab;
            this.Amount = amount;
            this.Texts = new List<Text>();
            this.Draggables = new List<Draggable>();
        }

        #endregion


        #region === UI Methods ===

        public void DefineUI(Text text, Draggable draggable, bool canEdit)
        {
            this.CanEdit = canEdit;
            this.Texts.Add(text);
            
            if (draggable != null)
            {
                this.Draggables.Add(draggable);
            }

            this.UpdateUI();
        }


        private void UpdateUI()
        {
            foreach (Text text in this.Texts)
            {
                text.text = this.Amount.ToString();
            }


            bool enabled = (this.CanEdit || this.Available());
            foreach (Draggable drag in this.Draggables)
            {
                drag.enabled = enabled;
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

        public void Increase(bool buttonClicked = false)
        {
            if (!this.CanEdit || buttonClicked)
            {
                this.Amount++;
                this.UpdateUI();
            }
        }


        public void Decrease(bool buttonClicked = false)
        {
            if (!this.CanEdit || buttonClicked)
            {
                this.Amount = Mathf.Max(0, this.Amount - 1);
                this.UpdateUI();
            }
        }

        #endregion
    }
}
