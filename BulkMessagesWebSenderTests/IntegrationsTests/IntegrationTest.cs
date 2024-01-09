using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BulkMessagesWebServer.Tests.Integrations
{
    public abstract class IntegrationTest
    {
        protected string m_ConnectionString;
        protected TestDBRepository m_DataRepository;
        //---------------------------------------------------------------
        public IntegrationTest(string _NameConnectionString)
        {
            m_ConnectionString = ConfigurationManager.ConnectionStrings[_NameConnectionString].ConnectionString;
            m_DataRepository = new TestDBRepository(m_ConnectionString);

            if (!m_ConnectionString.Contains("clinic_unit_tests"))
            {
                string _NameDBParameter = "Initial Catalog=";

                int _StartIndexNameDB = m_ConnectionString.IndexOf(_NameDBParameter) + _NameDBParameter.Length;

                if (_StartIndexNameDB <= 0)
                {
                    _NameDBParameter = "Database=";

                    _StartIndexNameDB = m_ConnectionString.IndexOf(_NameDBParameter) + _NameDBParameter.Length;
                }

                string _NameDB = new string (m_ConnectionString.Skip(_StartIndexNameDB).TakeWhile(c => c != ';').ToArray());

                string _ErrorMessage = "Данные тесты ориентированы на тестовую базу данных!\r\n" + 
                    "В результате их работы база данных подготавливается (в некоторых местах с очисткой сущностей)!\r\n" + 
                    $"Указанная база данных \"{_NameDB}\" может потерять важные данные, которые нельзя уже будет вернуть!\r\n" + 
                    $"Если Ваше решение осознанно, тогда отредактируйте код данного класса: \"{GetType().FullName}\"";

                MessageBox.Show(
                    _ErrorMessage,
                    "Защита от дурака",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                throw new NotImplementedException(_ErrorMessage);
            }

            PrepareDatabase();
        }
        //---------------------------------------------------------------
        private void PrepareDatabase()
        {
            using (SqlConnection connection = new SqlConnection(m_ConnectionString))
            {
                connection.Open();

                string _StoredProcedureName = "sp_PrepareDBBeforeUnitTestingMassMessages";

                if (StoredProcedureIsNotExistsInDataBase(_StoredProcedureName, connection))
                {
                    string _Query = CreateQueryTextFrom_PrepareDBBeforeUnitTestingMassMessagesStoredProcedure();

                    using (SqlCommand command = new SqlCommand(_Query, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }

                using (SqlCommand command = new SqlCommand(_StoredProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
            }
        }
        //---------------------------------------------------------------
        private bool StoredProcedureIsNotExistsInDataBase(string _StoredProcedureName, SqlConnection _Connection)
        {
            string _CheckQuery = $"SELECT object_id FROM sys.all_objects WHERE name = '{_StoredProcedureName}';";

            using (SqlCommand command = new SqlCommand(_CheckQuery, _Connection))
            {
                command.CommandType = CommandType.Text;

                try
                {
                    object obj = command.ExecuteScalar();

                    if (obj == null)
                    {
                        return true;
                    }

                    return false;
                }
                catch
                {
                    return true;
                }
            }
        }
        //---------------------------------------------------------------
        private string CreateQueryTextFrom_PrepareDBBeforeUnitTestingMassMessagesStoredProcedure()
        {
            string _Query = @"
-- ============================================= 
-- Author:          < Zhukov Victor > 
--Create date:      < 29.08.2023 > 
--Last modify date: < 29.08.2023 > 
--Description:	< Подготавливает базу данных к интеграционному тестированию сообщений массовой рассылки> 
-- ============================================= 
CREATE PROCEDURE[dbo].[sp_PrepareDBBeforeUnitTestingMassMessages] 
AS 
BEGIN
	--Количество статусов для сообщений массовой рассылки
	DECLARE @COUNT_MES_UMNICO_SENDER_STATUS_TYPES INT = ( 
		SELECT COUNT(*) FROM MES_UMNICO_SENDER_STATUS_TYPE 
	);
	-- Количество пациентов
	DECLARE @COUNT_PERSONS INT = ( 
		SELECT COUNT(*) FROM PERSON 
	);

	-- Очищаем таблицу с вложениями сообщений диалогов в мессенджере
	DELETE MESSENGER_DIALOG_MESSAGE_ATTACHMENT;

	-- Очищаем таблицу с сообщениями диалогов в мессенджере
	DELETE MESSENGER_DIALOG_MESSAGE;

	-- Очищаем таблицу с диалогами в мессенджере
	DELETE MESSENGER_DIALOG;

	-- Очищаем таблицу связок пациент-мессенджер
	DELETE PERSON_MESSENGER_TYPES;

	-- Очищаем все приоритетные мессенджеры у пациентов
	UPDATE PERSON
	SET MESSENGER_TYPE_ID = NULL;

	-- Очищаем таблицу с интеграциями
	DELETE SOURCE_TYPE;

	-- Очищаем таблицу с мессенджерами
	DELETE MESSENGER_TYPE;

	-- Очищаем таблицу массовой рассылки
    TRUNCATE TABLE MES_UMNICO_SENDER;

	-- Добавляем мессенджеры с MESSENGER_TYPE_ID = (1, 4)
	INSERT INTO MESSENGER_TYPE(MESSENGER_TYPE_ID, MESSENGER_TYPE_NAME, MESSENGER_TYPE_NEED_UID, 
		MESSENGER_TYPE_SHOW_FOR_PERSON, MESSENGER_TYPE_IS_MESSENGER)
		VALUES 
		   (4, 'Telegram', 0, 0, 0),
		   (1, 'Test', 0, 0, 0);

	-- Очищаем таблицу с интеграциями
	ALTER TABLE MESSENGER_DIALOG
	DROP FK_MESSENGE_SOURCE_TY_SOURCE_T;

	TRUNCATE TABLE SOURCE_TYPE;

	-- Добавляем интеграцию с MESSENGER_TYPE_ID = 4
	INSERT INTO SOURCE_TYPE(SOURCE_TYPE_ACC_NAME, MESSENGER_TYPE_ID, SOURCE_TYPE_UID)
	VALUES ('Интеграция с Telegram', 4, '49113');

	ALTER TABLE MESSENGER_DIALOG
		ADD CONSTRAINT FK_MESSENGE_SOURCE_TY_SOURCE_T FOREIGN KEY (SOURCE_TYPE_ID)
			REFERENCES SOURCE_TYPE (SOURCE_TYPE_ID);
	--------------------------------

	-- Если количество статусов для массовой рассылки не равно 3, то
	-- очищаем данную таблицу и заполняем правильными данными
	IF @COUNT_MES_UMNICO_SENDER_STATUS_TYPES <> 3 
	BEGIN 
		ALTER TABLE MES_UMNICO_SENDER 
		DROP FK_MES_UMNI_MES_UMN_S_MES_UMNI;

		TRUNCATE TABLE MES_UMNICO_SENDER_STATUS_TYPE;

		ALTER TABLE MES_UMNICO_SENDER 
			ADD CONSTRAINT FK_MES_UMNI_MES_UMN_S_MES_UMNI FOREIGN KEY(MES_UMN_SEND_STAT_TYPE_ID) 
			REFERENCES MES_UMNICO_SENDER_STATUS_TYPE(MES_UMN_SEND_STAT_TYPE_ID);

		-- Заполняем таблицу статусов сообщений массовой рассылки
		INSERT INTO MES_UMNICO_SENDER_STATUS_TYPE(MES_UMN_SEND_STAT_TYPE_ID, MES_UMN_SEND_STAT_TYPE_NAME) 
		VALUES 
			(1, 'Ожидает отправки'), 
			(2, 'Отправлено'), 
			(3, 'Не отправлено') 
	END;

	-- Если пациентов меньше 2, то подготавливаем данные и создаём пациентов с Id = (1, 2)
	IF @COUNT_PERSONS < 2 
	BEGIN
		-- Количество стран проживания с названием Российская Федерация
		DECLARE @COUNT_RUSSIA_CITIZENSHIP INT = ( 
			SELECT COUNT(*) FROM(SELECT * FROM CITIZENSHIP WHERE CIT_ID = 643) 
			AS RUSSUA_CITIZENSHIP 
		);
		-- Количество категорий пациента с названием Прочее
		DECLARE @COUNT_OTHER_PERSON_CATEGORY INT = ( 
				SELECT COUNT(*) FROM(SELECT * FROM PERSON_CATEGORY WHERE PER_CAT_NAME = 'Прочее') 
			AS OTHER_PERSON_CATEGORY 
		); 
		-- Количество статусов пациента с названием Новый пациент
		DECLARE @COUNT_NEW_PATIENT_PERSON_STATE INT = ( 
				SELECT COUNT(*) FROM(SELECT * FROM PERSON_STATE WHERE PER_STATE_NAME = 'Новый пациент') 
			AS NEW_PATIENT_PERSON_STATE 
		); 
		-- Количество типов пациента с Id = 1 (Name = Пациент)
		DECLARE @COUNT_PATIENT_PERSON_TYPE INT = ( 
				SELECT COUNT(*) FROM(SELECT * FROM PERSON_TYPE WHERE PER_TYPE_ID = 1) 
			AS PATIENT_PERSON_TYPE 
		); 
		-- Количество типов документов для проверки личности с Id = 21 (Name = Паспорт гражданина Российской Федерации)
		DECLARE @COUNT_PASPORT_PERSON_DOC_TYPE INT = ( 
				SELECT COUNT(*) FROM(SELECT * FROM PERSON_DOC_TYPE WHERE PERSON_DOC_TYPE_ID = 21) 
			AS PASPORT_PERSON_DOC_TYPE 
		); 
       
	   -- Если стран проживания нет, то подготавливаем данные и создаём страну проживания с Id = 171
       IF @COUNT_RUSSIA_CITIZENSHIP = 0 
       BEGIN 
            INSERT INTO CITIZENSHIP(CIT_ID, CIT_NAME, CIT_FULL_NAME, CIT_ALPHA_CODE2, CIT_ALPHA_CODE3, CIT_NSI_ID) 
            VALUES 
                (643, 'Россия', 'Российская Федерация', 'RU', 'RUS', 171) 
       END; 

	   -- Если категорий пациентов нет, то подготавливаем данные и создаём категорию пациента с Name = Прочее
       IF @COUNT_OTHER_PERSON_CATEGORY = 0 
       BEGIN 
           INSERT INTO PERSON_CATEGORY(PER_CAT_NAME, PER_CAT_SHOW_IN_REPORT) 
           VALUES 
               ('Прочее', 1) 
       END; 

	   -- Если статусов пациентов нет, то подготавливаем данные и создаём статус пациента с Name = Новый пациент
       IF @COUNT_NEW_PATIENT_PERSON_STATE = 0 
       BEGIN 
           INSERT INTO PERSON_STATE(PER_STATE_NAME, PER_STATE_CHAR, PER_STATE_MIN_VISIT, PER_STATE_MIN_SUM, PER_STATE_LEVEL, 
               PER_STATE_COLOR, PER_STATE_BUILTIN, PER_STATE_MANUAL) 
           VALUES 
               ('Новый пациент', '* ', 0, 0, 1, -16711681, 1, 0) 
       END; 

	   -- Если типов пациентов нет, то подготавливаем данные и создаём тип пациента с Name = Пациент
       IF @COUNT_PATIENT_PERSON_TYPE = 0 
       BEGIN 
           INSERT INTO PERSON_TYPE(PER_TYPE_ID, PER_TYPE_NAME) 
           VALUES 
               (1, 'Пациент'); 
       END; 

	   -- Если типов документов проверки личности нет, то подготавливаем данные и 
	   -- создаём тип документа проверки личности с Name = Паспорт гражданина Российской Федерации
       IF @COUNT_PASPORT_PERSON_DOC_TYPE = 0 
       BEGIN 
           INSERT PERSON_DOC_TYPE(PERSON_DOC_TYPE_ID, PERSON_DOC_TYPE_NAME, PERSON_DOC_TYPE_CODE, PERSON_DOC_TYPE_OBSOLETE) 
           VALUES 
               (21, 'Паспорт гражданина Российской Федерации', 1, 0); 
       END;

	   -- Удаляем пациентов
	   DELETE PERSON;

	   -- Очищаем адреса пациентов
       DELETE ADDRESS;

       -- Добавляем новые адреса пациентов
       INSERT INTO ADDRESS(ADDR_ID, ADDR_PHONE)
       VALUES
          (1, '9042194323'),
		  (2, '9513085221');

	   -- Наконец, после подготовки всех данных можем создать новых пациентов
       INSERT INTO PERSON(PER_ID, PER_SURNAME, PER_NAME, PER_INPUT_DATE, PER_STATE_ID, PER_CAT_ID, 
           PER_SEND_SMS_MESSAGE, PER_SEND_EMAIL_MESSAGE, PER_AMB_DATE, PER_LIVE_ADDR) 
           VALUES 
           ( 
               1, 
               'Жуков', 
               'Виктор', 
               CURRENT_TIMESTAMP, 
               1, 
               1, 
               1, 
               1, 
               CURRENT_TIMESTAMP,
			   1
		),

		( 
               2, 
               'Жуков', 
               'Сергей', 
               CURRENT_TIMESTAMP, 
               1, 
               1, 
               1, 
               1, 
               CURRENT_TIMESTAMP,
			   2
		);
   END;

   -- Очищаем таблицу со связками пациент-мессенджер
   ALTER TABLE PERSON
   DROP CONSTRAINT FK_PERSON_MESSENGER_MESSENGE;

   ALTER TABLE MESSENGER_DIALOG
   DROP CONSTRAINT FK_MESSENGE_PER_MES_T_PERSON_M;

   TRUNCATE TABLE PERSON_MESSENGER_TYPES;

   ALTER TABLE PERSON
   ADD  CONSTRAINT FK_PERSON_MESSENGER_MESSENGE FOREIGN KEY(MESSENGER_TYPE_ID)
      REFERENCES MESSENGER_TYPE (MESSENGER_TYPE_ID);

   ALTER TABLE MESSENGER_DIALOG
   ADD  CONSTRAINT FK_MESSENGE_PER_MES_T_PERSON_M FOREIGN KEY(PER_MES_TYPE_ID)
      REFERENCES PERSON_MESSENGER_TYPES (PER_MES_TYPE_ID);
   --------------------------------

   -- Добавляем новую связку пациента с Id = 1 и мессенджера Telegram (Id = 4)
   INSERT INTO PERSON_MESSENGER_TYPES(PER_ID, MESSENGER_TYPE_ID)
   VALUES
      (1, 4);

   -- Очищаем таблицу диалогов в мессенджере
   ALTER TABLE MESSENGER_DIALOG_MESSAGE
   DROP CONSTRAINT FK_MESSENGE_MES_DIAL__MESSENGE3;

   TRUNCATE TABLE MESSENGER_DIALOG;

   ALTER TABLE MESSENGER_DIALOG_MESSAGE
   ADD  CONSTRAINT FK_MESSENGE_MES_DIAL__MESSENGE3 FOREIGN KEY(MES_DIAL_ID)
      REFERENCES MESSENGER_DIALOG (MES_DIAL_ID)
         ON UPDATE CASCADE;
   --------------------------------

   -- Добавляем новый диалог
   INSERT INTO MESSENGER_DIALOG(SOURCE_TYPE_ID, MES_DIAL_TYPE_ID, MES_DIAL_STAT_TYPE_ID, MES_DIAL_CREATE_DATE,
      MES_DIAL_CLIENT_LOGIN, MES_DIAL_CLIENT_PHONE, MES_DIAL_MESSAGE_IS_READ)
   VALUES
      (1, 1, 1, GETUTCDATE(), '9042194323', '9042194323', 0);

   -- Очищаем таблицу сообщений для диалогов в мессенджере
   ALTER TABLE MESSENGER_DIALOG_MESSAGE_ATTACHMENT
   DROP CONSTRAINT FK_MESSENGE_MES_DIAL__MESSENGE4;

   TRUNCATE TABLE MESSENGER_DIALOG_MESSAGE;

   ALTER TABLE MESSENGER_DIALOG_MESSAGE_ATTACHMENT
   ADD  CONSTRAINT FK_MESSENGE_MES_DIAL__MESSENGE4 FOREIGN KEY(MES_DIAL_MES_ID)
      REFERENCES MESSENGER_DIALOG_MESSAGE (MES_DIAL_MES_ID);
   --------------------------------
   
   -- Добавляем новое сообщение для диалога с Id = 1
   INSERT INTO MESSENGER_DIALOG_MESSAGE(MES_TYPE_ID, MES_STAT_TYPE_ID, MES_DIAL_ID, MES_DIAL_MES_TEXT, 
      MES_DIAL_MES_DEPARTURE_DATE, MES_DIAL_MES_GUID, SEC_USER_FIO)
      VALUES
         (2, 2, 1, 'Тест', GETUTCDATE(), '7a93e9a3-ce8b-4b4c-937f-239e33c50b2d', 'Менеджер массовой рассылки');

   -- Очищаем таблицу вложений для сообщений диалогов в мессенджере
   TRUNCATE TABLE MESSENGER_DIALOG_MESSAGE_ATTACHMENT;

   -- Добавляем новое вложение для сообщения диалога в мессенджере с Id = 1
   INSERT INTO MESSENGER_DIALOG_MESSAGE_ATTACHMENT(MES_DIAL_MES_ID, MES_DIAL_MES_ATT_NAME, MES_DIAL_MES_ATT_DATA)
   VALUES
      (1, 'Тестовый_файл.txt', '');

   -- Очищаем таблицу для сообщений массовой рассылки
   DELETE MES_LOG;

   -- Очищаем таблицу массовых рассылок
   DELETE MES_BATCH;

   --------------------------------

   -- Определяем существование параметра ChatAggregatorAllowSendToWhatsApp в настройках
   DECLARE @CHAT_AGGREGATOR_ALLOW_SEND_TO_WHATS_APP_OPTIONS_IS_EXIST INT = (
      SELECT COUNT(*) FROM OPTIONS WHERE OPT_NAME LIKE '%ChatAggregatorAllowSendToWhatsApp%'
   );

   -- Если параметр существует, удаляем его из БД
   IF @CHAT_AGGREGATOR_ALLOW_SEND_TO_WHATS_APP_OPTIONS_IS_EXIST > 0
   BEGIN
      DELETE OPTIONS
	  WHERE OPT_NAME LIKE '%ChatAggregatorAllowSendToWhatsApp%'
   END;

END;
            ";

            return _Query;
        }
        //---------------------------------------------------------------
    }
}
