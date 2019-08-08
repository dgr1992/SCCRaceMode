namespace SCCRaceMode.Models
{
    public class Position
    {
        private int _posNum;
        public int PosNum
        {
            get{return _posNum;}
        }

        private Driver _driver;
        public Driver TheDriver
        {
            get{ return _driver;}
            set{ _driver = value;}
        }
        
        public string DriverText
        {
            get { 
                if(_driver != null)
                {
                    return  _posNum + "." + _driver.Description;
                } else 
                {
                    return _posNum + ".";
                }
            }
        }

        public Position(int position)
        {
            _posNum = position;
            _driver = null;
        }

        public bool IsFree()
        {
            return _driver == null;
        }
    }
}