using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIS_01_Robot.Core;
using GUI;
using GUI.Core;
using SimulationCore.Generators;

namespace SimulationCore {
	public class RobotMovementMC : SimCore {
		public int K { get; set; }
		public int StepsHigherThanK { get; set; }
		public int SumOfSteps { get; set; }

		public RobotMovementMC(Robot robot, GameMode gameMode) {
			Robot = robot;
			Worker = null;
			GameMode = gameMode;
			SumOfSteps = 0;
			StepsHigherThanK = 0;
		}

		public GameMode GameMode { get; set; }

		public ChartSettings ChartSettings { get; set; }

		public BackgroundWorker Worker { get; set; }

		public Robot Robot { get; set; }

		protected override void BeforeSimulation() {
			SumOfSteps = 0;
			StepsHigherThanK = 0;
		}

		protected override void BeforeReplication() {
			Robot.Reset();
		}

		protected override void AfterReplication() {
			
		}

		protected override void AfterSimulation() {
			
		}

		protected override void DoReplication() {
			switch (GameMode) {
				case GameMode.Random: {
					break;
				}
				case GameMode.OwnStrategy: {
					break;
				}
			}

			int currentSteps = Robot.WalkedSteps();
			SumOfSteps += currentSteps;
			if (currentSteps > K) {
				StepsHigherThanK++;
			}

			if ((Worker != null) && (ActualReplication >= ChartSettings.SkipReplications) && (ActualReplication % ChartSettings.Step == 0)) {
				int[] results = { SumOfSteps, StepsHigherThanK, ActualReplication}; // here enter results of one replication 
				Worker.ReportProgress(ActualReplication / NumberOfReplications, results); // send result to gui
			}
		}


		public string TextResult() {
			return $"Average number of robot steps:{SumOfSteps / (double)NumberOfReplications}\n" +
			       $"Probability that robot walk more than K steps: {(StepsHigherThanK/ (double)NumberOfReplications) * 100}%";
		}

	}
}
