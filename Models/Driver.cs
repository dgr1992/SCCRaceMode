using System;
using System.Collections.Generic;

namespace SCCRaceMode.Models
{
    public class Driver
    {
        private string _name;
        public string Name
        {
            get{ return _name;}
            set{ _name = value;}
        }

        private LinkedList<int> _startingPositions;
        public double AvgStartingPos
        {
            get{ 
                    double sum = 0;
                    foreach (int pos in _startingPositions)
                    {
                        sum += pos;
                    }
                    return sum/_startingPositions.Count;
                }
        }

        public string Description
        {
            get{ return _name + " (" + AvgStartingPos + ")";}
        }

        private Dictionary<Driver,int> _competitors;

        public Driver()
        {
            _startingPositions = new LinkedList<int>();
            _competitors = new Dictionary<Driver, int>();
        }

        public void addStartingPosition(int startingPos){
            _startingPositions.AddLast(startingPos);
        }

        public int timesDrivenAgainst(Driver driver)
        {
            return _competitors.GetValueOrDefault(driver,0);
        }

        public void incrementMeet(Driver driver)
        {   
            if(timesDrivenAgainst(driver) == 0)
            {
                _competitors.Add(driver,0);
            }
            _competitors[driver] += 1;
        }

        public double diffAvgPositionToExpected(double positionExpected)
        {
            return Math.Abs(AvgStartingPos - positionExpected);
        }
    }
}