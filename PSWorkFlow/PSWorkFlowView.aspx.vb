Imports MySql.Data.MySqlClient
Imports System.IO

Public Class PSWorkFlowView
    Inherits System.Web.UI.Page
    Dim dt As DataTable = New DataTable()
    Dim cmd As MySqlCommand = New MySqlCommand()
    Dim con As MySqlConnection = New MySqlConnection()
    Dim conString As String = ConfigurationManager.ConnectionStrings("dbcNoeth").ConnectionString
    Dim UserID As Integer = 0
    Dim Role As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("EmpId") Is Nothing OrElse Session("EmpId") = "" Then
            Response.Redirect("Login.aspx")
        Else
            UserID = Session("EmpId")
            Role = Session("Role").ToString().Trim()
            lblEmpName.Text = "User : " & Session("EmpName").ToString()
            If Not IsPostBack Then
                If Role = "Admin" Then
                    showadmin.Visible = True
                    showadd.Visible = True
                Else
                    showadmin.Visible = False
                    showadd.Visible = False
                End If
                BindDDLProject()
                ddlStage.Items.Insert(0, "--Select--")
                ddlStep.Items.Insert(0, "--Select--")
                ddlIteration.Items.Insert(0, "--Select--")
                BindDDLStatus()
                BindDDLAssignTo()
                BindGrid()
                dvMsg.Visible = False
                lblMsg.Visible = False
                If Not Session("SaveMsg") Is Nothing Then
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = Session("SaveMsg").ToString()
                Else
                    dvMsgSuccess.Visible = False
                    lblMsgSuccess.Visible = False
                End If
            End If
        End If
    End Sub

    Private Sub BindDDLStatus()
        Try
            Dim Query As String
            dt.Clear()
            Query = "Select StatusID,Status from Status where Stat=true order by Ord;"
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(Query, con)
            dt = New DataTable()
            dt.Columns.Clear()
            dt.Clear()
            con.Open()
            dt.Load(cmd.ExecuteReader())
            con.Close()
            If dt.Rows.Count > 0 Then
                ddlStatus.DataSource = dt
                ddlStatus.DataValueField = "StatusID"
                ddlStatus.DataTextField = "Status"
                ddlStatus.DataBind()
                ddlStatus.Items.Insert(0, "--Select--")
            Else
                ddlStatus.Items.Clear()
                ddlStatus.Items.Insert(0, "--Select--")
                ddlStatus.SelectedIndex = 0
            End If
        Catch ex As Exception
            dvMsg.Visible = True
            lblMsg.Text = ex.Message
        End Try
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

    Private Sub BindDDLAssignTo()
        Try
            Dim Query As String
            dt.Clear()
            Query = "select EmpID, concat(EmpFirst,' ',EmpLast) as EmpName from Employees where Status=1 Order By EmpName"
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(Query, con)
            dt = New DataTable()
            dt.Columns.Clear()
            dt.Clear()
            con.Open()
            dt.Load(cmd.ExecuteReader())
            con.Close()
            If dt.Rows.Count > 0 Then
                ddlAssignedTo.DataSource = dt
                ddlAssignedTo.DataValueField = "EmpID"
                ddlAssignedTo.DataTextField = "EmpName"
                ddlAssignedTo.DataBind()
                ddlAssignedTo.Items.Insert(0, "--Select--")
            Else
                ddlAssignedTo.Items.Clear()
                ddlAssignedTo.Items.Insert(0, "--Select--")
                ddlAssignedTo.SelectedIndex = 0
            End If
        Catch ex As Exception
            dvMsg.Visible = True
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Private Sub BindGrid()
        Try
            dvMsg.Visible = False
            lblMsg.Visible = False
            Dim sb As StringBuilder = New StringBuilder()
            dt.Clear()
            sb.Append("Select * from (Select w.WorkFlowId,w.ProjID,w.StageID,w.StepID,w.IterationID,w.AssignedTo,w.StatusID,w.Title,p.ProjName,ph.Stage,st.Step,i.Iteration,concat(e.EmpFirst, ' ', e.EmpLast) as AssignedToUser,s.Status,DATE_FORMAT(CONVERT_TZ(w.dt,'-1:30','+12:00'), '%d-%m-%Y %l:%i %p') as Updated,")
            sb.Append(" concat(w.WorkFlowId,' ',w.Title,' ',IF(p.ProjName IS NULL or p.ProjName = '', 'null', p.ProjName),' ',IF(ph.Stage IS NULL or ph.Stage = '', 'null', ph.Stage),' ',IF(st.Step IS NULL or st.Step = '', 'null', st.Step),' ',IF(i.Iteration IS NULL or i.Iteration = '', 'null', i.Iteration),' ',concat(e.EmpFirst, ' ', e.EmpLast),' ',IF(s.Status IS NULL or s.Status = '', 'null', s.Status)) as searchby, ")
            sb.Append(" DATE_FORMAT(PlannedStartDt,'%d-%m-%Y') PlannedStartDt,DATE_FORMAT(PlannedEndDt,'%d-%m-%Y') PlannedEndDt,DATE_FORMAT(ActualStartDt,'%d-%m-%Y') ActualStartDt,DATE_FORMAT(ActualEndDate,'%d-%m-%Y') ActualEndDate from WorkFlow w left outer join Projects p on p.ProjID=w.ProjID left outer join Stage ph on ph.StageID=w.StageID left outer join Steps st on st.StepID=w.StepID left outer join Iteration i on i.IterationID=w.IterationID left outer join Status s on s.StatusID=w.StatusID ")
            sb.Append(" left outer join Employees e on e.EmpID=w.AssignedTo order by w.WorkFlowId desc) tbl where WorkFlowId IS Not NULL ")
            If ddlProject.SelectedIndex > 0 Then
                sb.Append(" And ProjID =" & ddlProject.SelectedValue & "")
            End If
            If ddlStage.SelectedIndex > 0 Then
                sb.Append(" And StageID =" & ddlStage.SelectedValue & "")
            End If
            If ddlStep.SelectedIndex > 0 Then
                sb.Append(" And StepID =" & ddlStep.SelectedValue & "")
            End If
            If ddlIteration.SelectedIndex > 0 Then
                sb.Append(" And IterationID =" & ddlIteration.SelectedValue & "")
            End If
            If Role = "Admin" Then
                If ddlAssignedTo.SelectedIndex > 0 Then
                    sb.Append(" And AssignedTo= " & ddlAssignedTo.SelectedValue & "")
                End If
            Else
                sb.Append(" And AssignedTo= " & UserID & "")
            End If

            ' Date Filters
            If Len(txtPlannedStartDt.Text) > 0 Then
                sb.Append(" and PlannedStartDt = '" & Convert.ToDateTime(txtPlannedStartDt.Text.Trim()).ToString("dd-MM-yyyy") & "'")
            End If
            If Len(txtPlannedEndDt.Text) > 0 Then
                sb.Append(" and PlannedEndDt = '" & Convert.ToDateTime(txtPlannedEndDt.Text.Trim()).ToString("dd-MM-yyyy") & "'")
            End If
            If Len(txtActualStartDt.Text) > 0 Then
                sb.Append(" and ActualStartDt = '" & Convert.ToDateTime(txtActualStartDt.Text.Trim()).ToString("dd-MM-yyyy") & "'")
            End If
            If Len(txtActualEndDate.Text) > 0 Then
                sb.Append(" and ActualEndDate = '" & Convert.ToDateTime(txtActualEndDate.Text.Trim()).ToString("dd-MM-yyyy") & "'")
            End If

            If Len(txtSearch.Text) > 0 Then
                sb.Append(" and searchby like '%" & txtSearch.Text.Trim() & "%'")
            End If

            If ddlStatus.SelectedIndex > 0 Then
                sb.Append(" And StatusID = " & ddlStatus.SelectedValue & "")
            Else
                If Len(txtSearch.Text) > 0 Then
                    If txtSearch.Text.Trim() = "Clo" Then
                    Else
                        sb.Append(" And StatusID <> 5")
                    End If
                Else
                    sb.Append(" And (StatusID <> 5 or StatusID is null) ")
                End If
            End If

            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(sb.ToString(), con)

            dt = New DataTable()
            dt.Columns.Clear()
            dt.Clear()
            con.Open()
            dt.Load(cmd.ExecuteReader())
            con.Close()
            If dt.Rows.Count > 0 Then
                PnlGrid.Visible = True
                GvPartSales.DataSource = dt
                GvPartSales.DataBind()
                ViewState("dirState") = dt
                ViewState("sortdr") = "Asc"
            Else
                PnlGrid.Visible = False
                GvPartSales.DataSource = Nothing
                GvPartSales.DataBind()
                dvMsg.Visible = True
                lblMsg.Visible = True
                lblMsg.Text = "Data not found."
            End If

        Catch ex As Exception
            dvMsg.Visible = True
            lblMsg.Visible = True
            lblMsg.Text = ex.Message
        End Try
    End Sub
    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Try
            dvMsgSuccess.Visible = False
            lblMsgSuccess.Visible = False
            BindGrid()
        Catch ex As Exception
            dvMsgSuccess.Visible = False
            lblMsgSuccess.Visible = False
            dvMsg.Visible = True
            lblMsg.Visible = True
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Protected Sub GvPartSales_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If e.CommandName = "Edit" OrElse e.CommandName = "EditId" Then
            Dim row As GridViewRow = CType(((CType(e.CommandSource, LinkButton)).NamingContainer), GridViewRow)
            Dim RowIndex As Integer = row.RowIndex
            Dim lblId As Label = CType(GvPartSales.Rows(RowIndex).FindControl("lblId"), Label)
            Response.Redirect("PSWorkFlow.aspx?wid=" & lblId.Text)
        End If
    End Sub

    Protected Sub GvPartSales_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        GvPartSales.PageIndex = e.NewPageIndex
        BindGrid()
    End Sub

    Protected Sub GvPartSales_Sorting(sender As Object, e As GridViewSortEventArgs)
        Dim dtrslt As DataTable = CType(ViewState("dirState"), DataTable)
        If dtrslt.Rows.Count > 0 Then
            If Convert.ToString(ViewState("sortdr")) = "Asc" Then
                dtrslt.DefaultView.Sort = e.SortExpression & " Desc"
                ViewState("sortdr") = "Desc"
            Else
                dtrslt.DefaultView.Sort = e.SortExpression & " Asc"
                ViewState("sortdr") = "Asc"
            End If
            GvPartSales.DataSource = dtrslt
            GvPartSales.DataBind()
        End If
    End Sub

    Protected Sub GvPartSales_RowEditing(sender As Object, e As GridViewEditEventArgs)

    End Sub

    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        Session.Abandon()
        Session("EmpId") = Nothing
        Response.Redirect("~/Login.aspx")
    End Sub

    Protected Sub btnClear_Click(sender As Object, e As EventArgs)
        txtSearch.Text = String.Empty
        ddlProject.SelectedIndex = 0

        ddlStage.Items.Clear()
        ddlStage.Items.Insert(0, "--Select--")
        ddlStage.SelectedIndex = 0

        ddlStep.Items.Clear()
        ddlStep.Items.Insert(0, "--Select--")
        ddlStep.SelectedIndex = 0

        ddlIteration.Items.Clear()
        ddlIteration.Items.Insert(0, "--Select--")
        ddlIteration.SelectedIndex = 0

        ddlStatus.SelectedIndex = 0
        ddlAssignedTo.SelectedIndex = 0

        txtPlannedStartDt.Text = String.Empty
        txtPlannedEndDt.Text = String.Empty
        txtActualStartDt.Text = String.Empty
        txtActualEndDate.Text = String.Empty
        BindGrid()
    End Sub

    Protected Sub ddlStage_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlStage.SelectedIndex > 0 Then
            BindStep()
        Else
            ddlStep.Items.Clear()
            ddlStep.Items.Insert(0, "--Select--")
            ddlStep.SelectedIndex = 0

            ddlIteration.Items.Clear()
            ddlIteration.Items.Insert(0, "--Select--")
            ddlIteration.SelectedIndex = 0
        End If

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

            ddlIteration.Items.Clear()
            ddlIteration.Items.Insert(0, "--Select--")
            ddlIteration.SelectedIndex = 0
        End If
    End Sub

    Protected Sub ddlStep_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlStep.SelectedIndex > 0 Then
            BindIteration()
        Else
            ddlIteration.Items.Clear()
            ddlIteration.Items.Insert(0, "--Select--")
            ddlIteration.SelectedIndex = 0
        End If
    End Sub

    Private Sub BindIteration()
        If ddlStep.SelectedIndex > 0 Then
            Try
                Dim Query As String
                dt.Clear()
                Query = "Select IterationID,Iteration from Iteration where Status=true and ProjID=" & ddlProject.SelectedValue & " and StageID=" & ddlStage.SelectedValue & " and StepID=" & ddlStep.SelectedValue & " Order By Ord"
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(Query, con)
                dt = New DataTable()
                dt.Columns.Clear()
                dt.Clear()
                con.Open()
                dt.Load(cmd.ExecuteReader())
                con.Close()
                If dt.Rows.Count > 0 Then
                    ddlIteration.DataSource = dt
                    ddlIteration.DataValueField = "IterationID"
                    ddlIteration.DataTextField = "Iteration"
                    ddlIteration.DataBind()
                    ddlIteration.Items.Insert(0, "--Select--")
                Else
                    ddlIteration.Items.Clear()
                    ddlIteration.Items.Insert(0, "--Select--")
                    ddlIteration.SelectedIndex = 0
                End If
            Catch ex As Exception
                dvMsg.Visible = True
                lblMsg.Text = ex.Message
            End Try
        Else
            ddlIteration.Items.Clear()
            ddlIteration.Items.Insert(0, "--Select--")
            ddlIteration.SelectedIndex = 0
        End If
    End Sub

    Protected Sub ddlProject_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlProject.SelectedIndex > 0 Then
            BindDDLStage()
        Else
            ddlStage.Items.Clear()
            ddlStage.Items.Insert(0, "--Select--")
            ddlStage.SelectedIndex = 0

            ddlStep.Items.Clear()
            ddlStep.Items.Insert(0, "--Select--")
            ddlStep.SelectedIndex = 0

            ddlIteration.Items.Clear()
            ddlIteration.Items.Insert(0, "--Select--")
            ddlIteration.SelectedIndex = 0
        End If
    End Sub
End Class