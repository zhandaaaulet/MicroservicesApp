using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBusRabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private bool _disposed;
        public RabbitMQConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            if (!IsConnected)
            {
                TryConnect();
            }
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public bool TryConnect()
        {
            try
            {
                _connection = _connectionFactory.CreateConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection failed: " + e.Message);
                return false;
            }

            if (IsConnected)
            {
                Console.WriteLine("Connection acquired!");
                return true;
            }
            else
            {
                Console.WriteLine("Error: Connection cannot be reached!");
                return false;
            }
        }

        public IModel CreateConnect()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


    }
}
