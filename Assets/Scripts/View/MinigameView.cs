using LostControl.Model;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>
    /// Draws one kind of minigame inside the minigame panel and passes the player's input to its model.
    /// The panel activates the view of the connected ticket's minigame and hides the others.
    /// </summary>
    public abstract class MinigameView : MonoBehaviour
    {
        public abstract MinigameType Type { get; }

        /// <summary>Starts drawing this minigame, or stops with null. The model is always of this view's Type.</summary>
        public abstract void Show(MinigameModel minigame);
    }
}
