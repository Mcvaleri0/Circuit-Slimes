using UnityEngine;
using UnityEngine.UI;

using Creator.Editor;



namespace Creator.UI.Buttons
{
    public class OptionButton : MonoBehaviour
    {
        #region /* Availability Attibutes */

        private bool Selected { get; set; }
        private Color SelectedColor { get; set; }
        private Color NotSelectedColor { get; set; }

        #endregion


        #region /* Puzzle Attributes */

        private PuzzleEditor Editor { get; set; }
        private string Item { get; set; }

        #endregion



        #region === Init Methods ===

        public void Initialize(PuzzleEditor editor, string item)
        {
            this.Editor   = editor;
            this.Item     = item;
            this.Selected = false;

            this.SelectedColor = Color.gray;
            this.NotSelectedColor = Color.white;

            this.ChangeColor();
        }

        #endregion


        #region === Color Methods ===

        private void ChangeColor()
        {
            if (this.Selected)
            {
                this.GetComponent<Image>().color = this.SelectedColor;
            }
            else
            {
                this.GetComponent<Image>().color = this.NotSelectedColor;
            }
        }

        #endregion


        #region === Selection Methods ===

        public void Select()
        {
            if (this.Selected)
            {
                this.Editor.RemoveItemToPlace();
            }
            else
            {
                this.Selected = true;
                this.ChangeColor();

                this.Editor.ItemToPlace(this.Item, this);
            }
        }


        public void Deselect()
        {
            this.Selected = false;
            this.ChangeColor();
        }

        #endregion

    }
}