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
    
    public partial class Main_Form : Form
    {

        MainGamePanel mainGamePanel;
        public Main_Form()
        {
            InitializeComponent();
        }

        private void Main_Form_Load(object sender, EventArgs e)
        {
            mainGamePanel = new MainGamePanel(this);
            this.Controls.Add(mainGamePanel);

            //Hiện luật chơi ngay khi mở trò chơi
            fHowToPlay help = new fHowToPlay();
            help.Show();
            help.TopMost = true;
        }

        /// <summary>
        /// MenuStrip để save game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            saveGame.Filter = "Line98 file (*.ln98)|*.ln98";
            saveGame.FileName = "Game_Line98";

            DialogResult r = this.saveGame.ShowDialog();

            if(r == DialogResult.OK) {

                SaveAndLoad.Save(saveGame.FileName, mainGamePanel);
            }
        }

        /// <summary>
        /// MenuStrip để load game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            loadGame.Filter = "Line98 file (*.ln98)|*.ln98";

            DialogResult r = this.loadGame.ShowDialog();

            if (r == DialogResult.OK) { 
                SaveAndLoad.Load(loadGame.FileName, mainGamePanel);
            }
        }

        /// <summary>
        /// MenuStrip để hoàn lại bước đi trước đó
        /// Chỉ hoàn lại được 1 lần 
        /// Mỗi lần Undo tăng thêm 30 giây
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
            GameAlgorithm.Undo(ref mainGamePanel);
            mainGamePanel.GameTime += 20;

            mainGamePanel.Invalidate();
            mainGamePanel.Refresh();
            mainGamePanel.RepaintCurrentScore();
            mainGamePanel.RepaintGameTime();
        }

        /// <summary>
        /// MenuStrip hiển thị Form về Hướng dẫn chơi game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void howToPlayToolStripMenuItem_Click(object sender, EventArgs e) {
            fHowToPlay help = new fHowToPlay();
            help.Show();
        }

        /// <summary>
        /// MenuStrip về Thông tin đồ án
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            fAbout about = new fAbout();
            about.Show();
        }

        private void leaderboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fLeaderboard leaderboard = new fLeaderboard();
            leaderboard.Show();
        }
    }
}
