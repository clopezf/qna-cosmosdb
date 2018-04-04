using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using qna_demobot.Services;



namespace qna_demobot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {

        private bool userWelcomed;
        private const string Team1Option = "Head Office Users";
        private const string Team2Option = "Suppliers";
        private const string Team3Option = "Depot Users";
        private const string Team4Option = "M&S IT Support";
        private const string Yes = "Yes";
        private const string No = "No";
        private const string More = "I have more questions";
        private CosmosDb service;
        private string userTeam;
        private string userEmail;

        public RootDialog()
        {
            service = new CosmosDb();
        }


        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string userName;

            if (!context.UserData.TryGetValue("username", out userName))
            {
                PromptDialog.Text(context, ResumeAfterPrompt, "Before we start, please tell me your name?");
                //await context.PostAsync("Paso1");
                return;
            }

            if (userWelcomed == true)
            {
                //await context.PostAsync("Paso5");
                await context.PostAsync($"Welcome back {userName}! You can keep asking questions");
            }
            ShowOptions(context);
        }

        private async Task ResumeAfterPrompt(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var userName = await result;
                //await context.PostAsync("Paso2");
                userWelcomed = true;

                await context.PostAsync($"Welcome {userName}!  You can ask me simple questions on MOS like, What is ASN ? Can you provide GIST GLNs ? NFT Primary GLN please ? What is despatch location GLN ? Types of GLNs ? What is VaMoS ? What reports in VaMoS ? Can you detail Despatch variance report in VaMoS ? What is IDD + 1.How do I get access to VaMoS? How many days data does VaMoS contain? Can you provide the list of current user list? Any other questions which you like to know about MOS or VaMoS.");
                context.UserData.SetValue<string>("username", userName);
                //await context.PostAsync("Paso3");
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Oops! Something went wrong :( Technical Details: {ex}");
            }
            ShowOptions(context);
            //await context.PostAsync("Paso4");
        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { Team1Option, Team2Option, Team3Option, Team4Option }, "Could you please tell me your team?  ", "Not a valid option", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;
                string userTeam = optionSelected;
                context.UserData.SetValue("userteam", optionSelected);
                await context.PostAsync("Thanks, now can you please ask me your question");
                //await context.PostAsync(userTeam);

                switch (optionSelected)
                {
                    case Team1Option:
                        context.Call(new QnaDialog(), ResumeAfterOptionDialog);
                        break;

                    case Team2Option:
                        await context.PostAsync("Option2");
                        break;

                    case Team3Option:
                        await context.PostAsync("Option3");
                        break;

                    case Team4Option:
                        await context.PostAsync("Option4");
                        break;
                }
            }

            catch (TooManyAttemptsException)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {

            try
            {
                //await context.PostAsync("Final del chatbot");
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                //context.Wait(this.MessageReceivedAsync);
                IsItHelpful(context);
            }
        }

        private void IsItHelpful(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnButtonSelected, new List<string>() { Yes, No, More }, "Wast his what you were looking for?  ", "Not a valid option", 3);
        }

        private async Task OnButtonSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string OnButtonSelected = await result;
                //context.UserData.SetValue("userteam", optionSelected);
                //await context.PostAsync("testing");

                switch (OnButtonSelected)
                {
                    case Yes:
                        //context.Call(new QnaDialog(), ResumeAfterOptionDialog);
                        await context.PostAsync("Yes");
                        break;

                    case No:
                        // await context.PostAsync("No");
                        string message = "Could you please give me your email and I will send you an answer ASAP";
                        PromptDialog.Text(context, ResumeAfterDB, message);
                        break;

                    case More:
                        await context.PostAsync("Next Question?");
                        //context.Call(new QnaDialog(), ResumeAfterOptionDialog);
                        break;
                }
            }

            catch (TooManyAttemptsException)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterDB(IDialogContext context, IAwaitable<string> result)
        {

            try
            {
                string email = await result;
                string userEmail = email;
                await context.PostAsync($"This is your email: {userEmail}");

                Question question = new Question()
                {
                    ID = "5",
                    UserName = context.UserData.GetValue<string>("username"),
                    //UserName = "Test2",
                    Team = $"{userTeam}",
                    Email = userEmail,
                    question = "Question"

                };

                await service.CreateNewQuestion(question);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}