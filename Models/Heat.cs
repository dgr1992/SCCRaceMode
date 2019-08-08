using System.Collections.Generic;

namespace SCCRaceMode.Models
{
    public class Heat
    {
        private Position[] _startingGrid;
        public Position[] StartingGrid 
        {
            get{ return _startingGrid; }
        }

        private int _freeGridPositions;
        public int FreeGridPositions
        {
            get{ return _freeGridPositions; }
        }

        private int _heatNum;

        public string Description
        {
            get { return "Heat " + _heatNum; }
        }

        public Heat(int heatNum, int heatSize)
        {
            _freeGridPositions = heatSize;
            _heatNum = heatNum;
            initGrid();
        }

        private void initGrid()
        {
            _startingGrid = new Position[_freeGridPositions];
            for(int i = 0; i < _freeGridPositions; i++)
            {
                _startingGrid[i] = new Position(i + 1);
            }
        }

        public bool isPositionFree(int position)
        {
            return _startingGrid[position - 1].IsFree();
        }

        public void addDriverToGrid(int position, Driver driver){
            _startingGrid[position - 1].TheDriver = driver;
            driver.addStartingPosition(position);
            _freeGridPositions--;
        }
    }
}