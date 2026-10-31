Imports System

Public Class DispatchRecord
    Public Property Id As Integer
    Public Property VchNo As String = ""
    Public Property PartyName As String = ""
    Public Property PartyHindi As String = ""
    Public Property DispatchDate As String = ""
    Public Property Nag As String = ""
    Public Property GivenGadiNo As String = ""
    Public Property ActualGadiNo As String = ""
    Public Property CreatedAt As DateTime = DateTime.Now
    Public Property UpdatedAt As DateTime = DateTime.Now
End Class
