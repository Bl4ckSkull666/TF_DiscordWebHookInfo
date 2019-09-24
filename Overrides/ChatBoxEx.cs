using Bolt;
using DiscordWebhookInfo.Classes;
using DiscordWebhookInfo.Discord;
using System;
using TheForest.UI.Multiplayer;

namespace DiscordWebhookInfo.Overrides
{
    public class ChatBoxEx : ChatBox
    {
        private Config _myConfig = null;

        public override void AddLine(NetworkId? id, string message, bool system)
        {
            base.AddLine(id, message, system);

            if (id == null)
                return;

            if (_myConfig == null)
                _myConfig = new Config();

            //Send Chat to Discord
            if (!_myConfig.getBool("chat_webhook_use"))
                return;

            Webhook dw = new Webhook(_myConfig.getString("chat_webhook_url"));
            dw.Send($"{_myConfig.getString("chat_webhook_prefix")}{message}", _players[id.Value]._name);
        }

        public override void RegisterPlayer(string name, NetworkId id)
        {
            base.RegisterPlayer(name, id);

            if (_myConfig == null)
                _myConfig = new Config();

            if (_myConfig.getBool("join_webhook_use"))
                PlayerJoin(name);

            if(_myConfig.getBool("join_chat_use"))
                sendChatMessage(id, _myConfig.getString("join_chat_msg").Replace("%p", name));
        }

        public override void UnregisterPlayer(NetworkId id)
        {
            if (_myConfig == null)
                _myConfig = new Config();

            if (_players.ContainsKey(id) && _myConfig.getBool("left_webhook_use"))
                PlayerLeft(_players[id]._name);

            if (_players.ContainsKey(id) &&_myConfig.getBool("left_chat_use"))
                sendChatMessage(id, _myConfig.getString("left_chat_msg").Replace("%p", _players[id]._name));

            base.UnregisterPlayer(id);
        }

        private void PlayerJoin(String p)
        {
            Webhook dw = new Webhook(_myConfig.getString("join_webhook_url"));
            dw.Send(_myConfig.getString("join_webhook_msg").Replace("%p", p));
        }

        private void PlayerLeft(String p)
        {
            Webhook dw = new Webhook(_myConfig.getString("left_webhook_url"));
            dw.Send(_myConfig.getString("left_webhook_msg").Replace("%p", p));
        }

        private void sendChatMessage(NetworkId id, string message)
        {
            ChatEvent c = ChatEvent.Create(GlobalTargets.AllClients);
            c.Message = message;
            c.Sender = id;
            c.Send();
        }
    }
}
