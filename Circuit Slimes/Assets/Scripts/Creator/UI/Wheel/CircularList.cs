using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using Level;
using Creator.Editor;



namespace Creator.UI.Wheel
{
    public class CircularList 
    {
        #region /* Puzzle Editor */
        
        private PuzzleEditor Editor { get; set; }
        
        #endregion


        #region /* List Attributes */

        private Transform List { get; set; }
        private Transform Background { get; set; }

        private ListPositionCtrl ListController { get; set; }
        private MyListBank ListBank { get; set; }

        private Object OptionPrefab { get; set; }

        #endregion



        #region === Init Methods ===

        public CircularList(PuzzleEditor Editor, Transform list, List<string> options)
        {
            this.Editor = Editor;

            this.List = list;
            this.Background = this.List.Find("Background");

            this.ListController = this.List.GetComponent<ListPositionCtrl>();
            this.ListBank = this.List.GetComponent<MyListBank>();

            this.OptionPrefab = Resources.Load(FileHelper.WHEEL_OPTION_PATH);

            this.Populate(options);
        }

        #endregion


        #region === List Manipulation Methods ===
        
        private void Populate(List<string> options)
        {
            int nOptions = options.Count;

            int[] ids = new int[nOptions];
            ListBox[] items = new ListBox[nOptions];

            this.ListController.centeredContentID = nOptions / 2;

            int i = 0;

            foreach (string opt in options)
            {
                GameObject newObj = WheelOption.CreateOption(this.Editor, this.ListController,
                                        this.OptionPrefab, this.List, opt);

                ids[i] = i;
                items[i] = newObj.GetComponent<ListBox>();

                i++;
            }

            this.ListController.listBoxes = items;
            this.ListBank._contents = ids;
        }


        private void Clear()
        {
            foreach (Transform child in this.List)
            {
                if (child != this.Background)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }


        public void Update(List<string> newOptions)
        {
            this.Clear();
            this.Populate(newOptions);
        }

        #endregion
    }
}
