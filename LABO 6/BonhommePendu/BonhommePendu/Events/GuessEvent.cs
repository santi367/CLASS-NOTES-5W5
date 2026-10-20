using BonhommePendu.Models;

namespace BonhommePendu.Events
{
    // Un événement à créer chaque fois qu'un utilisateur essai une "nouvelle" lettre
    public class GuessEvent : GameEvent
    {
        public override string EventType { get { return "Guess"; } }

        // TODO: Compléter
        public GuessEvent(GameData gameData, char letter) {
            // TODO: Commencez par ICI
            Events = new List<GameEvent>();

            Events.Add(new GuessedLetterEvent(gameData, letter));
            bool found = false;
            
            for (int i = 0; i < gameData.RevealedWord.Length; i++)
            {
                if (gameData.HasSameLetterAtIndex(letter, i))
                {
                    found = true;
                    Events.Add(new RevealLetterEvent(gameData, i));
                }
            }

            if (!found)
            {
                Events.Add(new WrongGuessEvent(gameData));

                if (gameData.NbWrongGuesses >= GameData.NB_WRONG_TRIES_FOR_LOSING)
                {
                    Events.Add(new LostEvent(gameData));
                }
            }

            if (found && gameData.HasGuessedTheWord)
            {
                Events.Add(new WonEvent(gameData));
            }
        }
    }
}
