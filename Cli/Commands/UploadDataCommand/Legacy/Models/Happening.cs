namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models
{
    using System;
    using System.Collections.Generic;

    public class Happening
    {
        public Happening()
        {
            this.Checkpoints = new List<Checkpoint>();
        }

        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this happening is the default one that 
        /// should be automatically displayed when browsing to the site for the first time.
        /// </summary>
        public bool Default { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Checkpoint> Checkpoints { get; set; }
    }
}