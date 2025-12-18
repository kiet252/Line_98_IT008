using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Line_98.Components
{
    public class Player
    {
        public string Name { get; set;}
        public int HighestScore { get; set; }
        public int TimePlayed { get; set; }
        public Player(string name, int highestScore, int timePlayed)
        {
            Name = name;
            HighestScore = highestScore;
            TimePlayed = timePlayed;
        }
    }
}
