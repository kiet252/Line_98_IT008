using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Line_98.Components
{
    public partial class PlayerInfo : UserControl
    {
        private int rank;
        private readonly Color NumberOneColor = Color.Gold;
        private readonly Color SecondPlaceColor = Color.Silver;
        private readonly Color ThirdPlaceColor = Color.FromArgb(205, 127, 50); //Bronze
        private readonly Color NormalRankColor = Color.LightSteelBlue;
        public PlayerInfo(Player currentPlayer, int rank = 0)
        {
            InitializeComponent();
            labelPlayerName.Text = currentPlayer.Name;
            labelTimePlayed.Text = currentPlayer.TimePlayed.ToString();
            labelScore.Text = currentPlayer.HighestScore.ToString();
            this.rank = rank;

            RankToBackColor();
        }

        private void RankToBackColor()
        {
            Color ColorToSet = NormalRankColor;
            switch (rank)
            {
                case 1:
                    ColorToSet = NumberOneColor;
                    break;
                case 2:
                    ColorToSet = SecondPlaceColor;
                    break;
                case 3:
                    ColorToSet = ThirdPlaceColor;
                    break;
                default:
                    ColorToSet = NormalRankColor;
                    break;
            }

            this.BackColor = Color.FromArgb(200, ColorToSet);
            panelPlayerInfo.BackColor = ColorToSet;
        }
    }
}
