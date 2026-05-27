using System;
using System.Text;

namespace _Projects.Utils.HTTPUtils
{
    public static class HTTPConfig
    {
        // ベーシック認証のユーザー名とパスワード
        public const bool UseBasicAuth = true;
        public const string BasicAuthUsername = "morijyobi";
        public const string BasicAuthPassword = "game#1500";

        // タイムアウト時間（秒）
        public const int Timeout = 30;

        /// <summary>
        /// Basic認証用ヘッダー（"Authorization": "Basic xxx"）
        /// </summary>
        public static string AuthorizationHeader
        {
            get
            {
                string raw = $"{BasicAuthUsername}:{BasicAuthPassword}";
                string base64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(raw));
                return $"Basic {base64}";
            }
        }
    }
}