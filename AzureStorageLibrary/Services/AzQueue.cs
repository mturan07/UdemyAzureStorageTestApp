using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace AzureStorageLibrary.Services
{
    public class AzQueue
    {
        private readonly QueueClient _queueClient;

        public AzQueue(string queueName)
        {
            _queueClient = new QueueClient(ConnectionStrings.AzureConnectionString, queueName);
            _queueClient.CreateIfNotExists();
        }

        public async Task SendMessageAsync(string message)
        {
            //await _queueClient.SendMessageAsync(message, default, default);
            //await _queueClient.SendMessageAsync(message, default, TimeSpan.FromDays(30));
            //await _queueClient.SendMessageAsync(message, default, TimeSpan.FromSeconds(-1));
            
            // message 64K
            // visibility timeout 30 seconds / max. 7 days
            // time to live 7 days / -1 is unlimited
            await _queueClient.SendMessageAsync(message);
        }

        public async Task<QueueMessage> RetrieveNextMessageAsync()
        {
            QueueProperties properties = await _queueClient.GetPropertiesAsync();
            if (properties.ApproximateMessagesCount > 0)
            {
                QueueMessage[] queueMessages = await _queueClient.ReceiveMessagesAsync(1, TimeSpan.FromMinutes(1));

                if (queueMessages.Any())
                {
                    return queueMessages[0];
                }
            }
            return null;
        }

        public async Task DeleteMessage(string messageId, string popReceipt)
        {
            await _queueClient.DeleteMessageAsync(messageId, popReceipt);
        }
    }
}
