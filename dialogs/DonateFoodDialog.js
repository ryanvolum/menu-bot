const { ComponentDialog, ChoicePrompt, WaterfallDialog } = require('botbuilder-dialogs');
const { getValidDays, filterFoodBanks, createFoodBankCarousel } = require('../services/schedule-service');

class DonateFoodDialog extends ComponentDialog {
    constructor(dialogId) {
        super(dialogId);

        // ID of the child dialog that should be started anytime the component is started.
        this.initialDialogId = dialogId;

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
                let filteredFoodBanks = filterFoodBanks(day);
                let carousel = createFoodBankCarousel(filteredFoodBanks)
                return step.context.sendActivity(carousel);
            }
        ]));
    }
}

exports.DonateFoodDialog = DonateFoodDialog;