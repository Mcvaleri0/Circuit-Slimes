using UnityEngine;
using UnityEngine.UI;



namespace Creator
{
    public class AvailableButton : MonoBehaviour
    {
        private bool Available { get; set; }
        private Color AvailableColor    { get; set; }
        private Color NotAvailableColor { get; set; }

        private CreatorController Controller { get; set; }
        private string Prefab { get; set; }


        public void Initialize(CreatorController controller, string prefab, bool available = false)
        {
            this.Controller = controller;
            this.Prefab     = prefab;
            this.Available  = available;

            this.AvailableColor    = Color.green;
            this.NotAvailableColor = Color.white;

            this.ChangeColor(available);
        }

        private void ChangeColor(bool available)
        {
            if (available)
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
                this.Controller.AddPermission(this.Prefab);
            }
            else
            {
                this.Available = available;
                this.Controller.RemovePermission(this.Prefab);
            }

            this.ChangeColor(available);
        }

    }

}
