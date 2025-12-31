using System;
using System.Data;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class Event : Form
    {
        private int _eventId;
        private User currentUser; 

        public Event(User user, int eventId)
        {
            InitializeComponent();
            currentUser = user;
            _eventId = eventId;
        }

        private void Event_Load(object sender, EventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Hide();
            LoadEventDetails();

        }
        private void Event_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Show();
        }





        private void LoadEventDetails()
        {
            try
            {
                string query = $"SELECT EventName, EventDate, Location, Description FROM Event WHERE EventID = {_eventId}";
                var result = DbHelper.GetQueryData(query);

                if (!result.HasError && result.Data != null && result.Data.Rows.Count > 0)
                {
                    DataRow row = result.Data.Rows[0];
                    txtEventName.Text = row["EventName"].ToString();
                    txEvebtDate.Text = Convert.ToDateTime(row["EventDate"]).ToString("yyyy-MM-dd");
                    txtEventLocation.Text = row["Location"].ToString();
                    txtEventDescription.Text = row["Description"].ToString();
                }
                else
                {
                    MessageBox.Show("Event details not found.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading event details: " + ex.Message);
            }
        }

        private void LoadFormInPanel(Form childForm)
        {
            PanelContainer.Controls.Clear();
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            PanelContainer.Controls.Add(childForm);
            childForm.Show();
        }

        private void btnDonation_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new DonorView(currentUser, _eventId));
        }

       
        private void btnHelp_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new HelpSeeking(currentUser.UserID, _eventId));
        }


        private void btnVolunteer_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new VolunteerView(currentUser.UserID, _eventId));
        }

    }
}
