Imports MySql.Data.MySqlClient
Imports System.IO
Public Class Login
    Inherits System.Web.UI.Page
    Dim dt As DataTable = New DataTable()
    Dim cmd As MySqlCommand = New MySqlCommand()
    Dim con As MySqlConnection = New MySqlConnection()
    Dim conString As String = ConfigurationManager.ConnectionStrings("dbcNoeth").ConnectionString
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            txtUserID.Text = ""
            txtPassword.Text = ""
            dvMsg.Visible = False
        End If
    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs)

        Dim Query As String
        Dim UserNm As String = txtUserID.Text
        Dim Pass As String = txtPassword.Text
        Query = "Select EmpId,EmpLogin,EmpPass,Role,EmpFirst as EmpName From employees Where EmpLogin=@EmpLogin And EmpPass=@EmpPass and status=true"
        con = New MySqlConnection(conString)
        cmd = New MySqlCommand(Query, con)
        cmd.Parameters.AddWithValue("@EmpLogin", txtUserID.Text.Trim())
        cmd.Parameters.AddWithValue("@EmpPass", txtPassword.Text.Trim())
        dt = New DataTable()
        dt.Columns.Clear()
        dt.Clear()
        con.Open()
        dt.Load(cmd.ExecuteReader())
        con.Close()

        If dt.Rows.Count > 0 Then
            If UserNm = Convert.ToString(dt.Rows(0)("EmpLogin")) AndAlso Pass = Convert.ToString(dt.Rows(0)("EmpPass")) Then
                Session("EmpId") = Convert.ToString(dt.Rows(0)("EmpId"))
                Session("EmpName") = Convert.ToString(dt.Rows(0)("EmpName"))
                Session("Role") = Convert.ToString(dt.Rows(0)("Role"))
                FormsAuthentication.RedirectFromLoginPage(txtUserID.Text, False)
                Response.Redirect("~/PSWorkFlowView.aspx")
            Else
                dvMsg.Visible = True
                lblMsg.Text = "Check User ID and/or Password"
            End If
        Else
            dvMsg.Visible = True
            lblMsg.Text = "Check User ID and/or Password"
        End If

    End Sub


    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        txtUserID.Text = ""
        txtPassword.Text = ""
    End Sub
End Class