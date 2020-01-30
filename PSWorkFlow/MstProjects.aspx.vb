
Imports MySql.Data.MySqlClient
Imports System.IO
Public Class MstProjects
    Inherits System.Web.UI.Page
    Dim dt As DataTable = New DataTable()
    Dim cmd As MySqlCommand = New MySqlCommand()
    Dim con As MySqlConnection = New MySqlConnection()
    Dim conString As String = ConfigurationManager.ConnectionStrings("dbcNoeth").ConnectionString
    Dim UserId As Integer = 0
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("EmpId") Is Nothing OrElse Session("EmpId") = "" Then
            Response.Redirect("Login.aspx")
        Else
            UserId = Session("EmpId")
            lblEmpName.Text = "User : " & Session("EmpName").ToString()
            If Not IsPostBack Then
                dvMsgSuccess.Visible = False
                lblMsgSuccess.Visible = False
                dvMsg.Visible = False
                lblMsg.Visible = False
                BindProjects()
            End If
        End If
    End Sub

    Private Sub BindProjects()
        Try
            Dim sb As StringBuilder = New StringBuilder()
            sb.Append("Select ProjID,ProjName,Ord from Projects Where Status=@Status")
            sb.Append(" Order By Ord;")
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
                gvProjects.DataSource = dth
                gvProjects.DataBind()
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


    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        txtProject.Text = String.Empty
        txtOrder.Text = String.Empty
        dvMsgSuccess.Visible = False
        lblMsgSuccess.Visible = False
        dvMsg.Visible = False
        lblMsg.Visible = False
        btnSave.Text = "Save"
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Try
            dvMsgSuccess.Visible = False
            lblMsgSuccess.Visible = False
            dvMsg.Visible = False
            lblMsg.Visible = False
            If txtProject.Text.Length = 0 Then
                Throw New Exception("Please enter Project to proceed.")
            End If
            If btnSave.Text = "Save" Then
                Dim sbInsert As StringBuilder = New StringBuilder()
                sbInsert.Append(" Insert Into Projects (ProjName, Status,Ord) ")
                sbInsert.Append(" Values (@ProjName,@Status,@Ord)")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sbInsert.ToString(), con)
                cmd.Parameters.AddWithValue("@ProjName", txtProject.Text.Trim())
                cmd.Parameters.AddWithValue("@Status", True)
                cmd.Parameters.AddWithValue("@Ord", txtOrder.Text.Trim())

                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
                BindProjects()
                dvMsg.Visible = False
                lblMsg.Visible = False
                dvMsgSuccess.Visible = True
                lblMsgSuccess.Visible = True
                lblMsgSuccess.Text = "Project saved successfully."
            ElseIf btnSave.Text = "Update" Then
                Dim sb As StringBuilder = New StringBuilder()
                sb.Append(" Update Projects Set ProjName=@ProjName,Ord=@Ord Where ProjID=@ProjID")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sb.ToString(), con)
                cmd.Parameters.AddWithValue("@ProjName", txtProject.Text.Trim())
                cmd.Parameters.AddWithValue("@Ord", txtOrder.Text.Trim())
                cmd.Parameters.AddWithValue("@ProjID", ViewState("ProjID"))
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsg.Visible = False
                    lblMsg.Visible = False
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Project Updated successfully."
                    BindProjects()
                End If
            End If
            txtProject.Text = String.Empty
            txtOrder.Text = String.Empty
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try

    End Sub

    Protected Sub gvProjects_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Try
            If e.CommandName = "Edit" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblProjID As Label = CType(gvProjects.Rows(RowIndex).FindControl("lblProjID"), Label)
                Dim lblProjName As Label = CType(gvProjects.Rows(RowIndex).FindControl("lblProjName"), Label)
                Dim lblOrd As Label = CType(gvProjects.Rows(RowIndex).FindControl("lblOrd"), Label)
                ViewState("ProjID") = lblProjID.Text
                txtProject.Text = Convert.ToString(lblProjName.Text)
                txtOrder.Text = Convert.ToString(lblOrd.Text)
                btnSave.Text = "Update"
            ElseIf e.CommandName = "Delete" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblProjID As Label = CType(gvProjects.Rows(RowIndex).FindControl("lblProjID"), Label)
                Dim lblProjName As Label = CType(gvProjects.Rows(RowIndex).FindControl("lblProjName"), Label)
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand("Update Projects Set Status=@Status Where ProjID=@ProjID", con)
                cmd.Parameters.AddWithValue("@Status", False)
                cmd.Parameters.AddWithValue("@ProjID", lblProjID.Text)
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Project '" & lblProjName.Text & "' Deleted successfully."
                    ViewState("ProjID") = Nothing
                    BindProjects()
                    btnSave.Text = "Save"
                    txtOrder.Text = String.Empty
                    txtProject.Text = String.Empty
                End If
            End If
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Protected Sub gvProjects_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)

    End Sub

    Protected Sub gvProjects_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

    End Sub

    Protected Sub gvProjects_RowEditing(sender As Object, e As GridViewEditEventArgs)

    End Sub

    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        Session.Abandon()
        Session("EmpId") = Nothing
        Response.Redirect("~/Login.aspx")
    End Sub
End Class