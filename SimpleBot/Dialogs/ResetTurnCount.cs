using Microsoft.Bot.Builder.Dialogs;
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
                this.count = 1;
                await context.PostAsync("Reset count to 1.");

            }
            else
            {
                // current count was previously updated. Subtract 1 to get actual current count
                await context.PostAsync($"Did not reset count. (count={this.count - 1}) ");
            }
            context.Done(this.count);


        }
    }
}