<%@ Page Title="Game Detail" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameDetail.aspx.cs" Inherits="BasketballScores.GameDetailPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-header">
        <h1><asp:Literal ID="litTitle" runat="server" /></h1>
        <p><asp:Literal ID="litSubtitle" runat="server" /></p>
    </div>

    <asp:Label ID="lblMessage" runat="server" Visible="false" />

    <!-- Game Info Card -->
    <div class="card">
        <div class="section-header">
            <div class="card-title" style="margin-bottom:0;border-bottom:none;padding-bottom:0">Game Info</div>
            <a href="Games.aspx" class="btn btn-secondary btn-sm">&larr; Back to Games</a>
        </div>
        <div class="form-grid mt-2">
            <div class="form-group">
                <label>Game Date</label>
                <asp:TextBox ID="txtDate" runat="server" TextMode="Date" />
            </div>
            <div class="form-group">
                <label>Opponent</label>
                <asp:TextBox ID="txtOpponent" runat="server" MaxLength="100" />
            </div>
            <div class="form-group">
                <label>Location</label>
                <asp:TextBox ID="txtLocation" runat="server" MaxLength="200" />
            </div>
            <div class="form-group">
                <label>Our Score</label>
                <asp:TextBox ID="txtOurScore" runat="server" TextMode="Number" />
            </div>
            <div class="form-group">
                <label>Opponent Score</label>
                <asp:TextBox ID="txtOpponentScore" runat="server" TextMode="Number" />
            </div>
            <div class="form-group">
                <label>Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server">
                    <asp:ListItem Value="0">Scheduled</asp:ListItem>
                    <asp:ListItem Value="1">Completed</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="form-group mt-1">
            <label>Notes</label>
            <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" />
        </div>
        <div class="mt-2">
            <asp:HiddenField ID="hfGameId" runat="server" />
            <asp:Button ID="btnUpdateGame" runat="server" Text="Update Game" CssClass="btn btn-primary" OnClick="btnUpdateGame_Click" />
        </div>
    </div>

    <!-- Player Stats Card -->
    <div class="card">
        <div class="card-title">Player Statistics</div>
        <p class="text-muted" style="margin-bottom:1rem">Enter stats for each player. Points, errors (fouls/turnovers), assists, rebounds, and minutes played.</p>

        <asp:Panel ID="pnlNoPlayers" runat="server" Visible="false">
            <p class="text-muted">No active players found. <a href="Players.aspx">Add players</a> first.</p>
        </asp:Panel>

        <asp:Panel ID="pnlStats" runat="server">
            <div class="table-responsive">
                <table class="stats-table">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Player</th>
                            <th>Position</th>
                            <th title="Points scored">Points</th>
                            <th title="Errors / turnovers / fouls">Errors</th>
                            <th title="Assists">Assists</th>
                            <th title="Rebounds">Rebounds</th>
                            <th title="Minutes played">Min</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptStats" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><strong><%# Eval("JerseyNumber") %></strong></td>
                                    <td><%# Eval("PlayerName") %></td>
                                    <td class="text-muted"><%# Eval("Position") %></td>
                                    <td>
                                        <asp:HiddenField ID="hfPlayerId" runat="server" Value='<%# Eval("PlayerId") %>' />
                                        <asp:TextBox ID="txtPoints" runat="server" TextMode="Number"
                                            Text='<%# Eval("Points") %>' CssClass="stat-input" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtErrors" runat="server" TextMode="Number"
                                            Text='<%# Eval("Errors") %>' CssClass="stat-input" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAssists" runat="server" TextMode="Number"
                                            Text='<%# Eval("Assists") %>' CssClass="stat-input" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtRebounds" runat="server" TextMode="Number"
                                            Text='<%# Eval("Rebounds") %>' CssClass="stat-input" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMinutes" runat="server" TextMode="Number"
                                            Text='<%# Eval("MinutesPlayed") %>' CssClass="stat-input" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
            <div class="mt-2">
                <asp:Button ID="btnSaveStats" runat="server" Text="Save All Stats" CssClass="btn btn-success" OnClick="btnSaveStats_Click" />
            </div>
        </asp:Panel>
    </div>

</asp:Content>
