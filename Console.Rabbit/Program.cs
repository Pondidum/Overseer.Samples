using System;
using System.Threading;
using System.Threading.Tasks;
using Overseer;
using Overseer.RabbitMQ;
using Overseer.Sources;

namespace Console.Rabbit
{
	class Program
	{
		private const string RabbitHostName = "192.168.59.103";
		private const string RabbitExchangeName = "ConsoleRabbitDomainEvents";

		static void Main(string[] args)
		{
			var rabbitOptions = new RabbitOptions
			{
				HostName = RabbitHostName,
				ExchangeName = RabbitExchangeName,
				ExchangeAutoDelete = true,
				ExchangeDurable = false
			};

			var reader = new RabbitMessageReader(rabbitOptions);
			var converter = new RabbitMessageConverter();
			var source = new ResourceValidatorSource(typeof(Program).Assembly, "Console.Rabbit.Schemas.{messageType}.json");
			var output = new ConsoleValidationOutput();

			var monitor = new QueueMonitor(reader, converter, new MessageValidator(source), output);
			monitor.Start();

			var publisher = new Publisher(RabbitHostName, RabbitExchangeName);
			publisher.Start();

			System.Console.WriteLine("Monitoring {0} on {1}.  Press any key to stop.", rabbitOptions.ExchangeName, rabbitOptions.HostName);

			System.Console.ReadKey();
			System.Console.WriteLine("Stopping...");

			publisher.Stop();
			monitor.Stop();
		}
	}
}
