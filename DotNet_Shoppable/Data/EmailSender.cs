using Newtonsoft.Json.Linq;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;
using System.Diagnostics;

namespace DotNet_Shoppable.Data
{
    public class EmailSender
    {
        public static void SendEmail(string senderName, string senderEmail, string toName, string toEmail, string subject, string textContent) 
        {
            var apiInstance = new TransactionalEmailsApi();
            //string SenderName = "John Doe";
            //string SenderEmail = "example@example.com";
            SendSmtpEmailSender Email = new SendSmtpEmailSender(senderName, senderEmail);
            //string ToEmail = "example1@example1.com";
            //string ToName = "John Doe";
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(toEmail, toName);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
            To.Add(smtpEmailTo);
            //string BccName = "Janice Doe";
            //string BccEmail = "example2@example2.com";
            //SendSmtpEmailBcc BccData = new SendSmtpEmailBcc(BccEmail, BccName);
            //List<SendSmtpEmailBcc> Bcc = new List<SendSmtpEmailBcc>();
            //Bcc.Add(BccData);
            //string CcName = "John Doe";
            //string CcEmail = "example3@example2.com";
            //SendSmtpEmailCc CcData = new SendSmtpEmailCc(CcEmail, CcName);
            //List<SendSmtpEmailCc> Cc = new List<SendSmtpEmailCc>();
            //Cc.Add(CcData);
           //string HtmlContent = "<html><body><h1>This is my first transactional email {{params.parameter}}</h1></body></html>";
            //string TextContent = null;
            //string Subject = "My {{params.subject}}";
            //string ReplyToName = "John Doe";
            //string ReplyToEmail = "replyto@domain.com";
            //SendSmtpEmailReplyTo ReplyTo = new SendSmtpEmailReplyTo(ReplyToEmail, ReplyToName);
            //string AttachmentUrl = null;
            //string stringInBase64 = "aGVsbG8gdGhpcyBpcyB0ZXN0";
            //byte[] Content = System.Convert.FromBase64String(stringInBase64);
            //string AttachmentName = "test.txt";
            //SendSmtpEmailAttachment AttachmentContent = new SendSmtpEmailAttachment(AttachmentUrl, Content, AttachmentName);
            //List<SendSmtpEmailAttachment> Attachment = new List<SendSmtpEmailAttachment>();
            //Attachment.Add(AttachmentContent);
            //JObject Headers = new JObject();
            //Headers.Add("Some-Custom-Name", "unique-id-1234");
            //long? TemplateId = null;
            //JObject Params = new JObject();
            //Params.Add("parameter", "My param value");
            //Params.Add("subject", "New Subject");
            //List<string> Tags = new List<string>();
            //Tags.Add("mytag");
            //SendSmtpEmailTo1 smtpEmailTo1 = new SendSmtpEmailTo1(toEmail, toName);
            //List<SendSmtpEmailTo1> To1 = new List<SendSmtpEmailTo1>();
            //To1.Add(smtpEmailTo1);
            //Dictionary<string, object> _parmas = new Dictionary<string, object>();
            //_parmas.Add("params", Params);
            //SendSmtpEmailReplyTo1 ReplyTo1 = new SendSmtpEmailReplyTo1(ReplyToEmail, ReplyToName);
            //SendSmtpEmailMessageVersions messageVersion = new SendSmtpEmailMessageVersions(To1, _parmas, Bcc, Cc, ReplyTo1, Subject);
            //List<SendSmtpEmailMessageVersions> messageVersiopns = new List<SendSmtpEmailMessageVersions>();
            //messageVersiopns.Add(messageVersion);
            try
            {
                var sendSmtpEmail = new SendSmtpEmail(Email, To, null, null, null, textContent, subject); /*, ReplyTo, Attachment, Headers, TemplateId, Params, messageVersiopns, Tags);*/
                CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                Console.WriteLine("Email Sender Ok: \n" + result.ToJson());
            }
            catch (Exception e)
            {
                Console.WriteLine("Email Sender Failure: \n" + e.Message);
            }
        }
    }
}
