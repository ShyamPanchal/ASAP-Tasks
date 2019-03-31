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
    public partial class IssueManager
    {
        static IssueManager defaultInstance = new IssueManager();
        MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Issue> issueTable;
#else
        IMobileServiceTable<Issue> issueTable;
#endif
        const string offlineDbPath = @"localstore.db";

        /// <summary>
        /// Constructor
        /// </summary>
        private IssueManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Issue>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.issueTable = client.GetSyncTable<Issue>();
#else
            this.issueTable = client.GetTable<Issue>();
#endif
        }

        /// <summary>
        /// Singeton Variable
        /// </summary>
        public static IssueManager DefaultManager
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
            get { return issueTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Issue>; }
        }

        /// <summary>
        /// Get Issue from Issue Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="syncItems"></param>
        /// <returns>Issue Object</returns>
        public async Task<Issue> GetIssueFromIdAsync(string Id, bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                //get Issue with that 
                IEnumerable<Issue> items = await issueTable.Where(x => x.Id == Id).ToEnumerableAsync();


                return (items.FirstOrDefault());
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
        /// Get All Issues from Project Id
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="syncItems"></param>
        /// <returns>Issue Object</returns>
        public async Task<List<Issue>> GetIssueFromProjectIdAsync(string projId, bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                //get Issue with that 
                List<Issue> items = await issueTable.Where(x => x.ProjectId == projId).ToListAsync();


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
        /// Save Issue object
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task SaveIssueAsync(Issue issue)
        {
            try
            {
                if (issue.Id == null)
                {
                    await issueTable.InsertAsync(issue);
                }
                else
                {
                    await issueTable.UpdateAsync(issue);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
            }
        }
    }
}
