using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace QmsApp.Models
{
    public class QmsDbContext:DbContext
    {
        public QmsDbContext()
            : base("QmsConnString")
    {
        
    }
        public DbSet<Service> Services { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<RoleTask> RoleTasks { get; set; }
        public DbSet<Logins> Logins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
        public int SaveChanges(string occurrenceUserId)
        {
            if (!String.IsNullOrEmpty(occurrenceUserId))
            {
                int userId = Convert.ToInt32(occurrenceUserId);
                // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
                foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
                {
                    // For each changed record, get the audit record entries and add them
                    foreach (AuditLog x in GetAuditRecordsForChange(ent, userId))
                    {
                        this.AuditLogs.Add(x);
                    }
                }
            }

            try
            {
                // Call the original SaveChanges(), which will save both the changes made and the audit records
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);
                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);

            }

        }
        private List<AuditLog> GetAuditRecordsForChange(DbEntityEntry dbEntry, int? userId)
        {
            List<AuditLog> result = new List<AuditLog>();

            DateTime changeTime = DateTime.Now;

            // Get the Table() attribute, if one exists
            //TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
            TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault() as TableAttribute;

            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            string keyName = dbEntry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).Name;

            if (dbEntry.State == EntityState.Added)
            {
                try
                {
                    base.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
                    }
                }
                // For Inserts, just add the whole record
                // If the entity implements IDescribableEntity, use the description from Describe(), otherwise use ToString()
                result.Add(new AuditLog()
                {
                    AuditLogId = Guid.NewGuid(),
                    EventType = "I", // Added/Inserted
                    TableName = tableName,
                    PrimaryKeyName = keyName,
                    PrimaryKeyValue = dbEntry.GetDatabaseValues().GetValue<object>(keyName).ToString(),  // Again, adjust this if you have a multi-column key

                    //ColumnName = "*ALL",    // Or make it nullable, whatever you want
                    //NewValue = dbEntry.CurrentValues.ToObject().ToString(),
                    CreatedUser = userId,
                    UpdatedDate = changeTime,
                }
                    );
            }
            else if (dbEntry.State == EntityState.Deleted)
            {
                // Same with deletes, do the whole record, and use either the description from Describe() or ToString()
                result.Add(new AuditLog()
                {
                    AuditLogId = Guid.NewGuid(),
                    EventType = "D", // Deleted
                    TableName = tableName,
                    PrimaryKeyName = keyName,
                    PrimaryKeyValue = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                    //ColumnName = "*ALL",
                    // NewValue = (dbEntry.OriginalValues.ToObject() is IDescribableEntity) ? (dbEntry.OriginalValues.ToObject() as IDescribableEntity).Describe() : dbEntry.OriginalValues.ToObject().ToString(),
                    CreatedUser = userId,
                    UpdatedDate = changeTime
                }
                    );
            }
            else if (dbEntry.State == EntityState.Modified)
            {
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    var originalValue = dbEntry.GetDatabaseValues().GetValue<object>(propertyName) == null ? null : dbEntry.GetDatabaseValues().GetValue<object>(propertyName).ToString();
                    var newValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString();
                    // For updates, we only want to capture the columns that actually changed
                    // if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName).ToString(), dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()))
                    //if (!object.Equals(dbEntry.GetDatabaseValues().GetValue<object>(propertyName).ToString(), dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()))
                    if (!(originalValue == newValue))
                    {
                        result.Add(new AuditLog()
                        {
                            AuditLogId = Guid.NewGuid(),
                            EventType = "U",    // Modified/Updated
                            TableName = tableName,
                            PrimaryKeyName = keyName,
                            PrimaryKeyValue = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                            ColumnName = propertyName,
                            OldValue = originalValue,
                            NewValue = newValue,
                            CreatedUser = userId,
                            UpdatedDate = changeTime
                        });
                    }
                }
            }
            // Otherwise, don't do anything, we don't care about Unchanged or Detached entities

            return result;
        }
    }
    
}