using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace ManicDigger.Mods
{
	/// <summary>
	/// This is a fix for backward compatibility issues of old Mods.
	/// Do not reference this in your Mods. Instead reference the corresponding Core*.cs
	/// </summary>
	public class TPScraper : ClientMod
	{
		bool waitingForStep = false;
		int x;
		int y;
		bool initialPosNonce = false;

		public override void Start(ClientModManager modmanager)
		{
			// load last position from log file
			string lastPos = File.ReadLines("tpscraper.log").Where(s => s.StartsWith("tp: ")).Last();
			string[] xy = lastPos.Substring(4).Split(',');
			x = int.Parse(xy[0]);
			y = int.Parse(xy[1]);

			base.Start(modmanager);
		}

		public override void OnNewFrame(Game game, NewFrameEventArgs args)
		{
			if (initialPosNonce)
			{
				game.AddChatline($"{x}, {y}");
				initialPosNonce = true;
			}

			if (game.chunkTimer.Elapsed.TotalSeconds > 4d)
			{
				if (!waitingForStep)
				{
					game.AddChatline("chunk timeout");
					waitingForStep = true;  // prevent teleporting again before we start loading chunks


					// teleport player here
					NextStep(game);
				}
			}
			else
			{
				waitingForStep = false;
			}

			base.OnNewFrame(game, args);
		}

		public override bool OnClientCommand(Game game, ClientCommandArgs args)
		{
			game.AddChatline(args.command);
			return base.OnClientCommand(game, args);
		}

		private void NextStep(Game game)
		{
			while (y < 9840)
			{
				while (x < 9840)
				{
					game.SendChat($"/tp_pos {x} {y} 100");
					game.map.Reset(game.map.MapSizeX, game.map.MapSizeY, game.map.MapSizeZ);	// unload all chunks
					

					File.AppendAllText("tpscraper.log", $"tp: {x},{y}" + Environment.NewLine);

					x += 240;
					return;
				}
				x = 120;
				y += 240;
			}
		}
	}
}
