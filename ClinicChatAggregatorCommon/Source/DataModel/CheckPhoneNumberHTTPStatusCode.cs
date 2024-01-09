using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicChatAggregatorCommon.DataModel
{
    public enum CheckPhoneNumberHTTPStatusCode
    {
        PhoneNumberIsExists = 200,
        IntegrationNotSupported = 400,
        PhoneNumberIsNotExists = 404,
        ThrowException = 500
    }
}
