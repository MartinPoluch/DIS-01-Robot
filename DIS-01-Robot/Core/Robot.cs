using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIS_01_Robot.Core;
using SimulationCore.Generators;

namespace GUI.Core {

	public class Robot : Resettable {

		private Dictionary<int, UniformRNG> _generators;
		private HashSet<int> _visited;

		public Area Area { get; set; }

		public Robot(Random seeder) {
			Area = new Area();
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
			while (! _visited.Contains(currentNodeId)) {
				_visited.Add(currentNodeId);
				var possibleMoves = Area.PossibleMoves();
				Direction direction;
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
			}

			return _visited.Count;
		}
	}
}
