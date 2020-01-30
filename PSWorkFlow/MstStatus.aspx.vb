Imports MySql.Data.MySqlClient

Public Class MstStatus
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
                BindStatus()
            End If
        End If
    End Sub

    Private Sub BindStatus()
        Try
            Dim sb As StringBuilder = New StringBuilder()
            sb.Append("Select StatusID,Status,Ord from Status Where Stat=@Stat")
            sb.Append(" Order By Ord;")
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(sb.ToString(), con)
            cmd.Parameters.AddWithValue("@Stat", True)
            Dim dth As DataTable = New DataTable()
            dth.Columns.Clear()
            dth.Clear()
            con.Open()
            dth.Load(cmd.ExecuteReader())
            con.Close()
            If dth.Rows.Count > 0 Then
                gvStatus.DataSource = dth
                gvStatus.DataBind()
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
        txtStatus.Text = String.Empty
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
            If txtStatus.Text.Length = 0 Then
                Throw New Exception("Please enter Status to proceed.")
            End If
            If btnSave.Text = "Save" Then
                Dim sbInsert As StringBuilder = New StringBuilder()
                sbInsert.Append(" Insert Into Status (Status, Stat,Ord) ")
                sbInsert.Append(" Values (@Status,@Stat,@Ord)")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sbInsert.ToString(), con)
                cmd.Parameters.AddWithValue("@Status", txtStatus.Text.Trim())
                cmd.Parameters.AddWithValue("@Stat", True)
                cmd.Parameters.AddWithValue("@Ord", txtOrder.Text.Trim())
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
                BindStatus()
                dvMsg.Visible = False
                lblMsg.Visible = False
                dvMsgSuccess.Visible = True
                lblMsgSuccess.Visible = True
                lblMsgSuccess.Text = "Status saved successfully."
            ElseIf btnSave.Text = "Update" Then
                Dim sb As StringBuilder = New StringBuilder()
                sb.Append(" Update Status Set Status=@Status,Ord=@Ord Where StatusID=@StatusID")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sb.ToString(), con)
                cmd.Parameters.AddWithValue("@Status", txtStatus.Text.Trim())
                cmd.Parameters.AddWithValue("@Ord", txtOrder.Text.Trim())
                cmd.Parameters.AddWithValue("@StatusID", ViewState("StatusID"))
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsg.Visible = False
                    lblMsg.Visible = False
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Status Updated successfully."
                    BindStatus()
                End If
            End If
            txtStatus.Text = String.Empty
            txtOrder.Text = String.Empty
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try

    End Sub

    Protected Sub gvStatus_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Try
            If e.CommandName = "Edit" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblStatusID As Label = CType(gvStatus.Rows(RowIndex).FindControl("lblStatusID"), Label)
                Dim lblStatus As Label = CType(gvStatus.Rows(RowIndex).FindControl("lblStatus"), Label)
                Dim lblOrd As Label = CType(gvStatus.Rows(RowIndex).FindControl("lblOrd"), Label)
                ViewState("StatusID") = lblStatusID.Text
                txtStatus.Text = Convert.ToString(lblStatus.Text)
                txtOrder.Text = Convert.ToString(lblOrd.Text)
                btnSave.Text = "Update"
            ElseIf e.CommandName = "Delete" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblStatusID As Label = CType(gvStatus.Rows(RowIndex).FindControl("lblStatusID"), Label)
                Dim lblStatus As Label = CType(gvStatus.Rows(RowIndex).FindControl("lblStatus"), Label)
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand("Update Status Set Stat=@Stat Where StatusID=@StatusID", con)
                cmd.Parameters.AddWithValue("@Stat", False)
                cmd.Parameters.AddWithValue("@StatusID", lblStatusID.Text)
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Status '" & lblStatus.Text & "' Deleted successfully."
                    ViewState("StatusID") = Nothing
                    BindStatus()
                    btnSave.Text = "Save"
                    txtStatus.Text = String.Empty
                    txtOrder.Text = String.Empty
                End If
            End If
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Protected Sub gvStatus_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)

    End Sub

    Protected Sub gvStatus_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

    End Sub

    Protected Sub gvStatus_RowEditing(sender As Object, e As GridViewEditEventArgs)

    End Sub

    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        Session.Abandon()
        Session("EmpId") = Nothing
        Response.Redirect("~/Login.aspx")
    End Sub
End Class