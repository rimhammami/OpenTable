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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        ADO d = new ADO();
        //method that find the uder that is trying to log in.
        public int getUserId()
        {
            int idUser;
            d.cmd.CommandText = " select UserID from [User] where email ='" + txtemail.Text + "'";
            d.cmd.Connection = d.con;
            idUser = (int)d.cmd.ExecuteScalar();
            return idUser;
        }
        private void Login_Load(object sender, EventArgs e)
        {
            d.CONNECT();
        }

        private void btlogin_Click(object sender, EventArgs e)
        {
            bool tr = false;
            d.cmd.CommandText = "select UserID,email,password,type from [User]";
            d.cmd.Connection = d.con;

            d.dr = d.cmd.ExecuteReader();
            while (d.dr.Read())
            {
                if (txtemail.Text.Equals(d.dr[1].ToString()) && txtpwd.Text.Equals(d.dr[2].ToString()))
                {
                    tr = true;
                    break;
                }
            }
            if (tr == true)
            {
                //we store the email in a global variable so that we can use it in other forms.
                LoggedInUser.email = txtemail.Text;

                //depending on the type of the user, we open different forms.
                if ("standard".Equals(d.dr[3].ToString()))
                {
                    this.Hide();
                    FormReservation f = new FormReservation();
                    f.Show();
                }
                if ("admin".Equals(d.dr[3].ToString()))
                {
                    this.Hide();
                    AdminDashboard f = new AdminDashboard();
                    f.Show();
                }
            }
            else
            {
                MessageBox.Show("email and or password is incorrect");
            }

            d.dr.Close();


        }

        private void btsignup_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignUp f = new SignUp();
            f.Show();
        }

        private void txtemail_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtpwd_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
