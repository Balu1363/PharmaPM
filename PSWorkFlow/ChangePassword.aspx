<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ChangePassword.aspx.vb" Inherits="PSWorkFlow.ChangePassword" %>

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
                        <li class="nav-item"><a class="nav-link text-white font-weight-bold" href="Login.aspx">Login</a></li>
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
            <div style="height: 5px"></div>
            <div class="form-row">
                <div class="form-group col-md-12">
                    <div class="form-row">
                        <div class="form-group col-md-4">
                        </div>
                        <div class="form-group col-md-4 alert-info">
                            <asp:Label ID="Label1" runat="server" Font-Bold="true" ForeColor="#007bff" Text="Change Password"></asp:Label>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-4">
                        </div>
                        <div class="form-group col-md-2">
                            User ID:
                        </div>
                        <div class="form-group col-md-2">
                            <asp:TextBox ID="txtUserID" runat="server" MaxLength="100" CssClass="form-control" ValidationGroup="a"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtUserID"
                                Display="None" ErrorMessage="*" Text="*" ForeColor="Red" SetFocusOnError="True" ValidationGroup="a">
                            </asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-4">
                        </div>
                        <div class="form-group col-md-2">
                            Old Password:
                        </div>
                        <div class="form-group col-md-2">
                            <asp:TextBox ID="txtOldPassword" runat="server" MaxLength="100" TextMode="Password" CssClass="form-control" ValidationGroup="a"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtOldPassword" ValidationGroup="a"
                                Display="None" ErrorMessage="*" Text="*" ForeColor="Red" SetFocusOnError="True">
                            </asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-4">
                        </div>
                        <div class="form-group col-md-2">
                            New Password:
                        </div>
                        <div class="form-group col-md-2">
                            <asp:TextBox ID="txtNewPassword" runat="server" MaxLength="100" TextMode="Password" CssClass="form-control" ValidationGroup="a"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtNewPassword" ValidationGroup="a"
                                Display="None" ErrorMessage="*" Text="*" ForeColor="Red" SetFocusOnError="True">
                            </asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-4">
                        </div>
                        <div class="form-group col-md-2">
                            Confirm New Password :
                        </div>
                        <div class="form-group col-md-2">
                            <asp:TextBox ID="ConfirmPassword" runat="server" MaxLength="100" TextMode="Password" CssClass="form-control" ValidationGroup="a"></asp:TextBox>
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ValidationGroup="a"
                                ControlToCompare="txtNewPassword" ControlToValidate="ConfirmPassword" ForeColor="Red"
                                ErrorMessage="Password not same !!"></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-6">
                        </div>
                        <div class="form-group col-md-2">
                            <asp:Button ID="btnChangePassword" runat="server"
                                Text="Change Password" CssClass="btn btn-sm btn-primary" ValidationGroup="a" OnClick="btnChangePassword_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>

</html>
