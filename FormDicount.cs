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
    public partial class FormDicount : Form
    {
        public FormDicount()
        {
            InitializeComponent();
        }
        ADO d = new ADO();
        public bool reservationDateExist()
        {

            d.cmd.CommandText = "SELECT reservationDate from [Reservation] where discountID='" + textBoxID.Text + "'";
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
            d.cmd.CommandText = "SELECT reservationDate from [Reservation] where discountID='" + textBoxID.Text + "'";
            d.cmd.Connection = d.con;
            date = d.cmd.ExecuteScalar().ToString();
            return date;


        }
        //method to find the discount in the database
        public int search()
        {
            int cpt;
            d.cmd.CommandText = " select count(discountID) from [Discount] where discountID ='" + textBoxID.Text + "'";
            d.cmd.Connection = d.con;
            cpt = (int)d.cmd.ExecuteScalar();
            return cpt;
        }
        //add method
        public bool ADD()
        {
            string val=numericUpDown1.Value.ToString();
            val += "%";
            if (search() == 0)
            {
                d.cmd.CommandText = " insert into [Discount] values ('" + textBoxName.Text + "'" + ",'" + val + "')";
            }
            d.cmd.Connection = d.con;
            d.cmd.ExecuteNonQuery();
            return true;
        }
        //delete method
        public bool DELETE()
        {
            if (search() != 0)
            {
                int id = Convert.ToInt32(textBoxID.Text);
                d.cmd.CommandText = " delete from [Discount] where discountID ='" + id + "'";
                d.cmd.Connection = d.con;
                d.cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }


        //edit method
        public bool EDIT()
        {
            if (search() != 0)
            {
                int id = Convert.ToInt32(textBoxID.Text);
                string val = numericUpDown1.Value.ToString() + "%";
                d.cmd.CommandText = "update [Discount] set name= '" + textBoxName.Text + "' ,value = '" + val + "'where discountID = '" + id + "'";
                d.cmd.Connection = d.con;
                d.cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }

        //fill the datagrid with discounts from the database
        public void FillGrid()
        {
            if (d.dt.Rows != null)
            {
                d.dt.Clear();
            }
            d.cmd.CommandText = " select * from [Discount]";
            d.cmd.Connection = d.con;
            d.dr = d.cmd.ExecuteReader();
            d.dt.Load(d.dr);
            dataGridView1.DataSource = d.dt;
            d.dr.Close();
        }
        public void EMPTY()
        {
            textBoxID.Text = "";
            textBoxName.Text = "";
            numericUpDown1.Value = 0;
        }
        private void FormDicount_Load(object sender, EventArgs e)
        {
            d.CONNECT();
            FillGrid();
        }

        //method that fills the fields with content from the datagrid once you click on a column in the row
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            textBoxID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textBoxName.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            string val = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            val = val.Replace("%", "");
            numericUpDown1.Value = Convert.ToInt32(val);
        }

        private void buttonAddDiscount_Click(object sender, EventArgs e)
        {

            if (ADD() == true)
            {
                MessageBox.Show("Discount added successfully ", "Add Discount", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FillGrid();

            }
            else
            {
                MessageBox.Show("Couldn't add the discount", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonEditDiscount_Click(object sender, EventArgs e)
        {

            if (reservationDateExist() == true)
            {
                try
                {

                    if (DateTime.Parse(getReservationDate()) > DateTime.Now)
                    {
                        MessageBox.Show("You can't edit a discount if its in an upcoming reservation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if(reservationDateExist() == false)
            {
                if (EDIT() == true)
                {
                    MessageBox.Show("Sucessfully edited ", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FillGrid();

                }
            }
            
            
            else
                {
                    MessageBox.Show("Couldn't not make modification", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
        }

        private void buttonDeleteDiscount_Click(object sender, EventArgs e)
        {
            if (textBoxID.Text == "")
            {
                MessageBox.Show("Please specify the discount you'd like to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (reservationDateExist()==true)
            {
                try
                {
                    if (DateTime.Parse(getReservationDate()) > DateTime.Now)
                    {

                        MessageBox.Show("You can't delete a discount if its in an upcoming reservation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        if (DELETE() == true)
                        {
                            MessageBox.Show("Successfully deleted the discount ", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FillGrid();
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (reservationDateExist() == false)
            {

                if (DELETE() == true)
                {
                    MessageBox.Show("Successfully deleted the discount ", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FillGrid();
                }
            }


            else
                {
                    MessageBox.Show("Couldn't delete the discount", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
       
        }

        private void buttonEmptyField_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to empty the fields?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                EMPTY();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormUser f = new FormUser();
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
            FormTable f = new FormTable();
            f.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login f = new Login();
            f.Show();
        }
    }
}
