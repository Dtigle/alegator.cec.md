using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain.Print
{
    public class ExportPollingStation : PrintEntity
    {
        private readonly PrintSession _printSession;

        private PollingStation _pollingStation;

        private ElectionRound _electionRound;

        private Circumscription _circumscription;

        private string _numberPerElection;



        public ExportPollingStation()
        {
        }

        public ExportPollingStation(PrintSession printSession, PollingStation pollingStation)
        {
            _printSession = printSession;
            _pollingStation = pollingStation;
        }

        public ExportPollingStation(PrintSession printSession, PollingStation pollingStation, ElectionRound electionRound, Circumscription circumscription, string numberPerElection)
        {
            _printSession = printSession;
            _pollingStation = pollingStation;
            _electionRound = electionRound;
            _circumscription = circumscription;
            _numberPerElection = numberPerElection;
        }

        /// <summary>
        /// Message when exporting fail
        /// </summary>
        public virtual string StatusMessage { get; set; }

        /// <summary>
        /// Message when exporting fail
        /// </summary>
        public virtual string NumberPerElection
        {
            get
            {
                return _numberPerElection;
            }
            set
            {
                _numberPerElection = value;
            }
        }

        public virtual PrintSession PrintSession
        {
            get { return _printSession; }
        }

        public virtual PollingStation PollingStation
        {
            get
            {
                return _pollingStation;
            }
            set
            {
                _pollingStation = value;
            }
        }

        public virtual ElectionRound ElectionRound
        {
            get
            {
                return _electionRound;
            }
            set
            {
                _electionRound = value;
            }
        }

        public virtual Circumscription Circumscription
        {
            get
            {
                return _circumscription;
            }
            set
            {
                _circumscription = value;
            }
        }

        public virtual string GetObjectType()
        {
            return GetType().Name;
        }
    }
}