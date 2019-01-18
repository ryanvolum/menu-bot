using food_bot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace food_bot.Dialogs
{
    public class DonateFoodDialog : ComponentDialog
    {
        public DonateFoodDialog(string dialogId) : base(dialogId)
        {
            // ID of the child dialog that should be started anytime the component is started.
            this.InitialDialogId = dialogId;

            this.AddDialog(new ChoicePrompt("choicePrompt"));

            // Define the conversation flow using the waterfall model.
            this.AddDialog(
                new WaterfallDialog(dialogId, new WaterfallStep[]
                    {
                        async (stepContext, ct) =>
                        {
                            return await stepContext.PromptAsync(
                                "choicePrompt",
                                new PromptOptions
                                {
                                    Choices = ChoiceFactory.ToChoices(ScheduleHelpers.GetValidDonationDays()),
                                    Prompt = MessageFactory.Text("What day would you like to donate food?"),
                                    RetryPrompt= MessageFactory.Text("That's not a valid day! Please choose a valid day.")
                                },
                                ct
                            );
                        },
                        async (stepContext, ct) =>
                        {
                            var day = ((FoundChoice)stepContext.Result).Value;
                            var filteredFoodBanks = ScheduleHelpers.FilterFoodBanksByDonation(day);
                            var carousel = ScheduleHelpers.CreateFoodBankDonationCarousel(filteredFoodBanks).AsMessageActivity();

                            // Create the activity and attach a set of Hero cards.
                            await stepContext.Context.SendActivityAsync(carousel);

                            return await stepContext.EndDialogAsync();

                        }
                    }
                )
            );
        }
    }
}