using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleBot.Dialogs
{
    [Serializable]
    public class ResetTurnCount : IDialog<int>
    {

        private int count;
        private int attempts = 3;

        public ResetTurnCount(int count)
        {
            this.count = count;
        }

        public Task StartAsync(IDialogContext context)
        {
            PromptDialog.Confirm(
                context,
                AfterResetTurn,
                "Are you sure you want to reset the turn count?",
                "I didn't get that. I'm looking for 'yes' or 'no'"
            );
            return Task.CompletedTask;

        }



        private async Task AfterResetTurn(IDialogContext context, IAwaitable<bool> result)
        {
            var confirm = await result;

            if (confirm)
            {
                //this.count = 1;
                //await context.PostAsync("Reset count to 1.");

                await context.PostAsync("Sure, I can help with that. Choose a whole number > 0 (e.g. '1', '100')");
                context.Wait(ChooseNewTurn);

            }
            else
            {
                await context.PostAsync($"Did not reset turn count. (next count={this.count}) ");
                context.Done(this.count);
            }
            //context.Done(this.count);


        }

        private async Task ChooseNewTurn(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            int age;



            if (Int32.TryParse(message.Text, out age) && (age > 0))
            {
                await context.PostAsync($"Changed turn count to {age}.");
                context.Done(age);
            }
            else if ((--attempts) <= 0)
            {

                context.Fail(new TooManyAttemptsException("Message was not a valid whole number"));
            }
            else
            { 
                await context.PostAsync("I didn't get that. I'm looking for whole numbers greater than 0 (e.g. '1', '30')");
                context.Wait(ChooseNewTurn);
            }

        }
    }
}