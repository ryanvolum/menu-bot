// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using food_bot.Dialogs;
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
    public class Food_botBot : IBot
    {
        const string MENU_PROMPT = "menuPrompt";
        const string MENU_DIALOG = "menuDialog";
        const string DONATE_FOOD_DIALOG = "donateFoodDialog";
        const string FIND_FOOD_DIALOG = "findFoodDialog";
        const string CONTACT_DIALOG = "contactDialog";

        private readonly food_botAccessors _accessors;
        private readonly ILogger _logger;
        private readonly DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that is hooked to the Azure App Service provider.</param>
        public Food_botBot(food_botAccessors accessors, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<Food_botBot>();
            _logger.LogTrace("Turn start.");
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

            // Configure dialogs.
            this._dialogs = new DialogSet(accessors.ConversationDialogStateAccessor);
            this._dialogs.Add(new ChoicePrompt(MENU_PROMPT));
            this._dialogs.Add(new DonateFoodDialog(DONATE_FOOD_DIALOG));
            this._dialogs.Add(new FindFoodDialog(FIND_FOOD_DIALOG));
            this._dialogs.Add(new ContactDialog(CONTACT_DIALOG));

            // Adds a waterfall dialog that prompts users for the top level menu to the dialog set
            // Define the steps of the waterfall dialog and add it to the set.
            WaterfallStep[] steps = new WaterfallStep[] 
                {
                    this.PromptForMenuAsync,
                    this.HandleMenuResultAsync,
                    this.ResetDialogAsync
                };
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
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dialogContext = await this._dialogs.CreateContextAsync(turnContext, cancellationToken);

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                if (dialogContext.ActiveDialog != null)
                {
                    await dialogContext.ContinueDialogAsync(cancellationToken);
                }
                else
                {
                    await dialogContext.BeginDialogAsync(MENU_DIALOG, null, cancellationToken);
                }
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

        /// <summary>
        /// Determines whether this is a new user in an incoming ConversationUpdate activity
        /// </summary>
        /// <param name="activity">Incoming ConversationUpdate activity</param>
        /// <returns>Returns true if a new user is added to the conversation, which is useful for determining when to welcome a user.</returns>
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

        /// <summary>
        /// This step handles the result from the menu prompt above. It begins the appropriate dialog based on which button 
        /// was clicked.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> HandleMenuResultAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            switch (((FoundChoice)stepContext.Result).Value)
            {
                case "Donate Food":
                    return await stepContext.BeginDialogAsync(DONATE_FOOD_DIALOG, null, cancellationToken);
                case "Find a Food Bank":
                    return await stepContext.BeginDialogAsync(FIND_FOOD_DIALOG, null, cancellationToken);
                case "Contact Food Bank":
                    return await stepContext.BeginDialogAsync(CONTACT_DIALOG, null, cancellationToken);
                default:
                    break;
            }

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// This final step in our waterfall dialog replaces the dialog with itself, effectively starting the conversation over. 
        /// This is often referred to as a 'message loop'.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> ResetDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.ReplaceDialogAsync(MENU_DIALOG, null, cancellationToken);
        }
    }
}
