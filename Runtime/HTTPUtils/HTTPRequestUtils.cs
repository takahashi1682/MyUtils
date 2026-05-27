using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace _Projects.Utils.HTTPUtils
{
    /// <summary>
    /// UnityWebRequestを使用したHTTPリクエストを送信するためのユーティリティクラス
    /// </summary>
    public static class HTTPRequestUtils
    {
        /// <summary>
        /// GETリクエストを非同期で送信し、結果をデシリアライズして返します。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="jsonFormatter"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async UniTask<TResult> GetAsync<TResult>(
            string url,
            CancellationToken token,
            Func<string, string> jsonFormatter = null)
        {
            using var request = UnityWebRequest.Get(url);
            await SendRequestAsync(request, token);

            string responseJson = request.downloadHandler.text;
            
            // もしフォーマッターが指定されていれば、JSONを整形する
            if (jsonFormatter != null)
            {
                responseJson = jsonFormatter(responseJson);
            }

            return JsonUtility.FromJson<TResult>(responseJson);
        }

        /// <summary>
        /// POSTリクエストを非同期で送信し、結果をデシリアライズして返します。
        /// </summary>
        /// <param name="url">リクエスト先のURL</param>
        /// <param name="value">送信するオブジェクト</param>
        /// <param name="token">キャンセルトークン</param>
        public static async UniTask<TValue> PostAsync<TValue>(
            string url,
            TValue value,
            CancellationToken token)
        {
            // PUTもPOSTもリクエスト作成方法は同じ
            using var request = CreateJsonRequest(url, UnityWebRequest.kHttpVerbPOST, value);
            await SendRequestAsync(request, token);
            return JsonUtility.FromJson<TValue>(request.downloadHandler.text);
        }

        /// <summary>
        /// PUTリクエストを非同期で送信し、結果をデシリアライズして返します。
        /// </summary>
        /// <param name="url">リクエスト先のURL</param>
        /// <param name="value">送信するオブジェクト</param>
        /// <param name="token">キャンセルトークン</param>
        public static async UniTask<TValue> PutAsync<TValue>(
            string url,
            TValue value,
            CancellationToken token)
        {
            using var request = CreateJsonRequest(url, UnityWebRequest.kHttpVerbPUT, value);
            await SendRequestAsync(request, token);
            return JsonUtility.FromJson<TValue>(request.downloadHandler.text);
        }

        /// <summary>
        /// DELETEリクエストを非同期で送信し、HTTPステータスコードを返します。
        /// </summary>
        /// <param name="url">リクエスト先のURL（IDなどを含む完全なURL）</param>
        /// <param name="token">キャンセルトークン</param>
        public static async UniTask<long> DeleteAsync(string url, CancellationToken token)
        {
            using var request = UnityWebRequest.Delete(url);
            await SendRequestAsync(request, token);
            return request.responseCode;
        }


        // --- Private Helper Methods ---

        /// <summary>
        /// UnityWebRequestを送信し、エラーハンドリングを行う共通メソッド
        /// </summary>
        private static async UniTask SendRequestAsync(UnityWebRequest request, CancellationToken token)
        {
            // ベーシック認証が有効な場合、Authorizationヘッダーを追加
            if (HTTPConfig.UseBasicAuth)
            {
                request.SetRequestHeader("Authorization", HTTPConfig.AuthorizationHeader);
            }

            request.timeout = HTTPConfig.Timeout;

            try
            {
                await request.SendWebRequest().WithCancellation(token);
            }
            catch (UnityWebRequestException)
            {
                // エラー内容をログに出力し、例外を再スローして呼び出し元に失敗を通知する
                Debug.LogError($"Request to {request.url} failed. " +
                               $"Method: {request.method}, " +
                               $"Status Code: {request.responseCode}, " +
                               $"Error: {request.error}, " +
                               $"Response: {request.downloadHandler?.text}");
                throw; // 呼び出し元でさらにエラーハンドリングができるように再スロー
            }
        }

        /// <summary>
        /// JSONボディを持つリクエスト（POST, PUTなど）を生成します。
        /// </summary>
        private static UnityWebRequest CreateJsonRequest<TValue>(string url, string method, TValue value)
        {
            var request = new UnityWebRequest(url, method)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(value))),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            return request;
        }
    }
}