Imports MySql.Data.MySqlClient

Public Class MstStage
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
                BindDDLProject()
                BindProjectStage()
            End If
        End If
    End Sub

    Private Sub BindDDLProject()
        Try
            Dim Query As String
            dt.Clear()
            Query = "Select ProjID,ProjName from Projects where Status=true Order By Ord;"
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(Query, con)
            dt = New DataTable()
            dt.Columns.Clear()
            dt.Clear()
            con.Open()
            dt.Load(cmd.ExecuteReader())
            con.Close()
            If dt.Rows.Count > 0 Then
                ddlProject.DataSource = dt
                ddlProject.DataValueField = "ProjID"
                ddlProject.DataTextField = "ProjName"
                ddlProject.DataBind()
                ddlProject.Items.Insert(0, "--Select--")
            Else
                ddlProject.Items.Clear()
                ddlProject.Items.Insert(0, "--Select--")
                ddlProject.SelectedIndex = 0
            End If
        Catch ex As Exception
            dvMsg.Visible = True
            lblMsg.Text = ex.Message
        End Try
    End Sub


    Private Sub BindProjectStage()
        Try
            Dim sb As StringBuilder = New StringBuilder()
            sb.Append("Select s.StageID,s.Stage,s.ProjID,p.ProjName,s.Ord from Stage s inner join Projects p on s.ProjID=p.ProjID Where s.Status=@Status")
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
                gvStage.DataSource = dth
                gvStage.DataBind()
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
        ddlProject.SelectedIndex = 0
        txtStage.Text = String.Empty
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
            If txtStage.Text.Length = 0 Then
                Throw New Exception("Please enter Stage to proceed")
            End If
            If btnSave.Text = "Save" Then
                Dim sbInsert As StringBuilder = New StringBuilder()
                sbInsert.Append(" Insert Into Stage (Stage,ProjID,Status,Ord) ")
                sbInsert.Append(" Values (@Stage,@ProjID,@Status,@Ord)")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sbInsert.ToString(), con)
                cmd.Parameters.AddWithValue("@ProjID", ddlProject.SelectedValue)
                cmd.Parameters.AddWithValue("@Stage", txtStage.Text.Trim())
                cmd.Parameters.AddWithValue("@Status", True)
                cmd.Parameters.AddWithValue("@Ord", txtOrder.Text.Trim())
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
                BindProjectStage()
                dvMsg.Visible = False
                lblMsg.Visible = False
                dvMsgSuccess.Visible = True
                lblMsgSuccess.Visible = True
                lblMsgSuccess.Text = "Stage saved successfully."
            ElseIf btnSave.Text = "Update" Then
                Dim sb As StringBuilder = New StringBuilder()
                sb.Append(" Update Stage Set Stage=@Stage,ProjID=@ProjID,Ord=@Ord Where StageID=@StageID")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sb.ToString(), con)
                cmd.Parameters.AddWithValue("@ProjID", ddlProject.SelectedValue)
                cmd.Parameters.AddWithValue("@Stage", txtStage.Text.Trim())
                cmd.Parameters.AddWithValue("@Ord", txtOrder.Text.Trim())
                cmd.Parameters.AddWithValue("@StageID", ViewState("StageID"))
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsg.Visible = False
                    lblMsg.Visible = False
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Stage Updated successfully."
                    BindProjectStage()
                End If
            End If
            ddlProject.SelectedIndex = 0
            txtStage.Text = String.Empty
            txtOrder.Text = String.Empty
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try

    End Sub

    Protected Sub gvStage_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Try
            If e.CommandName = "Edit" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblStageID As Label = CType(gvStage.Rows(RowIndex).FindControl("lblStageID"), Label)
                Dim lblStage As Label = CType(gvStage.Rows(RowIndex).FindControl("lblStage"), Label)
                Dim lblProjID As Label = CType(gvStage.Rows(RowIndex).FindControl("lblProjID"), Label)
                Dim lblOrd As Label = CType(gvStage.Rows(RowIndex).FindControl("lblOrd"), Label)
                ViewState("StageID") = lblStageID.Text
                ddlProject.SelectedValue = lblProjID.Text
                txtStage.Text = Convert.ToString(lblStage.Text)
                txtOrder.Text = Convert.ToString(lblOrd.Text)
                btnSave.Text = "Update"
            ElseIf e.CommandName = "Delete" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblStageID As Label = CType(gvStage.Rows(RowIndex).FindControl("lblStageID"), Label)
                Dim lblStage As Label = CType(gvStage.Rows(RowIndex).FindControl("lblStage"), Label)
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand("Update Stage Set Status=@Status Where StageID=@StageID", con)
                cmd.Parameters.AddWithValue("@Status", False)
                cmd.Parameters.AddWithValue("@StageID", lblStageID.Text)
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Stage '" & lblStage.Text & "' Deleted successfully."
                    ViewState("StageID") = Nothing
                    BindProjectStage()
                    btnSave.Text = "Save"
                    txtOrder.Text = String.Empty
                    txtStage.Text = String.Empty
                End If
            End If
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Protected Sub gvStage_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)

    End Sub

    Protected Sub gvStage_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

    End Sub

    Protected Sub gvStage_RowEditing(sender As Object, e As GridViewEditEventArgs)

    End Sub

    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        Session.Abandon()
        Session("EmpId") = Nothing
        Response.Redirect("~/Login.aspx")
    End Sub
End Class