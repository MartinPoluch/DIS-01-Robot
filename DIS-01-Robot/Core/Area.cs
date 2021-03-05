using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIS_01_Robot.Core {
	public class Area : Resettable {

		public int Rows { get; set; }

		public int Columns { get; set; }

		public int StartRow { get; set; }

		public int StartColumn { get; set; }

		public int CurrentRow { get; set; }

		public int CurrentColumn { get; set; }

		public Direction? PreviousMove { get; set; }

		public Area() {
			Reset();
		}

		public List<Direction> PossibleMoves() {
			List<Direction> directions = new List<Direction>();
			if (CurrentRow > 1 && PreviousMove != Direction.Down) {
				directions.Add(Direction.Up);
			}
			if (CurrentRow < Rows && PreviousMove != Direction.Up) {
				directions.Add(Direction.Down);
			}
			if (CurrentColumn > 1 && PreviousMove != Direction.Right) {
				directions.Add(Direction.Left);
			}
			if (CurrentColumn < Columns && PreviousMove != Direction.Left) {
				directions.Add(Direction.Right);
			}

			return directions;
		}

		public void MoveTo(Direction direction) {
			switch (direction) {
				case Direction.Up: {
					CurrentRow--;
					break;
				}
				case Direction.Down: {
					CurrentRow++;
					break;
				}
				case Direction.Left: {
					CurrentColumn--;
					break;
				}
				case Direction.Right: {
					CurrentColumn++;
					break;
				}
			}
			PreviousMove = direction;
		}

		public int currentNodeId() {
			return ((CurrentRow - 1) * Columns) + CurrentColumn;
		}

		public void Reset() {
			PreviousMove = null;
			CurrentRow = StartRow + 1;
			CurrentColumn = StartColumn + 1;
		}
	}
}
