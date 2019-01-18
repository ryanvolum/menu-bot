// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace food_bot
{
    /// <summary>
    /// This bot demonstrates several concepts at once. At a high level, it offers a fully guided flow 
    /// so that the user is always aware of their options on any given conversation turn. When bots leave 
    /// conversations open-ended, users often don't know what to do or say - guiding conversations with 
    /// buttons (even if you also support natural language) helps mitigate this issue. 
    /// We construct the guided conversation by first welcoming the user, and then by using waterfall dialogs 
    /// and prompts to communicate with our user.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class food_botBot : IBot
    {
        const string MENU_PROMPT = "menuPrompt";
        const string MENU_DIALOG = "menuDialog";
        private readonly food_botAccessors _accessors;
        private readonly ILogger _logger;
        private DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that is hooked to the Azure App Service provider.</param>
        public food_botBot(food_botAccessors accessors, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<food_botBot>();
            _logger.LogTrace("Turn start.");
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

            // Configure dialogs.
            this._dialogs = new DialogSet(accessors.ConversationDialogStateAccessor);
            this._dialogs.Add(new ChoicePrompt(MENU_PROMPT));

            // Adds a waterfall dialog that prompts users for the top level menu to the dialog set
            // Define the steps of the waterfall dialog and add it to the set.
            WaterfallStep[] steps = new WaterfallStep[] { this.PromptForMenuAsync };
            this._dialogs.Add(new WaterfallDialog(MENU_DIALOG, steps));

        }

        /// <summary>
        /// Every conversation turn for our Bot will call this method.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dialogContext = await this._dialogs.CreateContextAsync(turnContext, cancellationToken);

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Echo back to the user whatever they typed.
                var responseMessage = $"You sent '{turnContext.Activity.Text}'\n";

                await turnContext.SendActivityAsync(responseMessage, cancellationToken: cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (this.MemberJoined(turnContext.Activity))
                {
                    await turnContext.SendActivityAsync("Hey there! Welcome to the food bank bot. I'm here to help orchestrate the delivery of excess food to local food banks!");

                    await dialogContext.BeginDialogAsync(MENU_DIALOG, null, cancellationToken);
                }
            }

            // Save the dialog state into the conversation state.
            await _accessors.ConversationState.SaveChangesAsync(turnContext);
        }

        private bool MemberJoined(Activity activity)
        {
            return ((activity.MembersAdded.Count != 0 && (activity.MembersAdded[0].Id != activity.Recipient.Id)));
        }

        /// <summary>
        /// The first function in our waterfall dialog prompts the user with three options, 'Donate Food' and 'Find a Food Bank'
        /// and 'Contact Food Bank'.
        /// It uses the ChoicePrompt added in the contructor and referenced by the MENU_PROMPT string. The array of
        /// strings passed in as choices will be rendered as suggestedAction buttons which the user can then click.If the
        /// user types anything other than the button text, the choice prompt will reject it and reprompt using the retryPrompt
        /// string. 
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> PromptForMenuAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(MENU_PROMPT, 
                new PromptOptions
                {
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Donate Food", "Find a Food Bank", "Contact Food Bank" }),
                    Prompt = MessageFactory.Text("Do you have food to donate, do you need food, or are you contacting a food bank?"),
                    RetryPrompt = MessageFactory.Text("I'm sorry, that wasn't a valid response. Please select one of the options")
                },
                cancellationToken);
        }
    }
}
