using System;
using Newtonsoft.Json;
using Overseer;
using Overseer.Converters;
using Overseer.Outputs;
using Overseer.Readers;
using Overseer.Sources;
using Shouldly;
using Testing.PostTest.Xunit.Events;
using Xunit;

namespace Testing.PostTest.Xunit
{
	public class Tests : IDisposable
	{
		private readonly InMemoryMessageReader _queue;
		private readonly QueueMonitor _monitor;
		private readonly InMemoryValidationOutput _output;

		public Tests()
		{
			var source = new ResourceValidatorSource(GetType().Assembly, "Testing.PostTest.Xunit.Schemas.{messageType}.json");

			_queue = new InMemoryMessageReader();
			_output = new InMemoryValidationOutput();

			_monitor = new QueueMonitor(_queue, new DirectMessageConverter(), new MessageValidator(source), _output);
			_monitor.Start();
		}

		/// <summary>
		/// This test will pass as the CandidateCreated object has all the properties required by it's schema.
		/// </summary>
		[Fact]
		public void When_publishing_a_valid_message()
		{
			PushMessage(new CandidateCreated
			{
				ID = 12345,
				Name = "Andy Dote",
				Email = "AndyDote@example.com"
			});
		}

		/// <summary>
		/// THis test will fail as the CandidateCreated object is missing the Name property required by it's schema.
		/// </summary>
		[Fact]
		public void When_publishing_an_invalid_message()
		{
			PushMessage(new CandidateCreated
			{
				ID = 9876,
			});
		}

		private void PushMessage(object domainEvent)
		{
			_queue.Push(new Message
			{
				Type = domainEvent.GetType().Name,
				Body = JsonConvert.SerializeObject(domainEvent)
			});
		}

		public void Dispose()
		{
			_monitor.Stop();
			foreach (var result in _output.Results)
			{
				result.Status.ShouldBe(Status.Pass);
			}
		}
	}
}
