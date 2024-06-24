using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amdaris;
using Amdaris.Domain;
using Amdaris.NHibernateProvider;
using CEC.SRV.Domain.Importer;

namespace CEC.SRV.BLL.Importer
{
    public abstract class ImportProcesser<T> where T : RawData
    {
        private readonly IRepository _repository;
        protected readonly ILogger Logger;
        private readonly int _batchSize;

        protected ImportProcesser(IRepository repository, ILogger logger, int batchSize)
        {
            _repository = repository;
            Logger = logger;
            _batchSize = batchSize;
        }

        public int BatchSize
        {
            get { return _batchSize; }
        }

        public T Get(long id)
        {
            return _repository.Get<T>(id);
        }

        public void Delete(T rawData)
        {
            _repository.Delete(rawData);
        }

        public void Retry(T rawData, string message)
        {
            rawData.SetRetry(message);
        }

        public void ProcessAllNew()
        {
            while (CountRawByStatus(RawDataStatus.New, RawDataStatus.Retry) != 0)
                ProcessNew();
            
        }

        public void ProcessNew(params RawDataStatus[] statuses)
        {
            IEnumerable<T> data = new List<T>();
            data = GetRawByStatus(statuses);

            var total = CountRawByStatus(statuses);
            var count = 0;
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            foreach (var raw in data)
            {
                if (count++ % 1000 == 0)
                {
                    Console.WriteLine("{0} of {1} processed", count, total);
                }

                Process(raw);
            }

            stopwatch.Stop();

            Console.WriteLine("Processed {0} in {1}", total, stopwatch.Elapsed);
        }

        public void Process(T rawData)
        {
            try
            {
                Logger.Info(string.Format("Process {0}", rawData.ToString()));
                if (rawData.IsValid())
                {
                    ProcessInternal(rawData);
                    NotifySuccess(rawData);
                }
                else
                {
                    SetErrorState(rawData, rawData.GetValidationString());
                }
            }
            catch (Exception ex)
            {
                Logger.Info(string.Format("Processing error for {0} with messasge: {1}", rawData.ToString(), ex));
                SetErrorState(rawData, ex.Message);
            }
        }

        private void SetErrorState(T rawData, string message)
        {
            Logger.Info(string.Format("Processing error for {0} with messasge: {1}", rawData.ToString(), message));
            rawData.SetError(message);
            NotifyFailure(rawData);
            _repository.SaveOrUpdate(rawData);
        }

        protected abstract void ProcessInternal(T rawData);
        protected abstract void NotifySuccess(T rawData);
        protected abstract void NotifyFailure(T rawData);

        protected IEnumerable<T> GetRawByStatus(params RawDataStatus[] statuses)
        {
            if (statuses == null || !statuses.Any())
                throw new ArgumentNullException("statuses");

            var query = _repository.Query<T>().Where(x => statuses.Contains(x.Status)).OrderBy(y => y.Created).Take(_batchSize);

            return query.ToList();
        }

        public int CountRawByStatus(params RawDataStatus[] statuses)
        {
            if (statuses == null || !statuses.Any())
                throw new ArgumentNullException("statuses");

            return _repository.Query<T>().Count(x => statuses.Contains(x.Status));
        }

    }
}