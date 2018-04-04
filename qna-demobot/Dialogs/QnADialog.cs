using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using System.Configuration;
using System.Linq;
using AdaptiveCards;
using System.Collections.Generic;

namespace qna_demobot.Dialogs
{
    [Serializable]
    public class QnaDialog : QnAMakerDialog
    {
        public QnaDialog() : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnaSubscriptionKey"], ConfigurationManager.AppSettings["QnaKnowledgebaseId"], "Sorry, I couldn't find an answer for that", 0.5)))
        {
        }

      /**  protected override async Task RespondWithDefaultMessageAsync(IDialogContext context, IMessageActivity request)
        {
            var activity = ((Activity)context.Activity).CreateReply("Sorry, I couldn't find an answer for that");
            activity.SuggestedActions = new SuggestedActions(actions:
                                            new List<CardAction>
                                            {
                                        new CardAction() {
                                            Title = "Try asking Bing!",
                                            Value = $"https://bing.com/search?q={request.Text}",
                                            Type = ActionTypes.OpenUrl }
                                            });
            await context.PostAsync(activity);
        }**/
    }




}