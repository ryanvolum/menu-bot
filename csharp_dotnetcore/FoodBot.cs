// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FoodBot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace FoodBot
{
    /// <summary>
    /// This bot demonstrates several concepts at once. At a high level, it offers a fully guided flow 
    /// so that the user is always aware of their options on any given conversation turn. When bots leave 
    /// conversations open-ended, users often don't know what to do or say - guiding conversations with 
    /// buttons (even if you also support natural language) helps mitigate this issue. 
    /// We construct the guided conversation by first welcoming the user, and then by using waterfall dialogs 
    /// and prompts to communicate with our user.
    /// </summary>
    public class FoodBot : IBot
    {
        private readonly BotState _botState;
        private readonly FoodBotDialogSet _foodBotDialogSet;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="botState">A class used to manage state.</param>
        /// <param name="dialogSet">The top level menu dialog set</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that is hooked to the Azure App Service provider.</param>
        public FoodBot(BotState botState, FoodBotDialogSet dialogSet, ILoggerFactory loggerFactory)
        {
            _botState = botState ?? throw new ArgumentNullException(nameof(botState));
            _foodBotDialogSet = dialogSet ?? throw new ArgumentNullException(nameof(dialogSet));
            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<FoodBot>();
            _logger.LogTrace("Turn start.");
        }

        /// <summary>
        /// Every conversation turn for our Bot will call this method.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create a dialog context for the turn in the dialog set which will be used to begin/continue the dialog flow
            var dialogContext = await this._foodBotDialogSet.CreateContextAsync(turnContext, cancellationToken);

            using (_logger.BeginScope("OnTurnAsync - ActivityType={ActivityType}", turnContext.Activity.Type))
            {
                if (turnContext.Activity.Type == ActivityTypes.Message)
                {
                    _logger.LogInformation("Handling a message, active dialog is: {ActiveDialogId}", dialogContext.ActiveDialog?.Id ?? "<NONE>");

                    if (dialogContext.ActiveDialog != null)
                    {
                        await dialogContext.ContinueDialogAsync(cancellationToken);
                    }
                    else
                    {
                        await dialogContext.BeginDialogAsync(FoodBotDialogSet.MenuDialogId, null, cancellationToken);
                    }
                }
                else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
                {
                    if (this.MemberJoined(turnContext.Activity))
                    {
                        _logger.LogInformation("Starting a new conversation...");

                        await turnContext.SendActivityAsync("Hey there! Welcome to the food bank bot. I'm here to help orchestrate the delivery of excess food to local food banks!");

                        await dialogContext.BeginDialogAsync(FoodBotDialogSet.MenuDialogId, null, cancellationToken);
                    }
                }

                _logger.LogInformation("Saving all changes to bot state...");

                // Always persist changes at the end of every turn, here this is the dialog state in the conversation state.
                await _botState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);

                _logger.LogInformation("Turn completed!");
            }
        }

        /// <summary>
        /// Determines whether this is a new user in an incoming ConversationUpdate activity
        /// </summary>
        /// <param name="activity">Incoming ConversationUpdate activity</param>
        /// <returns>Returns true if a new user is added to the conversation, which is useful for determining when to welcome a user.</returns>
        private bool MemberJoined(Activity activity)
        {
            return ((activity.MembersAdded.Count != 0 && (activity.MembersAdded[0].Id != activity.Recipient.Id)));
        }
    }
}
