using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;

namespace DiscordWebhookInfo.Discord
{
    [JsonObject]
    public class Webhook
    {
        private readonly WebRequest _httpClient;
        //private readonly string _webhookUrl;

        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
        // ReSharper disable once InconsistentNaming
        [JsonProperty("tts")]
        public bool IsTTS { get; set; }
        [JsonProperty("embeds")]
        public List<Embed> Embeds { get; set; } = new List<Embed>();

        public Webhook(string webhookUrl)
        {
            _httpClient = WebRequest.Create(webhookUrl);
        }

        public void Send()
        {
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

            _httpClient.Method = "POST";
            _httpClient.ContentType = "application/json";
            byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));

            Stream dataStream = _httpClient.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
        }

        // ReSharper disable once InconsistentNaming
        public void Send(string content, string username = null, string avatarUrl = null, bool isTTS = false, IEnumerable<Embed> embeds = null)
        {
            Content = content;
            Username = username;
            AvatarUrl = avatarUrl;
            IsTTS = isTTS;
            Embeds.Clear();
            if (embeds != null)
            {
                Embeds.AddRange(embeds);
            }

            Send();
        }

        public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain,
            // look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        continue;
                    }
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                        break;
                    }
                }
            }
            return isOk;
        }
    }
}
