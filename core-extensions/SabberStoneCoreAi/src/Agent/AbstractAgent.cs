﻿using System;
using System.Collections.Generic;
using System.Text;

using SabberStoneCore.Tasks;
using SabberStoneCoreAi.POGame;

namespace SabberStoneCoreAi.Agent
{
	abstract class AbstractAgent

	{
		
		public abstract void InitializeAgent();

		public abstract void InitializeGame();

		public abstract List<PlayerTask> GetMove(PartialObservationGame poGame);

		public abstract void FinalizeGame();

		public abstract void FinalizeAgent();

	}
}