namespace KsxEventTracker.Domain.Aggregates.Checkpoint
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Messages.Events;

    public class Checkpoint: AggregateRootBase<CheckpointState>
    {
        public Checkpoint(Action<IEvent> collectEvent, CheckpointState state)
            : base(collectEvent, state)
        {
        }

        public void Create(
            string happeningId, 
            string id, 
            CheckpointType checkpointType, 
            int order, 
            string name, 
            double latitude, 
            double longitude, 
            decimal distanceFromPrevious, 
            decimal distanceFromStart)
        {
            // Never fails, as everything has been checked already
            this.Apply(
                new CheckpointCreated(
                    happeningId, 
                    id, 
                    checkpointType, 
                    order, 
                    name, 
                    latitude, 
                    longitude, 
                    distanceFromPrevious, 
                    distanceFromStart));
        }

        public void AddScan(ScanInfo scanInfo)
        {
            if (this.State.CheckpointType == CheckpointType.Start && this.State.ReadingsOut.Count == 0)
            {
                // This is the first scan, emit information that happening has started
                this.Apply(new FirstAttendeeForHappeningScanned(scanInfo.HappeningId));
            }

            const int DoubleReadingThresholdSeconds = 5;
            Func<CheckpointScan, bool> match = reading => reading.PersonId.Equals(scanInfo.PersonId);
            AttendeeScanBase scan;

            var inReading = this.State.ReadingsIn.FirstOrDefault(match);

            if (inReading != null)
            {
                if (this.State.ReadingsOut.Any(match))
                {
                    // Both in and out readings already exists, do nothing
                    scan = new AttendeeDoubleScan(scanInfo)
                           {
                               Message = "Sekä sisään- että uloslukema on jo tallennettu."
                           };
                }
                else
                {
                    // Check, that this is not a double reading
                    var difference = scanInfo.Timestamp.Subtract(inReading.TimeStamp);

                    if (difference < TimeSpan.FromSeconds(DoubleReadingThresholdSeconds))
                    {
                        scan = new AttendeeDoubleScan(scanInfo)
                            {
                                DifferenceSinceLastRead = difference.TotalSeconds,
                                Message =
                                    string.Format(
                                        CultureInfo.CurrentCulture,
                                        "Luku tapahtui alle {0} s kuluessa edellisestä.",
                                        DoubleReadingThresholdSeconds)
                            };
                    }
                    else
                    {
                        // Check that this out reading is not before in reading
                        if (inReading.TimeStamp > scanInfo.ScanTimestamp)
                        {
                            scan = new AttendeeScanOutPreceedsScanIn(scanInfo);
                        }
                        else
                        {
                            scan = new AttendeeScanOut(scanInfo);
                        }
                    }
                }
            }
            else
            {
                if (this.State.CheckpointType == CheckpointType.Passthrough)
                {
                    scan = new AttendeePassthroughScan(scanInfo);
                }
                else
                {
                    // This is the first reading for this person on this checkpoint
                    scan = this.State.CheckpointType == CheckpointType.Start
                        ? (AttendeeScanBase)new AttendeeScanOut(scanInfo)
                        : new AttendeeScanIn(scanInfo);
                }
            }

            // Ensure, that event goes to the same checkpoint (should be correct already)
            scan.CheckpointId = this.State.Id;

            this.Apply(scan);

            if (this.State.CheckpointType == CheckpointType.Start && scan is AttendeeScanOut)
            {
                this.Apply(new AttendeeScannedAtStartCheckpoint(scanInfo.HappeningId, scanInfo.PersonId));
            }

            if (this.State.CheckpointType == CheckpointType.Finish && scan is AttendeeScanIn)
            {
                this.Apply(new AttendeeScannedAtFinishCheckpoint(scanInfo.HappeningId, scanInfo.PersonId));
            }
        }

        public void ChangeScanTime(ScanInfo scanInfo, ScanTimeType scanTimeType)
        {
            var newTime = scanInfo.ScanTimestamp;

            Func<CheckpointScan, bool> match = r => r.PersonId.Equals(scanInfo.PersonId, StringComparison.OrdinalIgnoreCase);
            var readingIn = this.State.ReadingsIn.FirstOrDefault(match);
            var readingOut = this.State.ReadingsOut.FirstOrDefault(match);

            switch (scanTimeType)
            {
                case ScanTimeType.In:
                    if (readingIn == null)
                    {
                        Trace.TraceWarning("Scan to change time not found (in).");
                        return;
                    }

                    if (readingOut != null && readingOut.TimeStamp < newTime)
                    {
                        this.Apply(new AttendeeScanOutPreceedsScanIn(scanInfo));
                        return;
                    }

                    this.Apply(new AttendeeScanInTimeChanged(scanInfo, newTime));
                    break;
                case ScanTimeType.Out:
                    if (readingOut == null)
                    {
                        Trace.TraceWarning("Scan to change time not found (out).");
                        return;
                    }

                    if (readingIn != null && readingIn.TimeStamp > newTime)
                    {
                        this.Apply(new AttendeeScanOutPreceedsScanIn(scanInfo));
                        return;
                    }

                    this.Apply(new AttendeeScanOutTimeChanged(scanInfo, newTime));
                    break;
                case ScanTimeType.Passthrough:
                    if (this.State.CheckpointType == CheckpointType.Passthrough)
                    {
                        if (readingIn == null)
                        {
                            Trace.TraceWarning("Scan to change time not found (passthrough).");
                            return;
                        }

                        this.Apply(new AttendeePassthroughScanTimeChanged(scanInfo, newTime));    
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException("scanTimeType");
            }
        }

        public void RemoveScan(ScanInfo scanInfo)
        {
            // TODO: Should check that user has not entered the next checkpoint already. Propably remove needs to go through some validation aggregate.

            Func<CheckpointScan, bool> match = m => m.PersonId.Equals(scanInfo.PersonId, StringComparison.OrdinalIgnoreCase);

            var readingIn = this.State.ReadingsIn.FirstOrDefault(match);
            var readingOut = this.State.ReadingsOut.FirstOrDefault(match);

            if (readingIn == null && readingOut == null)
            {
                Trace.TraceWarning("Scan to remove not found");
                return;
            }

            if (this.State.CheckpointType == CheckpointType.Passthrough 
                && readingIn != null && readingOut != null)
            {
                // Remove both in and out    
                this.RemoveAttendeeScanIn(scanInfo, readingIn.ScanId);
                this.RemoveAttendeeScanOut(scanInfo, readingOut.ScanId);
                return;
            }

            // Not a passthrough, decide which to remove
            if (readingOut != null)
            {
                this.RemoveAttendeeScanOut(scanInfo, readingOut.ScanId);
            }
            else
            {
                this.RemoveAttendeeScanIn(scanInfo, readingIn.ScanId);
            }
        }

        public void RemoveQuit(ScanInfo scanInfo)
        {
            // TODO: Should check that user has not entered the next checkpoint already. Propably quit needs to go through some validation aggregate.
            this.Apply(new AttendeeQuitRemoved(scanInfo));
        }

        private void RemoveAttendeeScanIn(ScanInfo scanInfo, string scanId)
        {
            var removeEvent = new AttendeeScanInRemoved(scanInfo) { ScanId = scanId };
            this.Apply(removeEvent);
        }

        private void RemoveAttendeeScanOut(ScanInfo scanInfo, string scanId)
        {
            var removeEvent = new AttendeeScanOutRemoved(scanInfo) { ScanId = scanId };
            this.Apply(removeEvent);
        }
    }
}