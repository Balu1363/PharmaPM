Imports MySql.Data.MySqlClient
Imports System.IO



Public Class PSWorkFlow
    Inherits System.Web.UI.Page
    Dim dt As DataTable = New DataTable()
    Dim cmd As MySqlCommand = New MySqlCommand()
    Dim con As MySqlConnection = New MySqlConnection()
    Dim conString As String = ConfigurationManager.ConnectionStrings("dbcNoeth").ConnectionString
    Dim _WorkFlowId As Integer
    Dim UserId As Integer = 0
    Dim Role As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _WorkFlowId = CInt(Request("wid"))
        Session("SaveMsg") = Nothing
        If Session("EmpId") Is Nothing OrElse Session("EmpId") = "" Then
            Response.Redirect("Login.aspx")
        Else
            UserId = Session("EmpId")
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
                BindDDLAssignTo()
                BindDDLStatus()
                dvHistory.Visible = False
                dvMsgSuccess.Visible = False
                lblMsgSuccess.Visible = False
                dvMsg.Visible = False
                lblMsg.Visible = False
                If _WorkFlowId = 0 Then
                    btnSubmit.Text = "Submit"
                Else
                    btnSubmit.Text = "Update"
                    BindWorkFlow()
                    BindHistory()
                End If
            End If
        End If
    End Sub

    'Bind WorkFlow Controls
    Private Sub BindWorkFlow()
        Try
            Dim sb As StringBuilder = New StringBuilder()
            sb.Append("Select WorkFlowId,Title,ProjID,StageID,StepID,IterationID,AssignedTo,StatusID,EmpId,PlannedStartDt,PlannedEndDt,ActualStartDt,ActualEndDate,")
            sb.Append(" (Select Note from Notes order by NoteId desc Limit 1) as Note From WorkFlow Where Status=@Status And WorkFlowId=@WorkFlowId")
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(sb.ToString(), con)
            cmd.Parameters.AddWithValue("@Status", True)
            cmd.Parameters.AddWithValue("@WorkFlowId", _WorkFlowId)
            Dim dtp As DataTable = New DataTable()
            dtp.Columns.Clear()
            dtp.Clear()
            con.Open()
            dtp.Load(cmd.ExecuteReader())
            con.Close()
            If dtp.Rows.Count > 0 Then
                lblId.Text = Convert.ToString(dtp.Rows(0)("WorkFlowId"))
                txtTitle.Text = Convert.ToString(dtp.Rows(0)("Title"))

                Dim checkProject As String
                checkProject = Convert.ToString(dtp.Rows(0)("ProjID"))
                If checkProject.ToString().Length > 0 Then
                    ddlProject.SelectedValue = Convert.ToInt32(dtp.Rows(0)("ProjID"))
                End If

                Dim checkStageID As String
                checkStageID = Convert.ToString(dtp.Rows(0)("StageID"))
                If checkStageID.ToString().Length > 0 Then
                    BindDDLStage()
                    ddlStage.SelectedValue = Convert.ToInt32(dtp.Rows(0)("StageID"))
                End If

                Dim checkStepID As String
                checkStepID = Convert.ToString(dtp.Rows(0)("StepID"))
                If checkStepID.ToString().Length > 0 Then
                    BindStep()
                    ddlStep.SelectedValue = Convert.ToInt32(dtp.Rows(0)("StepID"))
                End If

                Dim checkIterationID As String
                checkIterationID = Convert.ToString(dtp.Rows(0)("IterationID"))
                If checkIterationID.ToString().Length > 0 Then
                    BindIteration()
                    ddlIteration.SelectedValue = Convert.ToInt32(dtp.Rows(0)("IterationID"))
                End If

                Dim checkAssign As String
                checkAssign = Convert.ToString(dtp.Rows(0)("AssignedTo"))
                If checkAssign.ToString().Length > 0 Then
                    ddlAssignedTo.SelectedValue = Convert.ToInt32(dtp.Rows(0)("AssignedTo"))
                End If

                Dim checkStatus As String
                checkStatus = Convert.ToString(dtp.Rows(0)("StatusID"))
                If checkStatus.ToString().Length > 0 Then
                    ddlStatus.SelectedValue = Convert.ToInt32(dtp.Rows(0)("StatusID"))
                End If

                Dim checkPlannedStartDt As String
                checkPlannedStartDt = Convert.ToString(dtp.Rows(0)("PlannedStartDt"))
                If checkPlannedStartDt.ToString().Length > 0 Then
                    txtPlannedStartDt.Text = Convert.ToDateTime(dtp.Rows(0)("PlannedStartDt")).ToString("yyyy-MM-dd")
                End If

                Dim checkPlannedEndDt As String
                checkPlannedEndDt = Convert.ToString(dtp.Rows(0)("PlannedEndDt"))
                If checkPlannedEndDt.ToString().Length > 0 Then
                    txtPlannedEndDt.Text = Convert.ToDateTime(dtp.Rows(0)("PlannedEndDt")).ToString("yyyy-MM-dd")
                End If

                Dim checkActualStartDt As String
                checkActualStartDt = Convert.ToString(dtp.Rows(0)("ActualStartDt"))
                If checkActualStartDt.ToString().Length > 0 Then
                    txtActualStartDt.Text = Convert.ToDateTime(dtp.Rows(0)("ActualStartDt")).ToString("yyyy-MM-dd")
                End If

                Dim checkActualEndDate As String
                checkActualEndDate = Convert.ToString(dtp.Rows(0)("ActualEndDate"))
                If checkActualEndDate.ToString().Length > 0 Then
                    txtActualEndDate.Text = Convert.ToDateTime(dtp.Rows(0)("ActualEndDate")).ToString("yyyy-MM-dd")
                End If
            Else

            End If
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Private Sub CatchMsg()
        dvMsgSuccess.Visible = False
        lblMsgSuccess.Visible = False
        dvMsg.Visible = True
        lblMsg.Visible = True
    End Sub

    Private Sub BindHistory()
        Try
            Dim sb As StringBuilder = New StringBuilder()
            sb.Append("Select Notes.NoteId,  DATE_FORMAT(CONVERT_TZ(Notes.dt,'-1:30','+12:00'), '%d-%m-%Y %l:%i %p') As Dt, ")
            sb.Append(" Notes.Note As Note, concat(Employees.EmpFirst , ' ', Employees.EmpLast) AS Employee,Attachment.FileName, Attachment.ContentType,")
            sb.Append(" Attachment.Data From notes left outer Join Employees On Employees.EmpID = Notes.EmpId left outer join Attachment on Attachment.NoteId=Notes.NoteId ")
            sb.Append(" Where Notes.WorkFlowId = @WorkFlowId")

            'If Session("Role") = "Employee" Then
            '    sb.Append(" And Notes.EmpId= " & UserId & "")
            'End If
            sb.Append(" Order By Notes.NoteId Desc;")
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(sb.ToString(), con)
            cmd.Parameters.AddWithValue("@WorkFlowId", _WorkFlowId)
            Dim dth As DataTable = New DataTable()
            dth.Columns.Clear()
            dth.Clear()
            con.Open()
            dth.Load(cmd.ExecuteReader())
            con.Close()
            If dth.Rows.Count > 0 Then
                dvHistory.Visible = True
                gvHistory.DataSource = dth
                gvHistory.DataBind()
            End If

        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim pass As Boolean = False
        Try
            dvMsgSuccess.Visible = False
            lblMsgSuccess.Visible = False
            dvMsg.Visible = False
            lblMsg.Visible = False
            If txtTitle.Text.Length = 0 Then
                txtTitle.Focus()
                Throw New Exception("Please enter Title to proceed.")
            End If
            'If ddlAssignedTo.SelectedIndex = 0 Then
            '    ddlAssignedTo.Focus()
            '    Throw New Exception("Please select Assigned To to proceed.")
            'End If
            If btnSubmit.Text = "Submit" Then
                Dim sbInsert As StringBuilder = New StringBuilder()
                sbInsert.Append(" Insert Into WorkFLow (Title, ProjID, StageID, AssignedTo, StatusID,StepID,IterationID, Status, EmpId, dt,PlannedStartDt,PlannedEndDt,ActualStartDt,ActualEndDate) ")
                sbInsert.Append(" Values (@Title,@ProjID,@StageID,@AssignedTo,@StatusID,@StepID,@IterationID,@Status,@EmpId,now(),@PlannedStartDt,@PlannedEndDt,@ActualStartDt,@ActualEndDate ); SELECT LAST_INSERT_ID() as WorkFlowId; ")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sbInsert.ToString(), con)
                cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim)

                If ddlProject.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@ProjID", ddlProject.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@ProjID", DBNull.Value)
                End If
                If ddlStage.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@StageID", ddlStage.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@StageID", DBNull.Value)
                End If
                If ddlAssignedTo.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@AssignedTo", ddlAssignedTo.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@AssignedTo", UserId)
                End If
                If ddlStatus.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@StatusID", ddlStatus.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@StatusID", DBNull.Value)
                End If
                If ddlStep.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@StepID", ddlStep.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@StepID", DBNull.Value)
                End If
                If ddlIteration.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@IterationID", ddlIteration.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@IterationID", DBNull.Value)
                End If
                cmd.Parameters.AddWithValue("@Status", True)
                cmd.Parameters.AddWithValue("@EmpId", UserId)

                If txtPlannedStartDt.Text.Length > 0 Then
                    cmd.Parameters.AddWithValue("@PlannedStartDt", Convert.ToDateTime(txtPlannedStartDt.Text))
                Else
                    cmd.Parameters.AddWithValue("@PlannedStartDt", DBNull.Value)
                End If

                If txtPlannedEndDt.Text.Length > 0 Then
                    cmd.Parameters.AddWithValue("@PlannedEndDt", Convert.ToDateTime(txtPlannedEndDt.Text))
                Else
                    cmd.Parameters.AddWithValue("@PlannedEndDt", DBNull.Value)
                End If

                If txtActualStartDt.Text.Length > 0 Then
                    cmd.Parameters.AddWithValue("@ActualStartDt", Convert.ToDateTime(txtActualStartDt.Text))
                Else
                    cmd.Parameters.AddWithValue("@ActualStartDt", DBNull.Value)
                End If

                If txtActualEndDate.Text.Length > 0 Then
                    cmd.Parameters.AddWithValue("@ActualEndDate", Convert.ToDateTime(txtActualEndDate.Text))
                Else
                    cmd.Parameters.AddWithValue("@ActualEndDate", DBNull.Value)
                End If

                con.Open()
                Dim oRS1 As MySqlDataReader
                oRS1 = cmd.ExecuteReader()
                If oRS1.Read Then
                    _WorkFlowId = oRS1("WorkFlowId")
                    If _WorkFlowId > 0 Then
                        lblId.Text = _WorkFlowId.ToString()
                        InsertNote(True)
                        pass = True
                    End If
                    oRS1.Close()
                End If
                con.Close()
                BindHistory()
                dvMsg.Visible = False
                lblMsg.Visible = False
                Session("SaveMsg") = "Pharma PM Details saved successfully."
                ClearControls()

            ElseIf btnSubmit.Text = "Update" Then

                InsertNote(False)

                Dim sb As StringBuilder = New StringBuilder()
                sb.Append(" Update WorkFLow Set Title=@Title,ProjID=@ProjID,StageID=@StageID,AssignedTo=@AssignedTo,StatusID=@StatusID,StepID=@StepID,IterationID=@IterationID, ")
                sb.Append(" EmpId=@EmpId,Dt=now(),PlannedStartDt=@PlannedStartDt,PlannedEndDt=@PlannedEndDt,ActualStartDt=@ActualStartDt,ActualEndDate=@ActualEndDate Where WorkFlowId=@WorkFlowId")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sb.ToString(), con)
                cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim)

                If ddlProject.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@ProjID", ddlProject.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@ProjID", DBNull.Value)
                End If
                If ddlStage.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@StageID", ddlStage.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@StageID", DBNull.Value)
                End If
                If ddlAssignedTo.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@AssignedTo", ddlAssignedTo.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@AssignedTo", UserId)
                End If
                If ddlStatus.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@StatusID", ddlStatus.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@StatusID", DBNull.Value)
                End If
                If ddlStep.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@StepID", ddlStep.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@StepID", DBNull.Value)
                End If
                If ddlIteration.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@IterationID", ddlIteration.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@IterationID", DBNull.Value)
                End If
                cmd.Parameters.AddWithValue("@EmpId", UserId)

                If txtPlannedStartDt.Text.Length > 0 Then
                    cmd.Parameters.AddWithValue("@PlannedStartDt", Convert.ToDateTime(txtPlannedStartDt.Text))
                Else
                    cmd.Parameters.AddWithValue("@PlannedStartDt", DBNull.Value)
                End If

                If txtPlannedEndDt.Text.Length > 0 Then
                    cmd.Parameters.AddWithValue("@PlannedEndDt", Convert.ToDateTime(txtPlannedEndDt.Text))
                Else
                    cmd.Parameters.AddWithValue("@PlannedEndDt", DBNull.Value)
                End If

                If txtActualStartDt.Text.Length > 0 Then
                    cmd.Parameters.AddWithValue("@ActualStartDt", Convert.ToDateTime(txtActualStartDt.Text))
                Else
                    cmd.Parameters.AddWithValue("@ActualStartDt", DBNull.Value)
                End If

                If txtActualEndDate.Text.Length > 0 Then
                    cmd.Parameters.AddWithValue("@ActualEndDate", Convert.ToDateTime(txtActualEndDate.Text))
                Else
                    cmd.Parameters.AddWithValue("@ActualEndDate", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("@WorkFlowId", _WorkFlowId)
                con.Open()
                Dim i As Integer = cmd.ExecuteNonQuery()
                con.Close()
                If i > 0 Then
                    dvMsg.Visible = False
                    lblMsg.Visible = False
                    dvMsgSuccess.Visible = True
                    lblMsgSuccess.Visible = True
                    lblMsgSuccess.Text = "Pharma PM Updated successfully."
                    BindHistory()
                End If
            End If
            txtNotes.Text = String.Empty
        Catch ex As Exception
            CatchMsg()
            lblMsg.Text = ex.Message
        Finally
            If pass = True Then
                Response.Redirect("PSWorkFlowView.aspx")
            End If
        End Try
    End Sub

    Private Sub InsertNote(ByVal flag As Boolean)
        Try
            Dim sb As StringBuilder = New StringBuilder()
            Dim sbtask As StringBuilder = New StringBuilder()
            Dim sbNote As StringBuilder = New StringBuilder()
            Dim _NoteId As Integer
            If flag = True Then

                ' to select note for new note
                If txtNotes.Text.Length > 0 Then
                    sbNote.Append("Note   " & txtNotes.Text & "<br/>")
                    sbtask.Append("* Title:  " & txtTitle.Text & "<br/>")

                    sbtask.Append("* Link:  http://pharmapm.envigil.net/PSWorkFlow.aspx?wid=" & _WorkFlowId & "  <br/>")
                    sbtask.Append("* " & txtNotes.Text & "<br/>")
                End If
                If txtTitle.Text.Length > 0 Then
                    sbNote.Append("Title: " & txtTitle.Text & "<br/>")
                End If
                If ddlProject.SelectedIndex > 0 Then
                    sbNote.Append("Project: " & ddlProject.SelectedItem.Text & "<br/>")
                End If
                If ddlStage.SelectedIndex > 0 Then
                    sbNote.Append("Stage: " & ddlStage.SelectedItem.Text & "<br/>")
                End If
                If ddlStep.SelectedIndex > 0 Then
                    sbNote.Append("Step: " & ddlStep.SelectedItem.Text & "<br/>")
                End If
                If ddlIteration.SelectedIndex > 0 Then
                    sbNote.Append("Iteration: " & ddlIteration.SelectedItem.Text & "<br/>")
                End If
                ' Dates
                If txtPlannedStartDt.Text.Length > 0 Then
                    sbNote.Append("Planned Start Date: " & Convert.ToDateTime(txtPlannedStartDt.Text).ToString("dd-MM-yyyy") & "<br/>")
                End If
                If txtPlannedEndDt.Text.Length > 0 Then
                    sbNote.Append("Planned End Date: " & Convert.ToDateTime(txtPlannedEndDt.Text).ToString("dd-MM-yyyy") & "<br/>")
                End If
                If txtActualStartDt.Text.Length > 0 Then
                    sbNote.Append("Actual Start Date: " & Convert.ToDateTime(txtActualStartDt.Text).ToString("dd-MM-yyyy") & "<br/>")
                End If
                If txtActualEndDate.Text.Length > 0 Then
                    sbNote.Append("Actual End Date: " & Convert.ToDateTime(txtActualEndDate.Text).ToString("dd-MM-yyyy") & "<br/>")
                End If

                If ddlAssignedTo.SelectedIndex > 0 Then
                    sbNote.Append("Assigned To: " & ddlAssignedTo.SelectedItem.Text & "<br/>")
                End If
                If ddlStatus.SelectedIndex > 0 Then
                    sbNote.Append("Status: " & ddlStatus.SelectedItem.Text & "<br/>")
                End If

                If sbtask.ToString().Length() > 0 Then
                    ' add note for timesheet
                    con = New MySqlConnection(conString)
                    cmd = New MySqlCommand("Insert into taskdetails(WorkFlowID,Task,dt,EmpID) Values(@WorkFlowID,@Task,now(),@EmpID)", con)
                    cmd.Parameters.AddWithValue("@WorkFlowId", _WorkFlowId)
                    cmd.Parameters.AddWithValue("@Task", sbtask.ToString())
                    If ddlAssignedTo.SelectedIndex > 0 Then
                        cmd.Parameters.AddWithValue("@EmpID", ddlAssignedTo.SelectedValue)
                    Else
                        cmd.Parameters.AddWithValue("@EmpID", UserId)
                    End If
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()
                End If

                'Email Code to Create New Ticket
                Dim Gmail As New Gmail()
                Dim subject As String = String.Empty
                Dim body As String
                subject = "Pharma PM - New Task : " + _WorkFlowId.ToString() + " - " + txtTitle.Text
                Dim Link As String
                Link = "http://www.pharmapm.envigil.net/PSWorkFlow.aspx?wid=" & _WorkFlowId.ToString() & ""

                Dim FromEmp As DataTable = New DataTable()
                FromEmp = Employee(UserId)
                body = "<b>Task Created by : </b>" & Convert.ToString(FromEmp.Rows(0)("EmpName")) & " <br/> <br/>" & "<b>Description : </b><br/> " & sbNote.ToString() & " <br/> <br/>" & "<b>Link :</b> " & Link
                Gmail.MailFromName = "Pharma PM"
                Gmail.MailFromAddress = "mail@avantienv.com"
                Gmail.MailFromPassword = "john!manzo"

                'For AssignedTo
                Dim ToMailId As DataTable = New DataTable()
                If ddlAssignedTo.SelectedIndex > 0 Then
                    ToMailId = Employee(ddlAssignedTo.SelectedValue)
                    Gmail.MailToAddress.Add(Convert.ToString(ToMailId.Rows(0)("EmpEmail")))
                    'For  (User who create Ticket) in cc
                    FromEmp = Employee(UserId)
                    Gmail.MailCCAddress.Add(Convert.ToString(FromEmp.Rows(0)("EmpEmail")))
                Else
                    ToMailId = Employee(UserId)
                    Gmail.MailToAddress.Add(Convert.ToString(ToMailId.Rows(0)("EmpEmail")))
                End If

                Gmail.MailSubject = subject
                Gmail.MailBody = body.Replace(System.Environment.NewLine, "<br/>")
                Gmail.Send(True)
            End If
            If flag = False Then

                ' code to get notes for update the task deails
                If txtNotes.Text.Length > 0 Then
                    sbNote.Append("Note " & txtNotes.Text & "<br/>")
                End If
                ' code to append all existing note for timesheer
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand("Select Task from taskdetails where WorkFlowId=@WorkFlowId", con)
                cmd.Parameters.AddWithValue("@WorkFlowId", _WorkFlowId)
                Dim dt As DataTable = New DataTable()
                con.Open()
                dt.Load(cmd.ExecuteReader())
                If dt.Rows.Count > 0 Then
                    sbtask.Append("" & Convert.ToString(dt.Rows(0)("Task")) & "<br/>")
                    If txtNotes.Text.Length > 0 Then
                        sbtask.Append("* " & txtNotes.Text & "<br/>")
                    End If
                    If sbtask.ToString().Length() > 0 Then
                        con = New MySqlConnection(conString)
                        cmd = New MySqlCommand("Update taskdetails set Task=@Task,dt=now(),EmpID=@EmpID Where WorkFlowID=@WorkFlowID", con)
                        cmd.Parameters.AddWithValue("@WorkFlowId", _WorkFlowId)
                        cmd.Parameters.AddWithValue("@Task", sbtask.ToString())
                        If ddlAssignedTo.SelectedIndex > 0 Then
                            cmd.Parameters.AddWithValue("@EmpID", ddlAssignedTo.SelectedValue)
                        Else
                            cmd.Parameters.AddWithValue("@EmpID", UserId)
                        End If
                        con.Open()
                        cmd.ExecuteNonQuery()
                        con.Close()
                    End If
                Else
                    If txtNotes.Text.Length > 0 Then
                        sbtask.Append("* " & txtNotes.Text & "<br/>")
                    End If
                    If sbtask.ToString().Length() > 0 Then
                        con = New MySqlConnection(conString)
                        cmd = New MySqlCommand("Insert into taskdetails(WorkFlowID,Task,dt,EmpID) Values(@WorkFlowID,@Task,now(),@EmpID)", con)
                        cmd.Parameters.AddWithValue("@WorkFlowId", _WorkFlowId)
                        cmd.Parameters.AddWithValue("@Task", sbtask.ToString())
                        If ddlAssignedTo.SelectedIndex > 0 Then
                            cmd.Parameters.AddWithValue("@EmpID", ddlAssignedTo.SelectedValue)
                        Else
                            cmd.Parameters.AddWithValue("@EmpID", UserId)
                        End If
                        con.Open()
                        cmd.ExecuteNonQuery()
                        con.Close()
                    End If
                End If
                con.Close()
                sb.Append(" Select w.WorkFlowId,w.Title,p.ProjName,ph.Stage,st.Step,i.Iteration,concat(e.EmpFirst ,' ', e.EmpLast) as AssignedTo,s.Status,w.PlannedStartDt,w.PlannedEndDt,w.ActualStartDt,w.ActualEndDate  from WorkFlow w left outer join projects p on p.ProjID=w.ProjID left outer join Stage ph on ph.StageID=w.StageID ")
                sb.Append(" left outer join steps st on st.StepID=w.StepID left outer join iteration i on i.IterationID=w.IterationID left outer join Status s on s.StatusID=w.StatusID left outer join Employees e on e.EmpID=w.AssignedTo ")
                sb.Append(" where w.Status=@Status And w.WorkFlowId=@WorkFlowId")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sb.ToString(), con)
                cmd.Parameters.AddWithValue("@Status", True)
                cmd.Parameters.AddWithValue("@WorkFlowId", _WorkFlowId)
                Dim dcheck As DataTable = New DataTable()
                dcheck.Columns.Clear()
                dcheck.Clear()
                con.Open()
                dcheck.Load(cmd.ExecuteReader())
                con.Close()
                If dcheck.Rows.Count > 0 Then
                    If Convert.ToString(dcheck.Rows(0)("Title")) <> txtTitle.Text Then
                        If txtTitle.Text.Length > 0 Then
                            sbNote.Append("Title Changed:  " & Convert.ToString(dcheck.Rows(0)("Title")) & " To " & txtTitle.Text & "<br/>")
                        End If
                    End If

                    If Convert.ToString(dcheck.Rows(0)("ProjName")) <> ddlProject.SelectedItem.Text Then
                        If ddlProject.SelectedIndex > 0 Then
                            sbNote.Append("Project Changed: " & Convert.ToString(dcheck.Rows(0)("ProjName")) & " To " & ddlProject.SelectedItem.Text & "<br/> ")
                        End If
                    End If

                    If Convert.ToString(dcheck.Rows(0)("Stage")) <> ddlStage.SelectedItem.Text Then
                        If ddlStage.SelectedIndex > 0 Then
                            sbNote.Append("Stage Changed:  " & Convert.ToString(dcheck.Rows(0)("Stage")) & " To " & ddlStage.SelectedItem.Text & "<br/> ")
                        End If
                    End If

                    If Convert.ToString(dcheck.Rows(0)("Step")) <> ddlStep.SelectedItem.Text Then
                        If ddlStep.SelectedIndex > 0 Then
                            sbNote.Append("Step Changed:  " & Convert.ToString(dcheck.Rows(0)("Step")) & " To " & ddlStep.SelectedItem.Text & "<br/> ")
                        End If
                    End If

                    If Convert.ToString(dcheck.Rows(0)("Iteration")) <> ddlIteration.SelectedItem.Text Then
                        If ddlIteration.SelectedIndex > 0 Then
                            sbNote.Append("Iteration Changed:  " & Convert.ToString(dcheck.Rows(0)("Iteration")) & " To " & ddlIteration.SelectedItem.Text & "<br/> ")
                        End If
                    End If

                    'Dates
                    Dim PlannedStartDt As String = ""
                    If Convert.ToString(dcheck.Rows(0)("PlannedStartDt")).Length > 0 Then
                        PlannedStartDt = Convert.ToDateTime(dcheck.Rows(0)("PlannedStartDt")).ToString("dd-MM-yyyy")
                    Else
                        PlannedStartDt = ""
                    End If
                    If txtPlannedStartDt.Text.Length > 0 Then
                        If PlannedStartDt <> Convert.ToDateTime(txtPlannedStartDt.Text).ToString("dd-MM-yyyy") Then
                            sbNote.Append("Planned Start Date Changed:  " & PlannedStartDt & " To " & Convert.ToDateTime(txtPlannedStartDt.Text).ToString("dd-MM-yyyy") & "<br/>")
                        End If
                    End If

                    Dim PlannedEndDt As String = ""
                    If Convert.ToString(dcheck.Rows(0)("PlannedEndDt")).Length > 0 Then
                        PlannedEndDt = Convert.ToDateTime(dcheck.Rows(0)("PlannedEndDt")).ToString("dd-MM-yyyy")
                    Else
                        PlannedEndDt = ""
                    End If
                    If txtPlannedEndDt.Text.Length > 0 Then
                        If PlannedEndDt <> Convert.ToDateTime(txtPlannedEndDt.Text).ToString("dd-MM-yyyy") Then
                            sbNote.Append("Planned End Date Changed:  " & PlannedEndDt & " To " & Convert.ToDateTime(txtPlannedEndDt.Text).ToString("dd-MM-yyyy") & "<br/>")
                        End If
                    End If

                    Dim ActualStartDt As String = ""
                    If Convert.ToString(dcheck.Rows(0)("ActualStartDt")).Length > 0 Then
                        ActualStartDt = Convert.ToDateTime(dcheck.Rows(0)("ActualStartDt")).ToString("dd-MM-yyyy")
                    Else
                        ActualStartDt = ""
                    End If
                    If txtActualStartDt.Text.Length > 0 Then
                        If ActualStartDt <> Convert.ToDateTime(txtActualStartDt.Text).ToString("dd-MM-yyyy") Then
                            sbNote.Append("Actual Start Date Changed:  " & ActualStartDt & " To " & Convert.ToDateTime(txtActualStartDt.Text).ToString("dd-MM-yyyy") & "<br/>")
                        End If
                    End If

                    Dim ActualEndDate As String = ""
                    If Convert.ToString(dcheck.Rows(0)("ActualEndDate")).Length > 0 Then
                        ActualEndDate = Convert.ToDateTime(dcheck.Rows(0)("ActualEndDate")).ToString("dd-MM-yyyy")
                    Else
                        ActualEndDate = ""
                    End If
                    If txtActualEndDate.Text.Length > 0 Then
                        If ActualEndDate <> Convert.ToDateTime(txtActualEndDate.Text).ToString("dd-MM-yyyy") Then
                            sbNote.Append("Actual End Date Changed:  " & ActualEndDate & " To " & Convert.ToDateTime(txtActualEndDate.Text).ToString("dd-MM-yyyy") & "<br/>")
                        End If
                    End If

                    If Convert.ToString(dcheck.Rows(0)("AssignedTo")) <> ddlAssignedTo.SelectedItem.Text Then
                        If ddlAssignedTo.SelectedIndex > 0 Then
                            sbNote.Append("Assigned To Changed: " & Convert.ToString(dcheck.Rows(0)("AssignedTo")) & " To " & ddlAssignedTo.SelectedItem.Text & "<br/> ")
                        End If
                    End If

                    If Convert.ToString(dcheck.Rows(0)("Status")) <> ddlStatus.SelectedItem.Text Then
                        If ddlStatus.SelectedIndex > 0 Then
                            sbNote.Append("Status Changed: " & Convert.ToString(dcheck.Rows(0)("Status")) & " To " & ddlStatus.SelectedItem.Text & "<br/> ")
                        End If
                    End If

                    If FuFile.HasFile Then
                        Dim PostedFile As HttpPostedFile = FuFile.PostedFile
                        If PostedFile.ContentLength > 0 Then
                            Dim filename As String = Path.GetFileName(PostedFile.FileName)
                            sbNote.Append("New file " & filename & " is added<br/>")
                        End If
                    End If
                End If

                Dim Gmail As New Gmail()
                Dim subject As String = String.Empty
                Dim body As String
                Dim FromEmp As DataTable = New DataTable()
                FromEmp = Employee(UserId)
                subject = "Pharma PM - Task Updated Note : " + _WorkFlowId.ToString() + " - " + txtTitle.Text
                Dim Link As String
                Link = "http://www.pharmapm.envigil.net/PSWorkFlow.aspx?wid=" & _WorkFlowId.ToString() & ""

                body = "<b>Task Updated by : </b>" & Convert.ToString(FromEmp.Rows(0)("EmpName")) & " <br/> <br/>" & "<b>Description : </b> <br/>" & sbNote.ToString() & " <br/><br/>" & " <b> Link : </b> " & Link.ToString()
                Gmail.MailFromName = "Pharma PM"
                Gmail.MailFromAddress = "mail@avantienv.com"
                Gmail.MailFromPassword = "john!manzo"

                Dim ToMailId As DataTable = New DataTable()
                Dim CreatedEmpId As Integer = GetEmpId()
                If CreatedEmpId = UserId Then
                    'For AssignedTo
                    If ddlAssignedTo.SelectedIndex > 0 Then
                        ToMailId = Employee(ddlAssignedTo.SelectedValue)
                        Gmail.MailToAddress.Add(Convert.ToString(ToMailId.Rows(0)("EmpEmail")))
                        FromEmp = Employee(UserId)
                        Gmail.MailCCAddress.Add(Convert.ToString(FromEmp.Rows(0)("EmpEmail")))
                    Else
                        FromEmp = Employee(UserId)
                        Gmail.MailToAddress.Add(Convert.ToString(FromEmp.Rows(0)("EmpEmail")))
                    End If

                ElseIf ddlAssignedTo.SelectedValue = UserId Then
                    'For Created by if Assigned logged in
                    ToMailId = Employee(CreatedEmpId)
                    Gmail.MailToAddress.Add(Convert.ToString(ToMailId.Rows(0)("EmpEmail")))
                    FromEmp = Employee(UserId)
                    Gmail.MailCCAddress.Add(Convert.ToString(FromEmp.Rows(0)("EmpEmail")))
                Else
                    ' if not Created By not Assigned TO (CC User)

                    If ddlAssignedTo.SelectedIndex > 0 Then
                        ToMailId = Employee(ddlAssignedTo.SelectedValue)
                        Gmail.MailToAddress.Add(ToMailId.Rows(0)("EmpEmail"))
                        FromEmp = Employee(UserId)
                        Gmail.MailCCAddress.Add(Convert.ToString(FromEmp.Rows(0)("EmpEmail")))
                    Else
                        FromEmp = Employee(UserId)
                        Gmail.MailCCAddress.Add(Convert.ToString(FromEmp.Rows(0)("EmpEmail")))
                    End If
                End If
                Gmail.MailSubject = subject
                Gmail.MailBody = body.Replace(System.Environment.NewLine, "<br/>")
                Gmail.Send(True)

            End If
            If sbNote.ToString.Length > 0 Then
                Dim constr As String = ConfigurationManager.ConnectionStrings("dbcNoeth").ConnectionString
                Using con As New MySqlConnection(constr)
                    Dim query As String = "Insert into Notes (WorkFlowId,ProjID,Note,EmpId,Dt) values (@WorkFlowId,@ProjID,@Note,@EmpId,Now()); SELECT LAST_INSERT_ID() as NoteId"
                    Using cmd As New MySqlCommand(query)
                        cmd.Connection = con
                        cmd.Parameters.AddWithValue("@WorkFlowId", _WorkFlowId)
                        If ddlProject.SelectedIndex > 0 Then
                            cmd.Parameters.AddWithValue("@ProjID", ddlProject.SelectedValue)
                        Else
                            cmd.Parameters.AddWithValue("@ProjID", DBNull.Value)
                        End If
                        cmd.Parameters.AddWithValue("@Note", sbNote.ToString().Replace(System.Environment.NewLine, "<br/>"))
                        cmd.Parameters.AddWithValue("@EmpId", UserId)
                        con.Open()
                        Dim oRSNote As MySqlDataReader
                        oRSNote = cmd.ExecuteReader()
                        If oRSNote.Read Then
                            _NoteId = oRSNote("NoteId")
                            If _NoteId > 0 Then
                                If FuFile.HasFile Then
                                    Dim PostedFile As HttpPostedFile = FuFile.PostedFile
                                    If PostedFile.ContentLength > 0 Then
                                        Dim filename As String = Path.GetFileName(PostedFile.FileName)
                                        Dim contentType As String = PostedFile.ContentType
                                        Using fs As Stream = PostedFile.InputStream
                                            Using br As New BinaryReader(fs)
                                                Dim bytes As Byte() = br.ReadBytes(Convert.ToInt32(fs.Length))
                                                Dim constr1 As String = ConfigurationManager.ConnectionStrings("dbcNoeth").ConnectionString
                                                Using con1 As New MySqlConnection(constr1)
                                                    Dim query1 As String = "insert into Attachment(NoteId,WorkFlowId,FileName,ContentType,Data,EmpId,Dt) values (@NoteId,@WorkFlowId,@FileName, @ContentType,@Data,@EmpId,Now())"
                                                    Using cmd1 As New MySqlCommand(query1)
                                                        cmd1.Connection = con1
                                                        cmd1.Parameters.AddWithValue("@NoteId", _NoteId)
                                                        cmd1.Parameters.AddWithValue("@WorkFlowId", _WorkFlowId)
                                                        cmd1.Parameters.AddWithValue("@FileName", filename)
                                                        cmd1.Parameters.AddWithValue("@ContentType", contentType)
                                                        cmd1.Parameters.AddWithValue("@Data", bytes)
                                                        cmd1.Parameters.AddWithValue("@EmpId", UserId)
                                                        con1.Open()
                                                        cmd1.ExecuteNonQuery()
                                                        con1.Close()
                                                    End Using
                                                End Using
                                            End Using
                                        End Using
                                    End If
                                End If
                            End If
                            oRSNote.Close()
                        End If
                        con.Close()
                    End Using
                End Using
            End If
        Catch ex As Exception
            dvMsg.Visible = True
            lblMsg.Text = ex.Message
        End Try
    End Sub

    Private Sub ClearControls()
        txtTitle.Text = String.Empty
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

        ddlAssignedTo.SelectedIndex = 0
        ddlStatus.SelectedIndex = 0
        txtNotes.Text = String.Empty
        dvMsg.Visible = False
        lblMsg.Visible = False
        dvMsgSuccess.Visible = False
        lblMsgSuccess.Visible = False

        txtPlannedStartDt.Text = String.Empty
        txtPlannedEndDt.Text = String.Empty
        txtActualStartDt.Text = String.Empty
        txtActualEndDate.Text = String.Empty
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        ClearControls()
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
    Protected Sub gvHistory_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If e.CommandName = "getFile" Then
            Try
                Dim id As Integer = CInt(e.CommandArgument.ToString())
                Dim bytes As Byte()
                Dim fileName As String, contentType As String
                Dim conString As String = ConfigurationManager.ConnectionStrings("dbcNoeth").ConnectionString
                Using con As New MySqlConnection(conString)
                    Using cmd As New MySqlCommand()
                        cmd.CommandText = "select FileName,ContentType,Data from Attachment where NoteId=@Id"
                        cmd.Parameters.AddWithValue("@Id", id)
                        cmd.Connection = con
                        con.Open()
                        Using sdr As MySqlDataReader = cmd.ExecuteReader()
                            sdr.Read()
                            fileName = sdr("FileName").ToString()
                            contentType = sdr("ContentType").ToString()
                            bytes = DirectCast(sdr("Data"), Byte())
                        End Using
                        con.Close()
                    End Using
                End Using
                Response.Clear()
                Response.Buffer = True
                Response.Charset = ""
                Response.Cache.SetCacheability(HttpCacheability.NoCache)
                Response.ContentType = contentType
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName)
                Response.BinaryWrite(bytes)
                Response.Flush()
                Response.End()
            Catch ex As Exception
                dvMsg.Visible = True
                lblMsg.Text = ex.Message
            End Try

        End If
    End Sub

    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        Session.Abandon()
        Session("EmpId") = Nothing
        Response.Redirect("~/Login.aspx")
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

    Public Function Employee(ByVal EmpID) As DataTable
        Try
            Dim Query As String
            dt.Clear()
            Query = "Select concat(EmpFirst,' ',EmpLast) As EmpName,EmpEmail from employees where Status=true and EmpID=" & EmpID & ";"
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(Query, con)
            dt = New DataTable()
            dt.Columns.Clear()
            dt.Clear()
            con.Open()
            dt.Load(cmd.ExecuteReader())
            con.Close()
        Catch ex As Exception
            dvMsg.Visible = True
            lblMsg.Text = ex.Message
        End Try
        Return dt
    End Function

    Private Function GetEmpId() As Integer
        Dim FromEmpId As Integer
        Try
            Dim Query As String
            Query = "Select EmpID from workflow where WorkFlowId=" & _WorkFlowId & " and Status=true;"
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(Query, con)
            con.Open()
            FromEmpId = cmd.ExecuteScalar()
            con.Close()
        Catch ex As Exception
            Session("Alert") = ex.Message
        End Try
        Return FromEmpId
    End Function
End Class