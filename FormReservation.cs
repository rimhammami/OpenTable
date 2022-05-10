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
    public partial class FormReservation : Form
    {

        public FormReservation()
        {
            InitializeComponent();
            //customize the format of the datetimepicker
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "HH:mm";
            dateTimePicker2.ShowUpDown = true;
            

        }

        //method that checks if the reservation already exists in the database
        public int searchReservation()
        {
            int cpt;
            d.cmd.CommandText = " select count(reservationID) from [Reservation] where reservationID ='" + textBoxID.Text + "'";
            d.cmd.Connection = d.con;
            cpt = (int)d.cmd.ExecuteScalar();
            return cpt;
        }
        //method that returns the id of the user, using a global variable 
        public int searchUser()
        {
            int idUser;
            d.cmd.CommandText = " select UserID from [User] where email ='" + LoggedInUser.email + "'";
            d.cmd.Connection = d.con;
            idUser = (int)d.cmd.ExecuteScalar();
            return idUser;
        }
        //method that returns the role of the user
        public string UserRole()
        {
            String userRole;
            d.cmd.CommandText = " select type from [User] where email ='" + LoggedInUser.email + "'";
            d.cmd.Connection = d.con;
            userRole = (String)d.cmd.ExecuteScalar();
            return userRole;
        }

        //method to fill out the datagrid with reservations of the user
        public void FillGrid()
        {
            int clientid = searchUser(); //we get the id of the user through a method that uses a global variable
            if (d.dt.Rows != null)
            {
                d.dt.Clear();
            }
            string role = UserRole();
            if (role == "admin")
            {
                d.cmd.CommandText = " select * from Reservation";
            }
            else
            {
                d.cmd.CommandText = " select * from Reservation where clientID='" + clientid + "'";
            }
            d.cmd.Connection = d.con;
            d.dr = d.cmd.ExecuteReader();
            d.dt.Load(d.dr);
            dataGridView1.DataSource = d.dt;
            d.dr.Close();
        }
        //method thqt returns the number of reservations the user has made
        public int countRes()
        {
            int id = searchUser();

           
            d.cmd.CommandText = " select numberofReservations from [User] where UserID ='" + id + "'";
            d.cmd.Connection = d.con;
            var result = d.cmd.ExecuteScalar();
            if (result == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(result);
            }

        }
        //method that incremements the numbers of reservations the user has made
        public void accumulateReservation()
        {
            int id = searchUser();
            int cpt = countRes();
            cpt = cpt + 1;
            d.cmd.CommandText = "update [User] SET numberofReservations='"+cpt+"'where UserID= '" + id + "'";
            d.cmd.Connection = d.con;
            d.cmd.ExecuteNonQuery();

        }

        //method that decreases the numbers of reservations the user has made
        public void decreaseReservation()
        {
            int id = searchUser();
            int cpt = countRes();
            cpt = cpt - 1;
            d.cmd.CommandText = "update [User] SET numberofReservations='" + cpt + "'where UserID= '" + id + "'";
            d.cmd.Connection = d.con;
            d.cmd.ExecuteNonQuery();

        }
        //method to control who can see the discounts, depending on the number of reservations they've made
        public void discounts()
        {
            int cRes = countRes();
            if (cRes > 2)
            {
                label6.Visible = true;
                comboBoxDiscount.Visible = true;
                FillComboDiscount();
            }
            if (cRes < 2)
            {
                comboBoxDiscount.Visible = false;
                label6.Visible= false;
            }
        }
        ADO d = new ADO();

        //method to fill the combobox with the names of the discounts
 
        public void FillComboDiscount()
        {

            d.cmd.CommandText = " select discountID, name,value from Discount";
            d.cmd.Connection = d.con;
            d.dr = d.cmd.ExecuteReader();
            /*we use a global class Discounts to store the data which will go into the combobox
            in order to show the name and value together as  discription
            while passing the id as the actual value when the item is selected*/
            while (d.dr.Read() )
            {
                comboBoxDiscount.Items.Add(new Discounts() { id=Convert.ToInt32(d.dr["discountID"]), description = d.dr["name"].ToString()+": "+d.dr["value"].ToString() }); 
            }
            d.dr.Close();

            comboBoxDiscount.ValueMember = "id";
            comboBoxDiscount.DisplayMember = "description";

            
        }

        //method to fill the combobox with the seating capacity of availavle tables

        public void FillComboTables()
        {
            if (comboBoxTables.Items.Count != 0)
            {
                comboBoxTables.Items.Clear();
            }
            d.cmd.CommandText = "select * from [Table] where available='yes'";
            d.cmd.Connection = d.con;
            d.dr = d.cmd.ExecuteReader();

            while (d.dr.Read())
            {
                comboBoxTables.Items.Add(new Tables() { id = Convert.ToInt32(d.dr[0]), seatingcap = d.dr[1].ToString() });
            }
            d.dr.Close();
            comboBoxTables.ValueMember="id";
            comboBoxTables.DisplayMember = "seatingcap";
        }

        //method to alter the avilablilty of the table, which we use when the user books a table.
        public void EditAvailablityTable()
        {
            if (string.IsNullOrEmpty(comboBoxTables.Text) == false)
            {
                Tables t = (Tables)comboBoxTables.SelectedItem;
                int id = Convert.ToInt32(t.id);
                d.cmd.CommandText = "update [Table] SET available='no' where tableID= '" + id + "'";
                d.cmd.Connection = d.con;
                d.cmd.ExecuteNonQuery();
            }

        }

        //method which returns the id of the table so that we use it in the liberate table method
        public int TABLEid()
        {
            int id;
            d.cmd.CommandText = "select tableID from [Table] where tableID='" + textBoxTableID.Text + "'";
            d.cmd.Connection = d.con;
            id = (int)d.cmd.ExecuteScalar();
            return id;
        }
        //method that changes the availability of a table: used when we modify or delete a reservation
        public void LiberateTable()
        {
            int id=TABLEid();
            d.cmd.CommandText = "update [Table] SET available='yes' where tableID= '" + id + "'";
            d.cmd.Connection = d.con;
            d.cmd.ExecuteNonQuery();

        }

        // Add method 
        public bool ADD()
        {
            if (searchReservation() == 0)
            {
                Tables t =(Tables)comboBoxTables.SelectedItem;
                int idtable = Convert.ToInt32(t.id);
                int idclient = searchUser();
                string date = dateTimePicker1.Value.ToString("dd/MM/yyyy");
                string time = dateTimePicker2.Value.ToString("HH:mm");
                TimeSpan timee = dateTimePicker2.Value.TimeOfDay;
                TimeSpan startTime = new TimeSpan(11, 59, 0);
                TimeSpan endTime = new TimeSpan (20,29,0);
                DateTime earliestDate = DateTime.Now.AddDays(6);
                DateTime latesttDate = DateTime.Now.AddDays(13);
              
                if (comboBoxDiscount.SelectedItem == null && ((timee>startTime )&&(timee<endTime)) && ((dateTimePicker1.Value > earliestDate) && (dateTimePicker1.Value < latesttDate)))
                {
                   
                    d.cmd.CommandText = "insert into Reservation values ('" + idclient + "','" + idtable + "','" + date + "','" + time + "','" + "standard" + "',null)";
                    d.cmd.Connection = d.con;
                    d.cmd.ExecuteNonQuery();
                    return true;
                }
                else if (comboBoxDiscount.SelectedItem != null && ((timee > startTime) && (timee < endTime)) && ((dateTimePicker1.Value > earliestDate) && (dateTimePicker1.Value < latesttDate)))
                {
                    Discounts item = (Discounts)comboBoxDiscount.SelectedItem;
                    int discountId = Convert.ToInt32(item.id);
                    d.cmd.CommandText = "insert into Reservation values ('" + idclient + "','" + idtable + "','" + date + "','" + time + "','" + "standard" + "','" + discountId + "')";
                    d.cmd.Connection = d.con;
                    d.cmd.ExecuteNonQuery();
                    return true;
                }   
               
            }
            return false;
        }
      //method to empty the fields.
        public void EMPTY()
        {
            textBoxID.Text = "";
            textBoxTableID.Text = "";
            TextBoxuserID.Text = "";
            dateTimePicker1.Value = DateTime.Now.AddDays(7);
            dateTimePicker2.Value = DateTime.Now.AddDays(14);
            comboBoxDiscount.Text = "";
        }

        // declaration de la methode DELETE
        public bool DELETE()
        {
            if (searchReservation() != 0)
            {
                int id = Convert.ToInt32(textBoxID.Text);
                d.cmd.CommandText = " delete from Reservation where reservationID ='" + id + "'";
                d.cmd.Connection = d.con;
                d.cmd.ExecuteNonQuery();
                int idtable=TABLEid();
                return true;
            }
            
            return false;
        }

        // declaration de la methode EDIT
        public bool EDIT()
        {
            if (searchReservation() != 0)
            {
                string date = dateTimePicker1.Value.ToString("dd/MM/yyyy");
                string time = dateTimePicker2.Value.ToString("HH:mm");
                int id = Convert.ToInt32(textBoxID.Text);
                int idclient = searchUser();
                int idtable;
                if (string.IsNullOrEmpty(comboBoxTables.Text))
                {
                    idtable = Convert.ToInt32(textBoxTableID.Text);
                }
                else
                {
                    Tables t = (Tables)comboBoxTables.SelectedItem;
                    idtable = Convert.ToInt32(t.id);
                }
                if (comboBoxDiscount.SelectedItem == null || comboBoxDiscount.Visible== false)
                    {
                    d.cmd.CommandText = "update Reservation set reservationDate= '" + date + "' ,ArrivalHour = '" + time + "',clientID = '" + idclient + "',tableID = '" + idtable + "'where reservationID = '" + id + "'";
                    d.cmd.Connection = d.con;
                    d.cmd.ExecuteNonQuery();
                    return true;

                    }
                else if (comboBoxDiscount.SelectedItem != null)
                    {
                    Discounts item = (Discounts)comboBoxDiscount.SelectedItem;
                    int discountId = Convert.ToInt32(item.id);
                    d.cmd.CommandText = "update Reservation set reservationDate= '" + date + "' ,ArrivalHour = '" + time + "',clientID = '" + idclient + "',tableID = '" + idtable + "',discountID = '" + discountId + "'where reservationID = '" + id + "'";
                    d.cmd.Connection = d.con;
                    d.cmd.ExecuteNonQuery();
                    return true;
                    }
            }
            return false;
        }

        private void FormReservation_Load(object sender, EventArgs e)
        {
            d.CONNECT();
            FillGrid();
            FillComboTables();
            discounts();
            String role = UserRole();
            if (role=="standard")
            {
                button1.Hide();
                button2.Hide();
                button3.Hide();

            }
            

        }

        private void buttonAddReservation_Click(object sender, EventArgs e)
        {
            if (comboBoxTables.Text == "" )
            {
                MessageBox.Show(" Please pick a seating capacity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            else
            { 
                if (ADD() == true)
                {
                MessageBox.Show(" Reservation made successfull", "Resevation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EditAvailablityTable();
                FillGrid();
                discounts();
                FillComboTables();
                accumulateReservation();
                }
                else
                {
                    MessageBox.Show(" Reservation failed to complete. The earliest reservation has to no less than 7 days in advance. The arrival time has to be between 12:00 and 20:30", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

            
        
        //method that puts the values of the row in datagrid into the fields once clicked on.
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            textBoxID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            if (dataGridView1.CurrentRow.Cells[3].Value == System.DBNull.Value || dataGridView1.CurrentRow.Cells[4].Value == System.DBNull.Value)
            {
                dateTimePicker1.Value = DateTime.Now.AddDays(7);
                dateTimePicker2.Value = DateTime.Now;
            }
            else
            {
                dateTimePicker1.Value = DateTime.Parse(dataGridView1.CurrentRow.Cells[3].Value.ToString());
                dateTimePicker2.Value = DateTime.Parse(dataGridView1.CurrentRow.Cells[4].Value.ToString());
            }

            TextBoxuserID.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBoxTableID.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
        }

        private void buttonEditReservation_Click(object sender, EventArgs e)
        {

                if (EDIT() == true)
                {
                MessageBox.Show("Mofified with success ", "Modification", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (string.IsNullOrEmpty(comboBoxTables.Text) == false)
                {
                    LiberateTable();
                }
                FillGrid();
                FillComboTables();
                discounts();
            }
                else
                {
                    MessageBox.Show("Reservation editting failure.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            
        }

        private void buttonDeleteReservation_Click(object sender, EventArgs e)
        {
            if (textBoxID.Text == "")
            {
                MessageBox.Show("Please specify the reservation you'd like to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (DELETE() == true)
            {
                MessageBox.Show("Reservation deleted with success ", "Deletion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LiberateTable();
                FillGrid();
                FillComboTables();
                discounts();
                decreaseReservation();
            }
            else
            {
                MessageBox.Show("Reservation deletion failure", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            FormTable f = new FormTable();
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


    }
}
