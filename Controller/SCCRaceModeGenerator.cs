using System;
using System.Collections.Generic;
using System.Linq;
using SCCRaceMode.Models;

namespace SCCRaceMode.Controller
{
    public class SCCRaceModeGenerator
    {   
        private int _heatSize;
        private int _maxHeatSize;
        private int _numRounds;
        private List<Driver> _drivers;

        private int _numHeats;

        private int _expectedAvgStartingPosition;

        public SCCRaceModeGenerator(List<Driver> drivers, int numberHeats,int maxHeatSize, int numRounds)
        {
            _drivers = drivers;
            _maxHeatSize = maxHeatSize;
            _numRounds = numRounds;
            _numHeats = numberHeats;
            CalculateHeatSize();
        }

        private void CalculateHeatSize()
        {   
            //First try drivers divided by number of heats and check if heatSize is smaller or equal to max heat size
            int heatSize = (int)Math.Ceiling((double)_drivers.Count / _numHeats);

            //Increment the number of heats until the heat size is valid
            while(heatSize > _maxHeatSize)
            {
                _numHeats++;
                heatSize = (int)Math.Ceiling((double)_drivers.Count / _numHeats);
            }

            _heatSize = heatSize;

            _expectedAvgStartingPosition = (int)Math.Ceiling(_heatSize/2.0);
        }

        public LinkedList<Round> ArangeHeats()
        {
            LinkedList<Round> rounds = new LinkedList<Round>();
        
            //The first round is randmoly assigned
            Round round1 = randomAssignRound(1,_drivers);
            rounds.AddLast(round1);
            EvaluateWhichDriversMet(round1);
            //Sort the drivers list descending by the diffrence to the expected avg starting position
            IOrderedEnumerable<Driver> drivers = _drivers.OrderByDescending(o => o.diffAvgPositionToExpected(_expectedAvgStartingPosition));

            //For the other rounds assigne based on the statistics of diff to avg starting position ans drivers met
            for(int i = 2; i <= _numRounds; i++)
            {
                Round round = new Round(i,_numHeats,_heatSize);

                foreach (Driver driver in drivers)
                {
                    Heat heat = findHeatForDriver(round, driver);
                    int recommendedStartingPosition = nextStartingPosition(driver);
                    int startingPosition = findFreeStartingPosition(heat,recommendedStartingPosition);
                    heat.addDriverToGrid(startingPosition,driver);
                }
                rounds.AddLast(round);
                EvaluateWhichDriversMet(round);
                //Sort the drivers list descending by the diffrence to the expected avg starting position
                drivers = drivers.OrderByDescending(o => o.diffAvgPositionToExpected(_expectedAvgStartingPosition));
            }

            return rounds;
        }

        private int findFreeStartingPosition(Heat heat, int recommendedStartingPosition)
        {
            int pos = -1;
            //Try the recommended Position
            if(heat.isPositionFree(recommendedStartingPosition))
            {
                pos = recommendedStartingPosition;
            } else
            {   
                int posStep = 1;
                //Check all position above and below the recommended starting position, prefere position further back
                while(pos == -1)
                {
                    int tmpPosLow = recommendedStartingPosition - posStep;
                    int tmpPosHigh = recommendedStartingPosition + posStep;

                    if(tmpPosLow <= 0)
                    {
                        tmpPosLow = 1;
                    }

                    if(tmpPosHigh > _heatSize)
                    {
                        tmpPosHigh = _heatSize;
                    }

                    if(heat.isPositionFree(tmpPosHigh))
                    {
                        pos = tmpPosHigh;
                    } else if(heat.isPositionFree(tmpPosLow))
                    {
                        pos = tmpPosLow;
                    } else 
                    {
                        //Increase step size on grid
                        posStep++;
                    }

                }
            }
            return pos;
        }

        private Heat findHeatForDriver(Round round, Driver driver)
        {
            double[] heatMatchScore = new double[round.NumberHeats];
            
            //Calculate a heatMatchScore for eatch heat
            for(int i = 0; i < round.NumberHeats; i++)
            {
                heatMatchScore[i] = 0;
                int metCounter = 0;
                foreach(Position positionInHeat in round.Heats[i].StartingGrid)
                {
                    if(positionInHeat != null && positionInHeat.TheDriver != null)
                    {
                        int timesMet = driver.timesDrivenAgainst(positionInHeat.TheDriver);

                        //Create sort of weighted score
                        heatMatchScore[i] += timesMet^6;
                        metCounter++;
                    }
                }

                //Mean score based on the number of meets
                if(heatMatchScore[i] != 0){
                    heatMatchScore[i] = heatMatchScore[i] / metCounter;
                }
            }

            //The lower the score the better the match
            Heat bestHeat = null;
            double maxScore = heatMatchScore.Max();
            while(bestHeat == null && (heatMatchScore.Max() != (maxScore + 100) || heatMatchScore.Min() != (maxScore + 100)))
            {
                int indexBestHeat = Array.IndexOf(heatMatchScore, heatMatchScore.Min());
                //Check if some space is left in this heat
                if(round.Heats[indexBestHeat].FreeGridPositions > 0)
                {
                    bestHeat = round.Heats[indexBestHeat];
                } else
                {
                    //Eliminate this heat by setting it to maxScore + 100
                    heatMatchScore[indexBestHeat] = maxScore + 100;
                }
            }
            return bestHeat;
        }

        /// <summary> Sorts the drivers list desceding with respect to the difference between expected to average position
        /// </summary>
        private void SortTheDriversListDescending()
        {
            _drivers.Sort((driver1, driver2) => {
                if(driver1.diffAvgPositionToExpected((double)_expectedAvgStartingPosition) > driver2.diffAvgPositionToExpected((double)_expectedAvgStartingPosition))
                {
                    return 1;
                } else if (driver1.diffAvgPositionToExpected((double)_expectedAvgStartingPosition) < driver2.diffAvgPositionToExpected((double)_expectedAvgStartingPosition))
                {   
                    return -1;
                }
                else
                {
                    return 0;
                }
            });
        }

        /// <summary> Update the statistics of the drivers met
        /// </summary>
        private void EvaluateWhichDriversMet(Round round)
        {
            foreach(Heat heat in round.Heats)
            {
                foreach(Position position1 in heat.StartingGrid)
                {
                    foreach(Position position2 in heat.StartingGrid)
                    {
                        if(position1 != null && position2 != null && position1.TheDriver != null && position2.TheDriver != null && position1.TheDriver != position2.TheDriver)
                        {
                            position1.TheDriver.incrementMeet(position2.TheDriver);
                        }
                    }
                }
            }
        }

        /// <summary> Calculate the next starting position based on his current avg starting position and the avg starting position reach
        /// </summary>
        /// <param name="driver">The driver of which the next starting postion should be calculated</param>
        /// <returns>Postion the driver should start from to reach the avg starting position</returns>
        private int nextStartingPosition(Driver driver)
        {
            double currentAvgPosition = driver.AvgStartingPos;
            double nextStartingPosition = 2 * _expectedAvgStartingPosition - currentAvgPosition;
            nextStartingPosition = Math.Floor(nextStartingPosition);

            if(nextStartingPosition <= 0)
            {
                nextStartingPosition = 1;
            }

            return (int)nextStartingPosition;
        }

        /// <summary> Randomly assigne the drivers to heats and starting position
        /// </summary>
        private Round randomAssignRound(int roundNum, List<Driver> driversOrg)
        {
            List<Driver> drivers = new List<Driver>(driversOrg);

            Round round = new Round(roundNum, _numHeats,_heatSize);

            Random rnd = new Random();

            while (drivers.Count != 0)
            {   
                Driver[] driverArray = drivers.ToArray();
                
                //Pick random heat
                int rndHeat = rnd.Next(0, _numHeats - 1);
                //Check if heat as free grid positions
                while(round.Heats[rndHeat].FreeGridPositions <= 0)
                {
                    rndHeat = rnd.Next(0, _numHeats);
                }
                //Get the heat
                Heat heat = round.Heats[rndHeat];

                //Pick random driver
                int rndDriver = rnd.Next(0,driverArray.Length);

                //Pick random starting position and check if free
                int rndPostion = rnd.Next(1,heat.StartingGrid.Length + 1);
                while(!heat.StartingGrid[rndPostion - 1].IsFree())
                {
                    rndPostion = rnd.Next(1,heat.StartingGrid.Length + 1);

                    /*if(heat.FreeGridPositions == 1)
                    {
                        for(int i = 0; i < heat.StartingGrid.Length; i++)
                        {
                            if(heat.StartingGrid[i] == null)
                            {
                                rndPostion = i;
                                break;
                            }
                        }
                    }*/
                }

                //Get driver and remove him
                Driver driver = driverArray[rndDriver];
                drivers.Remove(driver);

                //Place the driver on the grid
                heat.addDriverToGrid(rndPostion,driver);
            }

            return round;
        }
    }
}