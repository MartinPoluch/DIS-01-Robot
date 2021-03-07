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

		private bool CanMoveUp() {
			return (CurrentRow > 1 && PreviousMove != Direction.Down);
		}

		private bool CanMoveDown() {
			return (CurrentRow < Rows && PreviousMove != Direction.Up);
		}

		private bool CanMoveLeft() {
			return (CurrentColumn > 1 && PreviousMove != Direction.Right);
		}

		private bool CanMoveRight() {
			return (CurrentColumn < Columns && PreviousMove != Direction.Left);
		}

		public List<Direction> PossibleMoves() {
			List<Direction> directions = new List<Direction>();
			if (CanMoveUp()) {
				directions.Add(Direction.Up);
			}
			if (CanMoveDown()) {
				directions.Add(Direction.Down);
			}
			if (CanMoveLeft()) {
				directions.Add(Direction.Left);
			}
			if (CanMoveRight()) {
				directions.Add(Direction.Right);
			}

			return directions;
		}

		public List<Direction> PossibleGoodMoves(HashSet<int> visited) {
			List<Direction> directions = new List<Direction>();
			if (CanMoveUp() && !visited.Contains(CalculateNodeId(CurrentRow - 1, CurrentColumn))) {
				directions.Add(Direction.Up);
			}
			if (CanMoveDown() && ! visited.Contains(CalculateNodeId(CurrentRow + 1, CurrentColumn))) {
				directions.Add(Direction.Down);
			}
			if (CanMoveLeft() && !visited.Contains(CalculateNodeId(CurrentRow, CurrentColumn - 1))) {
				directions.Add(Direction.Left);
			}
			if (CanMoveRight() && !visited.Contains(CalculateNodeId(CurrentRow, CurrentColumn + 1))) {
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

		private int CalculateNodeId(int row, int column) {
			return ((row - 1) * Columns) + column;
		}

		public int currentNodeId() {
			return CalculateNodeId(CurrentRow, CurrentColumn);
		}

		public void Reset() {
			PreviousMove = null;
			CurrentRow = StartRow + 1;
			CurrentColumn = StartColumn + 1;
		}
	}
}
