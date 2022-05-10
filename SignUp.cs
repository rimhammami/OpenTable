using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace OpenTableApp
{
    public partial class SignUp : Form
    {
        public SignUp()
        {
            InitializeComponent();
        }
        ADO d = new ADO();

        // Declaration de la method search which returns the number of times an email exists in the table
        public int search()
        {
            int cpt;
            d.cmd.CommandText = " select count(email) from [User] where email ='" + txtemail.Text + "'";
            d.cmd.Connection = d.con;
            cpt = (int)d.cmd.ExecuteScalar();
            return cpt;
        }

        // declaration of the method Register
        public bool REGISTER()
        {
            string type="standard";

            if (search() == 0)
            {

                d.cmd.CommandText = " insert into [User] values ('" + txtfirstname.Text + "'" + ",'" + txtlastname.Text + "','" + txtcontact.Text + "','" + txtemail.Text + "','" + txtpwd.Text + "','" + type + "',null)";
                d.cmd.Connection = d.con;
                d.cmd.ExecuteNonQuery();
                return true;
            }
            return false;

        }
        //method to control the email.
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        //method to control the phone number.
        bool IsValidContact(string contact)
        {
            const string motif = @"^\(?([0-9]{2})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";

            if (contact != null) return Regex.IsMatch(contact, motif);

            else return false;
        }
        //method to empty the fields
        public void EMPTY()
        {
            txtfirstname.Text = "";
            txtlastname.Text = "";
            txtcontact.Text = "";
            txtemail.Text = "";
            txtpwd.Text = "";
            txtpwdconfirm.Text = "";
        }
        private void SignUp_Load(object sender, EventArgs e)
        {
            d.CONNECT();
        }

        private void btsignup_Click(object sender, EventArgs e)
        {
            if (txtfirstname.Text == "" || txtlastname.Text == "" || txtcontact.Text == "" || txtemail.Text == "" || txtpwd.Text == "" || txtpwdconfirm.Text == "")
            {
                MessageBox.Show(" Please fill out all fields ", "Filling Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsValidContact(txtcontact.Text))
            {
                MessageBox.Show(" Please give a valid contact number under the following format: xx-xxx-xxxx ", "Contact Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsValidEmail(txtemail.Text))
            {
                MessageBox.Show(" Please give a valid email ", "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtpwd.Text != txtpwdconfirm.Text)
            {
                MessageBox.Show(" Passwords don't match ", "Password Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (REGISTER() == true)
            {
                MessageBox.Show("Thank you for registering", "Succesful registeration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EMPTY();
                
            }
            else
            {
                MessageBox.Show(" Please register using a different email", "Registeration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btlogin_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login l = new Login();
            l.Show();
        }
    }
}
