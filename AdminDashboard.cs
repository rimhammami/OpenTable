using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTableApp
{
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormTable f = new FormTable();
            f.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormUser f = new FormUser();
            f.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormReservation f = new FormReservation();
            f.Show();
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            //idLabel.Text = LoggedInUser.id.ToString();
            //emailLabel.Text= LoggedInUser.email.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormDicount f = new FormDicount();
            f.Show();
        }
    }
}
