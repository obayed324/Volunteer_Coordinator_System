using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class EventManager : Form
    {
        private int _managerId;

        public EventManager(int managerId)
        {
            InitializeComponent();
            _managerId = managerId;
        }

        private void EventManager_Load(object sender, EventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Hide();

            dgvEvent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEvent.MultiSelect = false;
            dgvEvent.ReadOnly = true;
            dgvEvent.AutoGenerateColumns = true;

            LoadEventData();
            LoadHelpSeekers();
            LoadVolunteers();
        }

        private void EventManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Show();
        }

        // ==========================
        // LOAD EVENTS ASSIGNED TO MANAGER
        // ==========================
        private void LoadEventData(string searchValue = "")
        {
            try
            {
                string query = "SELECT * FROM Event WHERE AssignEvent = @ManagerId";
                var parameters = new Dictionary<string, object> { { "@ManagerId", _managerId } };

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

                if (result.Data.Rows.Count == 0)
                {
                    MessageBox.Show("No matching events found.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading events: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            txtEventId.Text = "Auto Generated";
            txtEventName.Text = txtDescription.Text = txtLocation.Text = "";
            dtpEventDate.Value = DateTime.Now;
        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadEventData();
            dgvEvent.ClearSelection();
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            string searchValue = txtSearch.Text.Trim();
            LoadEventData(searchValue);
        }

        // ==========================
        // SAVE / UPDATE EVENT
        // ==========================
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string eventName = txtEventName.Text.Trim();
                string location = txtLocation.Text.Trim();
                string description = txtDescription.Text.Trim();
                DateTime eventDate = dtpEventDate.Value;

                if (string.IsNullOrWhiteSpace(eventName) || string.IsNullOrWhiteSpace(location))
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query;
                var parameters = new Dictionary<string, object>
                {
                    {"@EventName", eventName},
                    {"@EventDate", eventDate},
                    {"@Location", location},
                    {"@Description", description},
                    {"@AssignEvent", _managerId}
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
                                  Location=@Location, Description=@Description 
                              WHERE EventID=@EventID AND AssignEvent=@AssignEvent";
                    parameters["@EventID"] = Convert.ToInt32(txtEventId.Text);
                }

                var result = DbHelper.ExecuteNonQuery(query, parameters);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message);
                    return;
                }

                MessageBox.Show("Event saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadEventData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving event: " + ex.Message);
            }
        }

        // ==========================
        // DELETE EVENT
        // ==========================
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
                    {"@EventID", Convert.ToInt32(txtEventId.Text)},
                    {"@AssignEvent", _managerId}
                };

                string query = "DELETE FROM Event WHERE EventID=@EventID AND AssignEvent=@AssignEvent";
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

        // ==========================
        // SELECT EVENT ON DOUBLE CLICK
        // ==========================
        private void dgvEvent_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error selecting event: " + ex.Message);
            }
        }

        // ==========================
        // HELP SEEKER MANAGEMENT
        // ==========================
        private void LoadHelpSeekers()
        {
            try
            {
                string query = @"
                SELECT ehs.HelpID AS HelpSeekerID, u.Name, u.Email, ehs.Status
                FROM EventHelpSeeker ehs
                INNER JOIN [User] u ON ehs.UserID = u.UserID
                INNER JOIN Event ev ON ehs.EventID = ev.EventID
                WHERE ev.AssignEvent = @ManagerID";

                var parameters = new Dictionary<string, object>
                {
                    { "@ManagerID", _managerId }
                };

                var result = DbHelper.GetQueryData(query, parameters);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dgvHelpSeekers.DataSource = result.Data;
                dgvHelpSeekers.ClearSelection();
                txtHelpStatus.Clear();
                txtHelpStatus.Tag = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading help seekers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHelpConfirm_Click(object sender, EventArgs e)
        {
            if (txtHelpStatus.Tag == null)
            {
                MessageBox.Show("Please select a Help Seeker first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateHelpSeekerStatus(Convert.ToInt32(txtHelpStatus.Tag), "Confirmed");
        }

        private void btnHelpReject_Click_1(object sender, EventArgs e)
        {
            if (txtHelpStatus.Tag == null)
            {
                MessageBox.Show("Please select a Help Seeker first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateHelpSeekerStatus(Convert.ToInt32(txtHelpStatus.Tag), "Rejected");
        }
        private void btnHelpRefresh_Click_1(object sender, EventArgs e)
        {
            LoadHelpSeekers();
        }

        private void dgvHelpSeekers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvHelpSeekers.Rows[e.RowIndex];
                selectedRow.Selected = true;

                txtHelpStatus.Text = selectedRow.Cells["Status"].Value?.ToString() ?? "";
                txtHelpStatus.Tag = selectedRow.Cells["HelpSeekerID"].Value; // use the correct column name
            }
        }
        private void UpdateHelpSeekerStatus(int helpId, string newStatus)
        {
            try
            {
                string query = "UPDATE EventHelpSeeker SET Status = @Status WHERE HelpID = @HelpID";
                var parameters = new Dictionary<string, object>
                {
                    { "@Status", newStatus },
                    { "@HelpID", helpId }
                };

                var result = DbHelper.ExecuteNonQuery(query, parameters);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show($"Help Seeker status updated to '{newStatus}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadHelpSeekers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================
        // VOLUNTEER MANAGEMENT
        // ==========================
        private void LoadVolunteers()
        {
            try
            {
                string query = @"
                SELECT ev.EventVolunteerID, u.Name, u.Email, ev.AssignedRole, ev.Status
                FROM EventVolunteer ev
                INNER JOIN [User] u ON ev.UserID = u.UserID
                INNER JOIN Event e ON ev.EventID = e.EventID
                WHERE e.AssignEvent = @ManagerID";

                var parameters = new Dictionary<string, object>
                {
                    { "@ManagerID", _managerId }
                };

                var result = DbHelper.GetQueryData(query, parameters);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dgvVolunteer.DataSource = result.Data;
                dgvVolunteer.ClearSelection();
                txtVolunteerStatus.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading volunteers: " + ex.Message);
            }
        }

        private void btnVolunteerRefresh_Click_1(object sender, EventArgs e)
        {
            LoadVolunteers();
        }

        
        private void dgvVolunteer_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dgvVolunteer.ClearSelection();
                dgvVolunteer.Rows[e.RowIndex].Selected = true;

                DataGridViewRow row = dgvVolunteer.Rows[e.RowIndex];
                txtVolunteerStatus.Text = row.Cells["Status"].Value?.ToString() ?? "";
                txtVolunteerStatus.Tag = row.Cells["EventVolunteerID"].Value;
            }
        }

        private void btnVolunteerConfirm_Click_1(object sender, EventArgs e)
        {
            if (txtVolunteerStatus.Tag == null)
            {
                MessageBox.Show("Please select a Volunteer first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateVolunteerStatus(Convert.ToInt32(txtVolunteerStatus.Tag), "Confirmed");
        }

       
        private void btnVolunteerReject_Click_1(object sender, EventArgs e)
        {
            if (txtVolunteerStatus.Tag == null)
            {
                MessageBox.Show("Please select a Volunteer first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateVolunteerStatus(Convert.ToInt32(txtVolunteerStatus.Tag), "Rejected");
        }

        private void UpdateVolunteerStatus(int volunteerId, string newStatus)
        {
            try
            {
                string query = "UPDATE EventVolunteer SET Status = @Status WHERE EventVolunteerID = @VolunteerID";
                var parameters = new Dictionary<string, object>
                {
                    { "@Status", newStatus },
                    { "@VolunteerID", volunteerId }
                };

                var result = DbHelper.ExecuteNonQuery(query, parameters);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show($"Volunteer status updated to '{newStatus}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadVolunteers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating volunteer status: " + ex.Message);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.Yes)
            {
                this.Hide();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }

        
    }
}
