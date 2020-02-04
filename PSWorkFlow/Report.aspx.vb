Imports MySql.Data.MySqlClient
Imports System.Web.UI.DataVisualization.Charting
Public Class Report
    Inherits System.Web.UI.Page
    Dim dt As DataTable = New DataTable()
    Dim cmd As MySqlCommand = New MySqlCommand()
    Dim con As MySqlConnection = New MySqlConnection()
    Dim conString As String = ConfigurationManager.ConnectionStrings("dbcNoeth").ConnectionString
    Dim _WorkFlowId As Integer
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
                    showadmin.Visible = True
                    showadd.Visible = True
                Else
                    showadmin.Visible = False
                    showadd.Visible = False
                End If
                BindDDLProject()
                dvMsg.Visible = False
                lblMsg.Visible = False
                imgGanttChart.Visible = False
            End If
        End If
    End Sub

    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        Session.Abandon()
        Session("EmpId") = Nothing
        Response.Redirect("~/Login.aspx")
    End Sub

    Private Sub BindChart()
        Try
            Dim Query As String
            dt.Clear()
            Query = "Select stage.stage,Year(workflow.Dt) Year from stage inner join workflow on stage.StageID=workflow.StageID where workflow.ProjID=" & ddlProject.SelectedValue & " and stage.Status=true Order By stage.Ord;"
            con = New MySqlConnection(conString)
            cmd = New MySqlCommand(Query, con)
            dt = New DataTable()
            dt.Columns.Clear()
            dt.Clear()
            con.Open()
            dt.Load(cmd.ExecuteReader())
            con.Close()
            If dt.Rows.Count > 0 Then
                Dim stage As List(Of String) = (From p In dt.AsEnumerable()
                                                Select p.Field(Of String)("stage")).Distinct().ToList()

                'Loop through the Countries.
                For Each country As String In stage

                    'Get the Year for each Country.
                    Dim x As Integer() = (From p In dt.AsEnumerable()
                                          Where p.Field(Of String)("stage") = country
                                          Order By p.Field(Of Integer)("Year")
                                          Select p.Field(Of Integer)("Year")).ToArray()

                    'Get the Total of Orders for each Country.
                    Dim y As Integer() = (From p In dt.AsEnumerable()
                                          Where p.Field(Of String)("stage") = country
                                          Order By p.Field(Of Integer)("Year")
                                          Select p.Field(Of Integer)("Year")).ToArray()

                    'Add Series to the Chart.
                    Chart1.Series.Add(New Series(country))
                    Chart1.Series(country).IsValueShownAsLabel = True
                    Chart1.Series(country).ChartType = SeriesChartType.StackedBar
                    Chart1.Series(country).Points.DataBindXY(x, y)
                Next

                Chart1.Legends(0).Enabled = True
            Else
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

    Protected Sub ddlProject_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlProject.SelectedIndex > 0 Then
            imgGanttChart.Visible = True
            lblName.Text = "Gantt Chart for - " & ddlProject.SelectedItem.Text
            BindChart()
        Else
            imgGanttChart.Visible = False
            lblName.Text = ""
        End If
    End Sub
End Class