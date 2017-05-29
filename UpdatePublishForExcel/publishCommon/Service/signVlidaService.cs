using publishCommon.Common;
using publishCommon.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace publishCommon.Service
{
    public class signVlidaService
    {
        public string safebase64(string readablestring, string clientkey)
        {
            var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(clientkey));

            var digestByte = hmac.ComputeHash(Encoding.UTF8.GetBytes(readablestring));

            var sign = Convert.ToBase64String(digestByte).Replace('+', '-').Replace('/', '_');
            return sign;
        }

        public string Getclient_secret(ClientClass clientInfo, string userName, string pwd)
        {
            clientInfo.Client_secret = safebase64(string.Format("client_id={0}&device_id={1}&grant_type={2}&username={3}&password={4}&useSign={5}&debugger={6}",
                                          clientInfo.client_id, GetDiviceKey(), "password", userName, pwd, "0", "true"), clientInfo.client_key);
            return clientInfo.Client_secret;

        }

        public string GetDiviceKey()
        {
            return FingerPrint.Value();
        }
    }
}
