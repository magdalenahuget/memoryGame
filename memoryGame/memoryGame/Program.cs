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
        public GameController()
        {
        }

        public void run()
        {
            
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
    
}