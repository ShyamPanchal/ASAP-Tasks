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
    public partial class ProjectManager
    {
        static ProjectManager defaultInstance = new ProjectManager();
        MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Project> projectTable;
#else
        IMobileServiceTable<Project> projectTable;
#endif
        const string offlineDbPath = @"localstore.db";

        /// <summary>
        /// Constructor
        /// </summary>
        private ProjectManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Project>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.projectTable = client.GetSyncTable<Project>();
#else
            this.projectTable = client.GetTable<Project>();
#endif
        }

        /// <summary>
        /// Singeton Variable
        /// </summary>
        public static ProjectManager DefaultManager
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
            get { return projectTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Project>; }
        }

        /// <summary>
        /// Get Project from Project Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="syncItems"></param>
        /// <returns>Project Object</returns>
        public async Task<Project> GetProjectFromIdAsync(string Id, bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                //get Project with that 
                IEnumerable<Project> items = await projectTable.Where(x => x.Id == Id).ToEnumerableAsync();


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
        /// Save Project object
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task SaveProjectAsync(Project project)
        {
            try
            {
                if (project.Id == null)
                {
                    await projectTable.InsertAsync(project);
                }
                else
                {
                    await projectTable.UpdateAsync(project);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
            }
        }
    }
}
