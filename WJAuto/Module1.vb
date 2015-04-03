
' To learn more about Microsoft Azure WebJobs, please see http://go.microsoft.com/fwlink/?LinkID=401557
Module Module1

    Sub Main()

        Dim TagEventsAdapter As New DataSet1TableAdapters.TagEventsAfterAlertTableAdapter
        Dim TagEventsTable As New DataSet1.TagEventsAfterAlertDataTable
        Dim TagEventsRow As DataSet1.TagEventsAfterAlertRow

        Dim CommonCounter As Integer

        Dim ActiveAlertsAdapt As New DataSet1TableAdapters.TagAlertsActiveTableAdapter
        Dim ActiveAlertsTable As New DataSet1.TagAlertsActiveDataTable
        Dim ActiveAlertsRow As DataSet1.TagAlertsActiveRow
        Dim AutoHandleAdapt As New DataSet1TableAdapters.InsertAutoHandle
        Dim LastRoom As Integer


        ActiveAlertsTable = ActiveAlertsAdapt.GetData()

        For Each ActiveAlertsRow In ActiveAlertsTable
            TagEventsTable = TagEventsAdapter.GetData(ActiveAlertsRow.TagId, ActiveAlertsRow.Timestamp)
            CommonCounter = 0
            LastRoom = 0
            For Each TagEventsRow In TagEventsTable
                If TagEventsRow.RoomType <> 4 Then
                    If TagEventsRow.RoomId <> LastRoom Then
                        CommonCounter = CommonCounter + 1
                        LastRoom = TagEventsRow.RoomId
                    End If
                Else
                    CommonCounter = 0
                End If

                If CommonCounter = 2 Then
                    ' create auto handle
                    AutoHandleAdapt.InsertQuery(ActiveAlertsRow.Id, 1164)  ' Insert Alert Tag Status
                    AutoHandleAdapt.InsertTagAlertResponse(ActiveAlertsRow.Id, 1164)
                    AutoHandleAdapt.UpdateTagAlertActive(ActiveAlertsRow.Id)

                    Console.WriteLine("AutoHandle " & Now() & " AlertId=" & ActiveAlertsRow.Id)
                    Exit For
                End If

            Next
        Next


    End Sub


End Module
