const { ComponentDialog, ChoicePrompt, WaterfallDialog } = require('botbuilder-dialogs');
const { getValidPickupDays, filterFoodBanksByPickup, createFoodBankPickupCarousel } = require('../services/schedule-helpers');

class FindFoodDialog extends ComponentDialog {
    constructor(dialogId) {
        super(dialogId);

        // ID of the child dialog that should be started anytime the component is started.
        this.initialDialogId = dialogId;

        // Define the prompts used in this conversation flow.
        this.addDialog(new ChoicePrompt('choicePrompt'));

        // Define the conversation flow using a waterfall model.
        this.addDialog(new WaterfallDialog(dialogId, [
            async function (step) {
                const validPickupDays = getValidPickupDays();

                return await step.prompt('choicePrompt', {
                    choices: validPickupDays,
                    prompt: "What day would you like to pickup food?",
                    retryPrompt: "That's not a valid day! Please choose a valid day."
                });
            },
            async function (step) {
                const day = step.result.value;
                let filteredFoodBanks = filterFoodBanksByPickup(day);
                let carousel = createFoodBankPickupCarousel(filteredFoodBanks)
                return step.context.sendActivity(carousel);
            }
        ]));
    }
}

exports.FindFoodDialog = FindFoodDialog;