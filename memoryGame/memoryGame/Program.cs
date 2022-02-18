using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace memoryGame
{
    using System;


    public class Program
    {
        public static void Main(String[] args)
        {
            GameController gameController = new GameController();
            gameController.run();
        }
    }

    public class GameController
    {
        private String filename = "src/main/resources/Words.txt";
        private List<string> allWords = new List<string>();
        private List<string> modeWords = new List<string>();
        private Player player;
        private int WORDS_NUMBER;
        private int GUESS_CHANCES;
        private bool isRunning = true;
        // TODO: highscore
        
        public GameController()
        {
            player = new Player();
        }

        public void run()
        {
            Console.WriteLine("Welcome to my memory game!");
            DisplayManager.ClearScreen();
            string playerName = getPlayerName();
            Console.WriteLine("Welcome " + playerName);
            DisplayManager.PressAnyKeyToContinue();
            // TODO: menu
            playMenu(playerName);
        }
        
        private void playMenu(String name)
        {
            while (isRunning)
            {
                DisplayManager.displayMainMenu();
                var chooseOption = Console.ReadLine();
                DisplayManager.ClearScreen();
                switch (chooseOption)
                {
                    case "1":
                        playGame(name);
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
        
        private void playGame(String name)
        {
            while (isRunning)
            {
                DisplayManager.DisplayModes();
                var chooseOption = Console.ReadLine();
                switch (chooseOption)
                {
                    case "1":
                        //  Easy
                        playMode(name, 4, 10, "easy");
                        break;
                    case "2":
                        //  Hard
                        playMode(name, 8, 15, "hard");
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

        private void playMode(String name, int wordsNumberToGuess, int guessChances, String mode)
        {
            Console.WriteLine("Welcome to mode " + mode);
            DisplayManager.PressAnyKeyToContinue();
            int ROUND = 0;
            WORDS_NUMBER = wordsNumberToGuess;
            GUESS_CHANCES = guessChances;
            // GUESS_CHANCES = 1;
            int GUESS_CHANCES_LEFT = GUESS_CHANCES;
            player.setName(name);
            player.setHp(GUESS_CHANCES_LEFT);
            allWords = FileHandler.getWordsList("../../Words.txt");
            modeWords = getModeWords(allWords);
            List<String> aRowString = new List<String>(modeWords);
            List<String> bRowString = new List<String>(modeWords);
        }
        
        private List<String> getModeWords(List<string> allWords)
        {
            List<int> indexes = getIndexes(allWords);
            var modeWords = new List<String>();
            foreach (int index in indexes)
            {
                modeWords.Add(allWords[index]);
            }

            return modeWords;
        }
        
        public List<int> getIndexes(List<string> words)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < words.Count; i++)
            {
                list.Add(i);
            }

            // shuffle(list); // TODO:
            List<int> indexes = new List<int>();
            for (int i = 0; i < WORDS_NUMBER; i++)
            {
                indexes.Add(list[i]);
                Console.WriteLine(list[i]);
            }

            return indexes;
        }

        public String getPlayerName()
        {
            Console.WriteLine("What is the name of the Player?");
            return Console.ReadLine();
        }
        
        private bool isWinner(Player player)
        {
            if (player.getHp() > 0)
            {
                return true;
            }

            return false;
        }

    }
    
    public class Player
    {
        private int hp;
        private String name;
        private int hits;

        public int getHits()
        {
            return hits;
        }

        public void setHits(int hits)
        {
            this.hits = hits;
        }

        public Player()
        {
            this.hits = 0;
        }

        public int getHp()
        {
            return hp;
        }

        public void decreaseHp(int damage)
        {
            hp -= damage;
        }

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public void setHp(int hp)
        {
            this.hp = hp;
        }
    }
    
    public class Coordinates
    {
        private int x;
        private String y;

        public int getX()
        {
            return x;
        }

        public void setX(int x)
        {
            this.x = x;
        }

        public String getY()
        {
            return y;
        }

        public void setY(String y)
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

        public String getSkin()
        {
            return skin;
        }

        public void setSkin(String skin)
        {
            this.skin = skin;
        }

        public void toggleSkin()
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

        public void toggleSkinBasedOnDiscovered()
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

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public bool getIsDiscovered()
        {
            return isDiscovered;
        }

        public void setIsDiscovered(bool discovered)
        {
            isDiscovered = discovered;
        }
    }
    
    public class FileHandler
    {
        public static List<String> getWordsList(String fileName)
        {
            List<string> allLinesText = File.ReadAllLines(fileName).ToList();
            return allLinesText;
        }
    }

    public class DisplayManager
    {
        public static void displayMainMenu()
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
        
        public static void ClearScreen()
        {
            Console.WriteLine("[H[2J");
        }
    
        public static void DisplayHeader(Player player, int ROUND)
        {
            // TODO: add player name
            ClearScreen();
            Console.WriteLine("=============================== ROUND " + ROUND);
            Console.WriteLine("Hits = " + player.getHits());
            Console.WriteLine("Hp = " + player.getHp());
            Console.WriteLine();
        }
    }
}