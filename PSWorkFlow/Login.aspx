<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="PSWorkFlow.Login" %>

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
        <nav class="navbar navbar-expand-sm bg-info navbar-dark">
            <a class="navbar-brand" href="#"><b>Pharma PM</b></a>
            <div class="collapse navbar-collapse" id="navbarText">
                <div class="container-fluid">
                    <ul class="navbar-nav mr-auto">
                    </ul>
                    <ul class="navbar-nav navbar-right">
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="ChangePassword.aspx">Change Password</a></li>
                    </ul>
                </div>
            </div>
        </nav>
        <div class="container-fluid">

            <div class="form-row">
                <div class="form-group col-md-4">
                </div>
                <div class="form-group col-md-4 alert-danger" runat="server" id="dvMsg" align="center">
                    <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md-12 alert-info">
                    <asp:Label ID="Label1" runat="server" Font-Bold="true" ForeColor="#007bff" Text="Login"></asp:Label>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md-4">
                </div>
                <div class="form-group col-md-1">
                    User ID :
                </div>
                <div class="form-group col-md-2">
                    <asp:TextBox ID="txtUserID" runat="server" MaxLength="100" CssClass="form-control" ValidationGroup="a"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtUserID"
                        Display="None" ErrorMessage="*" Text="*" ForeColor="Red" SetFocusOnError="True">
                    </asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md-4">
                </div>
                <div class="form-group col-md-1">
                    Password :
                </div>
                <div class="form-group col-md-2">
                    <asp:TextBox ID="txtPassword" runat="server" MaxLength="100" TextMode="Password" CssClass="form-control" ValidationGroup="a"></asp:TextBox>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group col-md-5">
                </div>
                <div class="form-group col-md-2">
                    <asp:Button ID="btnLogin" runat="server"
                        Text="Login" CssClass="btn btn-sm btn-primary" ValidationGroup="a" OnClick="btnLogin_Click" />
                    <asp:Button ID="btnCancel" runat="server"
                        Text="Cancel" CssClass="btn btn-sm btn-primary" OnClick="btnCancel_Click" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
