using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace FoodBot.Dialogs
{
    public class FoodBotDialogSet : DialogSet
    {
        public static string StartDialogId => "mainMenuDialog";

        public FoodBotDialogSet(IStatePropertyAccessor<DialogState> dialogStatePropertyAccessor)
            : base(dialogStatePropertyAccessor)
        {
            // Add the top-level dialog
            Add(new MainMenuDialog(StartDialogId));
        }
    }
}
