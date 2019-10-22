// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using NLQueryApp;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class ShowOrdersDialog : CancelAndHelpDialog
    {
        private const string UserStepMsgText = "The loged in user is not recognized!";

        public ShowOrdersDialog()
            : base(nameof(ShowOrdersDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                UserStepAsync,
                GetOrdersStepAsync,
                ShowCardStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> UserStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var salesOrderHeader = (SalesOrderHeader)stepContext.Options;

            if (salesOrderHeader.CustomerID == 1)
            {
                var promptMessage = MessageFactory.Text(UserStepMsgText, UserStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(salesOrderHeader.CustomerID, cancellationToken);
        }

        private async Task<DialogTurnResult> GetOrdersStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            HttpClient client = CallAPI();

            var customers = client.GetAsync("api/SmartTradeApi/GetTotalOrdersByCustomer?customerId=29847").Result;
            if (customers.IsSuccessStatusCode)
            {
                string responseString = customers.Content.ReadAsStringAsync().Result;
            }

            IEnumerable<SalesOrderHeader> orders = customers.Content.ReadAsAsync<IEnumerable<SalesOrderHeader>>().Result;

            return await stepContext.NextAsync(orders, cancellationToken);
        }

        // Send a Rich Card response to the user based on their choice.
        // This method is only called when a valid prompt response is parsed from the user's response to the ChoicePrompt.
        private async Task<DialogTurnResult> ShowCardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Cards are sent as Attachments in the Bot Framework.
            // So we need to create a list of attachments for the reply activity.
            var attachments = new List<Attachment>();

            // Reply to the activity we received with an activity.
            var reply = MessageFactory.Attachment(attachments);

            reply.Attachments.Add(Cards.CreateAdaptiveCardAttachment(Path.Combine(".", "Cards", "OrderHeader.json")));

            // Send the card(s) to the user as an attachment to the activity
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            return await stepContext.EndDialogAsync();
        }

        private static HttpClient CallAPI()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:62449");
            client.DefaultRequestHeaders.Accept.Clear();
            return client;
        }

        private static bool IsAmbiguous(string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
        }
    }
}
