using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace rebbitmq.test
{
    class Program
    {
        static void Main(string[] args)
        {
            for(int i=0; i<1000;  i++)
                MakeProcess();
            Console.ReadKey();
        }
        static void MakeProcess()
        {
            var queueName = "hello";
            var connection = Connection();

            var queue = CreateQueue(queueName, connection);
            WriteMessageOnQueue("Message Included OK", queue.QueueName, connection);
            Console.WriteLine(RetrieveSingleMessage(queue.QueueName, connection));

        }

        static ConnectionFactory ConnectionFactory() => new ConnectionFactory() { 
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
        static IConnection Connection() => ConnectionFactory().CreateConnection();
        public static QueueDeclareOk CreateQueue(string queueName, IConnection connection)
        {
            return connection.CreateModel().QueueDeclare(queueName, false, false, false, null);
        }
        public static bool WriteMessageOnQueue(string message, string queueName, IConnection connection)
        {
            using (var channel = connection.CreateModel())
            {
                channel.BasicPublish(string.Empty, queueName, null, Encoding.ASCII.GetBytes(message));
                channel.Close();
            }

            return true;
        }
        public static string RetrieveSingleMessage(string queueName, IConnection connection)
        {
            BasicGetResult data;
            using (var channel = connection.CreateModel())
            {
                data = channel.BasicGet(queueName, true);
                channel.Close();
            }
            return data != null ? Encoding.UTF8.GetString(data.Body.ToArray()) : null;
        }
    }
}
