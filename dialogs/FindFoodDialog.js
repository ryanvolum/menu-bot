const { ComponentDialog, ChoicePrompt, WaterfallDialog } = require('botbuilder-dialogs');
const { getValidDays, filterFoodBanks } = require('../services/schedule-service');

class FindFoodDialog extends ComponentDialog {
    constructor(dialogId) {
        super(dialogId);

        // Define the prompts used in this conversation flow.
        this.addDialog(new ChoicePrompt('choicePrompt'));

        // Define the conversation flow using a waterfall model.
        this.addDialog(new WaterfallDialog(dialogId, [
            async function (step) {
                return await step.prompt('choicePrompt', {
                    choices: getValidDays(),
                    prompt: "What day would you like to donate food?",
                    retryPrompt: "That's not a valid day! Please choose a valid day."
                });
            },
            async function (step) {
                const day = step.result.value;
                step.context.sendActivity(`You clicked ${day}!`);
            },
            async function (step) {
                // Save the room number and "sign off".
                step.values.guestInfo.roomNumber = step.result;
                await step.context.sendActivity(`Great! Enjoy your stay in room ${step.result}!`);

                // End the dialog, returning the guest info.
                return await step.endDialog(step.values.guestInfo);
            }
        ]));
    }
}

exports.FindFoodDialog = FindFoodDialog;