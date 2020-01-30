Imports MySql.Data.MySqlClient

Public Class MstPhase
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
                BindProjectPhase()
            End If
        End If
    End Sub

    Private Sub BindProjectPhase()
        Try
            Dim sb As StringBuilder = New StringBuilder()
            sb.Append("Select PhaseID,Phase,Ord from ProjectPhase Where Status=@Status")
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
                gvPhase.DataSource = dth
                gvPhase.DataBind()
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
        txtPhase.Text = String.Empty
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
            If txtPhase.Text.Length = 0 Then
                Throw New Exception("Please enter Phase to proceed")
            End If
            If btnSave.Text = "Save" Then
                Dim sbInsert As StringBuilder = New StringBuilder()
                sbInsert.Append(" Insert Into ProjectPhase (Phase, Status,Ord) ")
                sbInsert.Append(" Values (@Phase,@Status,@Ord)")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sbInsert.ToString(), con)
                cmd.Parameters.AddWithValue("@Phase", txtPhase.Text.Trim())
                cmd.Parameters.AddWithValue("@Status", True)
                cmd.Parameters.AddWithValue("@Ord", txtOrder.Text.Trim())
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
                BindProjectPhase()
                dvMsg.Visible = False
                lblMsg.Visible = False
                dvMsgSuccess.Visible = True
                lblMsgSuccess.Visible = True
                lblMsgSuccess.Text = "Phase saved successfully."
            ElseIf btnSave.Text = "Update" Then
                Dim sb As StringBuilder = New StringBuilder()
                sb.Append(" Update ProjectPhase Set Phase=@Phase,Ord=@Ord Where PhaseID=@PhaseID")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sb.ToString(), con)
                cmd.Parameters.AddWithValue("@Phase", txtPhase.Text.Trim())
                cmd.Parameters.AddWithValue("@Ord", txtOrder.Text.Trim())
                cmd.Parameters.AddWithValue("@PhaseID", ViewState("PhaseID"))
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsg.Visible = False
                    lblMsg.Visible = False
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Phase Updated successfully."
                    BindProjectPhase()
                End If
            End If
            txtPhase.Text = String.Empty
            txtOrder.Text = String.Empty
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try

    End Sub

    Protected Sub gvPhase_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Try
            If e.CommandName = "Edit" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblPhaseID As Label = CType(gvPhase.Rows(RowIndex).FindControl("lblPhaseID"), Label)
                Dim lblPhase As Label = CType(gvPhase.Rows(RowIndex).FindControl("lblPhase"), Label)
                Dim lblOrd As Label = CType(gvPhase.Rows(RowIndex).FindControl("lblOrd"), Label)
                ViewState("PhaseID") = lblPhaseID.Text
                txtPhase.Text = Convert.ToString(lblPhase.Text)
                txtOrder.Text = Convert.ToString(lblOrd.Text)
                btnSave.Text = "Update"
            ElseIf e.CommandName = "Delete" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblPhaseID As Label = CType(gvPhase.Rows(RowIndex).FindControl("lblPhaseID"), Label)
                Dim lblPhase As Label = CType(gvPhase.Rows(RowIndex).FindControl("lblPhase"), Label)
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand("Update ProjectPhase Set Status=@Status Where PhaseID=@PhaseID", con)
                cmd.Parameters.AddWithValue("@Status", False)
                cmd.Parameters.AddWithValue("@PhaseID", lblPhaseID.Text)
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Phase '" & lblPhase.Text & "' Deleted successfully."
                    ViewState("PhaseID") = Nothing
                    BindProjectPhase()
                    btnSave.Text = "Save"
                    txtOrder.Text = String.Empty
                    txtPhase.Text = String.Empty
                End If
            End If
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Protected Sub gvPhase_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)

    End Sub

    Protected Sub gvPhase_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

    End Sub

    Protected Sub gvPhase_RowEditing(sender As Object, e As GridViewEditEventArgs)

    End Sub

    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        Session.Abandon()
        Session("EmpId") = Nothing
        Response.Redirect("~/Login.aspx")
    End Sub
End Class