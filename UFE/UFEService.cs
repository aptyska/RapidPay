using RapidPay.Core.Services.Interfaces;

namespace RapidPay.Core
{
    public class UFEService : IUFEService
    {
        private static UFEService? _instance = null;
        
        private static readonly object _instanceLock = new object();

        private static readonly object _feeLock = new object();

        private DateTime _feeTime = DateTime.UtcNow.AddHours(-1);//Calculate a fresh fee when run for first time

        private decimal _feeAmount = 1;//assume 1 for intial fee amount on start

        private static readonly Random _random = new Random();

        private static double RandomNumberBetween(double minValue, double maxValue)
        {
            var next = _random.NextDouble();

            return minValue + (next * (maxValue - minValue));
        }

        public UFEService()
        {

        }

        public static UFEService Instance
        {
            get
            {
                if (_instance == null)//check for null first to avoid lock if not needed
                {
                    lock(_instanceLock)
                    {
                        if (_instance == null)//check for null again for thread safety
                        {
                            _instance = new UFEService();
                        }
                    }
                }
                return _instance;
            }
        }

        public decimal CalculateFee()
        {
            if(_feeTime < DateTime.UtcNow.AddHours(-1))//check to see if lock is needed
            {
                lock (_feeLock)
                {
                    if (_feeTime < DateTime.UtcNow.AddHours(-1))//check again for thread safety
                    {
                        _feeAmount = _feeAmount * Convert.ToDecimal(RandomNumberBetween(0, 2));
                        _feeTime = DateTime.UtcNow;
                    }
                }
            }

            return _feeAmount;
        }

    }
}