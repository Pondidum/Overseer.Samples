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

			System.Console.ForegroundColor = _colorMap[result.Status];

			System.Console.WriteLine($"* [{result.Status}] {result.Message.Type}");
			WriteRecursive(result.Children, 1);

			System.Console.ResetColor();
		}

		private void WriteRecursive(IEnumerable<ValidationNode> nodes, int depth)
		{
			foreach (var node in nodes)
			{
				var offset = new string(' ', depth * 2);

				System.Console.WriteLine($"{offset}* [{node.Status}]: {node.ValidationMessage}");

				WriteRecursive(node.Children, depth + 1);
			}
		}
	}
}
