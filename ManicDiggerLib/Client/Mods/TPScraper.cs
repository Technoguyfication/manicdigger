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
		int x = 6600;
		int y = 360;

		public override void OnNewFrame(Game game, NewFrameEventArgs args)
		{
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
