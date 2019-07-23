using System;
using System.Collections.Generic;
using System.Text;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Tasks.PlayerTasks;
using Xunit;
using Xunit.Abstractions;

namespace SabberStoneCoreTest.src
{
	public class TestBase
	{
		protected readonly ITestOutputHelper Output;

		public TestBase(ITestOutputHelper tempOutput)
		{
			Output = tempOutput;
		}
	}

	public class ChuckTest : TestBase
	{
		public ChuckTest(ITestOutputHelper output) : base(output)
		{

		}

		private GameConfig GetGameConfig()
		{
			GameConfig gameConfig = new GameConfig();
			gameConfig.Player1HeroClass = CardClass.WARLOCK;
			gameConfig.Player2HeroClass = CardClass.SHAMAN;
			return gameConfig;
		}

		[Fact]
		public void Test()
		{
			try
			{
				GameConfig gameConfig = GetGameConfig();
				Game game = new Game(gameConfig);
				var currentPlayer = game.CurrentPlayer;
				if (currentPlayer == null)
				{
					Output.WriteLine("currentPlayer is null");
					return;
				}

				List<PlayerTask> options = currentPlayer.Options();
				foreach (var task in options)
				{
					Output.WriteLine(task.ToString());
				}
			}
			catch (Exception ex)
			{
				Output.WriteLine(ex.ToString());
			}
		}
	}
}
