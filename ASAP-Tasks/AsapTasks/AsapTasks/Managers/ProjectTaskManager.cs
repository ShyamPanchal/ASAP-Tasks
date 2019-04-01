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
    public partial class ProjectTaskManager
    {
        static ProjectTaskManager defaultInstance = new ProjectTaskManager();
        MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<ProjectTask> projectTaskTable;
#else
        IMobileServiceTable<ProjectTask> projectTaskTable;
#endif
        const string offlineDbPath = @"localstore.db";

        /// <summary>
        /// Constructor
        /// </summary>
        private ProjectTaskManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<ProjectTask>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.projectTaskTable = client.GetSyncTable<ProjectTask>();
#else
            this.projectTaskTable = client.GetTable<ProjectTask>();
#endif
        }

        /// <summary>
        /// Singeton Variable
        /// </summary>
        public static ProjectTaskManager DefaultManager
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
            get { return projectTaskTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<ProjectTask>; }
        }

        /// <summary>
        /// Get ProjectTask from ProjectTask Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="syncItems"></param>
        /// <returns>ProjectTask Object</returns>
        public async Task<ProjectTask> GetProjectTaskFromIdAsync(string Id, bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                //get ProjectTask with that 
                IEnumerable<ProjectTask> items = await projectTaskTable.Where(x => x.Id == Id).ToEnumerableAsync();


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
        /// Get All ProjectTasks from Project Id
        /// </summary>
        /// <param name="enrollmentId"></param>
        /// <param name="syncItems"></param>
        /// <returns>ProjectTask Object</returns>
        public async Task<List<ProjectTask>> GetProjectTaskFromProjectIdAsync(string projectId, bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                //get ProjectTask with that 
                List<ProjectTask> items = await projectTaskTable.Where(x => x.ProjectId == projectId).ToListAsync();


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
        /// Save ProjectTask object
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task SaveProjectTaskAsync(ProjectTask projectTask)
        {
            try
            {
                if (projectTask.Id == null)
                {
                    await projectTaskTable.InsertAsync(projectTask);
                }
                else
                {
                    await projectTaskTable.UpdateAsync(projectTask);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
            }
        }

        public async Task DeleteAsync(ProjectTask task)
        {
            try
            {
                if (task.Id != null)
                {
                    await projectTaskTable.DeleteAsync(task);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
            }
        }
    }
}
