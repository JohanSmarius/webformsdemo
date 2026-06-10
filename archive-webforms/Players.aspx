<%@ Page Title="Players" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Players.aspx.cs" Inherits="BasketballScores.PlayersPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-header">
        <h1>Players</h1>
        <p>Manage your team roster.</p>
    </div>

    <asp:Label ID="lblMessage" runat="server" Visible="false" />

    <!-- Add / Edit Form -->
    <div class="card">
        <div class="card-title"><asp:Literal ID="litFormTitle" runat="server" Text="Add New Player" /></div>
        <div class="form-grid">
            <div class="form-group">
                <label for="txtName">Full Name</label>
                <asp:TextBox ID="txtName" runat="server" placeholder="e.g. John Smith" MaxLength="100" />
                <asp:RequiredFieldValidator ControlToValidate="txtName" runat="server"
                    ErrorMessage="Name is required." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label for="txtJersey">Jersey Number</label>
                <asp:TextBox ID="txtJersey" runat="server" placeholder="e.g. 23" TextMode="Number" />
                <asp:RequiredFieldValidator ControlToValidate="txtJersey" runat="server"
                    ErrorMessage="Jersey number is required." ForeColor="Red" Display="Dynamic" />
                <asp:RangeValidator ControlToValidate="txtJersey" runat="server"
                    MinimumValue="0" MaximumValue="99" Type="Integer"
                    ErrorMessage="Must be 0-99." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label for="ddlPosition">Position</label>
                <asp:DropDownList ID="ddlPosition" runat="server">
                    <asp:ListItem Value="">-- Select --</asp:ListItem>
                    <asp:ListItem>Point Guard (PG)</asp:ListItem>
                    <asp:ListItem>Shooting Guard (SG)</asp:ListItem>
                    <asp:ListItem>Small Forward (SF)</asp:ListItem>
                    <asp:ListItem>Power Forward (PF)</asp:ListItem>
                    <asp:ListItem>Center (C)</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="mt-2 btn-actions">
            <asp:HiddenField ID="hfEditId" runat="server" Value="0" />
            <asp:Button ID="btnSave" runat="server" Text="Save Player" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary"
                OnClick="btnCancel_Click" Visible="false" CausesValidation="false" />
        </div>
    </div>

    <!-- Player List -->
    <div class="card">
        <div class="card-title">Roster</div>
        <div class="table-responsive">
            <asp:Repeater ID="rptPlayers" runat="server" OnItemCommand="rptPlayers_ItemCommand">
                <HeaderTemplate>
                    <table>
                        <tr>
                            <th>#</th>
                            <th>Name</th>
                            <th>Position</th>
                            <th>Games</th>
                            <th>Avg Points</th>
                            <th>Avg Errors</th>
                            <th>Actions</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><strong><%# Eval("JerseyNumber") %></strong></td>
                        <td><%# Eval("Name") %></td>
                        <td class="text-muted"><%# Eval("Position") %></td>
                        <td><%# Eval("GamesPlayed") %></td>
                        <td><%# Eval("AvgPoints") %></td>
                        <td><%# Eval("AvgErrors") %></td>
                        <td>
                            <div class="btn-actions">
                                <asp:LinkButton CommandName="Edit" CommandArgument='<%# Eval("PlayerId") %>'
                                    runat="server" CssClass="btn btn-secondary btn-sm"
                                    CausesValidation="false">Edit</asp:LinkButton>
                                <asp:LinkButton CommandName="Delete" CommandArgument='<%# Eval("PlayerId") %>'
                                    runat="server" CssClass="btn btn-danger btn-sm"
                                    OnClientClick="return confirm('Remove this player?');"
                                    CausesValidation="false">Remove</asp:LinkButton>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Label ID="lblNoPlayers" runat="server" Text="No players yet. Add your first player above." CssClass="text-muted" Visible="false" />
        </div>
    </div>

</asp:Content>
