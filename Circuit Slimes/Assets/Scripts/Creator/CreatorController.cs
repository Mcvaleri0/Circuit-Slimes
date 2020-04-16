using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace Creator
{
    public class CreatorController : MonoBehaviour
    {
        #region /* UI Attibutes */

        private ScrollMenu ScrollMenu { get; set; }

        #endregion

        #region === Unity Events ===

        void Start()
        {
            Transform menu  = this.transform.Find("Canvas").Find("Scroll Menu");
            this.ScrollMenu = new ScrollMenu(menu);
        }

        #endregion


    }
}
