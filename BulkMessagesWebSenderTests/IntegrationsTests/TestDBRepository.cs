using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BulkMessagesWebServer.Tests.Integrations
{
    public sealed class TestDBRepository
    {
        private string m_ConnectionString;
        //-----------------------------------------------------------
        public TestDBRepository(string _ConnectionString)
        {
            m_ConnectionString = _ConnectionString;
        }
        //-----------------------------------------------------------
        public int GetMesUmnSendIdByGuid(string _Guid)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_UMN_SEND_GUID char(36) = '7a93e9a3-ce8b-4b4c-937f-239e33c50b2d';

SELECT MES_UMN_SEND_ID
FROM MES_UMNICO_SENDER
WHERE MES_UMN_SEND_GUID = @MES_UMN_SEND_GUID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_UMN_SEND_GUID", System.Data.SqlDbType.Char, 36)
            {
                Value = _Guid
            });

            return Execute<int>(_Command);
        }
        //-----------------------------------------------------------
        public void UpdatePhoneNumberByPersonId(int _PersonId, string _PhoneNumber)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @PER_ID INT = 1;
--DECLARE @PHONE_NUMBER VARCHAR(100) = NULL;

WITH PERSON_ADDRESS AS (
SELECT ADDR_ID
FROM ADDRESS AS A
JOIN PERSON AS P ON A.ADDR_ID = P.PER_LIVE_ADDR
WHERE P.PER_ID = @PER_ID
)

UPDATE ADDRESS
SET ADDR_PHONE = @PHONE_NUMBER
WHERE ADDR_ID = (SELECT ADDR_ID FROM PERSON_ADDRESS);
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("PER_ID", System.Data.SqlDbType.Int)
            {
                Value = _PersonId
            });
            _Command.Parameters.Add(new SqlParameter("PHONE_NUMBER", System.Data.SqlDbType.VarChar, 100));

            if (_PhoneNumber == null)
            {
                _Command.Parameters["PHONE_NUMBER"].Value = DBNull.Value;
            }
            else
            {
                _Command.Parameters["PHONE_NUMBER"].Value = _PhoneNumber;
            }

            Execute(_Command);
        }
        //-----------------------------------------------------------
        public void UpdatePersonMessengerTypeIdByMessengerDialogId(int _MessengerDialogId, int? _PersonMessengerTypeId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_DIAL_ID INT = 1;
--DECLARE @PER_MES_TYPE_ID INT = 1;

UPDATE MESSENGER_DIALOG
SET PER_MES_TYPE_ID = @PER_MES_TYPE_ID
WHERE MES_DIAL_ID = @MES_DIAL_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_DIAL_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessengerDialogId
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
        //-----------------------------------------------------------
        public int GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(int _PersonId, int _MessengerTypeId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @PER_ID INT = 1;
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
        //-----------------------------------------------------------
        public int GetMessengerDialogIdBySourceTypeIdAndPhoneNumberAndCreationDateTime(int _SourceTypeId,
            string _PhoneNumber, DateTime _CreationDateTime)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @SOURCE_TYPE_ID INT = 1;
--DECLARE @PHONE_NUMBER NVARCHAR(100) = '9042194323';
--DECLARE @MES_DIAL_CREATE_DATE DATETIME = '2023-09-22T16:00:00';

SELECT MES_DIAL_ID
FROM MESSENGER_DIALOG
WHERE SOURCE_TYPE_ID = @SOURCE_TYPE_ID AND
MES_DIAL_CLIENT_LOGIN = @PHONE_NUMBER AND
MES_DIAL_CREATE_DATE = @MES_DIAL_CREATE_DATE;
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
            _Command.Parameters.Add(new SqlParameter("MES_DIAL_CREATE_DATE", System.Data.SqlDbType.DateTime)
            {
                Value = _CreationDateTime
            });

            return Execute<int>(_Command);
        }
        //-----------------------------------------------------------
        public int? GetPersonMessengerTypeIdByMessengerDialogId(int _MessengerDialogId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_DIAL_ID INT = 1;

SELECT PER_MES_TYPE_ID
FROM MESSENGER_DIALOG
WHERE MES_DIAL_ID = @MES_DIAL_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_DIAL_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessengerDialogId
            });

            int _ExecuteResult = Execute<int>(_Command);

            if (_ExecuteResult <= 0)
            {
                return null;
            }
            else
            {
                return _ExecuteResult;
            }
        }
        //-----------------------------------------------------------
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
        //-----------------------------------------------------------
        public int GetMessengerDialogMessageAttachmentIdByAllData(int _MessengerDialogMessageId, string _FileName, string _Data)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_DIAL_MES_ID INT = 1;
--DECLARE @MES_DIAL_MES_ATT_NAME NVARCHAR(1000) = 'Тестовый_файл.txt';
--DECLARE @MES_DIAL_MES_ATT_DATA VARCHAR(MAX) = '';

SELECT MES_DIAL_MES_ATT_ID
FROM MESSENGER_DIALOG_MESSAGE_ATTACHMENT
WHERE MES_DIAL_MES_ID = @MES_DIAL_MES_ID AND
MES_DIAL_MES_ATT_NAME = @MES_DIAL_MES_ATT_NAME AND
MES_DIAL_MES_ATT_DATA = @MES_DIAL_MES_ATT_DATA;
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

            return Execute<int>(_Command);
        }
        //-----------------------------------------------------------
        /// <summary>
        /// Данный метод создаёт сообщение массовой рассылки типа "Одно сообщение одному получателю"
        /// </summary>
        /// <param name="_PersonId"></param>
        /// <param name="_PhoneNumber"></param>
        /// <returns>MES_LOG_EXT_ID (Guid) для созданного сообщения в сущности MES_LOG</returns>
        public string CreateOneMessageToOneRecepientMessageLog(int _PersonId, string _PhoneNumber)
        {
            string _Result = Guid.NewGuid().ToString();
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @PER_ID INT = 1;
--DECLARE @MES_LOG_EXT_ID VARCHAR(50) = '7a93e9a3-ce8b-4b4c-937f-239e33c50b2d';
--DECLARE @PHONE_NUMBER VARCHAR(20) = '9042194323';

INSERT INTO MES_LOG(PER_ID, MES_LOG_DATETIME, MES_LOG_MESSAGE, MES_LOG_AUTHOR, 
	MES_LOG_PHONE, MES_LOG_COST, MES_LOG_PROVIDER, MES_LOG_EXT_ID, MES_SEND_STATE_ID)
VALUES
	(@PER_ID, CURRENT_TIMESTAMP, 'Тест', 'Администратор', @PHONE_NUMBER, 0.0,
	'Умнико чат-мессендже', @MES_LOG_EXT_ID, 1);
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("PER_ID", System.Data.SqlDbType.Int)
            {
                Value = _PersonId
            });
            _Command.Parameters.Add(new SqlParameter("MES_LOG_EXT_ID", System.Data.SqlDbType.VarChar, 50)
            {
                Value = _Result
            });
            _Command.Parameters.Add(new SqlParameter("PHONE_NUMBER", System.Data.SqlDbType.VarChar, 20)
            {
                Value = _PhoneNumber
            });

            Execute(_Command);

            return _Result;
        }
        //-----------------------------------------------------------
        public string CreateOneMessageToManyRecepientsMessageLog(Dictionary<int, string> _PersonsData)
        {
            string _Result = Guid.NewGuid().ToString();
            SqlCommand _FirstCommand = new SqlCommand();

            _FirstCommand.CommandText = @"
--DECLARE @MES_BAT_EXT_ID VARCHAR(50) = '7a93e9a3-ce8b-4b4c-937f-239e33c50b2d';

INSERT INTO MES_BATCH(MES_BAT_AUTHOR, MES_BAT_DATETIME, MES_BAT_TEMPLATE, 
	MES_BAT_COST, MES_BAT_PROVIDER, MES_BAT_EXT_ID)
VALUES
	('Администратор', CURRENT_TIMESTAMP, 'Тест', 0.00, 
		'Умнико чат-мессендже', @MES_BAT_EXT_ID);
            ";

            _FirstCommand.CommandType = System.Data.CommandType.Text;
            _FirstCommand.Parameters.Add(new SqlParameter("MES_BAT_EXT_ID", System.Data.SqlDbType.VarChar, 50)
            {
                Value = _Result
            });

            Execute(_FirstCommand);

            int _MessageBatchId = GetMessageBatchIdByGuid(_Result);

            foreach (KeyValuePair<int, string> pair in _PersonsData)
            {
                SqlCommand _SecondCommand = new SqlCommand();

                _SecondCommand.CommandText = @"
--DECLARE @PER_ID INT = 1;
--DECLARE @MES_BAT_ID INT = 1;
--DECLARE @PHONE_NUMBER VARCHAR(20) = '9042194323';

INSERT INTO MES_LOG(PER_ID, MES_LOG_DATETIME, MES_LOG_MESSAGE, MES_LOG_AUTHOR, 
	MES_LOG_PHONE, MES_LOG_COST, MES_LOG_PROVIDER, MES_BAT_ID, MES_SEND_STATE_ID)
VALUES
	(@PER_ID, CURRENT_TIMESTAMP, 'Тест', 'Администратор', @PHONE_NUMBER, 0.0,
	'Умнико чат-мессендже', @MES_BAT_ID, 1);
                ";

                _SecondCommand.CommandType = System.Data.CommandType.Text;
                _SecondCommand.Parameters.Add(new SqlParameter("PER_ID", System.Data.SqlDbType.Int)
                {
                    Value = pair.Key
                });
                _SecondCommand.Parameters.Add(new SqlParameter("MES_BAT_ID", System.Data.SqlDbType.Int)
                {
                    Value = _MessageBatchId
                });
                _SecondCommand.Parameters.Add(new SqlParameter("PHONE_NUMBER", System.Data.SqlDbType.VarChar, 20)
                {
                    Value = pair.Value
                });

                Execute(_SecondCommand);
            }

            return _Result;
        }
        //-----------------------------------------------------------
        public Dictionary<int, string> CreateManyPersonalMessagesToManyRecepientsMessageLog(Dictionary<int, string> _PersonsData)
        {
            Dictionary<int, string> _Result = new Dictionary<int, string>();
            SqlCommand _FirstCommand = new SqlCommand();

            _FirstCommand.CommandText = @"
INSERT INTO MES_BATCH(MES_BAT_AUTHOR, MES_BAT_DATETIME, MES_BAT_TEMPLATE, 
	MES_BAT_COST, MES_BAT_PROVIDER)
VALUES
	('Администратор', CURRENT_TIMESTAMP, 'Тест', 0.00, 'Умнико чат-мессендже');
            ";

            _FirstCommand.CommandType = System.Data.CommandType.Text;

            Execute(_FirstCommand);

            int _MessageBatchId = GetMaxMessageBatchId();

            foreach (KeyValuePair<int, string> pair in _PersonsData)
            {
                SqlCommand _SecondCommand = new SqlCommand();

                _SecondCommand.CommandText = @"
--DECLARE @PER_ID INT = 1;
--DECLARE @MES_BAT_ID INT = 1;
--DECLARE @PHONE_NUMBER VARCHAR(20) = '9042194323';
--DECLARE @MES_LOG_EXT_ID VARCHAR(50) = '7a93e9a3-ce8b-4b4c-937f-239e33c50b2d';

INSERT INTO MES_LOG(PER_ID, MES_LOG_DATETIME, MES_LOG_MESSAGE, MES_LOG_AUTHOR, MES_LOG_EXT_ID,
	MES_LOG_PHONE, MES_LOG_COST, MES_LOG_PROVIDER, MES_BAT_ID, MES_SEND_STATE_ID)
VALUES
	(@PER_ID, CURRENT_TIMESTAMP, 'Тест', 'Администратор', @MES_LOG_EXT_ID, @PHONE_NUMBER, 0.0,
		'Умнико чат-мессендже', @MES_BAT_ID, 1);
                ";

                _SecondCommand.CommandType = System.Data.CommandType.Text;
                _SecondCommand.Parameters.Add(new SqlParameter("PER_ID", System.Data.SqlDbType.Int)
                {
                    Value = pair.Key
                });
                _SecondCommand.Parameters.Add(new SqlParameter("MES_BAT_ID", System.Data.SqlDbType.Int)
                {
                    Value = _MessageBatchId
                });
                _SecondCommand.Parameters.Add(new SqlParameter("PHONE_NUMBER", System.Data.SqlDbType.VarChar, 20)
                {
                    Value = pair.Value
                });

                string _Guid = Guid.NewGuid().ToString();

                _SecondCommand.Parameters.Add(new SqlParameter("MES_LOG_EXT_ID", System.Data.SqlDbType.VarChar, 50)
                {
                    Value = _Guid
                });

                Execute(_SecondCommand);

                _Result.Add(pair.Key, _Guid);
            }

            return _Result;
        }
        //-----------------------------------------------------------
        public int? GetMessageLogIdByGuid(string _Guid)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_LOG_EXT_ID VARCHAR(50) = '7a93e9a3-ce8b-4b4c-937f-239e33c50b2d';

SELECT MES_LOG_ID
FROM MES_LOG
WHERE MES_LOG_EXT_ID = @MES_LOG_EXT_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_LOG_EXT_ID", System.Data.SqlDbType.VarChar, 50)
            {
                Value = _Guid
            });

            int _ExecuteResult = Execute<int>(_Command);

            if (_ExecuteResult <= 0)
            {
                return null;
            }
            else
            {
                return _ExecuteResult;
            }
        }
        //-----------------------------------------------------------
        public int GetMessageBatchIdByGuid(string _Guid)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_BAT_EXT_ID VARCHAR(50) = '7a93e9a3-ce8b-4b4c-937f-239e33c50b2d';

SELECT MES_BAT_ID
FROM MES_BATCH
WHERE MES_BAT_EXT_ID = @MES_BAT_EXT_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_BAT_EXT_ID", System.Data.SqlDbType.VarChar, 50)
            {
                Value = _Guid
            });

            return Execute<int>(_Command);
        }
        //-----------------------------------------------------------
        public int GetMaxMessageBatchId()
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
SELECT MAX(MES_BAT_ID)
FROM MES_BATCH;
            ";

            _Command.CommandType = System.Data.CommandType.Text;

            return Execute<int>(_Command);
        }
        //-----------------------------------------------------------
        public int? GetMessageLogIdByMessageBatchIdAndPersonId(int _MessageBatchId, int _PersonId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_BAT_ID INT = 1597;
--DECLARE @PER_ID INT = 3235;

SELECT MES_LOG_ID
FROM MES_LOG
WHERE MES_BAT_ID = @MES_BAT_ID AND
PER_ID = @PER_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_BAT_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessageBatchId
            });
            _Command.Parameters.Add(new SqlParameter("PER_ID", System.Data.SqlDbType.Int)
            {
                Value = _PersonId
            });

            int _ExecuteResult = Execute<int>(_Command);

            if (_ExecuteResult <= 0)
            {
                return null;
            }
            else
            {
                return _ExecuteResult;
            }
        }
        //-----------------------------------------------------------
        public int GetMessageLogSendStateIdByMessageLogId(int _MessageLogId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_LOG_ID INT = 1;

SELECT MES_SEND_STATE_ID
FROM MES_LOG
WHERE MES_LOG_ID = @MES_LOG_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_LOG_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessageLogId
            });

            return Execute<int>(_Command);
        }
        //-----------------------------------------------------------
        public string GetMessageLogSendErrorByMessageLogId(int _MessageLogId)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @MES_LOG_ID INT = 1;

SELECT MES_LOG_SEND_ERROR
FROM MES_LOG
WHERE MES_LOG_ID = @MES_LOG_ID;
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("MES_LOG_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessageLogId
            });

            return Execute<string>(_Command);
        }
        //-----------------------------------------------------------
        public void DeleteAllPersonMessengerTypes()
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
DELETE PERSON_MESSENGER_TYPES;
            ";

            _Command.CommandType = System.Data.CommandType.Text;

            Execute(_Command);
        }
        //-----------------------------------------------------------
        public void CreateMessengerDialog(int _SourceTypeId, DateTime _CreationDateTime, string _ExternalId = null,
            string _PhoneNumber = null, int? _PersonMessengerTypeId = null, int _MessengerDialogTypeId = 1)
        {
            SqlCommand _Command = new SqlCommand();

            _Command.CommandText = @"
--DECLARE @SOURCE_TYPE_ID INT = 1;
--DECLARE @MES_DIAL_CREATE_DATE DATETIME = CURRENT_TIMESTAMP;
--DECLARE @MES_DIAL_EXTERNAL_ID NVARCHAR(200) = '12345678' + CHAR(13) + CHAR(10) + '87654321';
--DECLARE @MES_DIAL_CLIENT_LOGIN NVARCHAR(100) = 'Собеседник';
--DECLARE @MES_DIAL_CLIENT_PHONE NVARCHAR(100) = '9042194323';
--DECLARE @PER_MES_TYPE_ID INT = NULL;
--DECLARE @MES_DIAL_TYPE_ID INT = 1;

INSERT INTO MESSENGER_DIALOG(SOURCE_TYPE_ID, MES_DIAL_TYPE_ID, PER_MES_TYPE_ID,
	MES_DIAL_STAT_TYPE_ID, MES_DIAL_CREATE_DATE, MES_DIAL_EXTERNAL_ID,
	MES_DIAL_CLIENT_LOGIN, MES_DIAL_CLIENT_PHONE, MES_DIAL_MESSAGE_IS_READ)
VALUES
	(@SOURCE_TYPE_ID, @MES_DIAL_TYPE_ID, @PER_MES_TYPE_ID, 1, @MES_DIAL_CREATE_DATE,
	@MES_DIAL_EXTERNAL_ID, @MES_DIAL_CLIENT_LOGIN, @MES_DIAL_CLIENT_PHONE, 0);
            ";

            _Command.CommandType = System.Data.CommandType.Text;
            _Command.Parameters.Add(new SqlParameter("SOURCE_TYPE_ID", System.Data.SqlDbType.Int)
            {
                Value = _SourceTypeId
            });

            _Command.Parameters.Add(new SqlParameter("MES_DIAL_CREATE_DATE", System.Data.SqlDbType.DateTime)
            {
                Value = _CreationDateTime
            });

            _Command.Parameters.Add(new SqlParameter("MES_DIAL_EXTERNAL_ID", System.Data.SqlDbType.NVarChar, 200));
            if (_ExternalId == null)
            {
                _Command.Parameters["MES_DIAL_EXTERNAL_ID"].Value = DBNull.Value;
            }
            else
            {
                _Command.Parameters["MES_DIAL_EXTERNAL_ID"].Value = _ExternalId;
            }

            _Command.Parameters.Add(new SqlParameter("MES_DIAL_CLIENT_LOGIN", System.Data.SqlDbType.NVarChar, 100)
            {
                Value = _PhoneNumber ?? "Собеседник"
            });

            _Command.Parameters.Add(new SqlParameter("MES_DIAL_CLIENT_PHONE", System.Data.SqlDbType.NVarChar, 100));
            if (_PhoneNumber == null)
            {
                _Command.Parameters["MES_DIAL_CLIENT_PHONE"].Value = DBNull.Value;
            }
            else
            {
                _Command.Parameters["MES_DIAL_CLIENT_PHONE"].Value = _PhoneNumber;
            }

            _Command.Parameters.Add(new SqlParameter("PER_MES_TYPE_ID", System.Data.SqlDbType.Int));
            if (_PersonMessengerTypeId == null)
            {
                _Command.Parameters["PER_MES_TYPE_ID"].Value = DBNull.Value;
            }
            else
            {
                _Command.Parameters["PER_MES_TYPE_ID"].Value = (int)_PersonMessengerTypeId;
            }

            _Command.Parameters.Add(new SqlParameter("MES_DIAL_TYPE_ID", System.Data.SqlDbType.Int)
            {
                Value = _MessengerDialogTypeId
            });

            Execute(_Command);
        }
        //-----------------------------------------------------------
        public void AddChatAggregatorAllowSendToWhatsAppParameter(bool _IsAllow)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText = @"
--DECLARE @ALLOW_SEND_TO_WHATS_APP CHAR(1) = '1';

INSERT INTO OPTIONS(OPT_NAME, OPT_VALUE)
VALUES
	('ChatAggregatorAllowSendToWhatsApp', @ALLOW_SEND_TO_WHATS_APP);
            ";

            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.Add(new SqlParameter("ALLOW_SEND_TO_WHATS_APP", System.Data.SqlDbType.Char, 1)
            {
                Value = _IsAllow ? "1" : "0"
            });

            Execute(command);
        }
        //-----------------------------------------------------------
        #region Вспомогательные закрытые методы
        //-----------------------------------------------------------
        private T Execute<T>(SqlCommand _Command)
        {
            using (SqlConnection _Connection = new SqlConnection(m_ConnectionString))
            {
                _Connection.Open();

                _Command.Connection = _Connection;

                using (_Command)
                {
                    object _QueryResult = _Command.ExecuteScalar();

                    if (_QueryResult != null)
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
