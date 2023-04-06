
namespace Meridian2 {
    public class GameData {
         
         //health
         private const int startMaxHealth = 3;
         public int maxHealth;
         public int health;
         

         public GameData() {
            resetHealth();
         }

         public void resetHealth() {
            maxHealth = startMaxHealth;
            health = maxHealth;
         }
    }
}