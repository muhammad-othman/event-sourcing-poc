using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UsersSubscriber
{
    class Program
    {

        static List<User> usersList = new List<User>();
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();

            factory.UserName = "guest";
            factory.Password = "guest";

            factory.HostName = "replacehere";
            factory.Port = 5672;


            var connection = factory.CreateConnection();

            var rabbitMQChannel = connection.CreateModel();

            var queueName = "users.subscribe.queue";

            rabbitMQChannel.QueueDeclare(queueName, true, false, false);
            rabbitMQChannel.QueueBind(queueName, "users.exchange", "");

            var consumer = new EventingBasicConsumer(rabbitMQChannel);

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {

                var body = e.Body.ToArray();
                var jsonString = Encoding.UTF8.GetString(body);

                var @event = JsonConvert.DeserializeObject<UserEvent>(jsonString);


                Console.WriteLine("=================================");
                Console.WriteLine("=================================");
                Console.WriteLine($"Event Type: ${@event.Type}");
                Console.WriteLine(@event.UserData);
                Console.WriteLine("=================================");
                Console.WriteLine("=================================");

                Console.WriteLine();
                Console.WriteLine();

                switch (@event.Type)
                {
                    case EventType.Create:
                        usersList.Add(@event.UserData);
                        break;
                    case EventType.Update:
                        var oldUser = usersList.First(e => @event.UserData.Id == e.Id);

                        oldUser.FirstName = @event.UserData.FirstName;
                        oldUser.LastName = @event.UserData.LastName;
                        oldUser.Email = @event.UserData.Email;

                        break;
                    case EventType.Delete:
                        usersList.RemoveAll(e => @event.UserData.Id == e.Id);
                        break;
                }

                rabbitMQChannel.BasicAck(e.DeliveryTag, false);
            };



            rabbitMQChannel.BasicConsume(queueName, false, consumer);


            Console.ReadLine();
        }

    }

    public class UserEvent
    {
        public Guid Id { get; set; }
        public EventType Type { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public User UserData { get; set; }
    }

    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return $"Name: {FirstName} {LastName} \nEmail: {Email}";
        }
    }

    public enum EventType
    {
        Create,
        Update,
        Delete
    }
}
