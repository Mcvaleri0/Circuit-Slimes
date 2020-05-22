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

        #endregion


        #region /* Drawer Attributes */
        
        private DrawerController Controller { get; set; }

        #endregion


        #region /* Option Attributes */
        
        private Option Option { get; set; }
        
        #endregion



        #region === Init Methods ===

        public void Initialize(DrawerController controller, Option option)
        {
            this.Controller = controller;
            this.Option = option;
        }
        
        #endregion


        #region === Drag Methods ===

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            this.StartPosition = this.transform.position;
            this.Controller.AddToQuick(this.Option.Name());
        }


        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            this.transform.position = Input.mousePosition;
        }


        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            this.transform.position = this.StartPosition;
            this.Option.PlaceItem();
        }

        #endregion
    }
}
