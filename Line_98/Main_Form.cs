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
        }

    }
}
