const { ComponentDialog, ChoicePrompt, WaterfallDialog } = require('botbuilder-dialogs');
const { getValidDonationDays, filterFoodBanksByDonation, createFoodBankDonationCarousel } = require('../services/schedule-helpers');

class DonateFoodDialog extends ComponentDialog {
    constructor(dialogId) {
        super(dialogId);

        // ID of the child dialog that should be started anytime the component is started.
        this.initialDialogId = dialogId;

        this.addDialog(new ChoicePrompt('choicePrompt'));

        // Define the conversation flow using a waterfall model.
        this.addDialog(new WaterfallDialog(dialogId, [
            async function (step) {
                const validDonationDays = getValidDonationDays();

                return await step.prompt('choicePrompt', {
                    choices: validDonationDays,
                    prompt: "What day would you like to donate food?",
                    retryPrompt: "That's not a valid day! Please choose a valid day."
                });
            },
            async function (step) {
                const day = step.result.value;
                let filteredFoodBanks = filterFoodBanksByDonation(day);
                let carousel = createFoodBankDonationCarousel(filteredFoodBanks);
                return step.context.sendActivity(carousel);
            }
        ]));
    }
}

exports.DonateFoodDialog = DonateFoodDialog;