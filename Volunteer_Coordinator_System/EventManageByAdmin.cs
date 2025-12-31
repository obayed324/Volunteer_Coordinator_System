using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class EventManageByAdmin : Form
    {
        public EventManageByAdmin()
        {
            InitializeComponent();
        }

        private void EventManageByAdmin_Load(object sender, EventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Hide();

            dgvEvent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEvent.MultiSelect = false;
            dgvEvent.ReadOnly = true;
            dgvEvent.AutoGenerateColumns = true;

            LoadManagerCombo(); // ✅ Load all managers in combo
            LoadEventData();
        }

        private void EventManageByAdmin_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Show();
        }

        private void LoadManagerCombo()
        {
            try
            {
                string query = @"SELECT u.UserID, u.Name
                                 FROM [User] u
                                 JOIN UserType ut ON u.UserTypeID = ut.UserTypeID
                                 WHERE ut.UserTypeName = 'Manager'";

                var result = DbHelper.GetQueryData(query);

                if (result.HasError)
                {
                    MessageBox.Show("Error loading managers: " + result.Message);
                    return;
                }

                cmbManager.DataSource = result.Data;
                cmbManager.DisplayMember = "Name";     // Shows manager name
                cmbManager.ValueMember = "UserID";     // Holds manager ID
                cmbManager.SelectedIndex = -1;         // No selection initially
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading manager list: " + ex.Message);
            }
        }

        private void LoadEventData(string searchValue = "")
        {
            try
            {
                string query = "SELECT * FROM Event WHERE 1=1";
                var parameters = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    query += " AND (EventName LIKE @Search OR Location LIKE @Search)";
                    parameters.Add("@Search", "%" + searchValue + "%");
                }

                var result = DbHelper.GetQueryData(query, parameters);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dgvEvent.DataSource = result.Data;
                dgvEvent.ClearSelection();
                ClearForm();

                if (result.Data.Rows.Count == 0 && !string.IsNullOrEmpty(searchValue))
                {
                    MessageBox.Show("No matching events found.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading events: " + ex.Message);
            }
        }

        private void dgvEvent_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || dgvEvent.Rows.Count == 0)
                    return;

                dgvEvent.Rows[e.RowIndex].Selected = true;

                DataGridViewRow selectedRow = dgvEvent.Rows[e.RowIndex];
                txtEventId.Text = selectedRow.Cells["EventID"].Value.ToString();
                txtEventName.Text = selectedRow.Cells["EventName"].Value.ToString();
                dtpEventDate.Value = Convert.ToDateTime(selectedRow.Cells["EventDate"].Value);
                txtLocation.Text = selectedRow.Cells["Location"].Value.ToString();
                txtDescription.Text = selectedRow.Cells["Description"].Value.ToString();

                // ✅ Set combo value based on AssignEvent column
                if (selectedRow.Cells["AssignEvent"].Value != DBNull.Value)
                {
                    int managerId = Convert.ToInt32(selectedRow.Cells["AssignEvent"].Value);
                    cmbManager.SelectedValue = managerId;
                }
                else
                {
                    cmbManager.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error selecting event: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtEventId.Text == "Auto Generated")
                {
                    MessageBox.Show("Please select an event to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var parameters = new Dictionary<string, object>
                {
                    {"@EventID", Convert.ToInt32(txtEventId.Text)}
                };

                string query = "DELETE FROM Event WHERE EventID=@EventID";

                var result = DbHelper.ExecuteNonQuery(query, parameters);

                if (result.HasError)
                {
                    MessageBox.Show(result.Message);
                    return;
                }

                MessageBox.Show("Event deleted successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadEventData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting event: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string eventName = txtEventName.Text.Trim();
                string location = txtLocation.Text.Trim();
                string description = txtDescription.Text.Trim();
                DateTime eventDate = dtpEventDate.Value;

                // ✅ Get manager from combo
                if (cmbManager.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a manager to assign this event.");
                    return;
                }

                int managerId = Convert.ToInt32(cmbManager.SelectedValue);

                // ✅ Insert or Update event
                string query;
                var parameters = new Dictionary<string, object>
                {
                    { "@EventName", eventName },
                    { "@EventDate", eventDate },
                    { "@Location", location },
                    { "@Description", description },
                    { "@AssignEvent", managerId }
                };

                if (txtEventId.Text == "Auto Generated")
                {
                    query = @"INSERT INTO Event (EventName, EventDate, Location, Description, AssignEvent)
                              VALUES (@EventName, @EventDate, @Location, @Description, @AssignEvent)";
                }
                else
                {
                    query = @"UPDATE Event 
                              SET EventName=@EventName, EventDate=@EventDate, 
                                  Location=@Location, Description=@Description,
                                  AssignEvent=@AssignEvent
                              WHERE EventID=@EventID";
                    parameters["@EventID"] = Convert.ToInt32(txtEventId.Text);
                }

                var result = DbHelper.ExecuteNonQuery(query, parameters);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message);
                    return;
                }

                MessageBox.Show("Event saved successfully and assigned to Manager!");
                ClearForm();
                LoadEventData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchValue = txtSearch.Text.Trim();
            LoadEventData(searchValue);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadEventData();
            dgvEvent.ClearSelection();
        }

        private void ClearForm()
        {
            txtEventId.Text = "Auto Generated";
            txtEventName.Text = txtDescription.Text = txtLocation.Text = "";
            dtpEventDate.Value = DateTime.Now;
            cmbManager.SelectedIndex = -1; // ✅ reset manager combo
        }
    }
}
