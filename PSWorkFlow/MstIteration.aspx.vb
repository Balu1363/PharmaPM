Imports MySql.Data.MySqlClient
Public Class MstIteration
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
                ddlStage.Items.Insert(0, "--Select--")
                ddlStep.Items.Insert(0, "--Select--")
                BindIteration()
            End If
        End If
    End Sub

    Private Sub BindDDLStage()
        Try
            Dim Query As String
            dt.Clear()
            Query = "Select StageID,Stage from Stage where Status=true and ProjID=" & ddlProject.SelectedValue & " Order By Ord"
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(Query, con)
            dt = New DataTable()
            dt.Columns.Clear()
            dt.Clear()
            con.Open()
            dt.Load(cmd.ExecuteReader())
            con.Close()
            If dt.Rows.Count > 0 Then
                ddlStage.DataSource = dt
                ddlStage.DataValueField = "StageID"
                ddlStage.DataTextField = "Stage"
                ddlStage.DataBind()
                ddlStage.Items.Insert(0, "--Select--")
            Else
                ddlStage.Items.Clear()
                ddlStage.Items.Insert(0, "--Select--")
                ddlStage.SelectedIndex = 0
            End If
        Catch ex As Exception
            dvMsg.Visible = True
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Private Sub BindIteration()
        Try
            Dim sb As StringBuilder = New StringBuilder()
            sb.Append("Select i.ProjID,p.ProjName,i.StageID,sg.Stage,i.StepID,st.Step,i.IterationID,i.Iteration,i.Ord from Iteration i inner join Stage sg on i.StageID=sg.StageID inner join steps st on i.StepID=st.StepID inner join Projects p on i.ProjID=p.ProjID Where i.Status=@Status")
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
                gvIteration.DataSource = dth
                gvIteration.DataBind()
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
    Protected Sub ddlStage_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindStep()
    End Sub

    Private Sub BindStep()
        If ddlStage.SelectedIndex > 0 Then
            Try
                Dim Query As String
                dt.Clear()
                Query = "Select StepID,Step from Steps where Status=true and ProjID=" & ddlProject.SelectedValue & " and StageID=" & ddlStage.SelectedValue & " Order By Ord"
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(Query, con)
                dt = New DataTable()
                dt.Columns.Clear()
                dt.Clear()
                con.Open()
                dt.Load(cmd.ExecuteReader())
                con.Close()
                If dt.Rows.Count > 0 Then
                    ddlStep.DataSource = dt
                    ddlStep.DataValueField = "StepID"
                    ddlStep.DataTextField = "Step"
                    ddlStep.DataBind()
                    ddlStep.Items.Insert(0, "--Select--")
                Else
                    ddlStep.Items.Clear()
                    ddlStep.Items.Insert(0, "--Select--")
                    ddlStep.SelectedIndex = 0
                End If
            Catch ex As Exception
                dvMsg.Visible = True
                lblMsg.Text = ex.Message
            End Try
        Else
            ddlStep.Items.Clear()
            ddlStep.Items.Insert(0, "--Select--")
            ddlStep.SelectedIndex = 0
        End If
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Try
            dvMsgSuccess.Visible = False
            lblMsgSuccess.Visible = False
            dvMsg.Visible = False
            lblMsg.Visible = False
            If txtIteration.Text.Length = 0 Then
                Throw New Exception("Please enter Iteration to proceed")
            End If
            If btnSave.Text = "Save" Then
                Dim sbInsert As StringBuilder = New StringBuilder()
                sbInsert.Append(" Insert Into Iteration (Iteration,ProjID,StageID,StepID,Status,Ord) ")
                sbInsert.Append(" Values (@Iteration,@ProjID,@StageID,@StepID,@Status,@Ord)")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sbInsert.ToString(), con)
                cmd.Parameters.AddWithValue("@Iteration", txtIteration.Text.Trim())
                cmd.Parameters.AddWithValue("@ProjID", ddlProject.SelectedValue)
                cmd.Parameters.AddWithValue("@StageID", ddlStage.SelectedValue)
                cmd.Parameters.AddWithValue("@StepID", ddlStep.SelectedValue)
                cmd.Parameters.AddWithValue("@Status", True)
                cmd.Parameters.AddWithValue("@Ord", txtOrder.Text.Trim())
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
                BindIteration()
                dvMsg.Visible = False
                lblMsg.Visible = False
                dvMsgSuccess.Visible = True
                lblMsgSuccess.Visible = True
                lblMsgSuccess.Text = "Iteration saved successfully."
            ElseIf btnSave.Text = "Update" Then
                Dim sb As StringBuilder = New StringBuilder()
                sb.Append(" Update Iteration Set Iteration=@Iteration,ProjID=@ProjID,StageID=@StageID,StepID=@StepID,Ord=@Ord Where IterationID=@IterationID")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sb.ToString(), con)
                cmd.Parameters.AddWithValue("@Iteration", txtIteration.Text.Trim())
                cmd.Parameters.AddWithValue("@ProjID", ddlProject.SelectedValue)
                cmd.Parameters.AddWithValue("@StageID", ddlStage.SelectedValue)
                cmd.Parameters.AddWithValue("@StepID", ddlStep.SelectedValue)
                cmd.Parameters.AddWithValue("@Ord", txtOrder.Text.Trim())
                cmd.Parameters.AddWithValue("@IterationID", ViewState("IterationID"))
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsg.Visible = False
                    lblMsg.Visible = False
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Iteration Updated successfully."
                    BindIteration()
                End If
            End If
            ddlProject.SelectedIndex = 0
            txtIteration.Text = String.Empty
            ddlStage.SelectedIndex = 0
            ddlStep.SelectedIndex = 0
            txtOrder.Text = String.Empty
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        ddlProject.SelectedIndex = 0
        ddlStage.SelectedIndex = 0
        ddlStep.SelectedIndex = 0
        txtIteration.Text = String.Empty
        txtOrder.Text = String.Empty
        dvMsgSuccess.Visible = False
        lblMsgSuccess.Visible = False
        dvMsg.Visible = False
        lblMsg.Visible = False
        btnSave.Text = "Save"
    End Sub

    Protected Sub gvIteration_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

    End Sub

    Protected Sub gvIteration_RowEditing(sender As Object, e As GridViewEditEventArgs)

    End Sub

    Protected Sub gvIteration_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Try
            If e.CommandName = "Edit" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)

                Dim RowIndex As Integer = row.RowIndex
                Dim lblIterationID As Label = CType(gvIteration.Rows(RowIndex).FindControl("lblIterationID"), Label)
                Dim lblProjID As Label = CType(gvIteration.Rows(RowIndex).FindControl("lblProjID"), Label)
                Dim lblStageID As Label = CType(gvIteration.Rows(RowIndex).FindControl("lblStageID"), Label)
                Dim lblStepID As Label = CType(gvIteration.Rows(RowIndex).FindControl("lblStepID"), Label)
                Dim lblIteration As Label = CType(gvIteration.Rows(RowIndex).FindControl("lblIteration"), Label)
                Dim lblOrd As Label = CType(gvIteration.Rows(RowIndex).FindControl("lblOrd"), Label)
                ViewState("IterationID") = lblIterationID.Text
                ddlProject.SelectedValue = lblProjID.Text
                If ddlProject.SelectedValue > 0 Then
                    BindDDLStage()
                    ddlStage.SelectedValue = lblStageID.Text
                End If
                If ddlStage.SelectedIndex > 0 Then
                    BindStep()
                    ddlStep.SelectedValue = lblStepID.Text
                End If
                txtIteration.Text = Convert.ToString(lblIteration.Text)
                txtOrder.Text = Convert.ToString(lblOrd.Text)
                btnSave.Text = "Update"
            ElseIf e.CommandName = "Delete" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblIterationID As Label = CType(gvIteration.Rows(RowIndex).FindControl("lblIterationID"), Label)
                Dim lblIteration As Label = CType(gvIteration.Rows(RowIndex).FindControl("lblIteration"), Label)
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand("Update Iteration Set Status=@Status Where IterationID=@IterationID", con)
                cmd.Parameters.AddWithValue("@Status", False)
                cmd.Parameters.AddWithValue("@IterationID", lblIterationID.Text)
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Iteration '" & lblIteration.Text & "' Deleted successfully."
                    ViewState("IterationID") = Nothing
                    BindIteration()
                    btnSave.Text = "Save"
                    ddlStage.SelectedIndex = 0
                    ddlStep.SelectedIndex = 0
                    txtOrder.Text = String.Empty
                    txtIteration.Text = String.Empty
                End If
            End If
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Protected Sub gvIteration_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)

    End Sub

    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        Session.Abandon()
        Session("EmpId") = Nothing
        Response.Redirect("~/Login.aspx")
    End Sub

    Protected Sub ddlProject_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlProject.SelectedIndex > 0 Then
            BindDDLStage()
        Else
            ddlStage.Items.Clear()
            ddlStage.Items.Insert(0, "--Select--")
            ddlStage.SelectedIndex = 0
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
End Class