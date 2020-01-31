<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PSWorkFlow.aspx.vb" Inherits="PSWorkFlow.PSWorkFlow" %>

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
                        <li class="nav-item"><a class="nav-link" style="color: white; font-weight: bold; border-bottom-style: solid; padding-bottom: 1px" href="PSWorkFlow.aspx">Add</a></li>
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="PSWorkFlowView.aspx">View</a></li>
                       
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="ExportTimesheet.aspx">Timesheet</a></li>
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="Report.aspx">Reports</a></li>
                    </ul>
                    <ul class="navbar-nav navbar-right">
                         <li class="nav-item dropdown" id="showadmin" runat="server">
                            <a href="#" class="nav-link dropdown-toggle" style="color: white; font-weight: bold;" data-toggle="dropdown">Admin</a>
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
                        <li class="nav-item">
                            <asp:LinkButton ID="lnkLogout" runat="server" Text="Logout" CssClass="nav-link text-white font-weight-bold" OnClick="lnkLogout_Click"></asp:LinkButton></li>
                    </ul>
                </div>
            </div>
        </nav>
        <div class="container-fluid">
            <div class="form-row">
                <div class="form-group col-md-12 alert-info" runat="server" id="dvMsgSuccess" align="center">
                    <asp:Label ID="lblMsgSuccess" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md-12 alert-danger" runat="server" id="dvMsg" align="center">
                    <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <br />
            <%--<div class="form-row">
                <div class="form-group col-md-12 alert-info">
                    <asp:Label ID="Label1" runat="server" Font-Bold="true" ForeColor="#007bff" Text="Add"></asp:Label>
                </div>
            </div>--%>
            <div class="form-row">
                <div class="form-group col-md-3">
                </div>
                <div class="form-group col-md-1">
                    Title
                    <asp:Label ID="lblId" runat="server" Font-Bold="true" Text=""></asp:Label>
                    :
                </div>
                <div class="form-group col-md-5">
                    <asp:TextBox ID="txtTitle" runat="server" MaxLength="100" CssClass="form-control" ValidationGroup="a"></asp:TextBox>
                </div>
                <div class="form-group col-md-1">
                    <small style="color: red">*</small>
                </div>

            </div>
            <div class="form-row">
                <div class="form-group col-md-3">
                </div>
                <div class="form-group col-md-1">
                    Project :
                </div>
                <div class="form-group col-md-2">
                    <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="form-group col-md-1">
                    Stage :
                </div>
                <div class="form-group col-md-2">
                    <asp:DropDownList ID="ddlStage" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStage_SelectedIndexChanged"></asp:DropDownList>
                </div>

            </div>
            <div class="form-row">
                <div class="form-group col-md-3">
                </div>
                <div class="form-group col-md-1">
                    Step :
                </div>
                <div class="form-group col-md-2">
                    <asp:DropDownList ID="ddlStep" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStep_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="form-group col-md-1">
                    Iteration :
                </div>
                <div class="form-group col-md-2">
                    <asp:DropDownList ID="ddlIteration" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md-3">
                </div>
                <div class="form-group col-md-1">
                    Status :
                </div>
                <div class="form-group col-md-2">
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
                <div class="form-group col-md-1">
                    Assigned To :
                </div>
                <div class="form-group col-md-2">
                    <asp:DropDownList ID="ddlAssignedTo" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md-3">
                </div>
                <div class="form-group col-md-1">
                    Note:
                </div>
                <div class="form-group col-md-5">
                    <asp:TextBox ID="txtNotes" runat="server" MaxLength="100" CssClass="form-control" Rows="4" TextMode="MultiLine"></asp:TextBox>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md-3">
                </div>
                <div class="form-group col-md-1">
                    Attachment :
                </div>
                <div class="form-group col-md-2">
                    <asp:FileUpload ID="FuFile" runat="server"></asp:FileUpload>
                    <asp:LinkButton ID="lnkFile" runat="server" Font-Size="12px" Text=""></asp:LinkButton>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md-4">
                </div>
                <div class="form-group col-md-2">
                    <asp:Button ID="btnSubmit" runat="server"
                        Text="Submit" CssClass="btn btn-sm btn-primary" OnClick="btnSubmit_Click" />
                    <asp:Button ID="btnCancel" runat="server"
                        Text="Clear" CssClass="btn btn-sm btn-primary" OnClick="btnCancel_Click" />
                </div>
            </div>

            <div class="form-row" runat="server" id="dvHistory">
                <div class="form-group col-md-12">
                    <asp:Label ID="Label2" runat="server" Font-Bold="true" ForeColor="#007bff" Text="History"></asp:Label>
                    <br />
                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto">
                        <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="false" Width="100%" HeaderStyle-BackColor="#d1ecf1"
                            CssClass="table table-sm table-striped table-bordered table-hover" HeaderStyle-ForeColor="#007bff"
                            OnRowCommand="gvHistory_RowCommand">
                            <Columns>
                                <asp:TemplateField HeaderText="NoteId" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNoteId" runat="server" Text='<%#Eval("NoteId") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date/Time" ItemStyle-Width="180px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDt" runat="server" Text='<%#Eval("Dt") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Note" ItemStyle-Width="800px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNote" runat="server" Text='<%#Eval("Note") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Employee">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmployee" runat="server" Text='<%#Eval("Employee") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Documents">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkFile" runat="server" Font-Size="12px" Text='<%# Eval("FileName")%>' CommandName="getFile" CommandArgument='<%# Eval("NoteId")%>'></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
