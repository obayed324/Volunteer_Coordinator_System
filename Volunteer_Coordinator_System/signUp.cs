using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class signUp : Form
    {
        public signUp()
        {
            InitializeComponent();
        }

        private void signUp_Load(object sender, EventArgs e)
        {
            this.Owner?.Hide();
        }

        private void signUp_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Owner?.Show();
        }

        private void btnRegisterNow_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string gender = rdbMale.Checked ? rdbMale.Text : rdbFemale.Checked ? rdbFemale.Text : "";
            string bloodGroup = cmbBloodGroup.SelectedItem?.ToString() ?? "";
            string role = cmbRole.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(gender) ||
                string.IsNullOrWhiteSpace(bloodGroup) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Please fill all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Name", name},
                    {"@Email", email},
                    {"@Password", password},
                    {"@Phone", phone},
                    {"@Location", address},
                    {"@Gender", gender},
                    {"@BloodGroup", bloodGroup},
                    {"@Role", role}
                };

                string query = @"
                    INSERT INTO [User] (Name, Email, Password, Phone, Location, Gender, BloodGroup, UserTypeID)
                    VALUES (@Name, @Email, @Password, @Phone, @Location, @Gender, @BloodGroup,
                    (SELECT UserTypeID FROM UserType WHERE UserTypeName=@Role))";

                var result = DbHelper.ExecuteNonQuery(query, parameters);

                if (result.HasError)
                {
                    MessageBox.Show(result.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("User Registered Successfully!");
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            txtName.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            cmbRole.SelectedIndex = -1;
            cmbBloodGroup.SelectedIndex = -1;
            rdbMale.Checked = false;
            rdbFemale.Checked = false;
        }

        
    }
}
