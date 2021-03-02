using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DIS_01_Robot.Core;
using GUI;
using GUI.Core;
using Microsoft.Win32;
using SimulationCore;
using Path = System.IO.Path;

namespace DIS_01_Robot {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private Robot _robot;

		public MainWindow() {
			InitializeComponent();
			DataContext = this;
			Replications = 1000000;
			ChartSettings = new ChartSettings(1000, 300000);
			Random seeder = new Random();
			Area = new Area() {
				Rows = 4,
				Columns = 4,
				CurrentRow = 0,
				CurrentColumn = 0,
			};
			_robot = new Robot(seeder);
			MonteCarlo = new RobotMovementMC(_robot, GameMode.Random) {
				ChartSettings = ChartSettings,
				K = 5,
			};

			EnableControls();
		}

		public RobotMovementMC MonteCarlo { get; set; }

		public ChartSettings ChartSettings { get; set; }

		public int Replications { get; set; }

		public Area Area { get; set; }

		private void EnableControls() {
			StartBtn.IsEnabled = true;
			StopBtn.IsEnabled = false;
			GameModeGroup.IsEnabled = true;
			ChartGroup.IsEnabled = true;
			ReplicationInput.IsEnabled = true;
			SimulationSetting.IsEnabled = true;
		}

		private void DisableControls() {
			StartBtn.IsEnabled = false;
			StopBtn.IsEnabled = true;
			GameModeGroup.IsEnabled = false;
			ChartGroup.IsEnabled = false;
			ReplicationInput.IsEnabled = false;
			SimulationSetting.IsEnabled = false;
		}

		private void CalculateStep() {
			if (Replications <= 100) {
				ChartSettings.Step = 1;
			}
			else {
				double sqrt = Math.Sqrt(Replications);
				ChartSettings.Step = (int)Math.Ceiling(sqrt);
			}
		}

		private void StartSimulation(object sender, RoutedEventArgs e) {
			if (Replications <= 0) {
				MessageBox.Show("Number of replications should be higher then zero.");
				return;
			}

			if ((ChartSettings.Step == 0) || (StepInput.Text.Trim() == "") || (StepInput.Text == "0")) {
				CalculateStep();
			}

			_robot.Area = Area;
			MonteCarlo.Stop = false;
			MonteCarlo.ChartSettings = ChartSettings;
			DisableControls();
			AverageStepsChart.Clear();
			ProbabilityChart.Clear();
			TextOutput.Text = $"Simulation is running ...";
			TextOutput.Text += $"\nReplications: {Replications}\nSkip results: {ChartSettings.SkipReplications}\nStep: {ChartSettings.Step}";
			RunWorker();
		}

		private void RunWorker() {
			BackgroundWorker worker = new BackgroundWorker {
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};

			MonteCarlo.Worker = worker;
			worker.DoWork += delegate (object o, DoWorkEventArgs args) {
				MonteCarlo.Simulate(Replications);
			};
			worker.ProgressChanged += UpdateChartsOutput;
			worker.RunWorkerCompleted += delegate (object o, RunWorkerCompletedEventArgs args) {
				EnableControls();
				LogTextOutput();
			};
			worker.RunWorkerAsync();
		}

		private void StopSimulation(object sender, RoutedEventArgs e) {
			MonteCarlo.Stop = true;
			EnableControls();
		}

		private void UpdateChartsOutput(object sender, ProgressChangedEventArgs e) {
			int[] results = (int[])e.UserState;
			int sumOfSteps = results[0];
			int currentReplication = results[2] + 1;
			double averageStep = (double)sumOfSteps / (double)currentReplication;
			AverageStepsChart.AddChartValue(currentReplication, averageStep);

			int stepsHigherThanK = results[1];
			double probability = ((double)stepsHigherThanK / (double)currentReplication) * 100;
			ProbabilityChart.AddChartValue(currentReplication, probability);
		}

		private void LogTextOutput() {
			TextOutput.Text = MonteCarlo.TextResult();
			TextOutput.Text += $"\nReplications: {Replications}\nSkip results: {ChartSettings.SkipReplications}\nStep: {ChartSettings.Step}";
		}

		private void RadioButton_Click(object sender, RoutedEventArgs e) {
			if (Equals(sender, RandomRb)) {
				MonteCarlo.GameMode = GameMode.Random;
			}
			else if (Equals(sender, OwnStrategyRb)) {
				MonteCarlo.GameMode = GameMode.OwnStrategy;
			}
			else {
				MessageBox.Show($"Radio button error. Game mode not detected.\n {sender.ToString()}");
			}
			TextOutput.Text = $"Game mode changed to {MonteCarlo.GameMode.ToString()}";
		}

		public void CheckNumericInput(object sender, TextCompositionEventArgs e) {
			Regex _regex = new Regex("[^0-9.-]+");
			e.Handled = _regex.IsMatch(e.Text);
		}
	}
}
