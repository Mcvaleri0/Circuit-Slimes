using UnityEngine;
using UnityEngine.UI;

using Creator.Editor;
using Puzzle;



namespace Creator.UI.Buttons
{
    public class AvailableButton : MonoBehaviour
    {
        #region /* Availability Attibutes */

        private bool Available { get; set; }
        private Color AvailableColor    { get; set; }
        private Color NotAvailableColor { get; set; }

        #endregion


        #region /* Resource Attributes */

        private Resource Resource { get; set; }

        #endregion



        #region /* Permission Attributes */
        
        private string prefab { get; set; }
        private PuzzleEditor Editor { get; set; }
        
        #endregion



        #region === Init Methods ===

        public void Initialize(PuzzleEditor editor, string prefab, bool available)
        //public void Initialize(Resource resource, bool available)
        {
            //this.Resource  = resource;
            this.prefab = prefab;
            this.Editor = editor;
            this.Available = available;

            this.AvailableColor = Color.green;
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
                //this.Resource.Increase();
                this.Editor.AddPermission(this.prefab);
            }
            else
            {
                this.Available = available;
                //this.Resource.Decrease();
                this.Editor.RemovePermission(this.prefab);
            }

            this.ChangeColor();
        }

        #endregion
    }

}
