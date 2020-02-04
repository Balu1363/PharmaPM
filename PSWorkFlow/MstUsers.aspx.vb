Imports MySql.Data.MySqlClient
Imports System.IO
Public Class MstUsers
    Inherits System.Web.UI.Page
    Dim dt As DataTable = New DataTable()
    Dim cmd As MySqlCommand = New MySqlCommand()
    Dim con As MySqlConnection = New MySqlConnection()
    Dim conString As String = ConfigurationManager.ConnectionStrings("dbcNoeth").ConnectionString
    Dim UserId As Integer = 0
    Dim Role As String = ""


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("EmpId") Is Nothing OrElse Session("EmpId") = "" Then
            Response.Redirect("Login.aspx")
        Else
            UserId = Session("EmpId")
            Role = Session("Role").ToString().Trim()
            lblEmpName.Text = "User : " & Session("EmpName").ToString()
            If Not IsPostBack Then
                If Role = "Admin" Then
                    showadd.Visible = True
                Else
                    showadd.Visible = False
                End If
                dvMsgSuccess.Visible = False
                lblMsgSuccess.Visible = False
                dvMsg.Visible = False
                lblMsg.Visible = False
                BindUsers()
            End If
        End If
    End Sub

    Private Sub BindUsers()
        Try
            Dim sb As StringBuilder = New StringBuilder()
            sb.Append("Select * From Employees Where Status=@Status")
            sb.Append(" Order By EmpFirst;")
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(sb.ToString(), con)
            cmd.Parameters.AddWithValue("@Status", True)
            Dim dth As DataTable = New DataTable()
            dth.Columns.Clear()
            dth.Clear()
            con.Open()
            dth.Load(cmd.ExecuteReader())
            con.Close()
            If dth.Rows.Count > 0 Then
                gvUsers.DataSource = dth
                gvUsers.DataBind()
            End If
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Private Sub CatchMsg()
        dvMsgSuccess.Visible = False
        lblMsgSuccess.Visible = False
        dvMsg.Visible = True
        lblMsg.Visible = True
    End Sub

    Protected Sub gvUsers_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Try
            If e.CommandName = "Edit" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblEmpID As Label = CType(gvUsers.Rows(RowIndex).FindControl("lblEmpID"), Label)
                Dim lblEmpFirst As Label = CType(gvUsers.Rows(RowIndex).FindControl("lblEmpFirst"), Label)
                Dim lblEmpLast As Label = CType(gvUsers.Rows(RowIndex).FindControl("lblEmpLast"), Label)
                Dim lblEmpEmail As Label = CType(gvUsers.Rows(RowIndex).FindControl("lblEmpEmail"), Label)
                Dim lblEmpLogin As Label = CType(gvUsers.Rows(RowIndex).FindControl("lblEmpLogin"), Label)
                Dim lblRole As Label = CType(gvUsers.Rows(RowIndex).FindControl("lblRole"), Label)
                ViewState("EmpID") = lblEmpID.Text
                txtFirstName.Text = Convert.ToString(lblEmpFirst.Text)
                txtLastName.Text = Convert.ToString(lblEmpLast.Text)
                txtEmail.Text = Convert.ToString(lblEmpEmail.Text)
                txtUserID.Text = Convert.ToString(lblEmpLogin.Text)
                If Convert.ToString(lblRole.Text.Trim() = "Admin") Then
                    ddlRole.SelectedValue = 1
                Else
                    ddlRole.SelectedValue = 2
                End If
                btnSave.Text = "Update"
            ElseIf e.CommandName = "Delete" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblEmpID As Label = CType(gvUsers.Rows(RowIndex).FindControl("lblEmpID"), Label)
                Dim lblEmpFirst As Label = CType(gvUsers.Rows(RowIndex).FindControl("lblEmpFirst"), Label)
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand("Update Employees Set Status=@Status Where EmpID=@EmpID", con)
                cmd.Parameters.AddWithValue("@Status", False)
                cmd.Parameters.AddWithValue("@EmpID", lblEmpID.Text)
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "User '" & lblEmpFirst.Text & "' Deleted successfully."
                    ViewState("EmpID") = Nothing
                    BindUsers()
                    btnSave.Text = "Save"
                End If
            End If
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Protected Sub gvUsers_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)

    End Sub

    Protected Sub gvUsers_RowEditing(sender As Object, e As GridViewEditEventArgs)

    End Sub

    Protected Sub gvUsers_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Try
            dvMsgSuccess.Visible = False
            lblMsgSuccess.Visible = False
            dvMsg.Visible = False
            lblMsg.Visible = False
            If txtFirstName.Text.Length = 0 Then
                Throw New Exception("Please enter First Name")
            End If
            If txtUserID.Text.Length = 0 Then
                Throw New Exception("Please enter User ID")
            End If

            If btnSave.Text = "Save" Then
                If txtPassword.Text.Length = 0 Then
                    Throw New Exception("Please enter Password")
                End If
                Dim sbInsert As StringBuilder = New StringBuilder()
                sbInsert.Append(" Insert Into Employees (EmpFirst,EmpLast,EmpEmail,EmpLogin,EmpPass,Status,Role) ")
                sbInsert.Append(" Values (@EmpFirst,@EmpLast,@EmpEmail,@EmpLogin,@EmpPass,@Status,@Role)")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sbInsert.ToString(), con)
                cmd.Parameters.AddWithValue("@EmpFirst", txtFirstName.Text.Trim())
                cmd.Parameters.AddWithValue("@EmpLast", txtLastName.Text.Trim())
                cmd.Parameters.AddWithValue("@EmpEmail", txtEmail.Text.Trim())
                cmd.Parameters.AddWithValue("@EmpLogin", txtUserID.Text.Trim())
                cmd.Parameters.AddWithValue("@EmpPass", txtPassword.Text.Trim())
                cmd.Parameters.AddWithValue("@Status", True)
                cmd.Parameters.AddWithValue("@Role", ddlRole.SelectedItem.Text.Trim())
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
                BindUsers()
                dvMsg.Visible = False
                lblMsg.Visible = False
                dvMsgSuccess.Visible = True
                lblMsgSuccess.Visible = True
                lblMsgSuccess.Text = "User saved successfully."
            ElseIf btnSave.Text = "Update" Then
                Dim sb As StringBuilder = New StringBuilder()
                sb.Append(" Update Employees Set EmpFirst=@EmpFirst,EmpLast=@EmpLast,EmpEmail=@EmpEmail,EmpLogin=@EmpLogin,EmpPass=@EmpPass,Role=@Role Where EmpID=@EmpID")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sb.ToString(), con)
                cmd.Parameters.AddWithValue("@EmpFirst", txtFirstName.Text.Trim())
                cmd.Parameters.AddWithValue("@EmpLast", txtLastName.Text.Trim())
                cmd.Parameters.AddWithValue("@EmpEmail", txtEmail.Text.Trim())
                cmd.Parameters.AddWithValue("@EmpLogin", txtUserID.Text.Trim())
                cmd.Parameters.AddWithValue("@EmpPass", txtPassword.Text.Trim())
                cmd.Parameters.AddWithValue("@Role", ddlRole.SelectedItem.Text.Trim())
                cmd.Parameters.AddWithValue("@EmpID", ViewState("EmpID"))
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsg.Visible = False
                    lblMsg.Visible = False
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "User Updated successfully."
                    BindUsers()
                End If
            End If
            Clear()
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Private Sub Clear()
        txtFirstName.Text = String.Empty
        txtLastName.Text = String.Empty
        txtEmail.Text = String.Empty
        txtUserID.Text = String.Empty
        txtPassword.Text = String.Empty
        ddlRole.SelectedIndex = 0
        btnSave.Text = "Save"
    End Sub
    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Clear()
        dvMsgSuccess.Visible = False
        lblMsgSuccess.Visible = False
        dvMsg.Visible = False
        lblMsg.Visible = False
    End Sub

    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        Session.Abandon()
        Session("EmpId") = Nothing
        Response.Redirect("~/Login.aspx")
    End Sub
End Class