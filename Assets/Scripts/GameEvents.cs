using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class GameEvents
{
    public delegate void OnEnemyKilled(Enemy enemy);
    public static OnEnemyKilled EnemyKilled;


}

