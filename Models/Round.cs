using System.Collections.Generic;

namespace SCCRaceMode.Models
{
    public class Round
    {
        public int NumberHeats
        {
            get{ return _heats.Length; }
        }

        private int _heatSize;
        public int HeatSize
        {
            get{ return _heatSize; }
        }

        private Heat[] _heats;
        public Heat[] Heats
        {
            get{ return _heats; }
        }

        private int _roundNum;

        public string Description
        {
            get{ return "Round " + _roundNum; }
        }

        /// <summary> Creates new round without creating
        /// </summary>
        public Round(int roundNum, int numberOfHeats)
        {
            _heats = new Heat[numberOfHeats];
            _roundNum = roundNum;
        }
        /// <summary> Creates new round and create all the heats
        /// </summary>
        public Round(int roundNum, int numberOfHeats, int heatSize)
        {
            _heats = new Heat[numberOfHeats];
            _roundNum = roundNum;
            _heatSize = heatSize;
            createHeats(heatSize);
        }

        private void createHeats(int size)
        {
            for(int i = 0; i < _heats.Length; i++)
            {
                _heats[i] = new Heat(i + 1,size);
            }
        }
        
        public bool addHeat(int heatNumber, Heat heat)
        {
            if(heatNumber > _heats.Length || heatNumber <= 0)
            {
                return false;
            } 

            _heats[heatNumber - 1] = heat;
            return true;
        }
    }
}