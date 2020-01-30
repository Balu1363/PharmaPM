
Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)
        ' Fires when the application is started

    End Sub

    Sub Session_Start(sender As Object, e As EventArgs)
        If Session("EmpId") IsNot Nothing Then
            Response.Redirect("~/PSWorkFlowView.aspx")
        Else
            Response.Redirect("~/Login.aspx")
        End If
    End Sub
End Class