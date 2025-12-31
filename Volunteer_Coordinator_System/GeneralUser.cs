using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class GeneralUser : Form
    {
        private User currentUser;

        // Constructor to receive User object
        public GeneralUser(User user)
        {
            InitializeComponent();
            currentUser = user;
        }

        private void GeneralUser_Load(object sender, EventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Hide();

            LoadProfile();
            if (currentUser != null)
            {
                lblName.Text = "Welcome " + currentUser.Name;
            }

            //Load Event Here panel name:
            LoadEvents();
        }

        private void GeneralUser_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Show();
        }

        private void LoadProfile()
        {
            try
            {
                if (currentUser != null)
                {
                    txtName.Text = currentUser.Name;
                    txtEmail.Text = currentUser.Email;
                    txtPhone.Text = currentUser.Phone;
                    txtgender.Text = currentUser.Gender;
                    txtLocation.Text = currentUser.Location;
                    txtBloodGroup.Text = currentUser.BloodGroup;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profile: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                
                DialogResult dr = MessageBox.Show("Are you sure you want to logout?",
                                                  "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    
                    if (this.Owner != null)
                    {
                        this.Owner.Show();
                    }

                    this.Close(); // close the GeneralUser form
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during logout: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentUser == null)
                {
                    MessageBox.Show("No user loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Read updated values from textboxes
                string name = txtName.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string gender = txtgender.Text.Trim();
                string location = txtLocation.Text.Trim();
                string bloodGroup = txtBloodGroup.Text.Trim();

                // Optional: Validation
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
                {
                    MessageBox.Show("Name and Phone cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Prepare parameters
                var parameters = new Dictionary<string, object>
                {
                        {"@Name", name},
                        {"@Phone", phone},
                        {"@Gender", gender},
                        {"@Location", location},
                        {"@BloodGroup", bloodGroup},
                        {"@UserID", currentUser.UserID}
                 };

                string query = @"
                UPDATE [User] 
                SET Name = @Name,
                    Phone = @Phone,
                    Gender = @Gender,
                    Location = @Location,
                    BloodGroup = @BloodGroup
                WHERE UserID = @UserID";

                var result = DbHelper.ExecuteNonQuery(query, parameters);

                if (result.HasError)
                {
                    MessageBox.Show("Update failed: " + result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Update local currentUser object
                currentUser.Name = name;
                currentUser.Phone = phone;
                currentUser.Gender = gender;
                currentUser.Location = location;
                currentUser.BloodGroup = bloodGroup;

                // Update welcome label if name changed
                lblName.Text = "Welcome " + currentUser.Name;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating profile: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadEvents()
        {
            try
            {
                string query = "SELECT EventID, EventName, EventDate, Location FROM Event ORDER BY EventDate DESC";

                var result = DbHelper.GetQueryData(query);

                if (!result.HasError && result.Data != null)
                {
                    dgvEvents.DataSource = result.Data;

                    // --- DataGridView Appearance ---
                    dgvEvents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // fill entire width
                    dgvEvents.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;   // auto-size row height
                    dgvEvents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;    // highlight full row
                    dgvEvents.MultiSelect = false;                                        // select only one row
                    dgvEvents.ReadOnly = true;                                            // prevent editing
                    dgvEvents.AllowUserToAddRows = false;                                 // hide empty row at bottom
                    dgvEvents.AllowUserToDeleteRows = false;

                    // Optional: adjust column headers
                    dgvEvents.Columns["EventID"].HeaderText = "ID";
                    dgvEvents.Columns["EventName"].HeaderText = "Event Name";
                    dgvEvents.Columns["EventDate"].HeaderText = "Date";
                    dgvEvents.Columns["Location"].HeaderText = "Location";

                    // --- Attach double-click event handler ---
                    dgvEvents.CellDoubleClick -= dgvEvents_CellDoubleClick; // remove previous handler if any
                    dgvEvents.CellDoubleClick += dgvEvents_CellDoubleClick;
                }
                else
                {
                    MessageBox.Show("No events found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading events: " + ex.Message);
            }
        }

        // --- Double-click event ---
        private void dgvEvents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the click is on a valid row, not the header
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvEvents.Rows[e.RowIndex];
                int eventId = Convert.ToInt32(row.Cells["EventID"].Value);

                // Open EventForm or perform your action
                Event evForm = new Event(currentUser, eventId);
                evForm.Show(this);
            }
        }

        private void btnMyActivity_Click(object sender, EventArgs e)
        {
            MyActivityForm myActivity = new MyActivityForm(currentUser);
            myActivity.Show(this);
        }

    }
}
