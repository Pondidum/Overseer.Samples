using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Console.Rabbit.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Console.Rabbit
{
	public class Publisher
	{
		private readonly string _hostName;
		private readonly string _exchangeName;
		private readonly CancellationTokenSource _tokenSource;

		public Publisher(string hostName, string exchangeName)
		{
			_hostName = hostName;
			_exchangeName = exchangeName;
			_tokenSource = new CancellationTokenSource();
		}

		public void Start()
		{

			var sender = new Task(SendMessagesAtRandom, _tokenSource.Token);
			sender.Start();

		}

		public void Stop()
		{
			_tokenSource.Cancel();
		}

		private void SendMessagesAtRandom()
		{
			var random = new Random();
			var factory = new ConnectionFactory { HostName = _hostName };

			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				while (_tokenSource.IsCancellationRequested == false)
				{
					var message = CreateMessage(random);

					var json = JsonConvert.SerializeObject(message);
					var body = Encoding.UTF8.GetBytes(json);


					var basicProperties = channel.CreateBasicProperties();
					basicProperties.Type = typeof(CandidateCreated).Name;

					channel.BasicPublish(_exchangeName, "", basicProperties, body);

					Thread.Sleep(250);
				}
			}
		}

		private CandidateCreated CreateMessage(Random random)
		{
			var e = new CandidateCreated();

			if (random.Next(10) > 2)
				e.ID = random.Next();

			if (random.Next(10) > 2)
				e.Email = "test@example.com";

			if (random.Next(10) > 2)
				e.Name = "Dave";

			return e;
		}
	}
}
