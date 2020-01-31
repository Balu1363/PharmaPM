<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Report.aspx.vb" Inherits="PSWorkFlow.Report" %>

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
                        <li class="nav-item"><a class="nav-link" style="color: white; font-weight: bold; border-bottom-style: solid; padding-bottom: 1px" href="Report.aspx">Reports</a></li>
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
                         <li class="nav-item"><asp:LinkButton ID="lnkLogout" runat="server" Text="Logout" CssClass="nav-link text-white font-weight-bold" OnClick="lnkLogout_Click"></asp:LinkButton></li>
                    </ul>
                </div>
            </div>
        </nav>
        <div class="container-fluid">
            <div class="form-row">
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
                            <asp:Label ID="Label1" runat="server" Font-Bold="true" ForeColor="#007bff" Text="Reports"></asp:Label>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-4">
                        </div>
                        <div class="form-group col-md-1">
                            Project Name:
                        </div>
                        <div class="form-group col-md-2">
                             <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div class="form-group col-md-1">
                            <asp:RequiredFieldValidator ID="reqProjects" runat="server" SetFocusOnError="true" Text="*" ForeColor="Red" ControlToValidate="ddlProject" InitialValue="--Select--" ErrorMessage="*" ValidationGroup="a"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    
                    
                    <div class="form-row">
                        <div class="form-group col-md-2">
                        </div>
                        <div class="form-group col-md-8">
                            <asp:Label ID="lblName" runat="server" Text="" Font-Bold="true" ForeColor="DodgerBlue"></asp:Label>
                            <asp:Image ID="imgGanttChart" runat="server" ImageUrl="~/Images/ganttchart.png" Height="450px" Width="800px" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>

</html>
