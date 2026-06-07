using System;
using System.Web.UI.WebControls;

namespace BasketballScores
{
    public partial class PlayersPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindPlayers();
        }

        private void BindPlayers()
        {
            var summaries = Database.GetPlayerSummaries();
            rptPlayers.DataSource = summaries;
            rptPlayers.DataBind();
            lblNoPlayers.Visible = summaries.Count == 0;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int editId = int.Parse(hfEditId.Value);
            var player = editId > 0 ? Database.GetPlayer(editId) : new Player();

            player.Name = txtName.Text.Trim();
            player.JerseyNumber = int.Parse(txtJersey.Text.Trim());
            player.Position = ddlPosition.SelectedValue;
            player.IsActive = true;

            Database.SavePlayer(player);

            ShowMessage(editId > 0 ? "Player updated." : "Player added.", success: true);
            ResetForm();
            BindPlayers();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void rptPlayers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Edit")
            {
                var p = Database.GetPlayer(id);
                if (p == null) return;
                hfEditId.Value = p.Id.ToString();
                txtName.Text = p.Name;
                txtJersey.Text = p.JerseyNumber.ToString();
                ddlPosition.SelectedValue = p.Position;
                litFormTitle.Text = "Edit Player";
                btnCancel.Visible = true;
            }
            else if (e.CommandName == "Delete")
            {
                Database.DeletePlayer(id);
                ShowMessage("Player removed from roster.", success: true);
                ResetForm();
                BindPlayers();
            }
        }

        private void ResetForm()
        {
            hfEditId.Value = "0";
            txtName.Text = "";
            txtJersey.Text = "";
            ddlPosition.SelectedIndex = 0;
            litFormTitle.Text = "Add New Player";
            btnCancel.Visible = false;
        }

        private void ShowMessage(string msg, bool success)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass = success ? "alert alert-success" : "alert alert-error";
            lblMessage.Visible = true;
        }
    }
}
