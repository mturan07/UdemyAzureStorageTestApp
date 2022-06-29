
using AzureStorageLibrary.Services;
using System.Text;

AzureStorageLibrary.ConnectionStrings.AzureConnectionString =
    "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

AzQueue queue = new AzQueue("samplequeue");

// sending message to queue
//string base64message = Convert.ToBase64String(Encoding.UTF8.GetBytes("My simple queue message"));
//queue.SendMessageAsync(base64message).Wait();/
//Console.WriteLine("Message sent");

// receiving message from queue
var message = queue.RetrieveNextMessageAsync().Result;
//string messageText = Encoding.UTF8.GetString(Convert.FromBase64String(message.Body.ToString()));
//Console.WriteLine("Received message : " + messageText);

// deleting message
await queue.DeleteMessage(message.MessageId, message.PopReceipt);
Console.WriteLine("Message deleted from queue");