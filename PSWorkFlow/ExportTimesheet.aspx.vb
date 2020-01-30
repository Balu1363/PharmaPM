Imports System.IO
Imports ClosedXML.Excel
Imports MySql.Data.MySqlClient
Imports DocumentFormat.OpenXml
Imports System.Globalization

Public Class ExportTimesheet
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
                dvMsg.Visible = False
                lblMsg.Text = String.Empty
                BindWeeks()
                If Role = "Admin" Then
                    showadmin.Visible = True
                    showemp.Visible = True
                    BindDDLEmployee()
                Else
                    showemp.Visible = False
                    showadmin.Visible = False
                End If
            End If
        End If
    End Sub

    Private Sub BindWeeks()
        Try
            Dim sbQuery As StringBuilder = New StringBuilder()
            sbQuery.Append("Select WeekId,Week From Weeks Order by WeekId")
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(sbQuery.ToString(), con)
            Dim dt As DataTable = New DataTable()
            con.Open()
            dt.Load(cmd.ExecuteReader())
            If dt.Rows.Count > 0 Then
                ddlweeks.DataTextField = "Week"
                ddlweeks.DataValueField = "WeekId"
                ddlweeks.DataSource = dt
                ddlweeks.DataBind()
            End If
        Catch ex As Exception
            Session("Alert") = ex.Message
        End Try
    End Sub

    Private Sub BindDDLEmployee()
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
                ddlEmployee.DataSource = dt
                ddlEmployee.DataValueField = "EmpID"
                ddlEmployee.DataTextField = "EmpName"
                ddlEmployee.DataBind()
                ddlEmployee.Items.Insert(0, "--Select--")
            Else
                ddlEmployee.Items.Clear()
                ddlEmployee.Items.Insert(0, "--Select--")
                ddlEmployee.SelectedIndex = 0
            End If
        Catch ex As Exception
            dvMsg.Visible = True
            lblMsg.Text = ex.Message
        End Try
    End Sub
    Protected Sub btnExport_Click(sender As Object, e As EventArgs)
        dvMsg.Visible = False
        lblMsg.Text = String.Empty
        Dim i As Integer = 0
        Dim result As String() = ddlweeks.SelectedItem.Text.Split(New String() {"Mon ", " to Sun"}, 3, StringSplitOptions.None)

        Dim from As DateTime = DateTime.ParseExact(result(1).ToString().Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture)
        Dim fromDate As String = from.ToString("yyyy-MM-dd")

        Dim todt As DateTime = DateTime.ParseExact(result(2).ToString().Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture)
        Dim toDate As String = todt.ToString("yyyy-MM-dd")

        Try
            Using con As New MySqlConnection(conString)
                Dim sb As StringBuilder = New StringBuilder()
                sb.Append("Select dayname(t.dt) as Day,DATE_FORMAT(CONVERT_TZ(w.dt,'-1:30','+12:00'), '%m-%d-%Y') As dt,p.ProjName as Project,'' as SubProject,ph.Stage as TaskType,t.Task as ProjectTask, '0' as Hours,'Non Project Task' as NPT,'0' Total from taskdetails t left outer join workflow w on t.WorkFlowID=w.WorkFlowId")
                sb.Append(" left outer join projects p on w.ProjID=p.ProjID left outer join projectphase ph on w.StageID=ph.StageID where  t.dt Between '" & fromDate & "' and '" & toDate & "' ")
                If Role = "Admin" Then
                    If ddlEmployee.SelectedIndex > 0 Then
                        sb.Append(" and t.EmpId=" & ddlEmployee.SelectedValue & "")
                    Else
                        sb.Append(" and t.EmpId=" & UserId & "")
                    End If
                Else
                    sb.Append(" and t.EmpId=" & UserId & "")
                End If
                sb.Append(" group by t.dt")
                Using cmd As New MySqlCommand(sb.ToString())
                    Using sda As New MySqlDataAdapter()
                        cmd.Connection = con
                        sda.SelectCommand = cmd
                        Using dt As New DataTable()
                            sda.Fill(dt)
                            Dim cnt As Integer = dt.Rows.Count - 1
                            Dim path = Server.MapPath("/TimesheetTemplate.xlsx")
                            Using wb As New XLWorkbook(path)
                                Dim ws As IXLWorksheet = wb.Worksheet(1)
                                If dt.Rows.Count > 0 Then
                                    For i = 0 To cnt
                                        Dim A As Integer = 2
                                        Dim J As Integer = 2
                                        A += i
                                        J += i
                                        Dim rng = ws.Range("A" & A & ":J" & J & "")
                                        Dim range As Integer = 0
                                        For Each cell In rng.Cells()
                                            If range = 0 Then
                                                cell.Value = dt.Rows(i)("Day").ToString()
                                            End If
                                            If range = 1 Then
                                                cell.Value = dt.Rows(i)("dt").ToString()
                                            End If
                                            If range = 2 Then
                                                cell.Value = dt.Rows(i)("Project").ToString()
                                            End If
                                            If range = 3 Then
                                                cell.Value = dt.Rows(i)("SubProject").ToString()
                                            End If
                                            If range = 4 Then
                                                cell.Value = dt.Rows(i)("TaskType").ToString()
                                            End If
                                            If range = 5 Then
                                                'Dim link As String() = dt.Rows(i)("ProjectTask").ToString().Replace("<br/>", Environment.NewLine).Split(New String() {" Link :", "*"}, 4, StringSplitOptions.None)
                                                cell.Value = dt.Rows(i)("ProjectTask").ToString().Replace("<br/>", Environment.NewLine)
                                                'cell.Hyperlink = New XLHyperlink(link(0))
                                            End If
                                            If range = 6 Then
                                                cell.Value = dt.Rows(i)("Hours").ToString()
                                            End If
                                            If range = 7 Then
                                                cell.Value = dt.Rows(i)("NPT").ToString()
                                            End If
                                            If range = 8 Then
                                                cell.Value = dt.Rows(i)("Hours").ToString()
                                            End If
                                            If range = 9 Then
                                                cell.Value = dt.Rows(i)("Total").ToString()
                                            End If
                                            range += 1
                                        Next
                                    Next
                                End If

                                Dim filename As String = PathStrip("" & Session("EmpName").ToString() & "") & PathStrip("_Timesheet_For_") & PathStrip(ddlweeks.SelectedItem.Text) & ".xlsx"
                                Response.Clear()
                                Response.Buffer = True
                                Response.Charset = ""
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                                Response.AddHeader("content-disposition", "attachment;filename=" & filename & "")
                                Response.ContentEncoding = System.Text.Encoding.UTF8
                                Using MyMemoryStream As New MemoryStream()
                                    wb.SaveAs(MyMemoryStream)

                                    MyMemoryStream.WriteTo(Response.OutputStream)
                                    Response.Flush()
                                    Response.End()
                                End Using
                            End Using
                        End Using
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Session("Alert") = ex.Message
        End Try

    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Try
            dvMsg.Visible = False
            lblMsg.Text = String.Empty
            Dim result As String() = ddlweeks.SelectedItem.Text.Split(New String() {"Mon ", " to Sun"}, 3, StringSplitOptions.None)

            Dim from As DateTime = DateTime.ParseExact(result(1).ToString().Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture)
            Dim fromDate As String = from.ToString("yyyy-MM-dd")

            Dim todt As DateTime = DateTime.ParseExact(result(2).ToString().Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture)
            Dim toDate As String = todt.ToString("yyyy-MM-dd")
            Dim sbQuery As StringBuilder = New StringBuilder()
            sbQuery.Append("Select dayname(t.dt) as Day,DATE_FORMAT(CONVERT_TZ(w.dt,'-1:30','+12:00'), '%m-%d-%Y') As dt,p.ProjName as Project,'' as SubProject,ph.Stage as TaskType,t.Task as ProjectTask, '0' as Hours,'Non Project Task' as NPT,'0' Total from taskdetails t left outer join workflow w on t.WorkFlowID=w.WorkFlowId")
            sbQuery.Append(" left outer join projects p on w.ProjID=p.ProjID left outer join projectphase ph on w.StageID=ph.StageID where t.dt Between '" & fromDate & "' and '" & toDate & "' ")

            If Role = "Admin" Then
                If ddlEmployee.SelectedIndex > 0 Then
                    sbQuery.Append(" and t.EmpId=" & ddlEmployee.SelectedValue & "")
                Else
                    sbQuery.Append(" and t.EmpId=" & UserId & "")
                End If
            Else
                sbQuery.Append(" and t.EmpId=" & UserId & "")
            End If
            sbQuery.Append(" group by t.dt ")
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(sbQuery.ToString(), con)
            Dim dt As DataTable = New DataTable()
            con.Open()
            dt.Load(cmd.ExecuteReader())
            If dt.Rows.Count > 0 Then

                'Dim St As String = dt.Rows(0)("ProjectTask").ToString()
                'Dim pFrom As Integer = St.IndexOf("Link : ") + "Link : ".Length
                'Dim pTo As Integer = St.LastIndexOf("wid=") + "wid=".Length + 3
                'Dim r As String = St.Substring(pFrom, pTo - pFrom)

                dvMsg.Visible = False
                lblMsg.Text = String.Empty
                gvTask.DataSource = dt
                gvTask.DataBind()
            Else
                gvTask.DataSource = Nothing
                gvTask.DataBind()
                dvMsg.Visible = True
                lblMsg.Text = "Records not found."
            End If
        Catch ex As Exception

        End Try

    End Sub


    Function PathStrip(ByVal Value As String) As String
        Value = Replace(Value, "/", "_")
        Value = Replace(Value, "|", "")
        Value = Replace(Value, ">", "")
        Value = Replace(Value, "<", "")
        Value = Replace(Value, ":", "")
        Value = Replace(Value, "*", "")
        Value = Replace(Value, ".", "")
        Value = Replace(Value, "?", "")
        Value = Replace(Value, "'", "")
        Value = Replace(Value, ",", "")
        Value = Replace(Value, """", "")
        Value = Replace(Value, "#", "")
        Value = Replace(Value, " ", "_")
        Value = Replace(Value, "&", "&&")
        Return Value
    End Function

    Protected Sub gvTask_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        'Dim task As String = e.Row.Cells(3).Text
        'e.Row.Cells(3).Text = task.ToString().Trim().Replace(Environment.NewLine, "<br/>")
    End Sub

    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        Session.Abandon()
        Session("EmpId") = Nothing
        Response.Redirect("~/Login.aspx")
    End Sub
End Class