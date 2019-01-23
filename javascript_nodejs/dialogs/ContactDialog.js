const { ComponentDialog, ConfirmPrompt, ChoicePrompt, TextPrompt, WaterfallDialog } = require('botbuilder-dialogs');
const { getFoodBanks, sendFoodBankMessage } = require('../services/schedule-helpers');

class ContactDialog extends ComponentDialog {
    constructor(dialogId) {
        super(dialogId);

        // ID of the child dialog that should be started anytime the component is started.
        this.initialDialogId = dialogId;

        // Define the prompts used in this conversation flow.
        this.addDialog(new ChoicePrompt('choicePrompt'));
        this.addDialog(new TextPrompt('textPrompt'));
        this.addDialog(new ConfirmPrompt('confirmPrompt'));

        // Define the conversation flow using a waterfall model.
        this.addDialog(new WaterfallDialog(dialogId, [
            async function (step) {
                const foodBanks = getFoodBanks();

                return await step.prompt('choicePrompt', {
                    choices: foodBanks,
                    prompt: "Which food bank would you like to contact?",
                    retryPrompt: "That's not a valid food bank! Please choose a valid food bank."
                });
            },
            async function (step) {
                // Persist the food bank name for later waterfall steps to be able to access it
                step.values.foodBankName = step.result.value;

                return await step.prompt('textPrompt', {
                    prompt: `Please enter an email address where ${step.values.foodBankName} can message you back at:`
                });
            },
            async function (step) {
                // Persist the email address for later waterfall steps to be able to access it
                step.values.email = step.result;

                return await step.prompt('textPrompt',
                    `Please enter the message you'd like to send to ${step.values.foodBankName}:`,
                );
            },
            async function (step) {
                // Persist the message for later waterfall steps to be able to access it
                step.values.message = step.result;

                return await step.prompt('confirmPrompt', {
                    prompt: `Are you sure you'd like to send:\r
                    "${step.values.message}"\r
                    to ${step.values.foodBankName}?`,
                })
            },
            async function (step) {
                //Simulates sending of a message (nothing is actually being sent anywhere)
                if (step.result) {
                    sendFoodBankMessage(step.result.foodBankName, step.result.message, step.result.email);
                    return await step.context.sendActivity(`Great! We've sent your message to ${step.values.foodBankName}. Expect your response to be sent to ${step.values.email}`);
                } else {
                    return await step.context.sendActivity(`No worries! I won't send your message`);
                }
            }
        ]));
    }
}

exports.ContactDialog = ContactDialog;