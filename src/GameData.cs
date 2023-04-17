using System;

namespace Meridian2 {
    public class GameData {
         
         //health
         private const int startMaxHealth = 3;
         public int maxHealth;
         public int Health { private set; get;}
         public int score = 0;

         public bool gameOver {private set; get;}

         private RopeGame _game;

         public GameData(RopeGame game) {
            resetHealth();
            gameOver = false;
            _game = game;
         }

         public void resetHealth() {
            maxHealth = startMaxHealth;
            Health = maxHealth;
         }

         public void RemoveHealth(int amount) {
            Health -= 1;
            if (Health <= 0) {
               gameOver = true;
            }
         }

         public void AddHealth(int amount) {
            Health = Math.Max(maxHealth, Health+amount);
         }
    }
}