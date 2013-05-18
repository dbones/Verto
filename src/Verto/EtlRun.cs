namespace Verto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rhino.Etl.Core;

    /// <summary>
    /// an EtlRun will execute all the processes in an ETL.
    /// </summary>
    public class EtlRun : IDisposable
    {
        public List<EtlProcess> Processes = new List<EtlProcess>();
        public List<Exception> Errors = new List<Exception>();
        public EtlProcess FailedOn;

        /// <summary>
        /// register any EtlProcess which is associated with this EtlRun.
        /// </summary>
        /// <param name="etlProcess">the process to add to the EtlRun</param>
        public void Register(EtlProcess etlProcess)
        {
            Processes.Add(etlProcess);
        }

        /// <summary>
        /// executes all the processes in the ETL
        /// </summary>
        public void Execute()
        {
            foreach (var etlProcess in Processes)
            {
                etlProcess.Execute();

                if (etlProcess.GetAllErrors().Count() <= 0) continue;
                Errors = etlProcess.GetAllErrors().ToList();
                FailedOn = etlProcess;
                break;
            }
        }

        /// <summary>
        /// if the EtlRun had any errors
        /// </summary>
        public bool HasErrors()
        {
            return Errors.Count > 0;
        }

        /// <summary>
        /// clean up all the processes
        /// </summary>
        public void Dispose()
        {
            foreach (var etlProcess in Processes)
            {
                etlProcess.Dispose();
            }
        }
    }
}
