namespace KsxEventTracker.Domain.Aggregates.Happening
{
    using System;

    using Checkpoint;
    using Messages.Events;

    public sealed class Happening : AggregateRootBase<HappeningState>
    {
        public Happening(Action<IEvent> collectEvent, HappeningState state)
            : base(collectEvent, state)
        {
        }

        public void Create(string id) => Apply(new HappeningCreated(id));

        public void SetCoordinates(string happeningId, string path)
        {
            // Gzip path, as it is otherwise too big
            Apply(new CoordinatePathSet(happeningId, path.ToGzip()));
        }

        /// <summary>
        /// Validates the scan that sends attendee into walking (the start scan, that is).
        /// Start scan does not need extensive validation as other scans do.
        /// </summary>
        /// <param name="scan">Scan information. </param>
        public void ValidateStartScan(ScanInfo scan) => Apply(new ScanValidated(scan));

        public void ValidateScan(ScanInfo scan)
        {
            // These are non-business events, just exit without an event to keep event count down.
            // These are checked on UI already, should never happen.
            if (State.Quitters.Contains(scan.PersonId) || State.Completed.Contains(scan.PersonId))
            {
                Trace.TraceWarning("User either quit or completed, scan canceled.");
                return;
            }

            // UI logic validates that this attendee actually exists on this happening. Just exit here.
            if (!State.Started.Contains(scan.PersonId))
            {
                Trace.TraceWarning("User has not started, scan canceled.");
                return;
            }

            Apply(new ScanValidated(scan));
        }

        public void ValidateRemoveScan(ScanInfo scanInfo)
        {
            if (State.Completed.Contains(scanInfo.PersonId) || State.Quitters.Contains(scanInfo.PersonId))
            {
                Trace.TraceWarning("User either quit or completed, scan removal canceled.");
                return;
            }

            Apply(new RemoveScanValidated(scanInfo));
        }

        public void Quit(ScanInfo scan, decimal walkedSinceLast, string description)
        {
            if (State.Quitters.Contains(scan.PersonId) || State.Completed.Contains(scan.PersonId))
            {
                Trace.TraceWarning("User either quit or completed, quit canceled.");
                return;
            }

            Apply(new AttendeeQuit(scan, walkedSinceLast, description));
        }

        public void RemoveQuit(ScanInfo scanInfo)
        {
            if (State.Completed.Contains(scanInfo.PersonId) || !State.Quitters.Contains(scanInfo.PersonId))
            {
                Trace.TraceWarning("User either quit or completed, remove quit canceled.");
                return;
            }

            Apply(new AttendeeQuitRemoved(scanInfo));
        }

        public void AttendeeStarted(string happening, string personId)
        {
            if (!State.Started.Contains(personId))
            {
                Apply(new AttendeeStarted(happening, personId));
            }
        }

        public void AttendeeCompleted(string happening, string personId)
        {
            if (!State.Completed.Contains(personId))
            {
                Apply(new AttendeeCompleted(happening, personId));
            }
        }
    }
}