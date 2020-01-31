<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PSWorkFlowView.aspx.vb" Inherits="PSWorkFlow.PSWorkFlowView" %>

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
        <nav class="navbar navbar-expand-sm navbar-dark bg-info">
            <a class="navbar-brand" href="#"><b>Pharma PM</b></a>
            <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarText" aria-controls="navbarText" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarText">
                <div class="container-fluid">
                    <ul class="navbar-nav mr-auto">
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="PSWorkFlow.aspx">Add</a></li>
                        <li class="nav-item"><a class="nav-link" style="color: white; font-weight: bold; border-bottom-style: solid; padding-bottom: 1px" href="PSWorkFlowView.aspx">View</a></li>
                        <%--<li class="nav-item"><a class="nav-link text-white font-weight-bold" href="MstProjects.aspx">Admin</a></li>--%>
                       
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="ExportTimesheet.aspx">Timesheet</a></li>
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="Report.aspx">Reports</a></li>
                    </ul>
                    <ul class="navbar-nav navbar-right">
                         <li class="nav-item dropdown" runat="server" id="showadmin">
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
            <div class="form-row" runat="server" id="dvMsgSuccess" align="center">
                <div class="form-group col-md-12 alert-info">
                    <asp:Label ID="lblMsgSuccess" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <div class="form-row" runat="server" id="dvMsg" align="center">
                <div class="form-group col-lg-12 alert-danger">
                    <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <br />
            <div class="form-row">
                <div class="form-group col-md-3">
                </div>
                <div class="form-group col-md-1">
                    Search :
                </div>
                <div class="form-group col-md-5">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search By - Id / Title / Project / Stage / Step / Iteration / Status / Assigned To"></asp:TextBox>
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
                <div class="form-group col-md-4">
                </div>
                <div class="form-group col-md-2" align="right">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-sm btn-primary" OnClick="btnSearch_Click" Text="Search" />
                    <asp:Button ID="btnClear" runat="server" CssClass="btn btn-sm btn-primary" OnClick="btnClear_Click" Text="Clear" />
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md-12">
                    <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto">
                        <asp:GridView ID="GvPartSales" runat="server" AllowPaging="True" PageSize="100" Width="100%"
                            AutoGenerateColumns="false" CssClass="table table-sm table-striped table-bordered table-hover" HeaderStyle-BackColor="#d1ecf1"
                            OnRowCommand="GvPartSales_RowCommand" OnPageIndexChanging="GvPartSales_PageIndexChanging"
                            OnRowEditing="GvPartSales_RowEditing" AllowSorting="True" OnSorting="GvPartSales_Sorting">
                            <Columns>
                                <asp:TemplateField HeaderText="Id" SortExpression="WorkFlowId" ItemStyle-Width="10px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblId" Visible="false" runat="server" Text='<%#Eval("WorkFlowId") %>'></asp:Label>
                                        <asp:LinkButton ID="btneditid" runat="server" Text='<%#Eval("WorkFlowId") %>' CommandName="EditId" CausesValidation="false"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Title" SortExpression="Title">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTitle" Visible="false" runat="server" Text='<%#Eval("Title") %>'></asp:Label>
                                        <asp:LinkButton ID="btnedit" runat="server" Text='<%#Eval("Title") %>' CommandName="Edit" CausesValidation="false"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Project" SortExpression="ProjName" ItemStyle-Width="180px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProjName" runat="server" Text='<%#Eval("ProjName") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Stage" SortExpression="Stage" ItemStyle-Width="180px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStage" runat="server" Text='<%#Eval("Stage") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Step" SortExpression="Step" ItemStyle-Width="180px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStep" runat="server" Text='<%#Eval("Step") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Iteration" SortExpression="Iteration" ItemStyle-Width="180px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIteration" runat="server" Text='<%#Eval("Iteration") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Assigned To" SortExpression="AssignedToUser" ItemStyle-Width="180px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAssignedTo" runat="server" Text='<%#Eval("AssignedToUser") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status" SortExpression="Status" ItemStyle-Width="150px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStatus" runat="server" Text='<%#Eval("Status") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Updated" SortExpression="Updated" ItemStyle-Width="180px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblUpdated" runat="server" Text='<%#Eval("Updated") %>'></asp:Label>
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
