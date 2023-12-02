Imports System
Imports System.IO
Imports System.Web
Imports System.Collections
Imports Npgsql

Public Class PostgreConnector
    Implements IDisposable

    '** Stringa comando SQL
    Private mSQLString As String

    '** Oggetto Connection MySQL
    Private mSQLConnection As NpgsqlConnection

    '** Oggetto Command MySQL
    Private mSQLCommand As NpgsqlCommand

    '** Hashtable Data Reader MySQL
    Private hashSQLDataReader As New Hashtable

    '** Parametro connection status
    Private mStatus As Boolean

    '** Parametro connection timeout
    Private mTimeOut As Integer

    Sub New(ByVal connectionString As String, Optional ByVal timeOut As Integer = 0)
        Try
            mSQLConnection = New NpgsqlConnection(connectionString)
            mSQLConnection.Open()

            mStatus = True
            mTimeOut = timeOut
        Catch ex As Exception
            mStatus = False
            Dim sw As New StreamWriter(HttpContext.Current.Request.PhysicalApplicationPath & "temp\log-" & Now.ToString("yyyy-MM-dd-HH-mm-ss") & ".log")
            sw.WriteLine("-----------------------")
            sw.WriteLine(ex)
            sw.WriteLine("-----------------------")
            sw.Close()
            sw.Dispose()
        End Try
    End Sub

    Public ReadOnly Property Status As Boolean
        Get
            Return mStatus
        End Get
    End Property

    Public Function AddReader(ByVal tablename As String, ByVal sql As String) As Boolean
        Try
            If hashSQLDataReader.Count > 0 Then
                For Each reader As DictionaryEntry In hashSQLDataReader
                    CType(reader.Value, NpgsqlDataReader).Close()
                Next
            End If

            If Not hashSQLDataReader.ContainsKey(tablename) Then
                mSQLCommand = New NpgsqlCommand(sql, mSQLConnection)
                If mTimeOut > 0 Then mSQLCommand.CommandTimeout = mTimeOut
                Dim SQLReader As NpgsqlDataReader = mSQLCommand.ExecuteReader
                mSQLCommand.Dispose()

                If SQLReader.HasRows Then
                    hashSQLDataReader.Add(tablename, SQLReader)

                    Return True
                Else
                    SQLReader.Close()
                    Return False
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Sub DelReader(ByVal tablename As String)
        Try
            If hashSQLDataReader.ContainsKey(tablename) Then
                CType(hashSQLDataReader(tablename), NpgsqlDataReader).Close()
                hashSQLDataReader.Remove(tablename)
            End If
        Catch ex As Exception
            '...
        End Try
    End Sub

    Public Function GetData(ByVal tablename As String) As NpgsqlDataReader
        Try
            If hashSQLDataReader.ContainsKey(tablename) Then
                If CType(hashSQLDataReader(tablename), NpgsqlDataReader).Read Then
                    Return CType(hashSQLDataReader(tablename), NpgsqlDataReader)
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetRecordCount(ByVal SQLString As String) As Integer
        Dim intTotalRecord As Integer = 0

        If mStatus Then
            Try
                If hashSQLDataReader.Count > 0 Then
                    For Each reader As DictionaryEntry In hashSQLDataReader
                        CType(reader.Value, NpgsqlDataReader).Close()
                    Next
                End If

                mSQLCommand = New NpgsqlCommand(SQLString, mSQLConnection)
                If mTimeOut > 0 Then mSQLCommand.CommandTimeout = mTimeOut
                intTotalRecord = mSQLCommand.ExecuteScalar()
                mSQLCommand.Dispose()
            Catch ex As Exception
                Return 0
            End Try
        Else
            Return 0
        End If

        Return intTotalRecord
    End Function

    Public Function ExecuteScalar(ByVal SQLString As String) As Integer
        Dim intResult As Integer = 0

        If mStatus Then
            Try
                If hashSQLDataReader.Count > 0 Then
                    For Each reader As DictionaryEntry In hashSQLDataReader
                        CType(reader.Value, NpgsqlDataReader).Close()
                    Next
                End If

                mSQLCommand = New NpgsqlCommand(SQLString, mSQLConnection)
                If mTimeOut > 0 Then mSQLCommand.CommandTimeout = mTimeOut
                intResult = mSQLCommand.ExecuteScalar()
                mSQLCommand.Dispose()

                Return intResult
            Catch ex As Exception
                Return intResult
            End Try
        Else
            Return intResult
        End If
    End Function

    Public Function ExecuteCommand(ByVal SQLString As String) As Boolean
        If mStatus Then
            Try
                If hashSQLDataReader.Count > 0 Then
                    For Each reader As DictionaryEntry In hashSQLDataReader
                        CType(reader.Value, NpgsqlDataReader).Close()
                    Next
                End If

                mSQLCommand = New NpgsqlCommand(SQLString, mSQLConnection)
                mSQLCommand.ExecuteNonQuery()
                mSQLCommand.Dispose()

                Return True
            Catch ex As Exception
                Return False
            End Try
        Else
            Return False
        End If
    End Function

    Public Function ExecuteCommandWithIdBack(ByVal SQLString As String) As Integer
        Dim intResult As Integer = 0
        If mStatus Then
            Try
                mSQLCommand = New NpgsqlCommand(SQLString, mSQLConnection)
                intResult = CType(mSQLCommand.ExecuteScalar().ToString, Integer)
                mSQLCommand.Dispose()

                Return intResult
            Catch ex As Exception
                Return intResult
            End Try
        Else
            Return intResult
        End If
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        Try
            If hashSQLDataReader.Count > 0 Then
                For Each reader As DictionaryEntry In hashSQLDataReader
                    Try
                        CType(reader.Value, NpgsqlDataReader).Close()
                    Catch ex As Exception
                        '...
                    End Try
                Next

                hashSQLDataReader.Clear()
            End If
            mSQLConnection.Close()
        Catch ex As Exception
            '...
        Finally
            GC.Collect()
        End Try
    End Sub

End Class
