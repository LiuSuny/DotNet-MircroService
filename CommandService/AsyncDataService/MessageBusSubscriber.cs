using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataService
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private  IConnection _connection;
        private IModel _channel;
        private string _QueueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor
        )
        {
            _eventProcessor = eventProcessor;
            _configuration = configuration;
            InitializeRabbitMQ();
    
        }

        private void InitializeRabbitMQ()
        {
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
                _QueueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _QueueName, exchange:"trigger", routingKey:"");
                  
                   Console.WriteLine("--> Listening to message bus");
                          
               //message connection shutdown
               _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                   Console.WriteLine("--> Connected to message bus");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sorry, could not connect to message bus: {ex.Message}");
            }

        }

    
        //Listening to event
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested(); //we can request to stop

            var consumer = new EventingBasicConsumer(_channel); //consuming event

            consumer.Received += (ModuleHandle, ea) =>
            {
             Console.WriteLine("--> Event Received!");
              
              //body of the message
              var body = ea.Body;
              // convert this body byte array of string
              var notificationMessage = Encoding.UTF8
                   .GetString(body.ToArray());
                
                //getting hold of event processor
                _eventProcessor.ProcessEvent(notificationMessage);
            };
            
            //Continue consuming
            _channel.BasicConsume(queue: _QueueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {    
             Console.WriteLine("--> Message bus dispose");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
        }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection shutdown");
        }
    }
}