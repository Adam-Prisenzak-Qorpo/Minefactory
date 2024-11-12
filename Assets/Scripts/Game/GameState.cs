using UnityEngine;

namespace Minefactory.Game
{

    [CreateAssetMenu(fileName = "GameState", menuName = "Game/State")]
    public class GameState : ScriptableObject
    {
        public Interaction interaction = Interaction.None;
    }

}