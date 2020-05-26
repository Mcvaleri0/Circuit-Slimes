using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



namespace Creator.UI.Drawer
{
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region /* Drag Attributes */

        private Vector3 StartPosition { get; set; }
        private bool Dragging { get; set; }

        #endregion


        #region /* Drawer Attributes */
        
        private DrawerController Controller { get; set; }

        #endregion


        #region /* Option Attributes */
        
        private Option Option { get; set; }

        #endregion



        #region === Unity Events ===

        private AudioManager AudioManager;
        private void Start()
        {
            this.AudioManager = FindObjectOfType<AudioManager>();
        }

        private void LateUpdate()
        {
            if (this.Dragging)
            {
                this.transform.position = Input.mousePosition;
            }
        }

        #endregion


        #region === Init Methods ===

        public void Initialize(DrawerController controller, Option option)
        {
            this.Controller = controller;
            this.Option = option;

            this.Dragging = false;
        }
        
        #endregion


        #region === Drag Methods ===

        public void InitializeDrag()
        {
            this.StartPosition = this.transform.localPosition;
            this.Dragging = true;

            if (this.Controller != null)
            {
                this.Controller.AddToQuick(this.Option.Name());
                this.Controller.Close();
            }

            AudioManager.Play("PickUp");
        }


        public void EndDrag()
        {
            this.transform.localPosition = this.StartPosition;
            this.Dragging = false;

            this.Option.PlaceItem();

            AudioManager.Play("Drop");
        }


        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            this.InitializeDrag();
        }


        void IDragHandler.OnDrag(PointerEventData eventData)
        {
        }


        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            this.EndDrag();
        }

        #endregion
    }
}
