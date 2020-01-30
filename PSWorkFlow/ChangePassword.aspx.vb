Imports MySql.Data.MySqlClient

Public Class ChangePassword
    Inherits System.Web.UI.Page
    Dim dt As DataTable = New DataTable()
    Dim cmd As MySqlCommand = New MySqlCommand()
    Dim con As MySqlConnection = New MySqlConnection()
    Dim conString As String = ConfigurationManager.ConnectionStrings("dbcNoeth").ConnectionString

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            ClearMsg()
        End If
    End Sub

    Private Sub ClearMsg()
        dvMsgSuccess.Visible = False
        lblMsgSuccess.Visible = False
        dvMsg.Visible = False
        lblMsg.Visible = False
    End Sub

    Protected Sub btnChangePassword_Click(sender As Object, e As EventArgs)
        Try
            ClearMsg()
            If ValidateUser() Then
                Dim sb As StringBuilder = New StringBuilder()
                sb.Append(" Update Employees Set EmpPass=@EmpPass Where EmpID=@EmpID")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sb.ToString(), con)
                cmd.Parameters.AddWithValue("@EmpPass", txtNewPassword.Text.Trim())
                cmd.Parameters.AddWithValue("@EmpID", ViewState("EmpID"))
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsg.Visible = False
                    lblMsg.Visible = False
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Password Changed successfully."
                End If
            Else
                lblMsg.Visible = True
                dvMsg.Visible = True
                lblMsg.Text = "Check User ID and/or Old Password"
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Function ValidateUser() As Boolean
        Dim Query As String
        Dim UserNm As String = txtUserID.Text
        Dim Pass As String = txtOldPassword.Text
        Query = "Select EmpId,EmpLogin,EmpPass,Role,EmpFirst as EmpName From employees Where EmpLogin=@EmpLogin And EmpPass=@EmpPass and status=true"
        con = New MySqlConnection(conString)
        cmd = New MySqlCommand(Query, con)
        cmd.Parameters.AddWithValue("@EmpLogin", txtUserID.Text.Trim())
        cmd.Parameters.AddWithValue("@EmpPass", txtOldPassword.Text.Trim())
        dt = New DataTable()
        dt.Columns.Clear()
        dt.Clear()
        con.Open()
        dt.Load(cmd.ExecuteReader())
        con.Close()

        If dt.Rows.Count > 0 Then

            If UserNm = Convert.ToString(dt.Rows(0)("EmpLogin")) AndAlso Pass = Convert.ToString(dt.Rows(0)("EmpPass")) Then
                ViewState("EmpID") = Convert.ToString(dt.Rows(0)("EmpId"))
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function
End Class