<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BasketballScores.DefaultPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-header">
        <h1>&#127936; Season Dashboard</h1>
        <p>Overview of your team's performance this season.</p>
    </div>

    <!-- Season stats -->
    <div class="stat-grid">
        <div class="stat-box">
            <div class="stat-value"><asp:Literal ID="litGames" runat="server" /></div>
            <div class="stat-label">Games Played</div>
        </div>
        <div class="stat-box win">
            <div class="stat-value"><asp:Literal ID="litWins" runat="server" /></div>
            <div class="stat-label">Wins</div>
        </div>
        <div class="stat-box loss">
            <div class="stat-value"><asp:Literal ID="litLosses" runat="server" /></div>
            <div class="stat-label">Losses</div>
        </div>
        <div class="stat-box draw">
            <div class="stat-value"><asp:Literal ID="litDraws" runat="server" /></div>
            <div class="stat-label">Draws</div>
        </div>
        <div class="stat-box">
            <div class="stat-value"><asp:Literal ID="litPlayers" runat="server" /></div>
            <div class="stat-label">Active Players</div>
        </div>
    </div>

    <!-- Recent games -->
    <div class="card">
        <div class="section-header">
            <div class="card-title" style="margin-bottom:0;border-bottom:none;padding-bottom:0">Recent Games</div>
            <a href="Games.aspx" class="btn btn-primary btn-sm">All Games</a>
        </div>
        <div class="table-responsive mt-2">
            <asp:Repeater ID="rptRecentGames" runat="server">
                <HeaderTemplate>
                    <table>
                        <tr>
                            <th>Date</th>
                            <th>Opponent</th>
                            <th>Location</th>
                            <th>Score</th>
                            <th>Result</th>
                            <th></th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("GameDate", "{0:dd MMM yyyy}") %></td>
                        <td><strong><%# Eval("OpponentTeam") %></strong></td>
                        <td class="text-muted"><%# Eval("Location") %></td>
                        <td><%# Eval("ScoreDisplay") %></td>
                        <td><span class="badge badge-<%# Eval("Result").ToString().ToLower() %>"><%# Eval("Result") %></span></td>
                        <td><a href='GameDetail.aspx?id=<%# Eval("Id") %>' class="btn btn-secondary btn-sm">Details</a></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Label ID="lblNoGames" runat="server" Text="No games recorded yet." CssClass="text-muted" Visible="false" />
        </div>
    </div>

    <!-- Top scorers -->
    <div class="card">
        <div class="section-header">
            <div class="card-title" style="margin-bottom:0;border-bottom:none;padding-bottom:0">Top Scorers</div>
            <a href="Players.aspx" class="btn btn-primary btn-sm">All Players</a>
        </div>
        <div class="table-responsive mt-2">
            <asp:Repeater ID="rptTopScorers" runat="server">
                <HeaderTemplate>
                    <table>
                        <tr>
                            <th>#</th>
                            <th>Player</th>
                            <th>Position</th>
                            <th>Games</th>
                            <th>Total Pts</th>
                            <th>Avg Pts</th>
                            <th>Avg Errors</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("JerseyNumber") %></td>
                        <td><strong><%# Eval("Name") %></strong></td>
                        <td class="text-muted"><%# Eval("Position") %></td>
                        <td><%# Eval("GamesPlayed") %></td>
                        <td><%# Eval("TotalPoints") %></td>
                        <td><%# Eval("AvgPoints") %></td>
                        <td><%# Eval("AvgErrors") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Label ID="lblNoPlayers" runat="server" Text="No player stats yet." CssClass="text-muted" Visible="false" />
        </div>
    </div>

</asp:Content>
