using AsapTasks.Data;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsapTasks.Managers
{
    public partial class EnrollmentManager
    {
        static EnrollmentManager defaultInstance = new EnrollmentManager();
        MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Enrollment> enrollmentTable;
#else
        IMobileServiceTable<Enrollment> enrollmentTable;
#endif
        const string offlineDbPath = @"localstore.db";

        /// <summary>
        /// Constructor
        /// </summary>
        private EnrollmentManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Enrollment>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.enrollmentTable = client.GetSyncTable<Enrollment>();
#else
            this.enrollmentTable = client.GetTable<Enrollment>();
#endif
        }

        /// <summary>
        /// Singeton Variable
        /// </summary>
        public static EnrollmentManager DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        /// <summary>
        /// Client object
        /// </summary>
        public MobileServiceClient CurrentClient
        {
            get { return client; }
        }

        public bool IsOfflineEnabled
        {
            get { return enrollmentTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Enrollment>; }
        }

        /// <summary>
        /// Get Enrollment from Developer Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="syncItems"></param>
        /// <returns>Enrollment Object</returns>
        public async Task<List<Enrollment>> GetEnrollmentFromIdAsync(string devId)
        {
            try
            {
                List<Enrollment> items = await enrollmentTable.Where(x => x.DeveloperId == devId).ToListAsync();                

                return (items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e });
            }
            return null;
        }

        /// <summary>
        /// Get Enrollment from Developer Id and Project Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="syncItems"></param>
        /// <returns>Enrollment Object</returns>
        public async Task<Enrollment> GetEnrollmentFromBothIdAsync(string devId, string projId)
        {
            try
            {
                Enrollment item = (await enrollmentTable.Where(x => x.DeveloperId == devId).Where(x => x.ProjectId == projId).ToEnumerableAsync()).FirstOrDefault();

                return (item);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e });
            }
            return null;
        }

        /// <summary>
        /// Get Enrollment from Project Id
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="syncItems"></param>
        /// <returns>Enrollment Object</returns>
        public async Task<List<Enrollment>> GetEnrollmentFromProjectIdAsync(string projId)
        {
            try
            {
                List<Enrollment> items = await enrollmentTable.Where(x => x.ProjectId == projId).ToListAsync();

                return (items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e });
            }
            return null;
        }

        public async Task<List<Enrollment>> GetAllEnrollments()
        {
            try
            {
                List<Enrollment> enrollments = await enrollmentTable.ToListAsync();

                return enrollments;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return null;
        }

        public async Task<Enrollment> CheckEnrollmentAsync(string projectId, string developerId)
        {
            try
            {
                Enrollment enrollment = (await enrollmentTable.Where(x => x.ProjectId == projectId).Where(x => x.DeveloperId == developerId).ToEnumerableAsync()).FirstOrDefault();

                return enrollment;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return null;
        }

        /// <summary>
        /// Save Enrollment object
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task SaveEnrollmentAsync(Enrollment enrollment)
        {
            try
            {
                if (enrollment.Id == null)
                {
                    await enrollmentTable.InsertAsync(enrollment);
                }
                else
                {
                    await enrollmentTable.UpdateAsync(enrollment);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
            }
        }
    }
}
