using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Volunteer_Coordinator_System
{
    public partial class AdminView : Form
    {
        public AdminView()
        {
            InitializeComponent();
        }

        private void AdminView_Load(object sender, EventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Hide();

            LoadUserData();
        }

        private void AdminView_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Show();
        }

        private void ClearForm()
        {
            txtUserID.Text = "Auto Generated";
            txtName.Text = txtEmail.Text = txtPassword.Text = txtPhone.Text = txtAddress.Text = "";
            rdbMale.Checked = rdbFemale.Checked = false;
            cmbBloodGroup.SelectedItem = null;
            cmbRole.SelectedItem = null;
            txtSearch.Text = "";
        }

        private void LoadUserData(string searchValue = "")
        {
            try
            {
                string query = "SELECT U.UserID, U.Name, U.Email, U.Password, U.Phone, U.Gender, U.Location, U.BloodGroup, UT.UserTypeName " +
                               "FROM [User] U JOIN UserType UT ON U.UserTypeID = UT.UserTypeID";

                if (!string.IsNullOrEmpty(searchValue))
                {
                    int idValue;
                    if (int.TryParse(searchValue, out idValue))
                        query += " WHERE U.UserID = " + idValue + " OR U.Name LIKE '%" + searchValue + "%'";
                    else
                        query += " WHERE U.Name LIKE '%" + searchValue + "%'";
                }

                var result = DbHelper.GetQueryData(query);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message);
                    return;
                }

                dgvUsers.DataSource = result.Data;
                dgvUsers.Refresh();
                dgvUsers.ClearSelection();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadUserData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUserData(txtSearch.Text);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text;
                string email = txtEmail.Text;
                string password = txtPassword.Text;
                string phone = txtPhone.Text;
                string gender = rdbMale.Checked ? "Male" : (rdbFemale.Checked ? "Female" : "");
                string address = txtAddress.Text;
                string bloodGroup = cmbBloodGroup.SelectedItem == null ? "" : cmbBloodGroup.SelectedItem.ToString();
                string role = cmbRole.SelectedItem == null ? "" : cmbRole.SelectedItem.ToString();

                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Error: Enter Name");
                    return;
                }
                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("Error: Enter Email");
                    return;
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Error: Enter Password");
                    return;
                }
                if (string.IsNullOrWhiteSpace(phone))
                {
                    MessageBox.Show("Error: Enter Phone");
                    return;
                }
                if (string.IsNullOrWhiteSpace(gender))
                {
                    MessageBox.Show("Error: Select Gender");
                    return;
                }
                if (string.IsNullOrWhiteSpace(role))
                {
                    MessageBox.Show("Error: Select Role");
                    return;
                }

                string getRoleQuery = "SELECT UserTypeID FROM UserType WHERE UserTypeName = @role";
                var roleResult = DbHelper.GetQueryData(getRoleQuery, new Dictionary<string, object> { { "@role", role } });

                if (roleResult.HasError || roleResult.Data.Rows.Count == 0)
                {
                    MessageBox.Show("Invalid Role Selected");
                    return;
                }

                int userTypeID = Convert.ToInt32(roleResult.Data.Rows[0]["UserTypeID"]);

                string query;
                var parameters = new Dictionary<string, object>
                {
                    {"@Name", name},
                    {"@Email", email},
                    {"@Password", password},
                    {"@Phone", phone},
                    {"@Gender", gender},
                    {"@Location", address},
                    {"@BloodGroup", bloodGroup},
                    {"@UserTypeID", userTypeID}
                };

                if (txtUserID.Text == "Auto Generated")
                {
                    query = "INSERT INTO [User] (Name, Email, Password, Phone, Gender, Location, BloodGroup, UserTypeID) " +
                            "VALUES (@Name, @Email, @Password, @Phone, @Gender, @Location, @BloodGroup, @UserTypeID)";
                }
                else
                {
                    query = "UPDATE [User] SET Name=@Name, Email=@Email, Password=@Password, Phone=@Phone, " +
                            "Gender=@Gender, Location=@Location, BloodGroup=@BloodGroup, UserTypeID=@UserTypeID " +
                            "WHERE UserID=" + txtUserID.Text;
                }

                var result = DbHelper.ExecuteNonQuery(query, parameters);
                if (result.HasError)
                {
                    MessageBox.Show(result.Message);
                    return;
                }

                MessageBox.Show("User Saved Successfully!");
                ClearForm();
                LoadUserData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvUsers_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            ClearForm();
            try
            {
                if (e.RowIndex < 0)
                {
                    dgvUsers.ClearSelection();
                    return;
                }

                int userId = Convert.ToInt32(dgvUsers.Rows[e.RowIndex].Cells["UserID"].Value);
                string query = "SELECT * FROM [User] WHERE UserID = " + userId;
                var result = DbHelper.GetQueryData(query);

                if (result.HasError || result.Data.Rows.Count == 0)
                {
                    MessageBox.Show("Data not found!");
                    return;
                }

                DataTable dt = result.Data;
                txtUserID.Text = dt.Rows[0]["UserID"].ToString();
                txtName.Text = dt.Rows[0]["Name"].ToString();
                txtEmail.Text = dt.Rows[0]["Email"].ToString();
                txtPassword.Text = dt.Rows[0]["Password"].ToString();
                txtPhone.Text = dt.Rows[0]["Phone"].ToString();
                txtAddress.Text = dt.Rows[0]["Location"].ToString();
                rdbMale.Checked = dt.Rows[0]["Gender"].ToString().ToLower() == "male";
                rdbFemale.Checked = dt.Rows[0]["Gender"].ToString().ToLower() == "female";
                cmbBloodGroup.SelectedItem = dt.Rows[0]["BloodGroup"].ToString();

                // ✅ Assign role directly to Text, no need to match Items
                string roleQuery = "SELECT UserTypeName FROM UserType WHERE UserTypeID = " + dt.Rows[0]["UserTypeID"];
                var roleResult = DbHelper.GetQueryData(roleQuery);
                if (!roleResult.HasError && roleResult.Data.Rows.Count > 0)
                    cmbRole.Text = roleResult.Data.Rows[0]["UserTypeName"].ToString(); // Use Text instead of SelectedItem
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btndelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbRole.Text == "Admin")
                {
                    MessageBox.Show("You cannot delete an Admin account!",
                        "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                if (txtUserID.Text == "Auto Generated")
                {
                    MessageBox.Show("Error: Select a User first!");
                    return;
                }

                string query = "DELETE FROM [User] WHERE UserID = " + txtUserID.Text;
                var result = DbHelper.ExecuteNonQuery(query);

                if (result.HasError)
                {
                    MessageBox.Show(result.Message);
                    return;
                }

                MessageBox.Show("User Deleted Successfully!");
                ClearForm();
                LoadUserData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnMnageEvent_Click(object sender, EventArgs e)
        {
            EventManageByAdmin adminForm = new EventManageByAdmin();
            adminForm.Show(this);
        }

        private void btnVolunteerAndHelpSeeker_Click(object sender, EventArgs e)
        {
            VolunteerAndHelpSeekerManageByAdmin admin = new VolunteerAndHelpSeekerManageByAdmin();
            admin.Show(this);
        }

        private void btnDonation_Click(object sender, EventArgs e)
        {
            Donationform donationForm = new Donationform(); 
            donationForm.Show(this);
        }
    }
}
