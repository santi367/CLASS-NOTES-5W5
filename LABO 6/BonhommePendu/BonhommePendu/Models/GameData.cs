using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

namespace BonhommePendu.Models
{
    public class GameData
    {
        public const char DEFAULT_LETTER = '*';
        public const int NB_WRONG_TRIES_FOR_LOSING = 6;

        private int NbRevealedLetters { get; set; }
        private int NbLetters { get; set; }

        [JsonIgnore]
        public string Word { get; private set; }

        // Une liste qui contient toutes les lettres essayées, qu'elles soient dans le mot ou pas
        public List<char> GuessedLetters { get; set; }

        public int NbWrongGuesses { get; set; }
        // Le mot avec un mélange des lettres trouvées et de *
        public string RevealedWord { get; set; }
        public bool Won { get; set; }
        public bool Lost { get; set; }

        public bool HasGuessedTheWord { get { return NbRevealedLetters >= NbLetters; } }

        public GameData(string word)
        {
            Word = word;
            NbLetters = word.Length;
            NbRevealedLetters = 0;
            NbWrongGuesses = 0;
            RevealedWord = "";
            GuessedLetters = new List<char>();
            for(int i = 0; i < NbLetters; i++)
            {
                RevealedWord += DEFAULT_LETTER;
            }
        }

        public bool HasSameLetterAtIndex(char letter, int index)
        {
            var letterInWord = Word[index];

            // Pas de support du français pour l'instant
            bool memeLettre = letter == letterInWord;
            
            // Si la lettre n'est pas encore découverte
            return memeLettre;
        }

        public char RevealLetter(int index)
        {
            char letter = Word[index];

            StringBuilder sb = new StringBuilder(RevealedWord);
            sb[index] = letter;
            RevealedWord = sb.ToString();
            NbRevealedLetters++;

            return letter;
        }
    }
}
