Imports MySql.Data.MySqlClient
Public Class MstSteps
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
                BindDDLProject()
                ddlStage.Items.Insert(0, "--Select--")
                BindProjectStep()
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

    Private Sub BindProjectStep()
        Try
            Dim sb As StringBuilder = New StringBuilder()
            sb.Append("Select st.ProjID,p.ProjName,st.StageID,sg.Stage,st.StepID,st.Step,st.Ord from Stage sg inner join steps st on sg.StageID=st.StageID inner join Projects p on st.ProjID=p.ProjID Where st.Status=@Status")
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
                gvStep.DataSource = dth
                gvStep.DataBind()
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

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Try
            dvMsgSuccess.Visible = False
            lblMsgSuccess.Visible = False
            dvMsg.Visible = False
            lblMsg.Visible = False
            If txtStep.Text.Length = 0 Then
                Throw New Exception("Please enter Step to proceed")
            End If
            If btnSave.Text = "Save" Then
                Dim sbInsert As StringBuilder = New StringBuilder()
                sbInsert.Append(" Insert Into Steps (Step,ProjID,StageID,Status,Ord) ")
                sbInsert.Append(" Values (@Step,@ProjID,@StageID,@Status,@Ord)")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sbInsert.ToString(), con)
                cmd.Parameters.AddWithValue("@ProjID", ddlProject.SelectedValue)
                cmd.Parameters.AddWithValue("@Step", txtStep.Text.Trim())
                cmd.Parameters.AddWithValue("@StageID", ddlStage.SelectedValue)
                cmd.Parameters.AddWithValue("@Status", True)
                cmd.Parameters.AddWithValue("@Ord", txtOrder.Text.Trim())
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
                BindProjectStep()
                dvMsg.Visible = False
                lblMsg.Visible = False
                dvMsgSuccess.Visible = True
                lblMsgSuccess.Visible = True
                lblMsgSuccess.Text = "Step saved successfully."
            ElseIf btnSave.Text = "Update" Then
                Dim sb As StringBuilder = New StringBuilder()
                sb.Append(" Update steps Set Step=@Step,ProjID=@ProjID,StageID=@StageID,Ord=@Ord Where StepID=@StepID")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sb.ToString(), con)
                cmd.Parameters.AddWithValue("@Step", txtStep.Text.Trim())
                cmd.Parameters.AddWithValue("@ProjID", ddlProject.SelectedValue)
                cmd.Parameters.AddWithValue("@StageID", ddlStage.SelectedValue)
                cmd.Parameters.AddWithValue("@Ord", txtOrder.Text.Trim())
                cmd.Parameters.AddWithValue("@StepID", ViewState("StepID"))
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsg.Visible = False
                    lblMsg.Visible = False
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Step Updated successfully."
                    BindProjectStep()
                End If
            End If
            ddlProject.SelectedIndex = 0
            txtStep.Text = String.Empty
            ddlStage.SelectedIndex = 0
            txtOrder.Text = String.Empty
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        ddlProject.SelectedIndex = 0
        ddlStage.SelectedIndex = 0
        txtStep.Text = String.Empty
        txtOrder.Text = String.Empty
        dvMsgSuccess.Visible = False
        lblMsgSuccess.Visible = False
        dvMsg.Visible = False
        lblMsg.Visible = False
        btnSave.Text = "Save"
    End Sub

    Protected Sub gvStep_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)

    End Sub

    Protected Sub gvStep_RowEditing(sender As Object, e As GridViewEditEventArgs)

    End Sub

    Protected Sub gvStep_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Try
            If e.CommandName = "Edit" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblStepID As Label = CType(gvStep.Rows(RowIndex).FindControl("lblStepID"), Label)
                Dim lblProjID As Label = CType(gvStep.Rows(RowIndex).FindControl("lblProjID"), Label)
                Dim lblStageID As Label = CType(gvStep.Rows(RowIndex).FindControl("lblStageID"), Label)
                Dim lblStep As Label = CType(gvStep.Rows(RowIndex).FindControl("lblStep"), Label)
                Dim lblOrd As Label = CType(gvStep.Rows(RowIndex).FindControl("lblOrd"), Label)
                ViewState("StepID") = lblStepID.Text
                ddlProject.SelectedValue = lblProjID.Text
                If ddlProject.SelectedValue > 0 Then
                    BindDDLStage()
                    ddlStage.SelectedValue = lblStageID.Text
                End If

                txtStep.Text = Convert.ToString(lblStep.Text)
                txtOrder.Text = Convert.ToString(lblOrd.Text)
                btnSave.Text = "Update"
            ElseIf e.CommandName = "Delete" Then
                Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
                Dim RowIndex As Integer = row.RowIndex
                Dim lblStepID As Label = CType(gvStep.Rows(RowIndex).FindControl("lblStepID"), Label)
                Dim lblStageID As Label = CType(gvStep.Rows(RowIndex).FindControl("lblStageID"), Label)
                Dim lblStep As Label = CType(gvStep.Rows(RowIndex).FindControl("lblStep"), Label)
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand("Update steps Set Status=@Status Where StepID=@StepID", con)
                cmd.Parameters.AddWithValue("@Status", False)
                cmd.Parameters.AddWithValue("@StepID", lblStepID.Text)
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Step '" & lblStep.Text & "' Deleted successfully."
                    ViewState("StepID") = Nothing
                    BindProjectStep()
                    btnSave.Text = "Save"
                    ddlStage.SelectedIndex = 0
                    txtOrder.Text = String.Empty
                    txtStep.Text = String.Empty
                End If
            End If
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Protected Sub gvStep_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)

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