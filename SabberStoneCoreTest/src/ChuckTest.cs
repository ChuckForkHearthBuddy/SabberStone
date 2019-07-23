using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

	/// <summary>
	/// Try to implement the 5th turn, shaman's behavior
	/// https://hsreplay.net/replay/EPYWPdrBBaELpVVq3aiBEc
	/// </summary>
	public class ChuckTest : TestBase
	{
		private readonly string _response;

		public ChuckTest(ITestOutputHelper output) : base(output)
		{
			_response = GetResponse().Result;
			//Output.WriteLine(_response);
		}

		private async Task<string> GetResponse()
		{
			string uri = "https://hsreplay.net/api/v1/games/EPYWPdrBBaELpVVq3aiBEc/";
			return await GetAsync(uri);
		}

		public async Task<string> GetAsync(string uri)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				return await reader.ReadToEndAsync();
			}
		}

		/// <summary>
		/// warlock
		/// https://hsreplay.net/api/v1/games/EPYWPdrBBaELpVVq3aiBEc/
		/// </summary>
		/// <returns></returns>
		private List<Card> GetPlayer1Deck()
		{
			List<Card> list = new List<Card>();
			dynamic obj = JsonConvert.DeserializeObject(_response);
			//Output.WriteLine(obj.ToString());
			JArray array = obj.opposing_deck.cards;
			foreach (var item in array)
			{
				var card = Cards.FromId(item.ToString());
				list.Add(card);
			}
			return list;
		}

		/// <summary>
		/// shaman
		/// https://hsreplay.net/api/v1/games/EPYWPdrBBaELpVVq3aiBEc/
		/// </summary>
		/// <returns></returns>
		private List<Card> GetPlayer2Deck()
		{
			List<Card> list = new List<Card>();
			dynamic obj = JsonConvert.DeserializeObject(_response);
			//Output.WriteLine(obj.ToString());
			JArray array = obj.friendly_deck.cards;
			foreach (var item in array)
			{
				var card = Cards.FromId(item.ToString());
				list.Add(card);
			}
			return list;
		}

		private GameConfig GetGameConfig()
		{
			GameConfig gameConfig = new GameConfig();
			gameConfig.Player1HeroClass = CardClass.WARLOCK;
			gameConfig.Player1Name = "小晴天";
			gameConfig.Player2HeroClass = CardClass.SHAMAN;
			gameConfig.Player2Name = "子非鱼";
			gameConfig.Player1Deck = GetPlayer1Deck();
			gameConfig.Player2Deck = GetPlayer2Deck();
			gameConfig.StartPlayer = 2;
			return gameConfig;
		}

		[Fact]
		public void Test()
		{
			try
			{
				GameConfig gameConfig = GetGameConfig();
				Game game = new Game(gameConfig);
				game.Player1.BaseMana = 5;
				game.Player2.BaseMana = 5;
				game.StartGame();
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
