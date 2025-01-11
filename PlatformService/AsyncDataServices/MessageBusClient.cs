using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using PlatformService.Data;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        
        public MessageBusClient(IConfiguration configuration)
        {
           _configuration = configuration;
            //config Rabbitmq connection 
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };
            
            try
            {
                //new to create connection
                _connection = factory.CreateConnection(); //setup a connection
                _channel = _connection.CreateModel();   //setup channel model
               _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout); //declearing exchange of our channel

               //message connection shutdown
               _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                   Console.WriteLine("--> Connected to message bus");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sorry, could not connect to message bus: {ex.Message}");
            }

        }

        public void PublishNewPlatform(PlatformPublishedDto publishedDto)
        {
             //create a message object
             var message = JsonSerializer.Serialize(publishedDto);
             //check if the connection is open
             if(_connection.IsOpen)
             {
                Console.WriteLine("--> Rabbitmq connection is open, sending messages...");
                 SendMessage(message);
             }
             else{
                Console.WriteLine("--> Rabbitmq connection is close, not sending messages");
             }
               
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
            Console.WriteLine($"--> We have sent a {message}");

        }
        
        private void Dispose()
        {
            Console.WriteLine("--> Message bus dispose");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
          private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection shutdown");
        }

    }
}