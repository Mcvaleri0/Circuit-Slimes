using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Level;

namespace Hint
{
    public class HintController
    {

        public LevelController LevelController;


        public HintController() {}


        public string[] HelpInfo()
        {
            var levelName = this.LevelController.CurrentLevel;
            var title = "Hint";
            var message = "Sorry, No hint available for this level ;) ";

            switch (levelName)
            {
                case "Tutorial Level 0":

                    title = "Tutorial";
                    message = "Press the Play button and watch the electric slimes.";
                    break;

                case "Tutorial Level 1":

                    title = "Tutorial";
                    message = "Press the Play button and watch the electric slimes.";
                    break;

                case "Tutorial Level 2":

                    title = "Hint";
                    message = "Hi hi.";
                    break;
            }

            return new string[] {title, message};
        }



        public void Help()
        {

            var info = HelpInfo();
            var title = info[0];
            var message = info[1];

            var popupsys = GameObject.Find("BackgroundPop").GetComponent<PopUpSystem>();
            popupsys.PopUp(title, message, false);

        }


        public void ShowTutorial()
        {

            var info = HelpInfo();
            var title = info[0];

            if (title == "Tutorial")
            {
                this.Help();
            }
        }

    }
}
