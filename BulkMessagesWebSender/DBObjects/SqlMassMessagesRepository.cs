using BulkMessagesWebServer.DBObjects.CreateMessengerDialog;
using BulkMessagesWebServer.DBObjects.MassMessages;
using BulkMessagesWebServer.DBObjects.MessengerDialog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer.DBObjects
{
    public sealed class SqlMassMessagesRepository : IMassMessagesRepository
    {
        private readonly string m_ConnectionString;
        //--------------------------------------------------------------------------------------------------
        public SqlMassMessagesRepository(string _ConnectionString)
        {
            m_ConnectionString = _ConnectionString;
        }
        //--------------------------------------------------------------------------------------------------
        public void AddNewMesUmnicoSender(int _PersonId, string _Text, string _Guid, string _ImageName, string _BranchName)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @PER_ID INT = 4995;
--DECLARE @MES_UMN_SEND_TEXT NVARCHAR(2000) = 'Тест';
--DECLARE @MES_UMN_SEND_GUID CHAR(36) = '7a93e9a3-ce8b-4b4c-937f-239e33c50b2d';
--DECLARE @MES_UMN_SEND_IMG_NAME NVARCHAR(1000) = 'Image1.jpg';
--DECLARE @BRANCH_NAME NVARCHAR(100) = 'Simplex';

INSERT INTO MES_UMNICO_SENDER(MES_UMN_SEND_STAT_TYPE_ID, MES_UMN_SEND_GUID, 
MES_UMN_SEND_TEXT, PER_ID, MES_UMN_SEND_IMG_NAME, BRANCH_NAME)
VALUES
	(1, @MES_UMN_SEND_GUID, @MES_UMN_SEND_TEXT, @PER_ID, @MES_UMN_SEND_IMG_NAME, @BRANCH_NAME);
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("PER_ID", System.Data.SqlDbType.Int)
            {
                Value = _PersonId
            });
            _Command.Parameters.Add(new SqlParameter("MES_UMN_SEND_TEXT", System.Data.SqlDbType.NVarChar, 2000)
            {
                Value = _Text
            });
            _Command.Parameters.Add(new SqlParameter("MES_UMN_SEND_GUID", System.Data.SqlDbType.Char, 36)
            {
                Value = _Guid
            });
            _Command.Parameters.Add(new SqlParameter("BRANCH_NAME", System.Data.SqlDbType.NVarChar, 100)
            {
                Value = _BranchName
            });
            _Command.Parameters.Add(new SqlParameter("MES_UMN_SEND_IMG_NAME", System.Data.SqlDbType.NVarChar, 1000));

            if (_ImageName == null)
            {
                _Command.Parameters["MES_UMN_SEND_IMG_NAME"].Value = DBNull.Value;
            }
            else
            {
                _Command.Parameters["MES_UMN_SEND_IMG_NAME"].Value = _ImageName;
            }

            Execute(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public void DeleteMesUmnicoSender(int _MesUmnSendId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_UMN_SEND_ID INT = 1;

DELETE MES_UMNICO_SENDER
WHERE MES_UMN_SEND_ID = @MES_UMN_SEND_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_UMN_SEND_ID", System.Data.SqlDbType.Int)
            {
                Value = _MesUmnSendId
            });

            Execute(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public int? GetMessageLogIdByGuidAndPersonId(string _Guid, int _PersonId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_EXT_ID VARCHAR(50) = '19e65a53-8364-4784-bc70-a21e4e31b54f';
--DECLARE @PER_ID INT = 4995;

WITH MES_BAT AS (SELECT MES_BAT_ID
FROM MES_BATCH
WHERE MES_BAT_EXT_ID = @MES_EXT_ID)

SELECT MES_LOG_ID
FROM MES_LOG
WHERE MES_BAT_ID = (SELECT MES_BAT_ID FROM MES_BAT) AND
PER_ID = @PER_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_EXT_ID", System.Data.SqlDbType.VarChar, 50)
            {
                Value = _Guid
            });
            _Command.Parameters.Add(new SqlParameter("PER_ID", System.Data.SqlDbType.Int)
            {
                Value = _PersonId
            });

            int? _Result = Execute<int>(_Command);

            if (_Result != null && _Result > 0)
            {
                return _Result;
            }


            _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_EXT_ID VARCHAR(50) = '19e65a53-8364-4784-bc70-a21e4e31b54f';

SELECT MES_LOG_ID
FROM MES_LOG
WHERE MES_LOG_EXT_ID = @MES_EXT_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_EXT_ID", System.Data.SqlDbType.VarChar, 50)
            {
                Value = _Guid
            });

            _Result = Execute<int>(_Command);

            return _Result;
        }
        //--------------------------------------------------------------------------------------------------
        public IEnumerable<Sender> GetSendersWhichNeedSendMessages(string _BranchName)
        {
            List<Sender> _Senders = new List<Sender>();
            
            using (SqlConnection _Connection = new SqlConnection(m_ConnectionString))
            {
                _Connection.Open();

                using (SqlCommand _Command = new SqlCommand())
                {
                    _Command.Connection = _Connection;
                    
                    _Command.CommandText = @"
--DECLARE @BRANCH_CODE INT = 12343423;

SELECT MES_UMN_SEND_ID, MES_UMN_SEND_STAT_TYPE_ID, PER_ID, MES_UMN_SEND_GUID, MES_UMN_SEND_TEXT, BRANCH_NAME, MES_UMN_SEND_IMG_NAME
FROM MES_UMNICO_SENDER
WHERE BRANCH_NAME = @BRANCH_NAME AND MES_UMN_SEND_STAT_TYPE_ID = 1
                    ";

                    _Command.CommandType = System.Data.CommandType.Text;
                    _Command.Parameters.Add(new SqlParameter("BRANCH_NAME", System.Data.SqlDbType.NVarChar, 100)
                    {
                        Value = _BranchName
                    });

                    using (SqlDataReader reader = _Command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Sender _Sender = new Sender()
                                {
                                    Id = reader.GetInt32(0),
                                    SenderStatusTypeId = reader.GetInt32(1),
                                    PersonId = reader.GetInt32(2),
                                    Guid = reader.GetString(3),
                                    Text = reader.GetString(4),
                                    BranchName = reader.GetString(5)
                                };

                                if (!reader.IsDBNull(6))
                                {
                                    _Sender.ImageName = reader.GetString(6);
                                }

                                _Senders.Add(_Sender);
                            }
                        }
                    }
                }
            }

            return _Senders;
        }
        //--------------------------------------------------------------------------------------------------
        public bool HasSendersWhichNeedSendMessages(string _BranchName)
        {
            int _CountSendersWhichNeedSendMessages = 0;
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @BRANCH_CODE INT = 1223412;

SELECT COUNT(*)
FROM MES_UMNICO_SENDER
WHERE BRANCH_NAME = @BRANCH_NAME AND MES_UMN_SEND_STAT_TYPE_ID = 1
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("BRANCH_NAME", System.Data.SqlDbType.NVarChar, 100)
            {
                Value = _BranchName
            });

            object _QueryResult = Execute<int>(_Command);

            if (_QueryResult != null)
            {
                _CountSendersWhichNeedSendMessages = (int)_QueryResult;
            }

            return _CountSendersWhichNeedSendMessages > 0;
        }
        //--------------------------------------------------------------------------------------------------
        public void SetErrorInMessageLogById(int _MessageLogId, string _ErrorMessage)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_LOG_ID INT = 5244;
--DECLARE @MES_LOG_SEND_ERROR VARCHAR(500) = 'Ошибка!';

UPDATE MES_LOG
SET MES_SEND_STATE_ID = 3, MES_LOG_SEND_ERROR = @MES_LOG_SEND_ERROR
WHERE MES_LOG_ID = @MES_LOG_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_LOG_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessageLogId
            });
            _Command.Parameters.Add(new SqlParameter("MES_LOG_SEND_ERROR", System.Data.SqlDbType.VarChar, 500)
            {
                Value = _ErrorMessage
            });

            Execute(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public void UpdateMessageLogSendStateIdById(int _MesssageLogId, int _NewMessageLogSendStateId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_LOG_ID INT = 5244;
--DECLARE @MES_SEND_STATE_ID INT = 2;

UPDATE MES_LOG
SET MES_SEND_STATE_ID = @MES_SEND_STATE_ID
WHERE MES_LOG_ID = @MES_LOG_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_LOG_ID", System.Data.SqlDbType.Int)
            {
                Value = _MesssageLogId
            });
            _Command.Parameters.Add(new SqlParameter("MES_SEND_STATE_ID", System.Data.SqlDbType.Int)
            {
                Value = _NewMessageLogSendStateId
            });

            Execute(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public Tuple<int, string> CreateSourceTypeData(string _MessengerTypeName, int _StartIDToFind = 1)
        {
            using (SqlConnection _Connection = new SqlConnection(m_ConnectionString))
            {
                _Connection.Open();

                using (SqlCommand _Command = new SqlCommand())
                {
                    _Command.Connection = _Connection;

                    _Command.CommandText = @"
--DECLARE @MESSENGER_TYPE_NAME VARCHAR(100) = 'Telegram';
--DECLARE @START_MESSENGER_TYPE_ID_TO_FIND INT = 1;

SELECT TOP(1) ST.SOURCE_TYPE_ID, ST.SOURCE_TYPE_UID
FROM SOURCE_TYPE AS ST
JOIN MESSENGER_TYPE AS MT ON ST.MESSENGER_TYPE_ID = MT.MESSENGER_TYPE_ID
WHERE MT.MESSENGER_TYPE_NAME = @MESSENGER_TYPE_NAME AND ST.SOURCE_TYPE_ID >= @START_MESSENGER_TYPE_ID_TO_FIND;
                    ";

                    _Command.CommandType = System.Data.CommandType.Text;
                    _Command.Parameters.Add(new SqlParameter("MESSENGER_TYPE_NAME", System.Data.SqlDbType.VarChar, 100)
                    {
                        Value = _MessengerTypeName
                    });
                    _Command.Parameters.Add(new SqlParameter("START_MESSENGER_TYPE_ID_TO_FIND", System.Data.SqlDbType.Int)
                    {
                        Value = _StartIDToFind
                    });

                    using (SqlDataReader reader = _Command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            return Tuple.Create(reader.GetInt32(0), reader.GetString(1));
                        }
                    }
                }

                
            }

            return null;
        }
        //--------------------------------------------------------------------------------------------------
        public string GetPhoneNumberFromPersonId(int _PersonId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @PER_ID INT = 4995;

SELECT A.ADDR_PHONE
FROM ADDRESS AS A
JOIN PERSON AS P ON A.ADDR_ID = P.PER_LIVE_ADDR
WHERE P.PER_ID = @PER_ID AND A.ADDR_PHONE IS NOT NULL AND LEN(A.ADDR_PHONE) >= 10;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("PER_ID", System.Data.SqlDbType.Int)
            {
                Value = _PersonId
            });

            return Execute<string>(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public IEnumerable<MessengerDialogDataModel> GetPersonMessengerTypes(int _PersonId, IEnumerable<string> _SupportedMessengerTypes)
        {
            List<MessengerDialogDataModel> _PersonMessengerTypes = new List<MessengerDialogDataModel>();

            using (SqlConnection _Connection = new SqlConnection(m_ConnectionString))
            {
                _Connection.Open();

                using (SqlCommand _Command = new SqlCommand())
                {
                    _Command.Connection = _Connection;

                    _Command.CommandText = @"
--DECLARE @PER_ID INT = 4995;
--DECLARE @SUPPORTED_MESSENGER_TYPES VARCHAR(1000) = 'WhatsApp Telegram Вконтакте';

SELECT MT.MESSENGER_TYPE_ID, MT.MESSENGER_TYPE_NAME,
CASE WHEN MT.MESSENGER_TYPE_ID = P.MESSENGER_TYPE_ID THEN 1 ELSE 0 END AS IS_PRIORITY,
ST.SOURCE_TYPE_ID, ST.SOURCE_TYPE_UID
FROM PERSON AS P
JOIN PERSON_MESSENGER_TYPES AS PMT ON P.PER_ID = PMT.PER_ID
JOIN MESSENGER_TYPE AS MT ON PMT.MESSENGER_TYPE_ID = MT.MESSENGER_TYPE_ID
JOIN SOURCE_TYPE AS ST ON MT.MESSENGER_TYPE_ID = ST.MESSENGER_TYPE_ID
WHERE P.PER_ID = @PER_ID AND @SUPPORTED_MESSENGER_TYPES LIKE '%' + MT.MESSENGER_TYPE_NAME + '%';
                    ";

                    _Command.CommandType = System.Data.CommandType.Text;
                    _Command.Parameters.Add(new SqlParameter("PER_ID", System.Data.SqlDbType.Int)
                    {
                        Value = _PersonId
                    });
                    _Command.Parameters.Add(new SqlParameter("SUPPORTED_MESSENGER_TYPES", System.Data.SqlDbType.VarChar, 1000)
                    {
                        Value = string.Join(" ", _SupportedMessengerTypes)
                    });

                    using (SqlDataReader reader = _Command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int _MessengerTypeId = reader.GetInt32(0);
                                string _MessengerTypeName = reader.GetString(1);
                                bool _IsPriority = reader.GetInt32(2) == 1;
                                int _SourceTypeId = reader.GetInt32(3);
                                string _SourceTypeUId = reader.GetString(4);

                                _PersonMessengerTypes.Add(new MessengerDialogDataModel(_MessengerTypeId, _MessengerTypeName)
                                {
                                    IsPriority = _IsPriority,
                                    SourceTypeId = _SourceTypeId,
                                    SourceTypeUId = _SourceTypeUId
                                });
                            }
                        }
                    }
                }
            }

            return _PersonMessengerTypes;
        }
        //--------------------------------------------------------------------------------------------------
        public IEnumerable<MessengerDialogDataModel> GetMessengerTypesWhereDialogIsExists(int _PersonId, string _PersonPhoneNumber)
        {
            List<MessengerDialogDataModel> _MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>();

            using (SqlConnection _Connection = new SqlConnection(m_ConnectionString))
            {
                _Connection.Open();

                using (SqlCommand _Command = new SqlCommand())
                {
                    _Command.Connection = _Connection;

                    _Command.CommandText = @"
--DECLARE @PER_ID INT = 4995;
--DECLARE @PER_PHONE_NUMBER NVARCHAR(100) = '9042194323';

(SELECT DISTINCT MT.MESSENGER_TYPE_ID, MT.MESSENGER_TYPE_NAME, MD.MES_DIAL_ID, 
MD.MES_DIAL_EXTERNAL_ID, ST.SOURCE_TYPE_ID,
CASE WHEN (
SELECT COUNT(MDM2.MES_DIAL_MES_ID) 
FROM MESSENGER_DIALOG_MESSAGE AS MDM2
WHERE MDM2.MES_DIAL_ID = MD.MES_DIAL_ID AND
MDM2.MES_TYPE_ID = 1
) > 0 THEN 1 ELSE 0 END AS HAS_MES_DIAL_MESSAGES_FROM_PERSON
FROM MESSENGER_TYPE AS MT
JOIN SOURCE_TYPE AS ST ON MT.MESSENGER_TYPE_ID = ST.MESSENGER_TYPE_ID
JOIN PERSON_MESSENGER_TYPES AS PMT ON MT.MESSENGER_TYPE_ID = PMT.MESSENGER_TYPE_ID
JOIN PERSON AS P ON PMT.PER_ID = P.PER_ID
JOIN MESSENGER_DIALOG AS MD ON PMT.PER_MES_TYPE_ID = MD.PER_MES_TYPE_ID
WHERE P.PER_ID = @PER_ID AND
MD.MES_DIAL_TYPE_ID = 1 AND
MD.MES_DIAL_EXTERNAL_ID IS NOT NULL)
UNION
(SELECT MT.MESSENGER_TYPE_ID, MT.MESSENGER_TYPE_NAME, MD.MES_DIAL_ID, 
MD.MES_DIAL_EXTERNAL_ID, ST.SOURCE_TYPE_ID,
CASE WHEN (
SELECT COUNT(MDM2.MES_DIAL_MES_ID) 
FROM MESSENGER_DIALOG_MESSAGE AS MDM2
WHERE MDM2.MES_DIAL_ID = MD.MES_DIAL_ID AND
MDM2.MES_TYPE_ID = 1
) > 0 THEN 1 ELSE 0 END AS HAS_MES_DIAL_MESSAGES_FROM_PERSON
FROM MESSENGER_DIALOG AS MD
JOIN SOURCE_TYPE AS ST ON MD.SOURCE_TYPE_ID = ST.SOURCE_TYPE_ID
JOIN MESSENGER_TYPE AS MT ON ST.MESSENGER_TYPE_ID = MT.MESSENGER_TYPE_ID
WHERE @PER_PHONE_NUMBER IS NOT NULL AND
(MD.MES_DIAL_CLIENT_LOGIN LIKE '%' + @PER_PHONE_NUMBER + '%' OR
MD.MES_DIAL_CLIENT_PHONE LIKE '%' + @PER_PHONE_NUMBER + '%') AND
MD.MES_DIAL_TYPE_ID = 1 AND 
MD.MES_DIAL_EXTERNAL_ID IS NOT NULL AND
MD.PER_MES_TYPE_ID IS NULL);
                    ";

                    _Command.CommandType = System.Data.CommandType.Text;
                    _Command.Parameters.Add(new SqlParameter("PER_ID", System.Data.SqlDbType.Int)
                    {
                        Value = _PersonId
                    });
                    _Command.Parameters.Add(new SqlParameter("PER_PHONE_NUMBER", System.Data.SqlDbType.NVarChar, 100));
                    if (_PersonPhoneNumber == null)
                    {
                        _Command.Parameters["PER_PHONE_NUMBER"].Value = DBNull.Value;
                    }
                    else
                    {
                        _Command.Parameters["PER_PHONE_NUMBER"].Value = _PersonPhoneNumber;
                    }

                    using (SqlDataReader reader = _Command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int _MessengerTypeId = reader.GetInt32(0);
                                string _MessengerTypeName = reader.GetString(1);
                                int _MessengerDialogId = reader.GetInt32(2);
                                string _MessengerDialogUId = reader.GetString(3);
                                int _SourceTypeId = reader.GetInt32(4);
                                bool _HasMessengerDialogMessagesFromPerson = reader.GetInt32(5) == 1;

                                _MessengerTypesWhereDialogIsExists.Add(new MessengerDialogDataModel(_MessengerTypeId, _MessengerTypeName)
                                {
                                    MessengerDialogId = _MessengerDialogId,
                                    MessengerDialogUId = _MessengerDialogUId,
                                    SourceTypeId = _SourceTypeId,
                                    HasMessengerDialogMessagesFromPerson = _HasMessengerDialogMessagesFromPerson
                                });
                            }
                        }
                    }
                }
            }

            return _MessengerTypesWhereDialogIsExists.Distinct();
        }
        //--------------------------------------------------------------------------------------------------
        public int GetMessengerTypeIdBySourceTypeId(int _SourceTypeId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @SOURCE_TYPE_ID INT = 1;

SELECT MESSENGER_TYPE_ID
FROM SOURCE_TYPE
WHERE SOURCE_TYPE_ID = @SOURCE_TYPE_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("SOURCE_TYPE_ID", System.Data.SqlDbType.Int)
            {
                Value = _SourceTypeId
            });

            return Execute<int>(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public int GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(int _PersonId, int _MessengerTypeId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @PER_ID INT = 4995;
--DECLARE @MESSENGER_TYPE_ID INT = 1;

SELECT PER_MES_TYPE_ID
FROM PERSON_MESSENGER_TYPES
WHERE PER_ID = @PER_ID AND MESSENGER_TYPE_ID = @MESSENGER_TYPE_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("PER_ID", System.Data.SqlDbType.Int)
            {
                Value = _PersonId
            });
            _Command.Parameters.Add(new SqlParameter("MESSENGER_TYPE_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessengerTypeId
            });

            return Execute<int>(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public bool PersonMessengerTypeIdInMessengerDialogIsEmpty(int _MessengerDialogId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_DIAL_ID INT = 240;

SELECT PER_MES_TYPE_ID
FROM MESSENGER_DIALOG
WHERE MES_DIAL_ID = @MES_DIAL_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_DIAL_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessengerDialogId
            });

            return Execute<int>(_Command) == 0;
        }
        //--------------------------------------------------------------------------------------------------
        public void AddPersonMessengerType(int _PersonId, int _MessengerTypeId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @PER_ID INT = 4995;
--DECLARE @MESSENGER_TYPE_ID INT = 1;

INSERT INTO PERSON_MESSENGER_TYPES(PER_ID, MESSENGER_TYPE_ID)
VALUES
	(@PER_ID, @MESSENGER_TYPE_ID);
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("PER_ID", System.Data.SqlDbType.Int)
            {
                Value = _PersonId
            });
            _Command.Parameters.Add(new SqlParameter("MESSENGER_TYPE_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessengerTypeId
            });

            Execute(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public void AddMessengerDialog(int _SourceTypeId, string _PhoneNumber, DateTime _CreationDateTime, int? _PersonMessengerTypeId = null)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @SOURCE_TYPE_ID INT = 1;
--DECLARE @PHONE_NUMBER NVARCHAR(100) = '9042194323';
--DECLARE @CREATION_DATETIME DATETIME = GETUTCDATE();
--DECLARE @PER_MES_TYPE_ID INT = NULL;

INSERT INTO MESSENGER_DIALOG(SOURCE_TYPE_ID, MES_DIAL_TYPE_ID, MES_DIAL_STAT_TYPE_ID, MES_DIAL_CREATE_DATE,
	MES_DIAL_CLIENT_LOGIN, MES_DIAL_CLIENT_PHONE, MES_DIAL_MESSAGE_IS_READ, PER_MES_TYPE_ID)
VALUES
	(@SOURCE_TYPE_ID, 1, 1, @CREATION_DATETIME, @PHONE_NUMBER, @PHONE_NUMBER, 0, @PER_MES_TYPE_ID);
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("SOURCE_TYPE_ID", System.Data.SqlDbType.Int)
            {
                Value = _SourceTypeId
            });
            _Command.Parameters.Add(new SqlParameter("PHONE_NUMBER", System.Data.SqlDbType.NVarChar, 100)
            {
                Value = _PhoneNumber
            });
            _Command.Parameters.Add(new SqlParameter("CREATION_DATETIME", System.Data.SqlDbType.DateTime)
            {
                Value = _CreationDateTime
            });
            _Command.Parameters.Add(new SqlParameter("PER_MES_TYPE_ID", System.Data.SqlDbType.Int));

            if (_PersonMessengerTypeId == null)
            {
                _Command.Parameters["PER_MES_TYPE_ID"].Value = DBNull.Value;
            }
            else
            {
                _Command.Parameters["PER_MES_TYPE_ID"].Value = (int)_PersonMessengerTypeId;
            }

            Execute(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public int GetMessengerDialogIdBySourceTypeIdAndPhoneNumberAndCreationDateTime(int _SourceTypeId, string _PhoneNumber, 
            DateTime _CreationDateTime)
        {
            using (SqlConnection _Connection = new SqlConnection(m_ConnectionString))
            {
                _Connection.Open();

                using (SqlCommand _Command = new SqlCommand())
                {
                    _Command.Connection = _Connection;

                    _Command.CommandText = @"
--DECLARE @SOURCE_TYPE_ID INT = 1;
--DECLARE @PHONE_NUMBER NVARCHAR(100) = '9042194323';

SELECT MES_DIAL_ID, MES_DIAL_CREATE_DATE
FROM MESSENGER_DIALOG
WHERE SOURCE_TYPE_ID = @SOURCE_TYPE_ID AND
(MES_DIAL_CLIENT_LOGIN = @PHONE_NUMBER OR MES_DIAL_CLIENT_PHONE = @PHONE_NUMBER);
                    ";

                    _Command.CommandType = System.Data.CommandType.Text;
                    _Command.Parameters.Add(new SqlParameter("SOURCE_TYPE_ID", System.Data.SqlDbType.Int)
                    {
                        Value = _SourceTypeId
                    });
                    _Command.Parameters.Add(new SqlParameter("PHONE_NUMBER", System.Data.SqlDbType.NVarChar, 100)
                    {
                        Value = _PhoneNumber
                    });

                    using (SqlDataReader reader = _Command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader.GetDateTime(1).DateTimeEquals(_CreationDateTime))
                                {
                                    return reader.GetInt32(0);
                                }
                            }
                        }
                    }
                }
            }

            return -1;
        }
        //--------------------------------------------------------------------------------------------------
        public void SetPersonMessengerTypeIdByMessengerDialogId(int _MessengerDialogId, int _PersonMessengerTypeId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_DIAL_ID INT = 268;
--DECLARE @PER_MES_TYPE_ID INT = 117;

UPDATE MESSENGER_DIALOG
SET PER_MES_TYPE_ID = @PER_MES_TYPE_ID
WHERE MES_DIAL_ID = @MES_DIAL_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_DIAL_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessengerDialogId
            });
            _Command.Parameters.Add(new SqlParameter("PER_MES_TYPE_ID", System.Data.SqlDbType.Int)
            {
                Value = _PersonMessengerTypeId
            });

            Execute(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public void AddMessengerDialogMessage(int _MessengerDialogId, string _Text, string _Guid, DateTime _CreationDateTime)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_DIAL_ID INT = 268;
--DECLARE @MES_DIAL_MES_TEXT NVARCHAR(2000) = 'Тест';
--DECLARE @MES_DIAL_MES_GUID NVARCHAR(100) = '7a93e9a3-ce8b-4b4c-937f-239e33c50b2d';
--DECLARE @CREATION_DATETIME DATETIME = GETUTCDATE();

INSERT INTO MESSENGER_DIALOG_MESSAGE(MES_TYPE_ID, MES_STAT_TYPE_ID, MES_DIAL_ID, MES_DIAL_MES_TEXT, 
MES_DIAL_MES_DEPARTURE_DATE, MES_DIAL_MES_GUID, SEC_USER_FIO)
VALUES
	(2, 2, @MES_DIAL_ID, @MES_DIAL_MES_TEXT, @CREATION_DATETIME, @MES_DIAL_MES_GUID, 'Менеджер массовой рассылки');
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_DIAL_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessengerDialogId
            });
            _Command.Parameters.Add(new SqlParameter("MES_DIAL_MES_TEXT", System.Data.SqlDbType.NVarChar, 2000)
            {
                Value = _Text
            });
            _Command.Parameters.Add(new SqlParameter("MES_DIAL_MES_GUID", System.Data.SqlDbType.NVarChar, 100)
            {
                Value = _Guid
            });
            _Command.Parameters.Add(new SqlParameter("CREATION_DATETIME", System.Data.SqlDbType.DateTime)
            {
                Value = _CreationDateTime
            });

            Execute(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public int GetMessengerDialogMessageIdByGuid(string _Guid)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_DIAL_MES_GUID NVARCHAR(100) = '7a93e9a3-ce8b-4b4c-937f-239e33c50b2d';

SELECT MES_DIAL_MES_ID
FROM MESSENGER_DIALOG_MESSAGE
WHERE MES_DIAL_MES_GUID = @MES_DIAL_MES_GUID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_DIAL_MES_GUID", System.Data.SqlDbType.NVarChar, 100)
            {
                Value = _Guid
            });

            return Execute<int>(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public void AddMessengerDialogMessageAttachment(int _MessengerDialogMessageId, string _FileName, string _Data)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_DIAL_MES_ID INT = 3781;
--DECLARE @MES_DIAL_MES_ATT_NAME NVARCHAR(1000) = 'Тест.txt';
--DECLARE @MES_DIAL_MES_ATT_DATA VARCHAR(MAX) = '';

INSERT INTO MESSENGER_DIALOG_MESSAGE_ATTACHMENT(MES_DIAL_MES_ID, MES_DIAL_MES_ATT_NAME, MES_DIAL_MES_ATT_DATA)
VALUES
	(@MES_DIAL_MES_ID, @MES_DIAL_MES_ATT_NAME, @MES_DIAL_MES_ATT_DATA);
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_DIAL_MES_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessengerDialogMessageId
            });
            _Command.Parameters.Add(new SqlParameter("MES_DIAL_MES_ATT_NAME", System.Data.SqlDbType.NVarChar, 1000)
            {
                Value = _FileName
            });
            _Command.Parameters.Add(new SqlParameter("MES_DIAL_MES_ATT_DATA", System.Data.SqlDbType.VarChar, int.MaxValue)
            {
                Value = _Data
            });

            Execute(_Command);
        }
        //--------------------------------------------------------------------------------------------------
        public bool GetChatAggregatorAllowSendToWhatsAppOptions()
        {
            SqlCommand command = new SqlCommand();

            command.CommandText = @"
SELECT OPT_VALUE
FROM OPTIONS
WHERE OPT_NAME LIKE '%ChatAggregatorAllowSendToWhatsApp%';
            ";

            command.CommandType = System.Data.CommandType.Text;

            string parameterValue = Execute<string>(command);
            if (parameterValue == null)
                return false;

            return byte.Parse(parameterValue) > 0;
        }
        //--------------------------------------------------------------------------------------------------
        #region Вспомогательные закрытые методы
        //--------------------------------------------------------------------------------------------------
        private T Execute<T>(SqlCommand _Command)
        {
            using (SqlConnection _Connection = new SqlConnection(m_ConnectionString))
            {
                _Connection.Open();

                _Command.Connection = _Connection;

                using (_Command)
                {
                    object _QueryResult = _Command.ExecuteScalar();

                    if (_QueryResult is T)
                    {
                        return (T)_QueryResult;
                    }
                }
            }

            return default;
        }
        //-----------------------------------------------------------
        private void Execute(SqlCommand _Command)
        {
            using (SqlConnection _Connection = new SqlConnection(m_ConnectionString))
            {
                _Connection.Open();

                _Command.Connection = _Connection;

                using (_Command)
                {
                    _Command.ExecuteNonQuery();
                }
            }
        }
        //-----------------------------------------------------------
        #endregion
        //-----------------------------------------------------------
    }
}
