<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ExportTimesheet.aspx.vb" Inherits="PSWorkFlow.ExportTimesheet" %>

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
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="PSWorkFlow.aspx" runat="server" id="showadd">Add</a></li>
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="PSWorkFlowView.aspx">View</a></li>
                       
                         <li class="nav-item dropdown" runat="server">
                            <a href="#" class="nav-link dropdown-toggle" style="color: white; font-weight: bold; border-bottom-style: solid; padding-bottom: 1px" data-toggle="dropdown">Reports</a>
                            <div class="dropdown-menu">
                                <a href="Report.aspx" class="dropdown-item">Gantt Charts</a>
                                <a href="ExportTimesheet.aspx" class="dropdown-item">Timesheet</a>     
                            </div>
                        </li>
                    </ul>
                    <ul class="navbar-nav navbar-right">
                         <li class="nav-item dropdown" runat="server" id="showadmin">
                            <a href="#" class="nav-link dropdown-toggle text-white font-weight-bold" data-toggle="dropdown">Admin</a>
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
                <%--<div class="form-group col-md-12 alert-info" runat="server" id="dvMsgSuccess" align="center">
                    <asp:Label ID="lblMsgSuccess" runat="server" Text=""></asp:Label>
                </div>--%>
                <div class="form-group col-md-12 alert-danger" runat="server" id="dvMsg" align="center">
                    <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <div style="height: 5px"></div>
            <div class="form-row">
                <div class="form-group col-md-12">
                    <div class="form-row">
                        <div class="form-group col-md-3">
                        </div>
                        <div class="form-group col-md-5 alert-info">
                            <asp:Label ID="Label1" runat="server" Font-Bold="true" ForeColor="#007bff" Text="Timesheet"></asp:Label>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-4">
                        </div>
                        <div class="form-group col-md-1">
                            Date:
                        </div>
                        <div class="form-group col-md-3">
                            <asp:DropDownList ID="ddlweeks" runat="server" CssClass="form-control" Width="250px" AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-row" id="showemp" runat="server">
                        <div class="form-group col-md-4">
                        </div>
                        <div class="form-group col-md-1">
                            Employee :
                        </div>
                        <div class="form-group col-md-3">
                            <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-control" Width="250px" AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                    </div>


                    <div class="form-row">
                        <div class="form-group col-md-5">
                        </div>
                        <div class="form-group col-md-3">
                            <asp:Button ID="btnExport" runat="server"
                                Text="Export" CssClass="btn btn-sm btn-primary" ValidationGroup="a" OnClick="btnExport_Click" />
                            <asp:Button ID="btnCancel" runat="server"
                                Text="Show" CssClass="btn  btn-sm btn-primary" OnClick="btnCancel_Click" />
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-md-12">
                            <asp:Panel ID="PnlGrid" runat="server" ScrollBars="Auto">
                                <asp:GridView ID="gvTask" runat="server" AutoGenerateColumns="false" HeaderStyle-BackColor="#d1ecf1"
                                    CssClass="table table-sm table-striped table-bordered table-hover" HeaderStyle-ForeColor="#007bff"
                                    OnRowDataBound="gvTask_RowDataBound">
                                    <Columns>
                                        <asp:BoundField HeaderText="Day" DataField="Day" HtmlEncode="False" ItemStyle-Width="100px" />
                                        <asp:BoundField HeaderText="Date" DataField="dt" HtmlEncode="False" ItemStyle-Width="100px"/>
                                        <asp:BoundField HeaderText="Project" DataField="Project" HtmlEncode="False" ItemStyle-Width="200px"/>
                                        <asp:BoundField HeaderText="Sub Project" DataField="SubProject" HtmlEncode="False" ItemStyle-Width="100px"/>
                                        <asp:BoundField HeaderText="Task Type" DataField="TaskType" HtmlEncode="False" ItemStyle-Width="150px"/>
                                        <asp:BoundField HeaderText="Project Task" DataField="ProjectTask" HtmlEncode="False" />
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
