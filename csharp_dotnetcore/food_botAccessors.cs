// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace food_bot
{
    /// <summary>
    /// This class is created as a Singleton and passed into the IBot-derived constructor.
    ///  - See the Startup.cs file for more details on creating the Singleton that gets
    ///    injected into the constructor.
    /// </summary>
    public class food_botAccessors
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// Contains the <see cref="ConversationState"/> and associated <see cref="IStatePropertyAccessor{T}"/>.
        /// </summary>
        /// <param name="conversationState">The state object that stores the counter.</param>
        public food_botAccessors(ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
        }

        public static string DialogStateName { get; } = "DialogState";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for the DialogState.
        /// </summary>
        /// <value>
        /// The accessor stores the dialog state.
        /// </value>
        public IStatePropertyAccessor<DialogState> ConversationDialogStateAccessor { get; set; }

        /// <summary>
        /// Gets the <see cref="ConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="ConversationState"/> object.</value>
        public ConversationState ConversationState { get; }
    }
}
