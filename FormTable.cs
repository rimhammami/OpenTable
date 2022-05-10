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
    public partial class FormTable : Form
    {
        public FormTable()
        {
            InitializeComponent();
        }
        ADO d = new ADO();

        //method to check if the table is reserved or not: this is so that we don't modify tables that are reserved.
        public bool Available()
        {
            string available;
            d.cmd.CommandText = " SELECT available from [Table] where tableID='" + textBoxID.Text + "'";
            d.cmd.Connection = d.con;
            available = d.cmd.ExecuteScalar().ToString();
            if (available == "yes")
            {
                return true;
            }
            else {
                return false;
            }

        }
        public int TABLEid()
        {
            int id;
            d.cmd.CommandText = "select tableID from [Table] where tableID='" + textBoxID.Text + "'";
            d.cmd.Connection = d.con;
            id = (int)d.cmd.ExecuteScalar();
            return id;
        }
        public void LiberateTable()
        {
            int id = TABLEid();
            d.cmd.CommandText = "update [Table] SET available='yes' where tableID= '" + id + "'";
            d.cmd.Connection = d.con;
            d.cmd.ExecuteNonQuery();

        }
        public DateTime getArrivalTime()
        {
            string time;
            d.cmd.CommandText = "SELECT ArrivalHour from [Table] where tableID=='" + textBoxID.Text + "'";
            d.cmd.Connection = d.con;
            time = d.cmd.ExecuteScalar().ToString();
            return DateTime.Parse(time);
        }
        public bool reservationDateExist()
        {

            d.cmd.CommandText = "SELECT reservationDate from [Reservation] where tableID='" + textBoxID.Text + "'";
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
            d.cmd.CommandText = "SELECT reservationDate from [Reservation] where tableID='" + textBoxID.Text + "'";
            d.cmd.Connection = d.con;
            date = d.cmd.ExecuteScalar().ToString();
            return date;


        }


        //method to find a table.
        public int search()
        {
            int cpt;
            d.cmd.CommandText = " select count(tableID) from [Table] where tableID ='" + textBoxID.Text + "'";
            d.cmd.Connection = d.con;
            cpt = (int)d.cmd.ExecuteScalar();
            return cpt;
        }

        //add method
        public bool ADD()
        {
            String available = "yes";
            if (radioButtonYes.Checked)
            {
                available = "yes";
            }
            else if (radioButtonNo.Checked)
            {
                available = "no";
            }
            if (search() == 0)
            {
                d.cmd.CommandText = " insert into [Table] values ('" + Convert.ToInt32(numericUpDownSeating.Value) + "'" + ",'" + available + "')";
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
                d.cmd.CommandText = " delete from [Table] where tableID ='" + id + "'";
                d.cmd.Connection = d.con;
                d.cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }

        //edit method
        public bool EDIT()
        {
            String available = "no";
            if (radioButtonYes.Checked)
            {
                available = "yes";
            }
            else if (radioButtonNo.Checked)
            {
                available = "no";
            }
            if (search() != 0)
            {
                int id = Convert.ToInt32(textBoxID.Text);
                d.cmd.CommandText = "update [Table] set maxCapacity= '" + Convert.ToInt32(numericUpDownSeating.Value) + "' ,available = '" + available + "'where tableID = '" + id + "'";
                d.cmd.Connection = d.con;
                d.cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }

        //method to fill the datagrid with content from the database
        public void FillGrid()
        {
            if (d.dt.Rows != null)
            {
                d.dt.Clear();
            }
            d.cmd.CommandText = " select * from [Table]";
            d.cmd.Connection = d.con;
            d.dr = d.cmd.ExecuteReader();
            d.dt.Load(d.dr);
            dataGridView1.DataSource = d.dt;
            d.dr.Close();
        }
        public void EMPTY()
        {
            textBoxID.Text = "";
            numericUpDownSeating.Value = 1;
            radioButtonYes.Checked = false;
            radioButtonNo.Checked = false;
        }
        private void FormTable_Load(object sender, EventArgs e)
        {
            d.CONNECT();
            FillGrid();
        }

        //method to fill the fields with the columns from the datagrid in a row when we click on a column in a row
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            textBoxID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            numericUpDownSeating.Value = Convert.ToInt32(dataGridView1.CurrentRow.Cells[1].Value);
            if (dataGridView1.CurrentRow.Cells[2].Value.ToString() == "yes")
            {
                radioButtonYes.Checked = true;
            }
            else if (dataGridView1.CurrentRow.Cells[2].Value.ToString() == "no")
            {
                radioButtonNo.Checked = false;
            }
        }

        private void buttonAddTable_Click(object sender, EventArgs e)
        {
            if (ADD() == true)
            {
                MessageBox.Show("Table added successfully ", "Add Table", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FillGrid();

            }
            else
            {
                MessageBox.Show("Couldn't add the table", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonEditTable_Click(object sender, EventArgs e)
        {
            if (reservationDateExist() == true)
            {
                if (DateTime.Parse(getReservationDate()) > DateTime.Now)
                {
                    MessageBox.Show("Can't make modifications when the table is in an upcoming reservation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    if (EDIT() == true)
                    {
                        MessageBox.Show("Sucessfully edited ", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FillGrid();

                    }
                }
            }
            if (reservationDateExist() == false)
            {
                if (Available() == true)
                {

                    if (EDIT() == true)
                    {
                        MessageBox.Show("Sucessfully edited ", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FillGrid();

                    }
                }
                else
                {
                    MessageBox.Show("failed to mofify the table", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }

        private void buttonDeleteTable_Click(object sender, EventArgs e)
        {
            if (textBoxID.Text == "")
            {
                MessageBox.Show("Please specify the table you'd like to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (reservationDateExist() == true)
            {

                try
                {
                    if (DateTime.Parse(getReservationDate()) > DateTime.Now)
                    {
                        MessageBox.Show("You can't delete a table which is in an upcoming reservation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        if (DELETE() == true)
                        {
                            MessageBox.Show("Successfully deleted the table ", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FillGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            if (reservationDateExist() == false )
            {
                if (DELETE() == true)
                {
                    MessageBox.Show("Successfully deleted the table ", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FillGrid();
                }

            }
            else
            {
                MessageBox.Show("failed to delete the table", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



        }
    


        private void buttonEmptyField_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to empty the fields?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                EMPTY();
            }
                
        }

        private void button3_Click(object sender, EventArgs e)
        {

            this.Hide();
            FormDicount f = new FormDicount();
            f.Show();
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

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login f = new Login();
            f.Show();
        }
    }
}
