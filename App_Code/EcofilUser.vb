Imports Npgsql

Public Class EcofilUser
    Private mIdUtente As Long
    Private mIdUtenteEncrypted As String
    Private mNomeUtente As String
    Private mIdComune As List(Of Integer)
    Private mIdNucleo As Long
    Private mNominativoNucleo As String
    Private mCodiceNucleo As String
    Private mCodiceTag As List(Of String)
    Private mAddress As String
    Private mAddressUser As String
    Private mAddressOther As String
    Private mFlagSend As String
    Private mPhone As String
    Private mMobile As String    
    Private mEmail As String
    Private mCompostiera As Boolean
    Private mPannolini As Boolean
    Private mLatitudine As Double
    Private mLongitudine As Double
    Private mUserType As UserTypeEnum
    Private mSessionId As String
    Private mLoggedOn As Boolean

    Sub New()
        mIdUtente = 0L
        mIdUtenteEncrypted = String.Empty
        mNomeUtente = String.Empty
        mIdComune = New List(Of Integer)
        mIdNucleo = 0L
        mNominativoNucleo = String.Empty
        mCodiceNucleo = String.Empty
        mCodiceTag = New List(Of String)
        mAddress = String.Empty
        mAddressUser = String.Empty
        mAddressOther = String.Empty
        mFlagSend = "0"
        mPhone = String.Empty
        mMobile = String.Empty        
        mEmail = String.Empty
        mCompostiera = False
        mPannolini = False
        mSessionId = String.Empty
        mLoggedOn = False
    End Sub

    ReadOnly Property IdUtente As Long
        Get
            Return mIdUtente
        End Get
    End Property

    ReadOnly Property IdUtenteEncrypted As String
        Get
            Return mIdUtenteEncrypted
        End Get
    End Property

    Property NomeUtente As String
        Get
            Return mNomeUtente
        End Get

        Set(value As String)
            mNomeUtente = value
        End Set
    End Property

    ReadOnly Property IdComune As List(Of Integer)
        Get
            Return mIdComune
        End Get
    End Property

    ReadOnly Property IdNucleo As Long
        Get
            Return mIdNucleo
        End Get
    End Property

    ReadOnly Property NominativoNucleo As String
        Get
            Return mNominativoNucleo
        End Get
    End Property

    ReadOnly Property CodiceNucleo As String
        Get
            Return mCodiceNucleo
        End Get
    End Property

    ReadOnly Property CodiceTag As List(Of String)
        Get
            Return mCodiceTag
        End Get
    End Property

    Property Address As String
        Get
            Return mAddress
        End Get
        Set(value As String)
            mAddress = value
        End Set
    End Property

    Property AddressUser As String
        Get
            Return mAddressUser
        End Get
        Set(value As String)
            mAddressUser = value
        End Set
    End Property

    Property AddressOther As String
        Get
            Return mAddressOther
        End Get
        Set(value As String)
            mAddressOther = value
        End Set
    End Property

    Property FlagSend As String
        Get
            Return mFlagSend
        End Get
        Set(value As String)
            mFlagSend = value
        End Set
    End Property

    Property Phone As String
        Get
            Return mPhone
        End Get
        Set(value As String)
            mPhone = value
        End Set
    End Property

    Property Mobile As String
        Get
            Return mMobile
        End Get
        Set(value As String)
            mMobile = value
        End Set
    End Property

    Property Email As String
        Get
            Return mEmail
        End Get
        Set(value As String)
            mEmail = value
        End Set
    End Property

    Property Compostiera As Boolean
        Get
            Return mCompostiera
        End Get
        Set(value As Boolean)
            mCompostiera = value
        End Set
    End Property

    Property Pannolini As Boolean
        Get
            Return mPannolini
        End Get
        Set(value As Boolean)
            mPannolini = value
        End Set
    End Property

    ReadOnly Property Latitudine As Double
        Get
            Return mLatitudine
        End Get
    End Property

    ReadOnly Property Longitudine As Double
        Get
            Return mLongitudine
        End Get
    End Property

    ReadOnly Property UserType As UserTypeEnum
        Get
            Return mUserType
        End Get
    End Property

    ReadOnly Property SessionId As String
        Get
            Return mSessionId
        End Get
    End Property

    ReadOnly Property LoggedOn As Boolean
        Get
            Return mLoggedOn
        End Get
    End Property

    Public Sub ConnectUser(ByRef userRecord As NpgsqlDataReader, ByVal sessionId As String)
        Try
            Integer.TryParse(userRecord("id").ToString, mIdUtente)
            mIdUtenteEncrypted = FunctionsClass.EncryptValue(mIdUtente)
            mNomeUtente = userRecord("username").ToString
            mEmail = userRecord("mail").ToString
            mNominativoNucleo = userRecord("nome").ToString & " " & userRecord("cognome").ToString            
            Double.TryParse(userRecord("latitudine").ToString, mLatitudine)
            Double.TryParse(userRecord("longitudine").ToString, mLongitudine)
            mUserType = CType(userRecord("usertype"), Integer)
            mSessionId = sessionId

            If UserType = 1 Then
                Integer.TryParse(userRecord("idnucleo").ToString, mIdNucleo)
                mCodiceNucleo = userRecord("codicenucleo").ToString
                mAddress = userRecord("indirizzo").ToString
                mAddressUser = userRecord("indirizzoutenza").ToString
                mAddressOther = userRecord("indirizzoalternativo").ToString
                mFlagSend = userRecord("flaginvio").ToString
                mPhone = userRecord("telefono").ToString
                mMobile = userRecord("cellulare").ToString
                Boolean.TryParse(userRecord("compostiera").ToString, mCompostiera)
                Boolean.TryParse(userRecord("pannolini").ToString, mPannolini)
            End If

            mLoggedOn = True
        Catch ex As Exception
            mLoggedOn = False
        End Try        
    End Sub

    Public Sub AddTag(ByVal tag As String)
        mCodiceTag.Add(tag.ToLower)
    End Sub

    Public Sub AddCity(ByVal idCity As Integer)
        mIdComune.Add(idCity)
    End Sub

    Public Enum UserTypeEnum
        NotDefined
        User
        Administration
        Manager
    End Enum
End Class
