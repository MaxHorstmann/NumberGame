using System;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Alexa.NET.Request.Type;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaFunction
{
    class NumberFact
    {
        public string Question { get; set; }
        public int Answer { get; set; }
    }

    public class Function
    {
        NumberFact[] numberFacts = new[] 
        {
            new NumberFact() { Question = "What is the population of New Zealand?", Answer = 4795070 },
            new NumberFact() { Question = "What year was Beyoncé born?", Answer = 1981 }
        };

        // LambdaFunction::LambdaFunction.Function::FunctionHandler
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            var log = context.Logger;
            try
            {
                SkillResponse response = new SkillResponse();
                response.Response = new ResponseBody();
                response.Response.ShouldEndSession = false;
                IOutputSpeech innerResponse = null;
                log.LogLine($"Skill Request Object:");
                log.LogLine(JsonConvert.SerializeObject(input));

                var currentQuestion = 0; // TODO get from session

                if (input.GetRequestType() == typeof(LaunchRequest))
                {
                    log.LogLine($"Default LaunchRequest made: 'Alexa, open numbers game");
                    innerResponse = new PlainTextOutputSpeech()
                    {
                        Text = "Welcome to the numbers game! Let's get started: " +
                            numberFacts[currentQuestion].Question
                    };
                }
                else if (input.GetRequestType() == typeof(IntentRequest))
                {
                    var intentRequest = (IntentRequest)input.Request;

                    switch (intentRequest.Intent.Name)
                    {
                        case "AnswerIntent":
                            log.LogLine($"Answer intent");

                            long answer;
                            if (!long.TryParse(intentRequest.Intent.Slots["Answer"].Value, out answer))
                            {
                                (innerResponse as PlainTextOutputSpeech).Text = "Sorry, I didn't quite get that. Please say a number.";
                                break;
                            }

                            log.LogLine($"Answer provided: {answer}");

                            innerResponse = new PlainTextOutputSpeech();

                            var correctAnswer = numberFacts[currentQuestion].Answer;

                            if (answer < correctAnswer)
                            {
                                (innerResponse as PlainTextOutputSpeech).Text = $"No, it's more than {answer}.";
                            }
                            else if (answer > correctAnswer)
                            {
                                (innerResponse as PlainTextOutputSpeech).Text = $"No, it's less than {answer}.";
                            }
                            else
                            {
                                (innerResponse as PlainTextOutputSpeech).Text = $"Yes! {answer} is correct!";
                                response.Response.ShouldEndSession = true;
                            }

                            break;

                        case "RepeatQuestionIntent":
                            log.LogLine($"RepeatQuestionIntent");
                            (innerResponse as PlainTextOutputSpeech).Text = numberFacts[currentQuestion].Question;
                            break;

                        case "AMAZON.CancelIntent":
                            log.LogLine($"AMAZON.CancelIntent: send StopMessage");
                            innerResponse = new PlainTextOutputSpeech();
                            //(innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                            response.Response.ShouldEndSession = true;
                            break;
                        case "AMAZON.StopIntent":
                            log.LogLine($"AMAZON.StopIntent: send StopMessage");
                            innerResponse = new PlainTextOutputSpeech();
                            //(innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                            response.Response.ShouldEndSession = true;
                            break;
                        case "AMAZON.HelpIntent":
                            log.LogLine($"AMAZON.HelpIntent: send HelpMessage");
                            innerResponse = new PlainTextOutputSpeech();
                            //(innerResponse as PlainTextOutputSpeech).Text = resource.HelpMessage;
                            break;
                        default:
                            log.LogLine($"Unknown intent: " + intentRequest.Intent.Name);
                            innerResponse = new PlainTextOutputSpeech();
                            //(innerResponse as PlainTextOutputSpeech).Text = resource.HelpReprompt;
                            break;
                    }
                }

                response.Response.OutputSpeech = innerResponse;
                response.Version = "1.0";

                if (response.SessionAttributes == null)
                {
                    response.SessionAttributes = new System.Collections.Generic.Dictionary<string, object>();
                }
                //response.SessionAttributes.Add("foo", count++);

                log.LogLine($"Skill Response Object...");
                log.LogLine(JsonConvert.SerializeObject(response));
                return response;

            }
            catch (Exception ex)
            {
                log.LogLine("Unhandled exception:");
                log.LogLine(ex.ToString());
                log.LogLine(JsonConvert.SerializeObject(ex));
                throw;
            }
        }
    }
}
