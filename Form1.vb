Public Class Form1
    Dim labels(10, 10) As Label ' 8 baris 8 kolom
    Dim textBoxes(9, 10) As TextBox '9 baris 10 kolom
    Dim currentRow As Integer = 0
    Dim currentCol As Integer = 0
    Dim timer As New Timer()
    Dim correctCount As Integer = 0
    Dim wrongCount As Integer = 0
    Dim changeColorCount As Integer = 0
    Dim SessionCount As Integer = 0
    'Total
    Dim TotalTrue As Integer = 0
    Dim TotalFalse As Integer = 0
    Dim TotalAll As Integer = 0
    Dim TempFalse As Integer = 0
    Dim GlobalAnswerCount As New List(Of Integer)()
    'array untuk menyimpan hasil persesi
    Public TrueResults As New List(Of Integer)()
    Public FalseResults As New List(Of Integer)()
    Public SessionResult As New List(Of List(Of Integer))

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.AutoScroll = True
        Me.WindowState = FormWindowState.Maximized
        InitializeRandomNumbers()
        CreateLabels()
        CreateTextBoxes()
        ' Konfigurasi timer
        timer.Interval = 10000 'dalam milidetik
        AddHandler timer.Tick, AddressOf Timer_Tick
        timer.Start()
    End Sub

    Private Sub InitializeRandomNumbers()
        Dim rand As New Random()
        For row As Integer = 0 To 9
            For col As Integer = 0 To 9
                labels(row, col) = New Label With {
                    .Text = rand.Next(1, 10).ToString()
                }
            Next
        Next
    End Sub

    Private Sub CreateLabels()
        Dim labelWidth As Integer = 50
        Dim labelHeight As Integer = 50
        Dim labelTop As Integer = 12
        Dim labelLeft As Integer = 9
        Dim labelRowSpacing As Integer = 120
        Dim labelColSpacing As Integer = 110

        For row As Integer = 0 To 9
            For col As Integer = 0 To 9
                labels(row, col).Width = labelWidth
                labels(row, col).Height = labelHeight
                labels(row, col).Top = labelTop + row * labelRowSpacing
                labels(row, col).Left = labelLeft + col * labelColSpacing
                labels(row, col).TextAlign = ContentAlignment.MiddleCenter
                labels(row, col).AutoSize = False
                labels(row, col).BorderStyle = BorderStyle.FixedSingle
                labels(row, col).Font = New Font("TimesNewsRoman", 12, FontStyle.Bold)
                Me.Controls.Add(labels(row, col))
            Next
        Next
    End Sub


    Private Sub CreateTextBoxes()
        Dim textBoxWidth As Integer = 50
        Dim textBoxHeight As Integer = 50
        Dim textBoxTop As Integer = 75
        Dim textBoxLeft As Integer = 55
        Dim textBoxRowSpacing As Integer = 120
        Dim textBoxColSpacing As Integer = 110

        For row As Integer = 0 To 8
            For col As Integer = 0 To 9
                textBoxes(row, col) = New TextBox With {
                    .Width = textBoxWidth,
                    .Height = textBoxHeight,
                    .Top = textBoxTop + row * textBoxRowSpacing,
                    .Left = textBoxLeft + col * textBoxColSpacing,
                    .TextAlign = HorizontalAlignment.Center,
                    .BorderStyle = BorderStyle.FixedSingle,
                    .Font = New Font("TimesNewsRoman", 12, FontStyle.Bold)
                }
                AddHandler textBoxes(row, col).KeyDown, AddressOf TextBox_KeyDown
                Me.Controls.Add(textBoxes(row, col))
            Next
        Next
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs)
        changeColorCount += 1
        If changeColorCount <= 3 Then
            Dim currentTextBox As TextBox = textBoxes(currentRow, currentCol)
            If currentTextBox.BackColor = Color.Red Then
                currentTextBox.BackColor = Color.White
            Else
                currentTextBox.BackColor = Color.Red
                timer.Stop()
                CalculateResults()
                DisplaySession()
                timer.Start()
            End If
        Else
            timer.Stop()
            ResultShow()
        End If
    End Sub

    Private Sub TextBox_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True

            If currentRow < 8 Then
                currentRow += 1
            Else
                currentRow = 0
                If currentCol < 9 Then
                    currentCol += 1
                Else
                    currentCol = 0
                End If
            End If
            textBoxes(currentRow, currentCol).Focus()
        End If
    End Sub

    Private Sub CalculateResults()
        Dim sum As Integer = GlobalAnswerCount.Sum()
        Dim TempCount As Integer = 0
        Dim TrueCountSession As Integer = 0
        Dim FalseCountSession As Integer = 0
        Dim LocalAnswerCount As Integer = 0
        For col As Integer = 0 To 9
            For row As Integer = 0 To 8
                If (TempCount >= sum) Then
                    Dim firstNumber As Integer = labels(row, col).Text
                    Dim secondNumber As Integer = labels(row + 1, col).Text
                    Dim expectedAnswer As Integer = firstNumber + secondNumber

                    Dim userAnswer As Integer
                    If Integer.TryParse(textBoxes(row, col).Text, userAnswer) Then
                        If userAnswer = expectedAnswer OrElse (expectedAnswer > 9 AndAlso userAnswer = expectedAnswer Mod 10) Then
                            TrueCountSession += 1
                        Else
                            FalseCountSession += 1
                        End If
                        LocalAnswerCount += 1
                    End If
                End If
                TempCount += 1
            Next
        Next
        TotalTrue += TrueCountSession
        TotalFalse += FalseCountSession
        TotalAll = TotalTrue + TotalFalse
        GlobalAnswerCount.add(LocalAnswerCount)
        SessionResult.add(New List(Of Integer)({TrueCountSession, FalseCountSession}))
    End Sub

    Private Sub DisplaySession()
        SessionCount += 1
        If SessionCount >= 1 AndAlso SessionCount <= SessionResult.Count Then
            Dim i As Integer = SessionCount - 1
            MessageBox.Show($"Benar : {SessionResult(i)(0)}{vbCrLf}Salah : {SessionResult(i)(1)}{vbCrLf}Total : {SessionResult(i)(0) + SessionResult(i)(1)}", $"Hasil Sesi Ke - {SessionCount}", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
        Else
            MessageBox.Show("Nomor sesi tidak valid.", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub
    Private Sub ResultShow()
        Dim message1 As String = "Hasil Persesi:" & vbCrLf
        For i As Integer = 0 To SessionResult.Count - 1
            message1 &= $"Session {i + 1},Benar: {SessionResult(i)(0)}, Salah: {SessionResult(i)(1)}, Total: {SessionResult(i)(0) + SessionResult(i)(1)}" & vbCrLf
        Next
        Dim message As String = $"Total Salah Keseluruhan : {TotalFalse}" & vbCrLf &
                                $"Total Benar Keseluruhan : {TotalTrue}" & vbCrLf &
                                $"Total Input keseluruhan : {TotalAll}"
        If TotalAll < 7 Then
            MessageBox.Show(message1 & vbCrLf & message, "Hasil Keseluruhan salah", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ElseIf SessionCount >= 2 AndAlso SessionCount <= 3 AndAlso TotalAll > 10 Then
            Dim totalTrueSesi As Integer = 0
            Dim totalFalseSesi As Integer = 0
            For j As Integer = 1 To 2 ' sesi 2 sampai sesi 3
                If j < SessionResult.Count Then
                    totalFalseSesi += SessionResult(j)(1) ' Sesuai indeks array, kurangi 1 dari nomor sesi
                    totalTrueSesi += SessionResult(j)(0)
                End If
            Next
            MessageBox.Show($"{message1}{vbCrLf}{message}{vbCrLf}Total salah dari Sesi 2 sampai Sesi 3: {totalFalseSesi}{vbCrLf}Total benar dari Sesi 2 sampai Sesi 3: {totalTrueSesi}", "Hasil Keseluruhan dihitung", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        '' Memeriksa kondisi untuk menghitung kesalahan berdasarkan total pengerjaan
        'If TotalAll < 1000 Then
        '    ' Jika total pengerjaan < 1000, hitung semua kesalahan
        '    MessageBox.Show(message1 & vbCrLf & message, "Hasil Keseluruhan salah", MessageBoxButtons.OK, MessageBoxIcon.Information)
        'ElseIf SessionCount >= 12 AndAlso SessionCount <= 20 AndAlso TotalAll > 3000 Then
        '    ' Jika total pengerjaan > 3000 dan sesi berada di antara 12 dan 20, hitung hanya kesalahan dari sesi 12 hingga 20
        '    Dim totalTrueSesi As Integer = 0
        '    Dim totalFalseSesi As Integer = 0
        '    For j As Integer = 11 To 19 ' sesi 2 sampai sesi 3
        '        If j < SessionResult.Count Then
        '            totalFalseSesi += SessionResult(j)(1) ' Sesuai indeks array, kurangi 1 dari nomor sesi
        '            totalTrueSesi += SessionResult(j)(0)
        '        End If
        '    Next
        '    MessageBox.Show($"{message1}{vbCrLf}{message}{vbCrLf}Total salah dari Sesi 12 sampai Sesi 20: {totalFalseSesi}{vbCrLf}Total benar dari Sesi 12 sampai Sesi 20: 
        '                    {totalTrueSesi}", "Hasil Keseluruhan dihitung", MessageBoxButtons.OK, MessageBoxIcon.Information)
        'End If
        For row As Integer = 0 To 8
            For col As Integer = 0 To 9
                textBoxes(row, col).Enabled = False
            Next
        Next
    End Sub
End Class
