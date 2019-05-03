using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleBot
{
    public class MessageHelpers
    {
        public static async Task SendOneToOneMessage(
            ConnectorClient connector,
            TeamsChannelData channelData,
            ChannelAccount botAccount, ChannelAccount userAccount,
            string tenantId, string welcomeMessage) 
        {

            // create or get existing chat conversation with user
            var response = connector.Conversations.CreateOrGetDirectConversation(botAccount, userAccount, tenantId);

            // Construct the message to post to conversation
            Activity newActivity = new Activity()
            {
                Text = welcomeMessage,
                Type = ActivityTypes.Message,
                Conversation = new ConversationAccount
                {
                    Id = response.Id
                },
            };

            // Post the message to chat conversation with user
            await connector.Conversations.SendToConversationAsync(newActivity);
        }

        public static async Task ReplyToUserSameChannel(Activity payload, string message)
        {

            // replies to the same channel as the sender
            ConnectorClient connector = new ConnectorClient(new Uri(payload.ServiceUrl));
            var reply = payload.CreateReply(message);
            await connector.Conversations.ReplyToActivityAsync(reply);
        }




    }
}