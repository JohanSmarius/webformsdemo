using System;
using System.Web.UI.WebControls;

namespace BasketballScores
{
    public partial class GamesPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGames();
        }

        private void BindGames()
        {
            var games = Database.GetGames();
            rptGames.DataSource = games;
            rptGames.DataBind();
            lblNoGames.Visible = games.Count == 0;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int editId = int.Parse(hfEditId.Value);
            var game = editId > 0 ? Database.GetGame(editId) : new Game();

            game.GameDate = DateTime.Parse(txtDate.Text);
            game.OpponentTeam = txtOpponent.Text.Trim();
            game.Location = txtLocation.Text.Trim();
            game.IsCompleted = ddlStatus.SelectedValue == "1";
            game.Notes = txtNotes.Text.Trim();

            game.OurScore = string.IsNullOrWhiteSpace(txtOurScore.Text)
                ? (int?)null : int.Parse(txtOurScore.Text);
            game.OpponentScore = string.IsNullOrWhiteSpace(txtOpponentScore.Text)
                ? (int?)null : int.Parse(txtOpponentScore.Text);

            int savedId = Database.SaveGame(game);
            ShowMessage(editId > 0 ? "Game updated." : "Game added.", success: true);
            ResetForm();
            BindGames();

            if (editId == 0)
                Response.Redirect("GameDetail.aspx?id=" + savedId + "&new=1");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void rptGames_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Edit")
            {
                var g = Database.GetGame(id);
                if (g == null) return;
                hfEditId.Value = g.Id.ToString();
                txtDate.Text = g.GameDate.ToString("yyyy-MM-dd");
                txtOpponent.Text = g.OpponentTeam;
                txtLocation.Text = g.Location;
                txtOurScore.Text = g.OurScore.HasValue ? g.OurScore.Value.ToString() : "";
                txtOpponentScore.Text = g.OpponentScore.HasValue ? g.OpponentScore.Value.ToString() : "";
                ddlStatus.SelectedValue = g.IsCompleted ? "1" : "0";
                txtNotes.Text = g.Notes;
                litFormTitle.Text = "Edit Game";
                btnCancel.Visible = true;
            }
            else if (e.CommandName == "Delete")
            {
                Database.DeleteGame(id);
                ShowMessage("Game deleted.", success: true);
                ResetForm();
                BindGames();
            }
        }

        private void ResetForm()
        {
            hfEditId.Value = "0";
            txtDate.Text = "";
            txtOpponent.Text = "";
            txtLocation.Text = "";
            txtOurScore.Text = "";
            txtOpponentScore.Text = "";
            ddlStatus.SelectedIndex = 0;
            txtNotes.Text = "";
            litFormTitle.Text = "Add New Game";
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
