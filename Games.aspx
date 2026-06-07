<%@ Page Title="Games" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Games.aspx.cs" Inherits="BasketballScores.GamesPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-header">
        <h1>Games</h1>
        <p>Schedule and manage all games.</p>
    </div>

    <asp:Label ID="lblMessage" runat="server" Visible="false" />

    <!-- Add / Edit Form -->
    <div class="card">
        <div class="card-title"><asp:Literal ID="litFormTitle" runat="server" Text="Add New Game" /></div>
        <div class="form-grid">
            <div class="form-group">
                <label>Game Date</label>
                <asp:TextBox ID="txtDate" runat="server" TextMode="Date" />
                <asp:RequiredFieldValidator ControlToValidate="txtDate" runat="server"
                    ErrorMessage="Date is required." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Opponent Team</label>
                <asp:TextBox ID="txtOpponent" runat="server" placeholder="e.g. City Hawks" MaxLength="100" />
                <asp:RequiredFieldValidator ControlToValidate="txtOpponent" runat="server"
                    ErrorMessage="Opponent is required." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Location</label>
                <asp:TextBox ID="txtLocation" runat="server" placeholder="e.g. Home / Away / Arena name" MaxLength="200" />
            </div>
        </div>
        <div class="form-grid mt-2">
            <div class="form-group">
                <label>Our Score</label>
                <asp:TextBox ID="txtOurScore" runat="server" TextMode="Number" placeholder="Leave blank if not played yet" />
            </div>
            <div class="form-group">
                <label>Opponent Score</label>
                <asp:TextBox ID="txtOpponentScore" runat="server" TextMode="Number" placeholder="Leave blank if not played yet" />
            </div>
            <div class="form-group">
                <label>Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server">
                    <asp:ListItem Value="0">Scheduled</asp:ListItem>
                    <asp:ListItem Value="1">Completed</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="form-grid mt-1">
            <div class="form-group" style="grid-column: 1 / -1">
                <label>Notes</label>
                <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" placeholder="Optional game notes..." />
            </div>
        </div>
        <div class="mt-2 btn-actions">
            <asp:HiddenField ID="hfEditId" runat="server" Value="0" />
            <asp:Button ID="btnSave" runat="server" Text="Save Game" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary"
                OnClick="btnCancel_Click" Visible="false" CausesValidation="false" />
        </div>
    </div>

    <!-- Game List -->
    <div class="card">
        <div class="card-title">All Games</div>
        <div class="table-responsive">
            <asp:Repeater ID="rptGames" runat="server" OnItemCommand="rptGames_ItemCommand">
                <HeaderTemplate>
                    <table>
                        <tr>
                            <th>Date</th>
                            <th>Opponent</th>
                            <th>Location</th>
                            <th>Score</th>
                            <th>Result</th>
                            <th>Actions</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("GameDate", "{0:dd MMM yyyy}") %></td>
                        <td><strong><%# Eval("OpponentTeam") %></strong></td>
                        <td class="text-muted"><%# Eval("Location") %></td>
                        <td><%# Eval("ScoreDisplay") %></td>
                        <td><span class="badge badge-<%# Eval("Result").ToString().ToLower() %>"><%# Eval("Result") %></span></td>
                        <td>
                            <div class="btn-actions">
                                <a href='GameDetail.aspx?id=<%# Eval("Id") %>' class="btn btn-primary btn-sm">Stats</a>
                                <asp:LinkButton CommandName="Edit" CommandArgument='<%# Eval("Id") %>'
                                    runat="server" CssClass="btn btn-secondary btn-sm"
                                    CausesValidation="false">Edit</asp:LinkButton>
                                <asp:LinkButton CommandName="Delete" CommandArgument='<%# Eval("Id") %>'
                                    runat="server" CssClass="btn btn-danger btn-sm"
                                    OnClientClick="return confirm('Delete this game and all its stats?');"
                                    CausesValidation="false">Delete</asp:LinkButton>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Label ID="lblNoGames" runat="server" Text="No games yet. Add your first game above." CssClass="text-muted" Visible="false" />
        </div>
    </div>

</asp:Content>
