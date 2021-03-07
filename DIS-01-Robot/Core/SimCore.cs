using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore {
	public abstract class SimCore {
		protected SimCore() {
			Stop = false;
			ActualReplication = 0;
		}

		public bool Stop { get; set; }
		public int NumberOfReplications { get; set; }
		public int ActualReplication { get; set; }

		protected abstract void BeforeSimulation();

		protected abstract void BeforeReplication();

		protected abstract void DoReplication();

		protected abstract void AfterReplication();

		protected abstract void AfterSimulation();


		public void Simulate(int replications) {
			try {
				NumberOfReplications = replications;
				Stop = false;
				BeforeSimulation();
				for (int replication = 0; replication < replications; replication++) {
					ActualReplication = replication;
					if (Stop) {
						break;
					}

					BeforeReplication();
					DoReplication();
					AfterReplication();
				}

				AfterSimulation();
			}
			catch (Exception exception) {
				Console.WriteLine(exception);
			}
		}
	}
}
