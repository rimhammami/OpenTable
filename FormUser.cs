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
    public partial class FormUser : Form
    {
        public FormUser()
        {
            InitializeComponent();
            comboBoxRole.Items.Add("admin");
            comboBoxRole.Items.Add("standard");
        }
        ADO d = new ADO();
        public bool reservationDateExist()
        {

            d.cmd.CommandText = "SELECT reservationDate from [Reservation] where clientID='" + textBoxID.Text + "'";
            d.cmd.Connection = d.con;
            d.dr = d.cmd.ExecuteReader();
            if (d.dr.HasRows)
            {
                d.dr.Close();
                return true;
            }
            else
            {
                d.dr.Close();
                return false;
            }


        }
        public string getReservationDate()
        {
            string date;
            d.cmd.CommandText = "SELECT reservationDate from [Reservation] where clientID='" + textBoxID.Text + "'";
            d.cmd.Connection = d.con;
            date = d.cmd.ExecuteScalar().ToString();
            return date;
        }


        //method to find user
        public int search()
        {
            int cpt;
            d.cmd.CommandText = " select count(UserID) from [User] where email ='" + txtemail.Text + "'";
            d.cmd.Connection = d.con;
            cpt = (int)d.cmd.ExecuteScalar();
            return cpt;
        }

        //method to add user
        public bool ADD()
        {
            if (search() == 0)
            {
                String role = comboBoxRole.SelectedItem.ToString();
                d.cmd.CommandText = " insert into [User] values ('" + txtfirstname.Text + "'" + ",'" + txtlastname.Text + "','" + txtcontact.Text + "','" + txtemail.Text + "','" + txtpwd.Text + "','" + role + "',null)";
                d.cmd.Connection = d.con;
                d.cmd.ExecuteNonQuery();
                return true;
            }
            return false;

        }
        //method to delete user
        public bool DELETE()
        {
            if (search() != 0)
            {
                int id = Convert.ToInt32(textBoxID.Text);
                d.cmd.CommandText = " delete from [User] where UserID ='" + id + "'";
                d.cmd.Connection = d.con;
                d.cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }
        //method to edit user
        public bool EDIT()
        {

            if (search() != 0)
            {
                int id = Convert.ToInt32(textBoxID.Text);
                String role = comboBoxRole.SelectedItem.ToString();
                d.cmd.CommandText = "update [User] set firstname= '" + txtfirstname.Text + "' ,lastname = '" + txtlastname.Text + "',contact = '" + txtcontact.Text + "',email = '" + txtemail.Text + "',password = '" + txtpwd.Text + "',type = '" + role + "'where UserID = '" + id + "'";
                d.cmd.Connection = d.con;
                d.cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }

        //method to fill the datagrid with rows from the database.
        public void FillGrid()
        {
            if (d.dt.Rows != null)
            {
                d.dt.Clear();
            }
            d.cmd.CommandText = " select * from [User]";
            d.cmd.Connection = d.con;
            d.dr = d.cmd.ExecuteReader();
            d.dt.Load(d.dr);
            dataGridView1.DataSource = d.dt;
            d.dr.Close();
        }
        public void EMPTY()
        {
            textBoxID.Text = "";
            txtfirstname.Text = "";
            txtlastname.Text = "";
            txtcontact.Text = "";
            txtemail.Text = "";
            txtpwd.Text = "";

        }
        private void FormUser_Load(object sender, EventArgs e)
        {
            d.CONNECT();
            FillGrid();
        }

        private void buttonAddTable_Click(object sender, EventArgs e)
        {
            if (txtfirstname.Text == "" || txtlastname.Text == "" || txtcontact.Text == "" || txtemail.Text == "" || txtpwd.Text == "")
            {
                MessageBox.Show(" Please fill out all the fields ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (ADD() == true)
            {
                MessageBox.Show("User added successfuly", "Add User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FillGrid();

            }
            else
            {
                {
                    MessageBox.Show("Couldn't add the User. Try a different email", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void buttonEditTable_Click(object sender, EventArgs e)
        {
            if (txtfirstname.Text == "" || txtlastname.Text == "" || txtcontact.Text == "" || txtemail.Text == "" || txtpwd.Text == "")
            {
                MessageBox.Show(" Please fill out all the fields ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (EDIT() == true)
            {
                MessageBox.Show("User modified successfuly", "Add User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FillGrid();

            }
            else
            {
                {
                    MessageBox.Show("Couldn't modify the User.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void buttonDeleteTable_Click(object sender, EventArgs e)
        {
            
            if (textBoxID.Text == "")
            {
                MessageBox.Show("Please specify the user you wand to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (reservationDateExist()==true)

            {
                try
                {
                    if (DateTime.Parse(getReservationDate()) > DateTime.Now)
                    {

                        MessageBox.Show("You can't delete a user who has an upcoming reservation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        if (DELETE() == true)
                        {
                            MessageBox.Show("Successfully deleted the user ", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FillGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            if (reservationDateExist() == false)
            {

                if (DELETE() == true)
                {
                    MessageBox.Show("User Delete Successfully ", "Delete User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FillGrid();

                }
            }

                
            
            else
            {
                MessageBox.Show("Couldn't Delete User", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //method to fill the fields with columns from the datagrid rows when we click on any column in a row
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            textBoxID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtfirstname.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            txtlastname.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            txtcontact.Text= dataGridView1.CurrentRow.Cells[3].Value.ToString();
            txtemail.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            txtpwd.Text= dataGridView1.CurrentRow.Cells[5].Value.ToString();
            comboBoxRole.SelectedItem = dataGridView1.CurrentRow.Cells[6].Value.ToString();

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
            FormReservation f = new FormReservation();
            f.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormDicount f = new FormDicount();
            f.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login f = new Login();
            f.Show();
        }

        private void buttonEmptyField_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to empty the fields?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                EMPTY();
            }
        }
    }
}