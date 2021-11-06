using Homework_6.DataObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_6.Generators
{
    public class RandomGenerators
    {
        public List<Color> RandomColorsList { get; }
        public List<DataPoint> Plotted { get; }
        public List<List<DataPoint>> Sequences { get; }
        private Random Random = new Random();
        public RandomGenerators()
        {
            this.RandomColorsList = new List<Color>();
            this.Plotted = new List<DataPoint>();
            this.Sequences = new List<List<DataPoint>>();
        }

        public void ResetLists()
        {
            this.Plotted.Clear();
            this.Sequences.Clear();
            this.RandomColorsList.Clear();

            this.PopulateSequenceList();
            this.PopulateColorList(this.Sequences.Count);
        }

        public void PopulateColorList(int N)
        {
            for(int i = 0; i < N; i++)
                RandomColorsList.Add(Color.FromArgb(Random.Next(0, 255), Random.Next(0, 255), Random.Next(0, 255)));
        }

        public List<List<DataPoint>> PopulateSequenceList()
        {
            List<Array> TempArraySequence = new List<Array>();
            List<Array> TempArrayTrajectory = new List<Array>();
            List<double> valuesAtT = new List<double>();
            List<double> valuesAtN = new List<double>();
            for (int i = 0; i < Form1.SequencesNumber; i++)
            {
                double[] currentSequence = GenerateSamplePath(Form1.SuccessProbability, Form1.SequencesSize);
                TempArraySequence.Add(currentSequence);

                double[] currentTrajectory = ComputeTrajectory(currentSequence, Form1.Mode);
                TempArrayTrajectory.Add(currentTrajectory);
                valuesAtN.Add(currentTrajectory[Form1.SequencesSize - 1]);
                valuesAtT.Add(currentTrajectory[Form1.InstantToPlotInstogram - 1]);
            }

            if (TempArrayTrajectory.Count > 0)
            {
                foreach (double[] trajectory in TempArrayTrajectory)
                {
                    List<DataPoint> currentTrajectoryPoints = new List<DataPoint>();

                    for (int i = 0; i < trajectory.Length; i++)
                    {
                        DataPoint point = new DataPoint(i, trajectory[i]);
                        this.Plotted.Add(point);
                        currentTrajectoryPoints.Add(point);
                    }
                    this.Sequences.Add(currentTrajectoryPoints);
                }
            }

            return this.Sequences;
        }

        #region Ugly Math Stuff
        private int BernoulliGenerator(double p)
        {
            double u = Random.NextDouble();
            if (u < p) return 1;
            return 0;
        }

        private double[] GenerateSamplePath(double p, int n)
        {
            double[] path = new double[n];

            for (int i = 0; i < n; i++)
                path[i] = BernoulliGenerator(p);

            return path;
        }

        private double[] ComputeTrajectory(double[] path, int mode)
        {
            double[] trajectory = path;
            double q = 1 - Form1.SuccessProbability;
            int count = 0;

            if (mode == 0)
            {
                for (int i = 0; i < path.Length; i++)
                {
                    if (trajectory[i] == 1)
                        count++;

                    trajectory[i] = (double)count / (i + 1);
                }
            }
            else if (mode == 1)
            {
                for (int i = 1; i < path.Length; i++)
                    trajectory[i] += trajectory[i - 1];
            }
            else
            {
                for (int i = 0; i < path.Length; i++)
                {
                    if (trajectory[i] == 1)
                        count++;

                    trajectory[i] = (((double)count / (i + 1)) - Form1.SuccessProbability) / Math.Sqrt((double)Form1.SuccessProbability * q / (i + 1));
                }
            }
            return trajectory;
        }
        #endregion
    }
}
