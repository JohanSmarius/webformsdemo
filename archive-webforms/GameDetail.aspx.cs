using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BasketballScores
{
    public partial class GameDetailPage : Page
    {
        private int GameId
        {
            get { return int.Parse(hfGameId.Value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(Request.QueryString["id"], out id) || id <= 0)
            {
                Response.Redirect("Games.aspx");
                return;
            }

            if (!IsPostBack)
            {
                hfGameId.Value = id.ToString();
                LoadGame(id);
                BindStats(id);

                if (Request.QueryString["new"] == "1")
                    ShowMessage("Game created. Enter player stats below.", success: true);
            }
        }

        private void LoadGame(int id)
        {
            var g = Database.GetGame(id);
            if (g == null) { Response.Redirect("Games.aspx"); return; }

            litTitle.Text = "vs. " + g.OpponentTeam;
            litSubtitle.Text = g.GameDate.ToString("dd MMMM yyyy") + " &mdash; " + g.Location;

            txtDate.Text = g.GameDate.ToString("yyyy-MM-dd");
            txtOpponent.Text = g.OpponentTeam;
            txtLocation.Text = g.Location;
            txtOurScore.Text = g.OurScore.HasValue ? g.OurScore.Value.ToString() : "";
            txtOpponentScore.Text = g.OpponentScore.HasValue ? g.OpponentScore.Value.ToString() : "";
            ddlStatus.SelectedValue = g.IsCompleted ? "1" : "0";
            txtNotes.Text = g.Notes;
        }

        private void BindStats(int gameId)
        {
            // Build a merged list: all active players with their existing stats (or zero defaults)
            var players = Database.GetPlayers(activeOnly: true);
            var existingStats = Database.GetStatsForGame(gameId);
            var statMap = new Dictionary<int, PlayerGameStat>();
            foreach (var s in existingStats) statMap[s.PlayerId] = s;

            var rows = new List<StatRow>();
            foreach (var p in players)
            {
                PlayerGameStat stat;
                statMap.TryGetValue(p.Id, out stat);
                rows.Add(new StatRow
                {
                    PlayerId = p.Id,
                    PlayerName = p.Name,
                    JerseyNumber = p.JerseyNumber,
                    Position = p.Position,
                    Points = stat != null ? stat.Points : 0,
                    Errors = stat != null ? stat.Errors : 0,
                    Assists = stat != null ? stat.Assists : 0,
                    Rebounds = stat != null ? stat.Rebounds : 0,
                    MinutesPlayed = stat != null ? stat.MinutesPlayed : 0
                });
            }

            pnlNoPlayers.Visible = players.Count == 0;
            pnlStats.Visible = players.Count > 0;
            rptStats.DataSource = rows;
            rptStats.DataBind();
        }

        protected void btnUpdateGame_Click(object sender, EventArgs e)
        {
            var g = Database.GetGame(GameId);
            if (g == null) return;

            g.GameDate = DateTime.Parse(txtDate.Text);
            g.OpponentTeam = txtOpponent.Text.Trim();
            g.Location = txtLocation.Text.Trim();
            g.IsCompleted = ddlStatus.SelectedValue == "1";
            g.Notes = txtNotes.Text.Trim();
            g.OurScore = string.IsNullOrWhiteSpace(txtOurScore.Text) ? (int?)null : int.Parse(txtOurScore.Text);
            g.OpponentScore = string.IsNullOrWhiteSpace(txtOpponentScore.Text) ? (int?)null : int.Parse(txtOpponentScore.Text);

            Database.SaveGame(g);
            litTitle.Text = "vs. " + g.OpponentTeam;
            litSubtitle.Text = g.GameDate.ToString("dd MMMM yyyy") + " &mdash; " + g.Location;
            ShowMessage("Game updated.", success: true);
        }

        protected void btnSaveStats_Click(object sender, EventArgs e)
        {
            int gameId = GameId;
            foreach (RepeaterItem item in rptStats.Items)
            {
                var hfPid = (HiddenField)item.FindControl("hfPlayerId");
                var txtPts = (TextBox)item.FindControl("txtPoints");
                var txtErr = (TextBox)item.FindControl("txtErrors");
                var txtAst = (TextBox)item.FindControl("txtAssists");
                var txtReb = (TextBox)item.FindControl("txtRebounds");
                var txtMin = (TextBox)item.FindControl("txtMinutes");

                int playerId;
                if (!int.TryParse(hfPid.Value, out playerId)) continue;

                var stat = new PlayerGameStat
                {
                    GameId = gameId,
                    PlayerId = playerId,
                    Points = ParseInt(txtPts.Text),
                    Errors = ParseInt(txtErr.Text),
                    Assists = ParseInt(txtAst.Text),
                    Rebounds = ParseInt(txtReb.Text),
                    MinutesPlayed = ParseInt(txtMin.Text)
                };
                Database.SaveStat(stat);
            }

            ShowMessage("Player stats saved.", success: true);
            BindStats(gameId);
        }

        private static int ParseInt(string s)
        {
            int v;
            return int.TryParse(s, out v) ? Math.Max(0, v) : 0;
        }

        private void ShowMessage(string msg, bool success)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass = success ? "alert alert-success" : "alert alert-error";
            lblMessage.Visible = true;
        }

        // ViewModel used only by the Repeater binding
        private class StatRow
        {
            public int PlayerId { get; set; }
            public string PlayerName { get; set; }
            public int JerseyNumber { get; set; }
            public string Position { get; set; }
            public int Points { get; set; }
            public int Errors { get; set; }
            public int Assists { get; set; }
            public int Rebounds { get; set; }
            public int MinutesPlayed { get; set; }
        }
    }
}
