using Line_98.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Line_98.Forms
{
    public partial class fPlayerName : Form
    {
        public string PlayerName { get; private set; } = "Unknown";
        public fPlayerName()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            BadWordFilter filter = new BadWordFilter();
            string NewName = textBoxName.Text;

            if (NewName.Length == 0 || filter.ContainsBadWord(NewName.ToLower()) || filter.IsBadWord(NewName.ToLower()))
            {
                MessageBox.Show("Tên người chơi không hợp lệ! Xin hãy nhập lại.", "Không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (NewName.Length > 15)
            {
                MessageBox.Show("Tên người chơi quá dài!, Hãy nhập tên người chơi trong khoảng 15 kí tự đổ lại.", "Quá dài", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PlayerName = NewName;
            this.Close();
        }
    }
}
