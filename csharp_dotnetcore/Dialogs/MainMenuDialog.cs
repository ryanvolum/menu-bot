using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FoodBot.Dialogs
{
    public class MainMenuDialog : ComponentDialog
    {
        const string MENUPROMPT = "menuPrompt";
        const string DONATEFOODDIALOG = "donateFoodDialog";
        const string FINDFOODDIALOG = "findFoodDialog";
        const string CONTACTDIALOG = "contactDialog";

        public MainMenuDialog(string dialogId) : base(dialogId)
        {
            // ID of the child dialog that should be started anytime the component is started.
            this.InitialDialogId = dialogId;

            // Adds a waterfall dialog that prompts users with the top level menu to the dialog set.
            // Define the steps of the waterfall dialog and add it to the set.
            this.AddDialog(new WaterfallDialog(
                dialogId,
                new WaterfallStep[]
                {
                    this.PromptForMenuAsync,
                    this.HandleMenuResultAsync,
                    this.ResetDialogAsync
                }));

            this.AddDialog(new ChoicePrompt(MENUPROMPT));
            this.AddDialog(new DonateFoodDialog(DONATEFOODDIALOG));
            this.AddDialog(new FindFoodDialog(FINDFOODDIALOG));
            this.AddDialog(new ContactDialog(CONTACTDIALOG));
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
            return await stepContext.PromptAsync(MENUPROMPT,
                new PromptOptions
                {
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Donate Food", "Find a Food Bank", "Contact Food Bank" }),
                    Prompt = MessageFactory.Text("Do you have food to donate, do you need food, or are you contacting a food bank?"),
                    RetryPrompt = MessageFactory.Text("I'm sorry, that wasn't a valid response. Please select one of the options")
                },
                cancellationToken).ConfigureAwait(false);
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
                    return await stepContext.BeginDialogAsync(DONATEFOODDIALOG, null, cancellationToken).ConfigureAwait(false); ;
                case "Find a Food Bank":
                    return await stepContext.BeginDialogAsync(FINDFOODDIALOG, null, cancellationToken).ConfigureAwait(false); ;
                case "Contact Food Bank":
                    return await stepContext.BeginDialogAsync(CONTACTDIALOG, null, cancellationToken).ConfigureAwait(false); ;
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
            return await stepContext.ReplaceDialogAsync(this.InitialDialogId, null, cancellationToken).ConfigureAwait(false); ;
        }
    }
}
