using System.Collections.Generic;
using System.Linq;
using Overseer;

namespace Console.Rabbit
{
	internal class ConsoleValidationOutput : IValidationOutput
	{
		public void Write(ValidationResult result)
		{
			var messages = BuildMessageLine(result);
			System.Console.WriteLine("{0}: {1}", result.Status, string.Join(", ", messages));
		}

		private IEnumerable<string> BuildMessageLine(ValidationResult result)
		{
			var node = result as ValidationResultNode;

			if (node == null)
			{
				return new[]
				{
					string.Format("[{0}] {1}", result.Status, result.Message)
				};
			}

			return node.Results.SelectMany(BuildMessageLine);
		}
	}
}
