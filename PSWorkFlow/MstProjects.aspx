<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="MstProjects.aspx.vb" Inherits="PSWorkFlow.MstProjects" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pharma PM</title>
    <meta name="viewport" content="width=device-width" />
    <script src="Scripts/jquery-3.4.1.min.js"></script>
    <script src="Scripts/umd/popper.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <link href="Content/bootstrap.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <nav class="navbar navbar-expand-lg navbar-dark bg-info">
            <a class="navbar-brand" href="#"><b>Pharma PM</b></a>
            <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarText" aria-controls="navbarText" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarText">
                <div class="container-fluid">
                    <ul class="navbar-nav mr-auto">
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="PSWorkFlow.aspx">Add</a></li>
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="PSWorkFlowView.aspx">View</a></li>
                       
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="ExportTimesheet.aspx">Timesheet</a></li>
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="Report.aspx">Reports</a></li>
                    </ul>
                    <ul class="navbar-nav navbar-right">
                         <li class="nav-item dropdown">
                            <a href="#" class="nav-link dropdown-toggle" style="color: white; font-weight: bold; border-bottom-style: solid; padding-bottom: 1px" data-toggle="dropdown">Admin</a>
                            <div class="dropdown-menu">
                                <a href="MstProjects.aspx" class="dropdown-item">Projects</a>
                                <a href="MstStage.aspx" class="dropdown-item">Stage</a>
                                <a href="MstSteps.aspx" class="dropdown-item">Steps</a>
                                <a href="MstIteration.aspx" class="dropdown-item">Iteration</a>
                                <a href="MstStatus.aspx" class="dropdown-item">Status</a>
                                <a href="MstUsers.aspx" class="dropdown-item">Users</a>
                            </div>
                        </li>
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold">
                            <asp:Label ID="lblEmpName" runat="server" Text="" ForeColor="White" Font-Bold="true"></asp:Label></a></li>
                        <%--<li class="nav-item"><a class="nav-link text-white font-weight-bold" href="Login.aspx">Logout</a></li>--%>
                         <li class="nav-item"><asp:LinkButton ID="lnkLogout" runat="server" Text="Logout" CssClass="nav-link text-white font-weight-bold" OnClick="lnkLogout_Click"></asp:LinkButton></li>
                    </ul>
                </div>
            </div>
        </nav>
        <div class="container-fluid">
            <div class="form-row">
                <div class="form-group col-md-12 alert-info" runat="server" id="dvMsgSuccess" align="center">
                    <asp:Label ID="lblMsgSuccess" runat="server" Text=""></asp:Label>
                </div>
                <div class="form-group col-md-12 alert-danger" runat="server" id="dvMsg" align="center">
                    <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <div style="height:5px"></div>
            <div class="form-row">
                <div class="form-group col-md-12">
                    <div class="form-row">
                        <div class="form-group col-md-3">
                        </div>
                        <div class="form-group col-md-5 alert-info">
                            <asp:Label ID="Label1" runat="server" Font-Bold="true" ForeColor="#007bff" Text="Projects"></asp:Label>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-4">
                        </div>
                        <div class="form-group col-md-1">
                            Project Name:
                        </div>
                        <div class="form-group col-md-2">
                            <asp:TextBox ID="txtProject" runat="server" MaxLength="100" CssClass="form-control" ValidationGroup="a"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-1">
                            <asp:RequiredFieldValidator ID="reqProjects" runat="server" SetFocusOnError="true" Text="*" ForeColor="Red" ControlToValidate="txtProject" ErrorMessage="*" ValidationGroup="a"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-4">
                        </div>
                        <div class="form-group col-md-1">
                            Order:
                        </div>
                        <div class="form-group col-md-2">
                            <asp:TextBox ID="txtOrder" runat="server" MaxLength="10" TextMode="Number" CssClass="form-control" ValidationGroup="a"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-1">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" SetFocusOnError="true" Text="*" ForeColor="Red" ControlToValidate="txtOrder" ErrorMessage="*" ValidationGroup="a"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-5">
                        </div>
                        <div class="form-group col-md-3">
                            <asp:Button ID="btnSave" runat="server"
                                Text="Save" CssClass="btn btn-sm btn-primary" ValidationGroup="a" OnClick="btnSave_Click" />
                            <asp:Button ID="btnCancel" runat="server"
                                Text="Clear" CssClass="btn  btn-sm btn-primary" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-3">
                        </div>
                        <div class="col-md-5">
                            <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto">
                                <asp:GridView ID="gvProjects" runat="server" AutoGenerateColumns="false" Width="100%" HeaderStyle-BackColor="#d1ecf1"
                                    CssClass="table table-sm table-striped table-bordered table-hover" HeaderStyle-ForeColor="#007bff" OnRowCommand="gvProjects_RowCommand" OnRowUpdating="gvProjects_RowUpdating"
                                    OnRowDeleting="gvProjects_RowDeleting" OnRowEditing="gvProjects_RowEditing">
                                    <Columns>
                                        <asp:TemplateField ItemStyle-Width="15px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnedit" runat="server" CommandName="Edit" CausesValidation="false"><img src="../Images/edit.jpg" height="15px" width="15px"/></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-Width="15px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btndelete" runat="server" CommandName="Delete" CausesValidation="false" OnClientClick="return confirm('Are you sure you want to delete this project?')"><img src="../Images/delete.png" height="15px" width="15px"/></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ProjID" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblProjID" runat="server" Text='<%#Eval("ProjID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Project Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblProjName" runat="server" Text='<%#Eval("ProjName") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Order" ItemStyle-Width="25px">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOrd" runat="server" Text='<%#Eval("Ord") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
