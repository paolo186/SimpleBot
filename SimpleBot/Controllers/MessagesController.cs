using System;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;

namespace SimpleBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.GetActivityType() == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {

                await HandleOtherActivities(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }


        

        private async Task<Activity> HandleOtherActivities(Activity payload)
        {
            string messageType = payload.GetActivityType();
            ChannelAccount botAccount = payload.Recipient;
            if (messageType == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (messageType == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                // Ignore if the bot was the member added
                if (payload.MembersAdded.Any(m => m.Id.Equals(botAccount.Id)))
                {
                    return null;
                }
                await determineFromChannel(payload);


            }
            else if (messageType == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (messageType == ActivityTypes.Typing)
            {
                // Handle knowing that the user is typing
            }
            else if (messageType == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private async Task determineFromChannel(Activity payload)
        {

            string channelId = payload.ChannelId;

            switch (channelId)
            {
                case "webchat":
                    string welcomeMessage = "Welcome Webchat, I am Jarvis. How can I help you?";
                    await MessageHelpers.ReplyToUserSameChannel(payload, welcomeMessage);
                    break;
                case "msteams":
                    await HandleTeamsMessageAsync(payload);
                    break;
                default:
                    // other events
                    break;
            }
        }

        private async Task<Activity> HandleTeamsMessageAsync(Activity payload)
        {
            TeamEventBase eventData = payload.GetConversationUpdateData();
            ChannelAccount botAccount = payload.Recipient;
            string tenantId = payload.GetTenantId();
            TeamsChannelData channelData = payload.GetChannelData<TeamsChannelData>();
            
            switch (eventData.EventType)
            {
                // send a welcome message
                case TeamEventType.MembersAdded:
                    var connector = new ConnectorClient(new Uri(payload.ServiceUrl));
                    IEnumerable<TeamsChannelAccount> newMembers = payload.MembersAdded.AsTeamsChannelAccounts();
                    string welcomeMessage = "Welcome Teams, I am Jarvis. How can I help you?";
                    foreach (TeamsChannelAccount member in newMembers)
                    {
                        // send a 1:1 message to new members
                        await MessageHelpers.SendOneToOneMessage(connector, channelData, botAccount, member, tenantId, welcomeMessage);
                    }

                    break;
            }



            return null;
        }
    }
}