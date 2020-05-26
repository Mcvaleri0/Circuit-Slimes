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
                    message = "Press play and watch the Electric Slime move.";
                    break;

                case "Tutorial Level 1":

                    title = "Tutorial";
                    message = "Press play to see how the Solder Slime behaves. \n Add the Solder Candy, from the right, by dragging it onto the board. \n Place it so that it no longer blocks the line.";
                    break;

                case "Tutorial Level 2":

                    title = "Tutorial";
                    message = "Press play to see how the Water Slime behaves with Electric Slimes around. Notice what happens to the circuit where the Water Slime attacks the Electric Slime.";
                    break;

                case "Tutorial Level 3":

                    title = "Tutorial";
                    message = "Water Slimes would rather eat the candy than attack an Electric Slime, use that to your advantage.";
                    break;

                case "World 0 Level 0":

                    title = "Hint";
                    message = "Remember how on Tutorial Level 2, the Solder Slime blocked the Electric Slime's path? Sometimes that can help you.";
                    break;

                case "World 0 Level 1":

                    title = "Hint";
                    message = "Electric Slimes can only walk along the cables and make 90 degree turns. Other Slimes can move diagonally, maybe consider that.";
                    break;

                case "World 0 Level 2":

                    title = "Hint";
                    message = "By now, if you have observed the Electric Slimes with care, you might have an idea of how they chose to move through the circuits. Think on that and the solution will be clear.";
                    break;

                case "World 0 Level 3":

                    title = "Hint";
                    message = "This board is mirrored, so... If you solve one side, you will know the answer to both. \nHow do you keep the Red LEDs from getting lit up ? ";
                    break;

                case "World 0 Level 4":

                    title = "Hint";
                    message = "The Green LED on the left is simple enough to light up. The right one requires that the candy be placed strategically around near the Slime to the Right.";
                    break;

                case "World N Chip Choke":

                    title = "Hint";
                    message = "Chips can be a useful tool, slimes cannot go through them and must try to go around.";
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
