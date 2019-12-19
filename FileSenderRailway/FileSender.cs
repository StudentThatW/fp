using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using ResultOf;

namespace FileSenderRailway
{
    public class FileSender
    {
        private readonly ICryptographer cryptographer;
        private readonly IRecognizer recognizer;
        private readonly Func<DateTime> now;
        private readonly ISender sender;

        public FileSender(
            ICryptographer cryptographer,
            ISender sender,
            IRecognizer recognizer,
            Func<DateTime> now)
        {
            this.cryptographer = cryptographer;
            this.sender = sender;
            this.recognizer = recognizer;
            this.now = now;
        }

        public IEnumerable<FileSendResult> SendFiles(FileContent[] files, X509Certificate certificate)
        {
            foreach (var file in files)
            {         
                //Document doc = recognizer.Recognize(file);
                //var errorMessage = PrepareFileToSend(doc);
                var result = ResultOf.Result.AsResult(file);
                result.Then(sender.Send);
                
                
                if (result.IsSuccess)
                {
                    //doc = doc.ChangeContent(cryptographer.Sign(doc.Content, certificate));
                    /*
                    try
                    {
                        sender.Send(doc);
                    }
                    catch(InvalidOperationException e)
                    {
                        errorMessage = "Can't send. " + e.Message;
                    }
                    */
                }
                
                yield return new FileSendResult(file, errorMessage);


                /*
                try
                {
                    Document doc = recognizer.Recognize(file);
                    if (!IsValidFormatVersion(doc))
                        throw new FormatException("Invalid format version");
                    if (!IsValidTimestamp(doc))
                        throw new FormatException("Too old document");
                    doc = doc.ChangeContent(cryptographer.Sign(doc.Content, certificate));
                    //doc.Content = cryptographer.Sign(doc.Content, certificate);
                    sender.Send(doc);
                }
                catch (FormatException e)
                {
                    errorMessage = "Can't prepare file to send. " + e.Message;
                }
                
                catch (InvalidOperationException e)
                {
                    errorMessage = "Can't send. " + e.Message;
                }
                yield return new FileSendResult(file, errorMessage);
                */
            }
        }

        private  Result<Document> PrepareFileToSend(FileContent file, X509Certificate certificate)
        {
            var before = "Can't prepare file to send. ";
            Document doc = recognizer.Recognize(file);
            if (!IsValidFormatVersion(doc))
                return Result.Fail<Document>(before + "Invalid format version");
                //throw new FormatException(before + "Invalid format version");
            if (!IsValidTimestamp(doc))
                return Result.Fail<Document>(before + "Too old document");
            doc = doc.ChangeContent(cryptographer.Sign(doc.Content, certificate));
            return Result.Ok(doc);
        }
        private bool IsValidFormatVersion(Document doc)
        {
            return doc.Format == "4.0" || doc.Format == "3.1";
        }

        private bool IsValidTimestamp(Document doc)
        {
            var oneMonthBefore = now().AddMonths(-1);
            return doc.Created > oneMonthBefore;
        }
    }
}