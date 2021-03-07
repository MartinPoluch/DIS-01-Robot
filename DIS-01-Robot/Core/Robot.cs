using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIS_01_Robot.Core;
using SimulationCore.Generators;

namespace GUI.Core {

	public class Robot : Resettable {

		private readonly Dictionary<int, UniformRNG> _generators;
		private readonly HashSet<int> _visited;

		public Area Area { get; set; }

		public Strategy Strategy { get; set; }

		public Robot(Random seeder) {
			Area = new Area();
			Strategy = Strategy.Random;
			_visited = new HashSet<int>();
			_generators = new Dictionary<int, UniformRNG>();
			_generators.Add(2, new UniformRNG(seeder.Next(), 0, 1)); // 2 directions
			_generators.Add(3, new UniformRNG(seeder.Next(), 0, 2)); // 3 directions
			_generators.Add(4, new UniformRNG(seeder.Next(), 0, 3)); // 4 directions
			Reset();
		}

		public void Reset() {
			Area.Reset();
			_visited.Clear();
		}

		public int WalkedSteps() {
			int currentNodeId = Area.currentNodeId();
			Debug.Assert(_visited.Count == 0);
			while (! _visited.Contains(currentNodeId)) {
				_visited.Add(currentNodeId);
				var possibleMoves = (Strategy == Strategy.Random) ? 
					Area.PossibleMoves() : 
					Area.PossibleGoodMoves(_visited);
				Direction direction;
				Debug.Assert((possibleMoves.Count < 4) || (possibleMoves.Count == 4 && _visited.Count == 1));
				if (possibleMoves.Count == 0) {
					Debug.Assert(Strategy == Strategy.OwnStrategy);
					break;
				}

				if (possibleMoves.Count == 1) {
					direction = possibleMoves[0];
				}
				else {
					UniformRNG generator = _generators[possibleMoves.Count];
					int randomIndex = generator.NextInt();
					direction = possibleMoves[randomIndex];
				}
				Area.MoveTo(direction);
				currentNodeId = Area.currentNodeId();
				Debug.Assert(0 < currentNodeId && currentNodeId <= Area.Rows * Area.Columns);
			}

			return _visited.Count;
		}
	}
}
