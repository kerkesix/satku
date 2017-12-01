namespace KsxEventTracker.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    using KsxEventTracker.Domain.Aggregates.Registration;

    using Microsoft.WindowsAzure.Storage.Table;

    public class RegistrationRepository : AzureTableStorageRepositoryBase
    {
        private const string tableName = "registrations";

        public RegistrationRepository(AzureTableStorageOptions options) : base(tableName, options)
        {
        }

        public async Task<IEnumerable<RegistrationEntity>> All(string happening)
        {
            var query =
                new TableQuery<RegistrationEntity>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, happening));

            // Old non-async version:
            //return this.Table.ExecuteQuery(query).Where(m => m.DeletedAt == null);

            TableQuerySegment<RegistrationEntity> querySegment = null;
            var returnList = new List<RegistrationEntity>();

            while (querySegment == null || querySegment.ContinuationToken != null)
            {
                querySegment = await Table.ExecuteQuerySegmentedAsync(
                    query, querySegment != null ? querySegment.ContinuationToken : null);
                returnList.AddRange(querySegment);
            }

            return returnList;
        }

        /// <summary>
        /// Inserts a new registration into database.
        /// </summary>
        /// <param name="registrationEntity">The registration entity to add.</param>
        /// <returns>Operation result.</returns>
        public async Task<TableResult> Add(RegistrationEntity registrationEntity)
        {
            var tableOperation = TableOperation.Insert(registrationEntity);

            var result = await this.Table.ExecuteAsync(tableOperation);

            return result;
        }

        public async Task<Tuple<ConfirmResult, RegistrationEntity>> Confirm(string happening, Guid id)
        {
            var registration = await this.GetRegistration(happening, id);

            if (registration == null)
            {
                return new Tuple<ConfirmResult, RegistrationEntity>(ConfirmResult.NotFound, null);
            }

            if (registration.ConfirmedAt.HasValue)
            {
                return new Tuple<ConfirmResult, RegistrationEntity>(ConfirmResult.AlreadyConfirmed, registration);
            }

            registration.ConfirmedAt = DateTime.UtcNow;

            var tableOperation = TableOperation.Replace(registration);
            var result = await this.Table.ExecuteAsync(tableOperation);

            // See table service error codes http://msdn.microsoft.com/en-us/library/windowsazure/dd179438.aspx
            if (result.HttpStatusCode >= 200 && result.HttpStatusCode < 300)
            {
                return new Tuple<ConfirmResult, RegistrationEntity>(ConfirmResult.Success, registration);
            }

            // Fail, write some trace for debugging purposes
            TraceTableOperationFailure(registration, result, tableOperation);

            return new Tuple<ConfirmResult, RegistrationEntity>(ConfirmResult.Failed, registration);
        }

        /// <summary>
        /// If registration has not been confirmed, deletes it. If it has, marks it as deleted. 
        /// </summary>
        /// <param name="happening"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RegistrationEntity> DeleteOrMarkDeleted(string happening, Guid id)
        {
            var registration = await this.GetRegistration(happening, id);

            if (registration == null)
            {
                return null;
            }

            registration.DeletedAt = DateTime.UtcNow;

            // Mark as deleted or delete
            var tableOperation = registration.ConfirmedAt.HasValue ?
                TableOperation.Replace(registration) : TableOperation.Delete(registration);

            var result = await this.Table.ExecuteAsync(tableOperation);

            // See table service error codes http://msdn.microsoft.com/en-us/library/windowsazure/dd179438.aspx
            if (result.HttpStatusCode >= 200 && result.HttpStatusCode < 300)
            {
                return registration;
            }

            // Fail, write some trace for debugging purposes
            TraceTableOperationFailure(registration, result, tableOperation);

            return registration;
        }

        private async Task<RegistrationEntity> GetRegistration(string happening, Guid id)
        {
            var tableOperation = TableOperation.Retrieve<RegistrationEntity>(happening, id.ToString());
            var result = await this.Table.ExecuteAsync(tableOperation);

            if (result.HttpStatusCode != 200)
            {
                return null;
            }

            return (RegistrationEntity)result.Result;
        }

        private static void TraceTableOperationFailure(
            RegistrationEntity registration,
            TableResult result,
            TableOperation tableOperation)
        {
            Trace.TraceError(
               string.Format(
                   CultureInfo.InvariantCulture,
                   "Table storage operation {3} for registration {0}:{1} " + "failed. Status code is {2}, "
                   + "check http://msdn.microsoft.com/en-us/library/windowsazure/dd179438.aspx "
                   + "for detailed status code information.",
                   registration.PartitionKey,
                   registration.RowKey,
                   result.HttpStatusCode,
                   tableOperation.GetType().Name));
        }
    }
}