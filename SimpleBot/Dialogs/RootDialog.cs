using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using System.Text;

namespace SimpleBot.Dialogs
{

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private int turnCount = 1;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            Activity payload = await result as Activity;
            string message = payload.GetTextWithoutMentions();

            if (message.ToLower().Contains("reset"))
            {
    
                context.Call(new ResetTurnCount(this.turnCount), AfterResetTurnCount);

            }
            else
            {
                int length = (message ?? string.Empty).Length;
                await context.PostAsync($"Turn {this.turnCount++}: You sent '{message}' which has {length} characters.");
                context.Wait(MessageReceivedAsync);
            }


        }


        private async Task AfterResetTurnCount(IDialogContext context, IAwaitable<int> result)
        {
            try
            {
                var number = await result;
                this.turnCount = number;
            }
            catch (TooManyAttemptsException e)
            {
                StringBuilder sb = new StringBuilder($"I'm sorry, I'm having issues understanding you. {e.Message}.");
                sb.Append($"\nDid not reset turn count. (next count={this.turnCount})");
                await context.PostAsync(sb.ToString());
                
            } finally
            {
                context.Wait(MessageReceivedAsync);
            }

            

        }



    }
}