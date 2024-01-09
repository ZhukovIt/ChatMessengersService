using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiMed.ChatMessengers.Umnico
{
    public class AuthorizationInfo
    {
        public string accessToken{ get; set; }
        public string refreshToken{ get; set; }
        public AuthorizationInfo(string _accessToken, string _refreshToken)
        {
            accessToken = _accessToken;
            refreshToken = _refreshToken;
        }
    }
}
