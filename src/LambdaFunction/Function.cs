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
    public class Function
    {
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            SkillResponse response = new SkillResponse();
            response.Response = new ResponseBody();
            response.Response.ShouldEndSession = false;
            IOutputSpeech innerResponse = null;
            var log = context.Logger;
            log.LogLine($"Skill Request Object:");
            log.LogLine(JsonConvert.SerializeObject(input));

            if (input.GetRequestType() == typeof(LaunchRequest))
            {
                log.LogLine($"Default LaunchRequest made: 'Alexa, open numbers game");
                innerResponse = new PlainTextOutputSpeech()
                {
                    Text = "Welcome to the numbers game!"
                };
                response.Response.ShouldEndSession = true;

            }
            //else if (input.GetRequestType() == typeof(IntentRequest))
            //{
            //    var intentRequest = (IntentRequest)input.Request;

            //    switch (intentRequest.Intent.Name)
            //    {
            //        case "AMAZON.CancelIntent":
            //            log.LogLine($"AMAZON.CancelIntent: send StopMessage");
            //            innerResponse = new PlainTextOutputSpeech();
            //            (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
            //            response.Response.ShouldEndSession = true;
            //            break;
            //        case "AMAZON.StopIntent":
            //            log.LogLine($"AMAZON.StopIntent: send StopMessage");
            //            innerResponse = new PlainTextOutputSpeech();
            //            (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
            //            response.Response.ShouldEndSession = true;
            //            break;
            //        case "AMAZON.HelpIntent":
            //            log.LogLine($"AMAZON.HelpIntent: send HelpMessage");
            //            innerResponse = new PlainTextOutputSpeech();
            //            (innerResponse as PlainTextOutputSpeech).Text = resource.HelpMessage;
            //            break;
            //        case "GetFactIntent":
            //            log.LogLine($"GetFactIntent sent: send new fact");
            //            innerResponse = new PlainTextOutputSpeech();
            //            (innerResponse as PlainTextOutputSpeech).Text = emitNewFact(resource, false);
            //            break;
            //        case "GetNewFactIntent":
            //            log.LogLine($"GetFactIntent sent: send new fact");
            //            innerResponse = new PlainTextOutputSpeech();
            //            (innerResponse as PlainTextOutputSpeech).Text = emitNewFact(resource, false);
            //            break;
            //        default:
            //            log.LogLine($"Unknown intent: " + intentRequest.Intent.Name);
            //            innerResponse = new PlainTextOutputSpeech();
            //            (innerResponse as PlainTextOutputSpeech).Text = resource.HelpReprompt;
            //            break;
            //    }
            //}

            response.Response.OutputSpeech = innerResponse;
            response.Version = "1.0";
            log.LogLine($"Skill Response Object...");
            log.LogLine(JsonConvert.SerializeObject(response));
            return response;
        }
    }
}
