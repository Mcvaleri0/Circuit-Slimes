using UnityEngine;
using UnityEngine.UI;

using Creator.Editor;



namespace Creator.UI.Buttons
{
    public class AvailableButton : MonoBehaviour
    {
        #region /* Availability Attibutes */

        private bool Available { get; set; }
        private Color AvailableColor    { get; set; }
        private Color NotAvailableColor { get; set; }

        #endregion


        #region /* Puzzle Attributes */

        private PuzzleEditor Editor { get; set; }
        private string Prefab { get; set; }

        #endregion



        #region === Init Methods ===

        public void Initialize(PuzzleEditor editor, string prefab, bool available = false)
        {
            this.Editor    = editor;
            this.Prefab    = prefab;
            this.Available = available;

            this.AvailableColor    = Color.green;
            this.NotAvailableColor = Color.white;

            this.ChangeColor();
        }

        #endregion


        #region === Availability Methods ===

        private void ChangeColor()
        {
            if (this.Available)
            {
                this.GetComponent<Image>().color = this.AvailableColor;
            }
            else
            {
                this.GetComponent<Image>().color = this.NotAvailableColor;
            }
        }


        public void ToogleAvailability()
        {
            this.ChangeAvailability(!this.Available);
        }

        
        private void ChangeAvailability(bool available)
        {
            if (available)
            {
                this.Available = available;
                this.Editor.AddPermission(this.Prefab);
            }
            else
            {
                this.Available = available;
                this.Editor.RemovePermission(this.Prefab);
            }

            this.ChangeColor();
        }

        #endregion
    }

}
