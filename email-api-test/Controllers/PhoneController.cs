﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Reflection;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;
using Twilio.AspNet.Mvc;
using Twilio.Http;
using email_api_test.Utilities;
using email_api_test.Models;
using email_api_test.Logging;

namespace email_api_test.Controllers
{
    public class PhoneController : TwilioController
    {
        
        private readonly IConfigurationUtility _configurationUtility;

        public PhoneController(IConfigurationUtility config)
        {
            _configurationUtility = config;
        }
        
        public ActionResult MakeCall()
        {
            var accountSid = _configurationUtility.TwilioAccountSid;
            var authToken = _configurationUtility.TwilioAuthToken;

            TwilioClient.Init(accountSid, authToken);

            var to = new PhoneNumber(_configurationUtility.MyPhoneNumber);
            var from = _configurationUtility.TwilioNumber;

            var call = CallResource.Create(
                 to: to,
                 from: from,
                 url: new Uri("http://demo.twilio.com/docs/voice.xml"));


            return Content(call.Sid);
        }

        [HttpPost]
        public ActionResult ReceiveCall(Callback cb)
        {
            Response.ContentType = "text/xml";

            var accountSid = _configurationUtility.TwilioAccountSid;
            var authToken = _configurationUtility.TwilioAuthToken;

            TwilioClient.Init(accountSid, authToken);

            LogRequest(cb);

            var response = new VoiceResponse();
            response.Say("Hi");

            return TwiML(response);

        }

        private void LogRequest(Callback callback)
        {
            PropertyInfo[] properties = callback.GetType().GetProperties();
            foreach (PropertyInfo cb in properties)
            {
                Logger.Info($"{cb}: {cb.GetValue(callback)}");
            }
        }


    }
}