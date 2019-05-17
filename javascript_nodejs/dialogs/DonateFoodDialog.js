const { ComponentDialog, ChoicePrompt, WaterfallDialog } = require('botbuilder-dialogs');
const { getValidDonationDays, filterFoodBanksByDonation, createFoodBankDonationCarousel } = require('../services/schedule-helpers');

const DONATION_DIALOG = 'donationDialog';

class DonateFoodDialog extends ComponentDialog {
    constructor(dialogId) {
        super(dialogId);

        /**  
         * THIS IS REQUIRED to tell the framework which dialog
         * will be use as the starting one.  
         * 
         * Value doesn't matter, you could use the passed-in dialogId 
         * or something new
         * 
         * Without this, the first Diaglog added will be used, which in 
         * this example will be the Choice Prompt
         * **/
        this.initialDialogId = DONATION_DIALOG; // or dialogId

        this.addDialog(new ChoicePrompt('choicePrompt'));

        // Define the conversation flow using a waterfall model.
        // The id used here need to match the value given to `initialId`
        this.addDialog(new WaterfallDialog(DONATION_DIALOG, [
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