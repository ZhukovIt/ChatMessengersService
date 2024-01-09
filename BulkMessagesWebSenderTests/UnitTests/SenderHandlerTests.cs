using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ChatMessengersService.MassMessagesUmnicoSenders.SenderHandlers;
using BulkMessagesWebServer.Tests.Common;
using BulkMessagesWebServer.DBObjects.MessengerDialog;
using SiMed.Clinic.DataModel;

namespace BulkMessagesWebServer.Tests.Unit
{
    public sealed class SenderHandlerTests
    {
        [Fact]
        public void Select_Send_Message_In_WhatsApp_Because_Of_WhatsApp_Is_Priority_And_Has_Dialogs_With_Client_Answers()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = "",
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = "",
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        IsPriority = true,
                        SourceTypeId = 3,
                        SourceTypeUId = ""
                    },
                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        IsPriority = false,
                        SourceTypeId = 4,
                        SourceTypeUId = ""
                    }
                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        MessengerDialogId = 1,
                        MessengerDialogUId = "1",
                        SourceTypeId = 3,
                        HasMessengerDialogMessagesFromPerson = true
                    },

                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        MessengerDialogId = 2,
                        MessengerDialogUId = "2",
                        SourceTypeId = 4,
                        HasMessengerDialogMessagesFromPerson = true
                    },

                    new MessengerDialogDataModel(3, "Вконтакте")
                    {
                        MessengerDialogId = 3,
                        MessengerDialogUId = "3",
                        SourceTypeId = 5,
                        HasMessengerDialogMessagesFromPerson = true
                    }
                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.Equal(MessengerSendMethods.SendMessage, result.MessengerSendMethod);
            Assert.Equal(MessengersType.WhatsApp, result.MessengerType);
            Assert.Null(sut.SendMessageError);
            Assert.True(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
        [Fact]
        public void Select_Telegram_Because_Of_WhatsApp_Is_Priority_But_Has_Not_Dialogs_With_Client_Answers()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = "",
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = "",
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        IsPriority = true,
                        SourceTypeId = 3,
                        SourceTypeUId = ""
                    },
                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        IsPriority = false,
                        SourceTypeId = 4,
                        SourceTypeUId = ""
                    }
                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        MessengerDialogId = 1,
                        MessengerDialogUId = "1",
                        SourceTypeId = 3,
                        HasMessengerDialogMessagesFromPerson = false
                    },

                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        MessengerDialogId = 2,
                        MessengerDialogUId = "2",
                        SourceTypeId = 4,
                        HasMessengerDialogMessagesFromPerson = true
                    },

                    new MessengerDialogDataModel(3, "Вконтакте")
                    {
                        MessengerDialogId = 3,
                        MessengerDialogUId = "3",
                        SourceTypeId = 5,
                        HasMessengerDialogMessagesFromPerson = true
                    }
                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.Equal(MessengersType.Telegram, result.MessengerType);
            Assert.Null(sut.SendMessageError);
            Assert.True(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
        [Fact]
        public void Select_Send_Message_In_Telegram_Because_Of_Telegram_Is_Priority_And_Has_Dialogs()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = "",
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = "",
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        IsPriority = false,
                        SourceTypeId = 3,
                        SourceTypeUId = ""
                    },
                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        IsPriority = true,
                        SourceTypeId = 4,
                        SourceTypeUId = ""
                    }
                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        MessengerDialogId = 1,
                        MessengerDialogUId = "1",
                        SourceTypeId = 3,
                        HasMessengerDialogMessagesFromPerson = true
                    },

                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        MessengerDialogId = 2,
                        MessengerDialogUId = "2",
                        SourceTypeId = 4,
                        HasMessengerDialogMessagesFromPerson = false
                    },

                    new MessengerDialogDataModel(3, "Вконтакте")
                    {
                        MessengerDialogId = 3,
                        MessengerDialogUId = "3",
                        SourceTypeId = 5,
                        HasMessengerDialogMessagesFromPerson = true
                    }
                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.Equal(MessengerSendMethods.SendMessage, result.MessengerSendMethod);
            Assert.Equal(MessengersType.Telegram, result.MessengerType);
            Assert.Null(sut.SendMessageError);
            Assert.True(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
        [Fact]
        public void Select_First_Send_Message_In_Telegram_Because_Of_Telegram_Is_Priority_But_Has_Not_Dialogs()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = "9042194323",
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = "",
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        IsPriority = false,
                        SourceTypeId = 3,
                        SourceTypeUId = ""
                    },
                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        IsPriority = true,
                        SourceTypeId = 4,
                        SourceTypeUId = ""
                    }
                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        MessengerDialogId = 1,
                        MessengerDialogUId = "1",
                        SourceTypeId = 3,
                        HasMessengerDialogMessagesFromPerson = true
                    },

                    new MessengerDialogDataModel(3, "Вконтакте")
                    {
                        MessengerDialogId = 3,
                        MessengerDialogUId = "3",
                        SourceTypeId = 5,
                        HasMessengerDialogMessagesFromPerson = true
                    }
                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.Equal(MessengerSendMethods.FirstSendMessage, result.MessengerSendMethod);
            Assert.Equal(MessengersType.Telegram, result.MessengerType);
            Assert.Null(sut.SendMessageError);
            Assert.True(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
        [Fact]
        public void Select_Send_Message_In_Vkontakte_Because_Of_Vkontakte_Is_Priority_And_Has_Dialogs()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = "",
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = "",
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        IsPriority = false,
                        SourceTypeId = 3,
                        SourceTypeUId = ""
                    },
                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        IsPriority = false,
                        SourceTypeId = 4,
                        SourceTypeUId = ""
                    },
                    new MessengerDialogDataModel(3, "Вконтакте")
                    {
                        IsPriority = true,
                        SourceTypeId = 5,
                        SourceTypeUId = ""
                    }
                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        MessengerDialogId = 1,
                        MessengerDialogUId = "1",
                        SourceTypeId = 3,
                        HasMessengerDialogMessagesFromPerson = true
                    },

                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        MessengerDialogId = 2,
                        MessengerDialogUId = "2",
                        SourceTypeId = 4,
                        HasMessengerDialogMessagesFromPerson = false
                    },

                    new MessengerDialogDataModel(3, "Вконтакте")
                    {
                        MessengerDialogId = 3,
                        MessengerDialogUId = "3",
                        SourceTypeId = 5,
                        HasMessengerDialogMessagesFromPerson = true
                    }
                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.Equal(MessengerSendMethods.SendMessage, result.MessengerSendMethod);
            Assert.Equal(MessengersType.Vk, result.MessengerType);
            Assert.Null(sut.SendMessageError);
            Assert.True(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
        [Fact]
        public void Select_Send_Message_In_Telegram_Because_Of_Priority_Messenger_Is_Not_Exists_And_Telegram_Has_Dialogs()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = "",
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = "",
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        IsPriority = false,
                        SourceTypeId = 3,
                        SourceTypeUId = ""
                    },
                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        IsPriority = false,
                        SourceTypeId = 4,
                        SourceTypeUId = ""
                    }
                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        MessengerDialogId = 1,
                        MessengerDialogUId = "1",
                        SourceTypeId = 3,
                        HasMessengerDialogMessagesFromPerson = true
                    },

                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        MessengerDialogId = 2,
                        MessengerDialogUId = "2",
                        SourceTypeId = 4,
                        HasMessengerDialogMessagesFromPerson = false
                    },

                    new MessengerDialogDataModel(3, "Вконтакте")
                    {
                        MessengerDialogId = 3,
                        MessengerDialogUId = "3",
                        SourceTypeId = 5,
                        HasMessengerDialogMessagesFromPerson = true
                    }
                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.Equal(MessengerSendMethods.SendMessage, result.MessengerSendMethod);
            Assert.Equal(MessengersType.Telegram, result.MessengerType);
            Assert.Null(sut.SendMessageError);
            Assert.True(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
        [Fact]
        public void Select_Send_Message_In_WhatsApp_Because_Of_Priority_Messenger_Is_Not_Exists_But_Telegram_Has_Not_Dialogs()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = "",
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = "",
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        IsPriority = false,
                        SourceTypeId = 3,
                        SourceTypeUId = ""
                    },
                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        IsPriority = false,
                        SourceTypeId = 4,
                        SourceTypeUId = ""
                    }
                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        MessengerDialogId = 1,
                        MessengerDialogUId = "1",
                        SourceTypeId = 3,
                        HasMessengerDialogMessagesFromPerson = true
                    },

                    new MessengerDialogDataModel(3, "Вконтакте")
                    {
                        MessengerDialogId = 3,
                        MessengerDialogUId = "3",
                        SourceTypeId = 5,
                        HasMessengerDialogMessagesFromPerson = true
                    }
                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.Equal(MessengerSendMethods.SendMessage, result.MessengerSendMethod);
            Assert.Equal(MessengersType.WhatsApp, result.MessengerType);
            Assert.Null(sut.SendMessageError);
            Assert.True(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
        [Fact]
        public void Not_Select_WhatsApp_Because_Of_Priority_Messenger_Is_Not_Exists_And_WhatsApp_Has_Not_Dialogs_With_Client_Answers()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = "",
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = "",
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        IsPriority = false,
                        SourceTypeId = 3,
                        SourceTypeUId = ""
                    },
                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        IsPriority = false,
                        SourceTypeId = 4,
                        SourceTypeUId = ""
                    }
                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        MessengerDialogId = 1,
                        MessengerDialogUId = "1",
                        SourceTypeId = 3,
                        HasMessengerDialogMessagesFromPerson = false
                    },

                    new MessengerDialogDataModel(3, "Вконтакте")
                    {
                        MessengerDialogId = 3,
                        MessengerDialogUId = "3",
                        SourceTypeId = 5,
                        HasMessengerDialogMessagesFromPerson = true
                    }
                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.NotEqual(MessengersType.WhatsApp, result.MessengerType);
            Assert.Null(sut.SendMessageError);
            Assert.True(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
        [Fact]
        public void Select_Send_Message_In_Vk_Because_Of_Priority_Messenger_Is_Not_Exists_And_Vk_Has_Dialogs()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = "",
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = "",
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        IsPriority = false,
                        SourceTypeId = 3,
                        SourceTypeUId = ""
                    },
                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        IsPriority = false,
                        SourceTypeId = 4,
                        SourceTypeUId = ""
                    }
                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(3, "Вконтакте")
                    {
                        MessengerDialogId = 3,
                        MessengerDialogUId = "3",
                        SourceTypeId = 5,
                        HasMessengerDialogMessagesFromPerson = false
                    }
                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.Equal(MessengerSendMethods.SendMessage, result.MessengerSendMethod);
            Assert.Equal(MessengersType.Vk, result.MessengerType);
            Assert.Null(sut.SendMessageError);
            Assert.True(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
        [Fact]
        public void Select_First_Send_Message_In_Telegram_As_Default_Variant_If_Telephone_Number_Is_Correct()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = "9042194323",
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = "",
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {

                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {

                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.Equal(MessengerSendMethods.FirstSendMessage, result.MessengerSendMethod);
            Assert.Equal(MessengersType.Telegram, result.MessengerType);
            Assert.Null(sut.SendMessageError);
            Assert.True(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
        [Fact]
        public void Not_Select_First_Send_Message_In_Telegram_As_Default_Variant_If_Telephone_Number_Is_Empty()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = "",
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = "",
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {

                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {

                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.Equal(MessengerSendMethods.None, result.MessengerSendMethod);
            Assert.Equal(null, result.MessengerType);
            Assert.NotNull(sut.SendMessageError);
            Assert.False(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
        [Fact]
        public void Not_Select_Messenger_If_Not_Variants_And_Telegram_Source_Type_UId_Equal_Null()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = "",
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = null,
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {

                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {

                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.Equal(MessengerSendMethods.None, result.MessengerSendMethod);
            Assert.Equal(null, result.MessengerType);
            Assert.NotNull(sut.SendMessageError);
            Assert.False(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
        [Fact]
        public void Not_Select_Messenger_If_Person_Phone_Number_Is_Null()
        {
            AbstractSenderHandler sut = MotherObject.CreateMainSenderHandler();
            MessengerDialogDBContent _Content = new MessengerDialogDBContent(4995)
            {
                PersonPhoneNumber = null,
                TelegramSourceTypeId = 4,
                TelegramSourceTypeUId = "",
                PersonMessengerTypes = new List<MessengerDialogDataModel>()
                {
                    new MessengerDialogDataModel(1, "WhatsApp")
                    {
                        IsPriority = false,
                        SourceTypeId = 3,
                        SourceTypeUId = ""
                    },
                    new MessengerDialogDataModel(2, "Telegram")
                    {
                        IsPriority = true,
                        SourceTypeId = 4,
                        SourceTypeUId = ""
                    }
                },

                MessengerTypesWhereDialogIsExists = new List<MessengerDialogDataModel>()
                {

                }
            };

            MessengerSendAction result = sut.HandleRequest(_Content);

            Assert.Equal(MessengerSendMethods.None, result.MessengerSendMethod);
            Assert.Equal(null, result.MessengerType);
            Assert.NotNull(sut.SendMessageError);
            Assert.False(sut.CanSendMessage);
            Assert.NotNull(sut.TypeDefinition);
        }
        //--------------------------------------------------------------------------------------------
    }
}
