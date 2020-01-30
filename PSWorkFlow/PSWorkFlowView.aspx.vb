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
                Else
                    showadmin.Visible = False
                End If
                BindDDLProject()
                BindDDLPhase()
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

    Private Sub BindDDLPhase()
        Try
            Dim Query As String
            dt.Clear()
            Query = "Select PhaseID,Phase from ProjectPhase where Status=true Order By Ord"
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(Query, con)
            dt = New DataTable()
            dt.Columns.Clear()
            dt.Clear()
            con.Open()
            dt.Load(cmd.ExecuteReader())
            con.Close()
            If dt.Rows.Count > 0 Then
                ddlPhase.DataSource = dt
                ddlPhase.DataValueField = "PhaseID"
                ddlPhase.DataTextField = "Phase"
                ddlPhase.DataBind()
                ddlPhase.Items.Insert(0, "--Select--")
            Else
                ddlPhase.Items.Clear()
                ddlPhase.Items.Insert(0, "--Select--")
                ddlPhase.SelectedIndex = 0
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
            sb.Append("Select * from (Select w.WorkFlowId,w.ProjID,w.PhaseID,w.AssignedTo,w.StatusID,w.Title,p.ProjName,ph.Phase,concat(e.EmpFirst, ' ', e.EmpLast) as AssignedToUser,s.Status,DATE_FORMAT(CONVERT_TZ(w.dt,'-1:30','+12:00'), '%m/%d/%Y %l:%i %p') as Updated,")
            sb.Append(" concat(w.WorkFlowId,' ',w.Title,' ',IF(p.ProjName IS NULL or p.ProjName = '', 'null', p.ProjName),' ',IF(ph.Phase IS NULL or ph.Phase = '', 'null', ph.Phase),' ',concat(e.EmpFirst, ' ', e.EmpLast),' ',IF(s.Status IS NULL or s.Status = '', 'null', s.Status)) as searchby ")
            sb.Append(" from WorkFlow w left outer join Projects p on p.ProjID=w.ProjID left outer join ProjectPhase ph on ph.PhaseID=w.PhaseID   left outer join Status s on s.StatusID=w.StatusID ")
            sb.Append(" left outer join Employees e on e.EmpID=w.AssignedTo order by w.WorkFlowId desc) tbl where WorkFlowId IS Not NULL ")
            If ddlProject.SelectedIndex > 0 Then
                sb.Append(" And ProjID =" & ddlProject.SelectedValue & "")
            End If
            If ddlPhase.SelectedIndex > 0 Then
                sb.Append(" And PhaseID =" & ddlPhase.SelectedValue & "")
            End If
            If Role = "Admin" Then
                If ddlAssignedTo.SelectedIndex > 0 Then
                    sb.Append(" And AssignedTo= " & ddlAssignedTo.SelectedValue & "")
                End If
            Else
                sb.Append(" And AssignedTo= " & UserID & "")
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
                    sb.Append(" And StatusID <> 5 or StatusID is null ")
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

    'Private Sub BindDDLSearchBy()
    '    Try
    '        Dim Query As String
    '        dt.Clear()
    '        Query = "Select Id,SearchBy from SearchBy order by Id;"
    '        con = New MySqlConnection(conString)
    '        cmd = New MySqlCommand(Query, con)
    '        dt = New DataTable()
    '        dt.Columns.Clear()
    '        dt.Clear()
    '        con.Open()
    '        dt.Load(cmd.ExecuteReader())
    '        con.Close()
    '        If dt.Rows.Count > 0 Then
    '            ddlSearch.DataSource = dt
    '            ddlSearch.DataValueField = "Id"
    '            ddlSearch.DataTextField = "SearchBy"
    '            ddlSearch.DataBind()
    '            ddlSearch.Items.Insert(0, "--Select--")
    '        Else
    '            ddlSearch.Items.Clear()
    '            ddlSearch.Items.Insert(0, "--Select--")
    '            ddlSearch.SelectedIndex = 0
    '        End If
    '    Catch ex As Exception
    '        dvMsgSuccess.Visible = False
    '        lblMsgSuccess.Visible = False
    '        dvMsg.Visible = True
    '        lblMsg.Visible = True
    '        lblMsg.Text = ex.Message
    '    End Try
    'End Sub

    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        Session.Abandon()
        Session("EmpId") = Nothing
        Response.Redirect("~/Login.aspx")
    End Sub

    Protected Sub btnClear_Click(sender As Object, e As EventArgs)
        txtSearch.Text = String.Empty
        ddlProject.SelectedIndex = 0
        ddlPhase.SelectedIndex = 0
        ddlStatus.SelectedIndex = 0
        ddlAssignedTo.SelectedIndex = 0
        BindGrid()
    End Sub
End Class