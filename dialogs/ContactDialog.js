const { ComponentDialog, ChoicePrompt, WaterfallDialog } = require('botbuilder-dialogs');
const { getFoodBanks } = require('../services/schedule-helpers');

class ContactDialog extends ComponentDialog {
    constructor(dialogId) {
        super(dialogId);

        // ID of the child dialog that should be started anytime the component is started.
        this.initialDialogId = dialogId;

        // Define the prompts used in this conversation flow.
        this.addDialog(new ChoicePrompt('choicePrompt'));

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
                const day = step.result.value;
                let filteredFoodBanks = filterFoodBanksByPickup(day);
                let carousel = createFoodBankPickupCarousel(filteredFoodBanks)
                return step.context.sendActivity(carousel);
            }
        ]));
    }
}

exports.ContactDialog = ContactDialog;