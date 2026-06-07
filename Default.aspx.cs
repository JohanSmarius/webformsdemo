using System;
using System.Linq;
using System.Web.UI;

namespace BasketballScores
{
    public partial class DefaultPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindData();
        }

        private void BindData()
        {
            var games = Database.GetGames().Where(g => g.IsCompleted).ToList();
            litGames.Text = games.Count.ToString();
            litWins.Text = games.Count(g => g.Result == "Win").ToString();
            litLosses.Text = games.Count(g => g.Result == "Loss").ToString();
            litDraws.Text = games.Count(g => g.Result == "Draw").ToString();
            litPlayers.Text = Database.GetPlayers(activeOnly: true).Count.ToString();

            var allGames = Database.GetGames().Take(5).ToList();
            rptRecentGames.DataSource = allGames;
            rptRecentGames.DataBind();
            lblNoGames.Visible = allGames.Count == 0;

            var summaries = Database.GetPlayerSummaries()
                .OrderByDescending(p => p.TotalPoints).Take(10).ToList();
            rptTopScorers.DataSource = summaries;
            rptTopScorers.DataBind();
            lblNoPlayers.Visible = summaries.Count == 0;
        }
    }
}
