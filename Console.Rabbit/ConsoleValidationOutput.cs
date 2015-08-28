using System;
using System.Collections.Generic;
using System.Linq;
using Overseer;

namespace Console.Rabbit
{
	internal class ConsoleValidationOutput : IValidationOutput
	{
		private readonly Dictionary<Status, ConsoleColor> _colorMap;

		public ConsoleValidationOutput()
		{
			_colorMap = new Dictionary<Status, ConsoleColor>
			{
				{ Status.Pass, ConsoleColor.Green},
				{ Status.NotInterested, System.Console.ForegroundColor },
				{ Status.Warning, ConsoleColor.Yellow },
				{ Status.Fail, ConsoleColor.Red }
			};
		}
		public void Write(ValidationResult result)
		{
			var messages = BuildMessageLine(result);

			System.Console.ForegroundColor = _colorMap[result.Status];
            System.Console.WriteLine("{0}: {1}", result.Status, string.Join(", ", messages));
			System.Console.ResetColor();
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
