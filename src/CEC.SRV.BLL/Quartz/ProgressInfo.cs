using System;
using System.Threading;
using System.Xml.Serialization;

namespace CEC.SRV.BLL.Quartz
{
    [Serializable]
    public class ProgressInfo
    {
        private readonly Guid _id;
        [NonSerialized]
        private Action _onComplete;
        [NonSerialized]
        private Action _onProgress;

        public ProgressInfo(string comments = "", long minimum = 0, long maximum = 0, long value = 0, Action onComplete = null, Action onProgress = null)
        {
            _id = Guid.NewGuid();
            Comments = string.IsNullOrEmpty(comments) ? _id.ToString() : comments;
            Minimum = minimum;
            Maximum = maximum;
            Value = value;
            OnComplete = onComplete;
            OnProgress = onProgress;
        }

        public Guid Id 
        {
            get { return _id; }
        }

        public long Minimum { get; private set; }

        public long Maximum { get; private set; }

        public long Value { get; private set; }

        public double Ratio
        {
            get { return Maximum == 0 ? 0.0 : (double) Value/(double) Maximum*100.0; }
        }

        public string Comments { get; set; }

        [XmlIgnore]
        public Action OnComplete
        {
            get { return _onComplete; }
            set { _onComplete = value; }
        }

        [XmlIgnore]
        public Action OnProgress
        {
            get { return _onProgress; }
            set { _onProgress = value; }
        }

        public void Increase()
        {
            var value = Value + 1;
            SetProgress(value);
        }

        public void SetProgress(long value)
        {
            Value = value;

            if (OnProgress != null)
            {
                OnProgress();
            }

            if (Value == Maximum && OnComplete != null)
            {
                OnComplete();
            }
        }

        public void SetMaximum(long value)
        {
            Maximum = value;
        }
    }
}
