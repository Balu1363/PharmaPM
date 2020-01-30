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
                Else
                    showadmin.Visible = False
                End If
                BindDDLProject()
                BindDDLPhase()
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

    Private Sub BindWorkFlow()
        Try
            Dim sb As StringBuilder = New StringBuilder()
            sb.Append("Select WorkFlowId,Title,ProjID,PhaseID,AssignedTo,StatusID,EmpId,Dt,")
            sb.Append(" (Select Note from Notes order by NoteId desc Limit 1) as Note From WorkFlow Where Status =@Status And WorkFlowId=@WorkFlowId")
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
                Dim checkPhaseID As String
                checkPhaseID = Convert.ToString(dtp.Rows(0)("PhaseID"))
                If checkPhaseID.ToString().Length > 0 Then
                    ddlPhase.SelectedValue = Convert.ToInt32(dtp.Rows(0)("PhaseID"))
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
            sb.Append("Select Notes.NoteId,  DATE_FORMAT(CONVERT_TZ(Notes.dt,'-1:30','+12:00'), '%m/%d/%Y %l:%i %p') As Dt, ")
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
                sbInsert.Append(" Insert Into WorkFLow (Title, ProjID, PhaseID, AssignedTo, StatusID, Status, EmpId, dt) ")
                sbInsert.Append(" Values (@Title,@ProjID,@PhaseID,@AssignedTo,@StatusID,@Status,@EmpId,now() ); SELECT LAST_INSERT_ID() as WorkFlowId; ")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sbInsert.ToString(), con)
                cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim)

                If ddlProject.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@ProjID", ddlProject.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@ProjID", DBNull.Value)
                End If
                If ddlPhase.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@PhaseID", ddlPhase.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@PhaseID", DBNull.Value)
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
                cmd.Parameters.AddWithValue("@Status", True)
                cmd.Parameters.AddWithValue("@EmpId", UserId)

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
                sb.Append(" Update WorkFLow Set Title=@Title,ProjID=@ProjID,PhaseID=@PhaseID, ")
                sb.Append(" AssignedTo=@AssignedTo,StatusID=@StatusID,EmpId=@EmpId,Dt=now() Where WorkFlowId=@WorkFlowId")
                con = New MySqlConnection(conString)
                cmd = New MySqlCommand(sb.ToString(), con)
                cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim)

                If ddlProject.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@ProjID", ddlProject.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@ProjID", DBNull.Value)
                End If
                If ddlPhase.SelectedIndex > 0 Then
                    cmd.Parameters.AddWithValue("@PhaseID", ddlPhase.SelectedValue)
                Else
                    cmd.Parameters.AddWithValue("@PhaseID", DBNull.Value)
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
                cmd.Parameters.AddWithValue("@EmpId", UserId)
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
                If txtNotes.Text.Length > 0 Then
                    sbNote.Append("Note :  " & txtNotes.Text & "<br/>")
                    sbtask.Append("* Title : " & txtTitle.Text & "<br/>")

                    sbtask.Append("* Link : http://pharmapm.envigil.net/PSWorkFlow.aspx?wid=" & _WorkFlowId & "  <br/>")
                    sbtask.Append("* " & txtNotes.Text & "<br/>")
                End If
                If txtTitle.Text.Length > 0 Then
                    sbNote.Append("Title: " & txtTitle.Text & "<br/>")
                End If
                If ddlProject.SelectedIndex > 0 Then
                    sbNote.Append("Project: " & ddlProject.SelectedItem.Text & "<br/>")
                End If
                If ddlPhase.SelectedIndex > 0 Then
                    sbNote.Append("Phase: " & ddlPhase.SelectedItem.Text & "<br/>")
                End If
                If ddlAssignedTo.SelectedIndex > 0 Then
                    sbNote.Append("Assigned To: " & ddlAssignedTo.SelectedItem.Text & "<br/>")
                End If
                If ddlStatus.SelectedIndex > 0 Then
                    sbNote.Append("Status: " & ddlStatus.SelectedItem.Text & "<br/>")
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


            If flag = False Then
                If txtNotes.Text.Length > 0 Then
                    sbNote.Append("Note: " & txtNotes.Text & "<br/>")
                End If

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

                sb.Append(" Select w.WorkFlowId,w.Title,p.ProjName,ph.Phase,concat(e.EmpFirst ,' ', e.EmpLast) as AssignedTo,s.Status from WorkFlow w left outer join projects p on p.ProjID=w.ProjID left outer join ProjectPhase ph on ph.PhaseID=w.PhaseID ")
                sb.Append("  left outer join Status s on s.StatusID=w.StatusID left outer join Employees e on e.EmpID=w.AssignedTo ")
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

                    If Convert.ToString(dcheck.Rows(0)("Phase")) <> ddlPhase.SelectedItem.Text Then
                        If ddlPhase.SelectedIndex > 0 Then
                            sbNote.Append("Phase Changed:  " & Convert.ToString(dcheck.Rows(0)("Phase")) & " To " & ddlPhase.SelectedItem.Text & "<br/> ")
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
        ddlPhase.SelectedIndex = 0
        ddlAssignedTo.SelectedIndex = 0
        ddlStatus.SelectedIndex = 0
        txtNotes.Text = String.Empty
        dvMsg.Visible = False
        lblMsg.Visible = False
        dvMsgSuccess.Visible = False
        lblMsgSuccess.Visible = False
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
End Class