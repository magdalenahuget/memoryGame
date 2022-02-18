using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace memoryGame
{
    using System;


    public class Program
    {
        public static void Main(String[] args)
        {
            GameController gameController = new GameController();
            gameController.Run();
        }
    }

    public class GameController
    {
        private List<string> allWords = new List<string>();
        private List<string> modeWords = new List<string>();
        private Player player;
        private int WORDS_NUMBER;
        private int GUESS_CHANCES;
        private bool isRunning = true;
        private List<HighScore> highScores = new List<HighScore>();

        
        public GameController()
        {
            player = new Player();
        }

        public void Run()
        {
            Console.WriteLine("Welcome to my memory game!");
            DisplayManager.ClearScreen();
            string playerName = GetPlayerName();
            Console.WriteLine("Welcome " + playerName);
            DisplayManager.PressAnyKeyToContinue();
            PlayMenu(playerName);
        }

        public String GetPlayerName()
        {
            // TODO: validate name
            Console.WriteLine("What is the name of the Player?");
            return Console.ReadLine();
        }

        private void PlayMenu(String name)
        {
            while (isRunning)
            {
                DisplayManager.DisplayMainMenu();
                var chooseOption = Console.ReadLine();
                DisplayManager.ClearScreen();
                switch (chooseOption)
                {
                    case "1":
                        PlayGame(name);
                        break;
                    case "2":
                        DisplayManager.DisplayCredits();
                        break;
                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Wrong input!");
                        break;
                }
            }
        }
        
        private void PlayGame(String name)
        {
            while (isRunning)
            {
                DisplayManager.DisplayModes();
                var chooseOption = Console.ReadLine();
                switch (chooseOption)
                {
                    case "1":
                        //  Easy
                        PlayMode(name, 4, 10, "easy");
                        break;
                    case "2":
                        //  Hard
                        PlayMode(name, 8, 15, "hard");
                        break;
                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Wrong input!");
                        break;
                }
            }
        }

        private void PlayMode(String name, int wordsNumberToGuess, int guessChances, String mode)
        {
            Console.WriteLine("Welcome to mode " + mode);
            DisplayManager.PressAnyKeyToContinue();
            int ROUND = 0;
            WORDS_NUMBER = wordsNumberToGuess;
            GUESS_CHANCES = guessChances;
            // GUESS_CHANCES = 1;
            int GUESS_CHANCES_LEFT = GUESS_CHANCES;
            player.SetName(name);
            player.SetHp(GUESS_CHANCES_LEFT);
            allWords = FileHandler.GetWordsList("../../Words.txt");
            modeWords = GetModeWords(allWords);
            List<String> aRowString = new List<String>(modeWords);
            List<String> bRowString = new List<String>(modeWords);
            Shuffle(aRowString);
            Shuffle(bRowString);
            List<Word> aRow = ConvertToWordsList(aRowString);
            List<Word> bRow = ConvertToWordsList(bRowString);
            
            // Helper - to see shuffled A and B rows
            // Console.WriteLine("aRowString = " + string.Join(", ", aRowString));
            // Console.WriteLine("bRowString = " + string.Join(", ", bRowString));
            
            bool isPlaying = true;
            while (isPlaying)
            {
                if (player.GetHp() == 0 || this.player.GetHits() == WORDS_NUMBER)
                {
                    isPlaying = false;
                    continue;
                }
                ROUND++;
                DisplayManager.DisplayHeader(player, ROUND);
                DisplayManager.DisplayTable(aRow, bRow);

                Coordinates firstCoordinates = AskForCoordinates();
                
                DisplayManager.DisplayHeader(player, ROUND);
                ToggleWord(aRow, bRow, firstCoordinates);
                DisplayManager.DisplayTable(aRow, bRow);
                Coordinates secondCoordinates = AskForCoordinates();
                ToggleWord(aRow, bRow, secondCoordinates);
                DisplayManager.DisplayTable(aRow, bRow);
                
                int hits = VerifyCompatibilityAndGetHitNumber(aRow, bRow);
                CoverTable(aRow, bRow);
                
                player.SetHits(hits);
                GUESS_CHANCES_LEFT--;
                player.SetHp(GUESS_CHANCES_LEFT);
                DisplayManager.PressAnyKeyToContinue();
            }
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            stopwatch.Stop();
            long gameTime = GetTimeSeconds(stopwatch);
            bool playerWon = IsWinner(player);
            if (playerWon)
            {
                int chancesAfterWon = GUESS_CHANCES - GUESS_CHANCES_LEFT;
                int playerScore = CalculatePlayerScore(gameTime, player, chancesAfterWon);
                Console.WriteLine("You solved the memory game after " + chancesAfterWon + " chances." +
                                  " It took you " + gameTime + " seconds. Your score is: " +
                                  playerScore + "\n\n");
                AddScoreToHighScores(playerScore);
            }
            else
            {
                Console.WriteLine("Sorry, You lost!");
            }
            
            
            NewGameMenu(player);
            DisplayManager.PressAnyKeyToContinue();
            DisplayManager.ClearScreen();
        }
        
        private List<String> GetModeWords(List<string> allWords)
        {
            List<int> indexes = GetIndexes(allWords);
            var modeWords = new List<String>();
            foreach (int index in indexes)
            {
                modeWords.Add(allWords[index]);
            }

            return modeWords;
        }
        
        public List<int> GetIndexes(List<string> words)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < words.Count; i++)
            {
                list.Add(i);
            }

            Shuffle(list); 
            List<int> indexes = new List<int>();
            for (int i = 0; i < WORDS_NUMBER; i++)
            {
                indexes.Add(list[i]);
                Console.WriteLine(list[i]);
            }

            return indexes;
        }
        
        private static Random rng = new Random();

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        
        private List<Word> ConvertToWordsList(List<string> aRowString)
        {
            var words = new List<Word>();
            foreach (String word in aRowString)
            {
                words.Add(new Word(word));
            }

            return words;
        }

        private void ToggleWord(List<Word> aWords, List<Word> bWords, Coordinates coordinates)
        {
            if (coordinates.GetY().Equals("A"))
            {
                aWords[coordinates.GetX()].ToggleSkin();
            }

            if (coordinates.GetY().Equals("B"))
            {
                bWords[coordinates.GetX()].ToggleSkin();
            }
        }
        
        private int VerifyCompatibilityAndGetHitNumber(List<Word> aRow, List<Word> bRow)
        {
            int hits = 0;
            foreach (Word aRowWord in aRow)
            {
                foreach (Word bRowWord in bRow)
                {
                    if (aRowWord.GetSkin().Length > 1 && bRowWord.GetSkin().Length > 1 &&
                        aRowWord.GetSkin().Equals(bRowWord.GetSkin()))
                    {
                        aRowWord.SetIsDiscovered(true);
                        bRowWord.SetIsDiscovered(true);
                        hits++;
                    }
                }
            }

            return hits;
        }
        
        private void CoverTable(List<Word> aRow, List<Word> bRow)
        {
            foreach (Word aRowWord in aRow)
            {
                aRowWord.ToggleSkinBasedOnDiscovered();
            }

            foreach (Word bRowWord in bRow)
            {
                bRowWord.ToggleSkinBasedOnDiscovered();
            }
        }
        
        private Coordinates AskForCoordinates()
        {
            string inputCoordinates = "";
            bool isInputNotCorrect = true;
            while (isInputNotCorrect)
            {
                Console.WriteLine("\nEnter coordinates (e.g. 'A3'):");
                inputCoordinates = Console.ReadLine();
                if (inputCoordinates.Length < 3)
                {
                    isInputNotCorrect = false;
                    break;
                }
                Console.WriteLine("You entered too long coordinates.");
            }
            
            string[] inputCoordinatesArray = CreateInputCoordinatesArray(inputCoordinates);

            bool yCoordsNotCorrect = true;
            while (yCoordsNotCorrect)
            {
                string yCoord = inputCoordinatesArray[0];
                if (yCoord.Equals("A") || yCoord.Equals("B"))
                {
                    yCoordsNotCorrect = false;
                }
                else
                {
                    Console.WriteLine("Given vertical coordinate (first one) is wrong. Try again.");
                    inputCoordinates = Console.ReadLine();
                    inputCoordinatesArray = CreateInputCoordinatesArray(inputCoordinates);
                }
            }

            bool xCoordsNotCorrect = true;
            while (xCoordsNotCorrect)
            {
                string wordsNumberRegex = "[1-" + WORDS_NUMBER + "]";
                Regex regex = new Regex(wordsNumberRegex);
                Match match = regex.Match(inputCoordinatesArray[1]);
                if (match.Success)
                {
                    xCoordsNotCorrect = false;
                }
                else
                {
                    Console.WriteLine("Given horizontal coordinate (second one) is wrong. Try again.");
                    Console.WriteLine("Enter coordinates:");
                    inputCoordinates = Console.ReadLine();
                    inputCoordinatesArray = CreateInputCoordinatesArray(inputCoordinates);
                }
            }

            var coordinates = new Coordinates();
            coordinates.SetX(int.Parse(inputCoordinatesArray[1]) - 1);
            coordinates.SetY(inputCoordinatesArray[0]);
            // Console.WriteLine("Your Coordinates:\n" + coordinates.toString()); // Helper - to see coordinates
            return coordinates;
        }

        private static string[] CreateInputCoordinatesArray(string inputCoordinates)
        {
            string firstCoord = inputCoordinates[0].ToString().ToUpper();
            string secondCoord = inputCoordinates[1].ToString();
            String[] inputCoordinatesArray = new string[2];
            inputCoordinatesArray[0] = firstCoord;
            inputCoordinatesArray[1] = secondCoord;
            return inputCoordinatesArray;
        }

        private void NewGameMenu(Player player)
        {
            Console.WriteLine("Would You like to play again, " + player.GetName() + "?");
            bool isRunningRestartMenu = true;
            while (isRunningRestartMenu)
            {
                DisplayManager.DisplayRestartMenu();
                var chooseOption = Console.ReadLine();
                DisplayManager.ClearScreen();
                switch (chooseOption)
                {
                    case "1":
                        isRunningRestartMenu = false;
                        break;
                    case "2":
                        DisplayManager.DisplayHighScoreTable(highScores);
                        break;
                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Wrong input!");
                        break;
                }
            }
        }
        
        private long GetTimeSeconds(Stopwatch stopwatch)
        {
            return stopwatch.Elapsed.Seconds;
        }
        
        private bool IsWinner(Player player)
        {
            if (player.GetHp() > 0)
            {
                return true;
            }

            return false;
        }

        private int CalculatePlayerScore(long gameTime, Player player, int chancesAfterWon)
        {
            int result = 100;
            result = (int) (result - gameTime + chancesAfterWon);
            return result;
        }
        
        private void AddScoreToHighScores(int playerScore)
        {
            HighScore highScore = new HighScore();
            highScore.SetPlayerName(player.GetName());
            highScore.SetScore(playerScore);
            highScore.SetGameLocalDateTime(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            highScores.Add(highScore);
        }

    }
    
    public class Player
    {
        private int hp;
        private String name;
        private int hits;

        public int GetHits()
        {
            return hits;
        }

        public void SetHits(int hits)
        {
            this.hits = hits;
        }

        public Player()
        {
            hits = 0;
        }

        public int GetHp()
        {
            return hp;
        }

        public void DecreaseHp(int damage)
        {
            hp -= damage;
        }

        public String GetName()
        {
            return name;
        }

        public void SetName(String name)
        {
            this.name = name;
        }

        public void SetHp(int hp)
        {
            this.hp = hp;
        }
    }
    
    public class Coordinates
    {
        private int x;
        private String y;

        public int GetX()
        {
            return x;
        }

        public void SetX(int x)
        {
            this.x = x;
        }

        public String GetY()
        {
            return y;
        }

        public void SetY(String y)
        {
            this.y = y;
        }

        public String toString()
        {
            return "Coordinates{" + "x=" + x + ", y=" + y + '}';
        }
    }
    
    public class Word
    {
        private String name;
        private bool isFound;
        private bool isDiscovered;
        private String skin;

        public Word(String name)
        {
            this.name = name;
            isFound = false;
            isDiscovered = false;
            skin = "X";
        }

        public String GetSkin()
        {
            return skin;
        }

        public void SetSkin(String skin)
        {
            this.skin = skin;
        }

        public void ToggleSkin()
        {
            if (isDiscovered)
            {
                skin = name;
                return;
            }

            if (skin.Equals("X"))
            {
                skin = name;
            }
            else
            {
                skin = "X";
            }
        }

        public void ToggleSkinBasedOnDiscovered()
        {
            if (isDiscovered)
            {
                skin = name;
            }
            else
            {
                skin = "X";
            }
        }

        public String GetName()
        {
            return name;
        }

        public void SetName(String name)
        {
            this.name = name;
        }

        public bool GetIsDiscovered()
        {
            return isDiscovered;
        }

        public void SetIsDiscovered(bool discovered)
        {
            isDiscovered = discovered;
        }
    }
    
    public class HighScore
    {
        private String playerName;
        private int score;
        private String gameLocalDateTime;

        public String GetGameLocalDateTime()
        {
            return gameLocalDateTime;
        }

        public void SetGameLocalDateTime(String gameLocalDateTime)
        {
            this.gameLocalDateTime = gameLocalDateTime;
        }

        public String GetPlayerName()
        {
            return playerName;
        }

        public void SetPlayerName(String playerName)
        {
            this.playerName = playerName;
        }

        public int GetScore()
        {
            return score;
        }

        public void SetScore(int score)
        {
            this.score = score;
        }
    }
    
    public class FileHandler
    {
        public static List<string> GetWordsList(string fileName)
        {
            List<string> allLinesText = File.ReadAllLines(fileName).ToList();
            return allLinesText;
        }
    }

    public class DisplayManager
    {
        public static void DisplayMainMenu()
        {
            Console.WriteLine("MEMORY GAME");
            Console.WriteLine("\n[1]-start game");
            Console.WriteLine("[2]-credits");
            Console.WriteLine("[3]-exit game");
            Console.WriteLine("\nChoose one option: ");
        }

        public static void PressAnyKeyToContinue()
        {
            Console.WriteLine("\n\n-----------------------------");
            Console.WriteLine("| Press any key to continue |");
            Console.WriteLine("-----------------------------");
            Console.ReadLine();
        }

        public static void DisplayModes()
        {
            Console.WriteLine("MEMORY GAME MODES");
            Console.WriteLine("\n[1]-easy");
            Console.WriteLine("[2]-hard");
            Console.WriteLine("[3]-exit game");
            Console.WriteLine("\nChoose one option: ");
        }

        public static void DisplayCredits()
        {
            Console.WriteLine("Produced by Magdalena Huget");
        }
        
        public static void DisplayTable(List<Word> aWords, List<Word> bWords)
        {
            var firstLine = new StringBuilder();
            firstLine.Append("  ");
            for (int i = 1; i <= aWords.Count; i++)
            {
                firstLine.Append(i).Append(" ");
            }

            var aRow = CreateRow(aWords, "A");
            var bRow = CreateRow(bWords, "B");
            Console.WriteLine(string.Join(", ", firstLine) + "\n" + aRow + "\n" + bRow);
        }
        
        public static String CreateRow(List<Word> words, string lineTitle)
        {
            StringBuilder row = new StringBuilder();
            row.Append(lineTitle).Append(" ");
            foreach (Word word in words)
            {
                row.Append(word.GetSkin()).Append(" ");
            }

            return row.ToString();
        }
        
        public static void DisplayDiscoverdTable(List<Word> aWords, List<Word> bWords)
        {
            StringBuilder firstLine = new StringBuilder();
            firstLine.Append("  ");
            for (int i = 1; i <= aWords.Count; i++)
            {
                firstLine.Append(i).Append(" ");
            }

            string aRow = CreateRowDiscovered(aWords, "A");
            string bRow = CreateRowDiscovered(bWords, "B");
            Console.WriteLine(string.Join(", ", firstLine) + "\n" + aRow + "\n" + bRow);
        }
        
        public static string CreateRowDiscovered(List<Word> words, string lineTitle)
        {
            StringBuilder row = new StringBuilder();
            row.Append(lineTitle).Append(" ");
            foreach (Word word in words)
            {
                row.Append(word.GetIsDiscovered()).Append(" ");
            }

            return row.ToString();
        }
        
        public static void ClearScreen()
        {
            Console.WriteLine("[H[2J");
        }
    
        public static void DisplayHeader(Player player, int ROUND)
        {
            ClearScreen();
            Console.WriteLine("=============================== ROUND " + ROUND);
            Console.WriteLine("Player = " + player.GetName());
            Console.WriteLine("Hits = " + player.GetHits());
            Console.WriteLine("Hp = " + player.GetHp());
            Console.WriteLine();
        }
        
        public static void DisplayRestartMenu()
        {
            Console.WriteLine("RESTART MENU");
            Console.WriteLine("\n[1]-Yes, I want to play again.");
            Console.WriteLine("[2]-Display high score table");
            Console.WriteLine("[3]-exit game");
            Console.WriteLine("\nChoose one option: ");
        }
        
        public static void DisplayHighScoreTable(List<HighScore> highScores)
        {
            Console.WriteLine("---------- High score table ----------");
            Console.WriteLine("#, name, score, dateTime");
            foreach (HighScore highScore in highScores)
            {
                Console.WriteLine("1. " + highScore.GetPlayerName() + " " + highScore.GetScore() + " " +
                                  string.Join(", ", highScore.GetGameLocalDateTime()));
            }

            if (highScores.Count == 0)
            {
                Console.WriteLine("Empty table.");
            }
        }
    }
}