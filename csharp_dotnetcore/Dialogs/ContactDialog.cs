using FoodBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodBot.Dialogs
{
    public class ContactDialog : ComponentDialog
    {
        public ContactDialog(string dialogId) : base(dialogId)
        {
            const string FOODBANKNAME = "foodBankName";
            const string EMAILADDRESS = "emailAddress";
            const string MESSAGE = "message";

            // ID of the child dialog that should be started anytime the component is started.
            this.InitialDialogId = dialogId;

            this.AddDialog(new ChoicePrompt("choicePrompt"));
            this.AddDialog(new TextPrompt("textPrompt"));
            this.AddDialog(new ConfirmPrompt("confirmPrompt"));

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
                                    Choices = ChoiceFactory.ToChoices(ScheduleHelpers.GetFoodBanks()),
                                    Prompt = MessageFactory.Text("Which food bank would you like to contact?"),
                                    RetryPrompt= MessageFactory.Text("That's not a valid food bank! Please choose a valid food bank.")
                                },
                                ct
                            ).ConfigureAwait(false);;
                        },
                        async (stepContext, ct) =>
                        {
                            // Persist the food bank name for later waterfall steps to be able to access it
                            stepContext.Values.Add(FOODBANKNAME, ((FoundChoice)stepContext.Result).Value);

                            return await stepContext.PromptAsync("textPrompt", new PromptOptions
                                {
                                    Prompt = MessageFactory.Text($"Please enter an email address where {stepContext.Values[FOODBANKNAME]} can message you back at:")
                                },
                                ct
                            ).ConfigureAwait(false);;
                        },
                        async (stepContext, ct) =>
                        {
                            // Persist the email address for later waterfall steps to be able to access it
                            stepContext.Values.Add(EMAILADDRESS, (string)stepContext.Result);

                            return await stepContext.PromptAsync("textPrompt", new PromptOptions
                                {
                                    Prompt = MessageFactory.Text($"Please enter the message you'd like to send to {stepContext.Values[FOODBANKNAME]}:")
                                },
                                ct
                            ).ConfigureAwait(false);;
                        },
                        async (stepContext, ct) =>
                        {
                            // Persist the message for later waterfall steps to be able to access it
                            stepContext.Values.Add(MESSAGE, (string)stepContext.Result);

                            return await stepContext.PromptAsync("confirmPrompt", new PromptOptions
                                {
                                    Prompt = MessageFactory.Text($"Are you sure you'd like to send:\r \"{stepContext.Values[MESSAGE]}\"\r to {stepContext.Values[FOODBANKNAME]}?")
                                },
                                ct
                            ).ConfigureAwait(false);
                        },
                        async (stepContext, ct) =>
                        {
                            // Simulates sending of a message (nothing is actually being sent anywhere)
                            if ((bool)stepContext.Result)
                            {
                                ScheduleHelpers.SendFoodbankMessage((string)stepContext.Values[FOODBANKNAME], (string)stepContext.Values[EMAILADDRESS], (string)stepContext.Values[MESSAGE]);
                                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Great! We've sent your message to {stepContext.Values[FOODBANKNAME]}. Expect your response to be sent to {stepContext.Values[EMAILADDRESS]}"), ct).ConfigureAwait(false);;
                            }
                            else
                            {
                                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"No worries! I won't send your message"), ct).ConfigureAwait(false);
                            }

                            return await stepContext.EndDialogAsync().ConfigureAwait(false);;
                        }
                    }
                )
            );
        }
    }
}
