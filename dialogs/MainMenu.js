const { ComponentDialog, DialogSet, ChoicePrompt, WaterfallDialog, WaterfallStepContext } = require('botbuilder-dialogs');

const MENU_DIALOG = 'mainMenu';
const MENU_PROMPT = 'menuPrompt';

class MainMenuDialog extends ComponentDialog {
    constructor(dialogId) {
        super(dialogId);

        this.addDialog(new ChoicePrompt(MENU_PROMPT));

        // Adds a waterfall dialog that prompts users for the top level menu to the dialog set
        this.addDialog(new WaterfallDialog(MENU_DIALOG, [
            this.promptForMenu,
            this.handleMenuResult,
            this.resetDialog,
        ]));
    }

    /**
     * The first function in our waterfall dialog prompts the user with two options, 'Donate Food' and 'Food Bank'.
     * It uses the ChoicePrompt added in the contructor and referenced by the MENU_PROMPT string. The array of 
     * strings passed in as choices will be rendered as suggestedAction buttons which the user can then click. If the 
     * user types anything other than the button text, the choice prompt will reject it and reprompt using the retryPrompt
     * string. 
     * @param step Waterfall dialog step
     */
    async promptForMenu(step) {
        return step.prompt(MENU_PROMPT, {
            choices: ["Donate Food", "Find a Food Bank"],
            prompt: "Do you have food to donate or do you need to find a food bank?",
            retryPrompt: "I'm sorry, that wasn't a valid response. Are you looking to donate food or find a food bank?"
        });
    }

    /**
     * This step handles the result from the menu prompt above. It begins the appropriate dialog based on which button 
     * was clicked. 
     * @param step Waterfall Dialog Step 
     */
    async handleMenuResult(step) {
        switch (step.result.value) {
            case "Donate Food":
                step.context.sendActivity('Donate Food');
                break;
            case "Find Food":
                step.context.sendActivity('Food Bank Hours');
                break;
        }
        return step.next();
    }

    /**
     * This final step in our waterfall dialog replaces the dialog with itself, effectively starting the conversation over. This is often referred to as a 'message loop'.
     * @param step Waterfall Dialog Step
     */
    async resetDialog(step) {
        return step.replaceDialog(MENU_DIALOG);
    }
}

module.exports.MainMenuDialog = MainMenuDialog;