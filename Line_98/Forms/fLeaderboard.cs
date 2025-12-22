using Line_98.Components;
using Line_98.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Line_98
{
    public partial class fLeaderboard : Form
    {
        private int _currentScore;
        private int _currentTimePlayed;

        public fLeaderboard()
        {
            InitializeComponent();
            OnlyLoadAsync();
        }
        public fLeaderboard(int CurrentScore, int CurrentTimePlayed)
        {
            InitializeComponent();

            _currentScore = CurrentScore;
            _currentTimePlayed = CurrentTimePlayed;

            InitializeAndLoadAsync();
        }

        private async void InitializeAndLoadAsync()
        {
            fPlayerName GetPlayerName = new fPlayerName();
            GetPlayerName.ShowDialog();

            string playerName = GetPlayerName.PlayerName;
            Player player = new Player(playerName, _currentScore, _currentTimePlayed);

            PlayerInfo playerinfo = new PlayerInfo(player);
            if (flowLayoutPanelLeaderBoard.InvokeRequired)
            {
                flowLayoutPanelLeaderBoard.Invoke(new Action(() =>
                flowLayoutPanelLeaderBoard.Controls.Add(playerinfo)));
            }
            else
            {
                flowLayoutPanelLeaderBoard.Controls.Add(playerinfo);
            }

            await SupabaseManager.SaveOrUpdatePlayer(player);

            await SupabaseManager.LoadLeaderboard(flowLayoutPanelLeaderBoard);
        }

        private async void OnlyLoadAsync()
        {
            await SupabaseManager.LoadLeaderboard(flowLayoutPanelLeaderBoard);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            OnlyLoadAsync();
        }
    }
}
