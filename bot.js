const { ActivityTypes } = require('botbuilder');
const { DialogSet, ChoicePrompt, WaterfallDialog } = require('botbuilder-dialogs');

const MENU_PROMPT = 'menuPrompt';
const MENU_DIALOG = 'menuDialog';
const DIALOG_STATE_PROPERTY = 'dialogState';

class FoodBot {
    constructor(conversationState) {
        this.conversationState = conversationState;

        // Configure dialogs
        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROPERTY);
        this.dialogs = new DialogSet(this.dialogState);
        this.dialogs.add(new ChoicePrompt(MENU_PROMPT));

        // Adds a waterfall dialog that prompts users for the top level menu to the dialog set
        this.dialogs.add(new WaterfallDialog(MENU_DIALOG, [
            this.promptForMenu,
            this.handleMenuResult,
            this.resetDialog,
        ]));
    }

    /**
     * This function gets called on every conversation 'turn' (whenever your bot receives an activity). If the bot receives a 
     * Message, it determines where in our dialog we are and continues the dialog accordingly. If the bot receives a 
     * ConversationUpdate (received when the user and bot join the conversation) it sends a welcome message and starts the 
     * menu dialog. 
     * @param turnContext The context of a specific turn. Includes the incoming activity as well as several helpers for sending
     * messages and handling conversations. 
     */
    async onTurn(turnContext) {
        const dialogContext = await this.dialogs.createContext(turnContext);

        if (turnContext.activity.type === ActivityTypes.Message) {
            if (dialogContext.activeDialog) {
                await dialogContext.continueDialog();
            } else {
                await dialogContext.beginDialog(MENU_DIALOG);
            }
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate) {
            if (this.memberJoined(turnContext.activity)) {
                await turnContext.sendActivity(`Hey there! Welcome to the food bank bot. I'm here to help orchestrate the delivery of excess food to local food banks!`);
                await dialogContext.beginDialog(MENU_DIALOG);
            }
        }
        await this.conversationState.saveChanges(turnContext);
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

    /**
     * @param activity Incoming conversationUpdate activity
     * @returns Returns true if a new user is added to the conversation, which is useful for determining when to welcome a user. 
     */
    memberJoined(activity) {
        return ((activity.membersAdded.length !== 0 && (activity.membersAdded[0].id !== activity.recipient.id)));
    }
}

module.exports.FoodBot = FoodBot;