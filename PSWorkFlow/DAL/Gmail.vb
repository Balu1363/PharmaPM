Imports System.IO
Imports System.Net.Mail
Imports System.Text.RegularExpressions
Public Class Gmail
    Private _MailFromAddress As String
    Private _MailFromPassword As String
    Private _MailFromName As String
    Private _MailToAddress As New ArrayList
    Private _MailCCAddress As New ArrayList
    Private _MailBccAddress As New ArrayList
    Private _MailSubject As String
    Private _MailBody As String
    Private _MailAttachments As New ArrayList
    Sub New()
    End Sub
    Public Function Send(Optional ByVal HTML As Boolean = False) As Boolean
        Dim MailServer As New SmtpClient
        Dim MailMessage As New MailMessage
        Dim MailAddress As New Regex("\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
        MailServer.Credentials = New Net.NetworkCredential(_MailFromAddress, _MailFromPassword)
        MailServer.EnableSsl = True
        MailServer.Host = "smtp.gmail.com"
        MailServer.Port = 587
        MailMessage.IsBodyHtml = True
        MailMessage.From = New MailAddress(_MailFromAddress, _MailFromName, System.Text.Encoding.UTF8)
        For i As Integer = 0 To _MailToAddress.Count - 1
            If MailAddress.IsMatch(_MailToAddress(i).ToString) Then
                MailMessage.To.Add(_MailToAddress(i).ToString)
            End If
        Next
        For i As Integer = 0 To _MailCCAddress.Count - 1
            If MailAddress.IsMatch(_MailCCAddress(i).ToString) Then
                MailMessage.CC.Add(_MailCCAddress(i).ToString)
            End If
        Next
        For i As Integer = 0 To _MailBccAddress.Count - 1
            If MailAddress.IsMatch(_MailBccAddress(i).ToString) Then
                MailMessage.Bcc.Add(_MailBccAddress(i).ToString)
            End If
        Next
        MailMessage.Subject = _MailSubject
        MailMessage.Body = _MailBody
        MailMessage.IsBodyHtml = HTML
        If _MailAttachments.Count > 0 Then
            For i As Integer = 0 To _MailAttachments.Count - 1
                If File.Exists(_MailAttachments(i).ToString) Then
                    Dim MailAttachment As New Attachment(_MailAttachments(i).ToString)
                    MailMessage.Attachments.Add(MailAttachment)
                End If
            Next
        End If
        MailServer.Send(MailMessage)
        MailMessage.Dispose()
        Return True
    End Function
    Public Property MailFromAddress() As String
        Get
            Return _MailFromAddress
        End Get
        Set(ByVal Value As String)
            _MailFromAddress = Value
        End Set
    End Property
    Public Property MailFromPassword() As String
        Get
            Return _MailFromPassword
        End Get
        Set(ByVal Value As String)
            _MailFromPassword = Value
        End Set
    End Property
    Public Property MailFromName() As String
        Get
            Return _MailFromName
        End Get
        Set(ByVal Value As String)
            _MailFromName = Value
        End Set
    End Property
    Public Property MailToAddress() As ArrayList
        Get
            Return _MailToAddress
        End Get
        Set(ByVal Value As ArrayList)
            _MailToAddress.Add(Value)
        End Set
    End Property
    Public Property MailCCAddress() As ArrayList
        Get
            Return _MailCCAddress
        End Get
        Set(ByVal Value As ArrayList)
            _MailCCAddress.Add(Value)
        End Set
    End Property
    Public Property MailBccAddress() As ArrayList
        Get
            Return _MailBccAddress
        End Get
        Set(ByVal Value As ArrayList)
            _MailBccAddress.Add(Value)
        End Set
    End Property
    Public Property MailSubject() As String
        Get
            Return _MailSubject
        End Get
        Set(ByVal Value As String)
            _MailSubject = Value
        End Set
    End Property
    Public Property MailBody() As String
        Get
            Return _MailBody
        End Get
        Set(ByVal Value As String)
            _MailBody = Value
        End Set
    End Property
    Public Property MailAttachments() As ArrayList
        Get
            Return _MailAttachments
        End Get
        Set(ByVal Value As ArrayList)
            _MailAttachments = Value
        End Set
    End Property
End Class