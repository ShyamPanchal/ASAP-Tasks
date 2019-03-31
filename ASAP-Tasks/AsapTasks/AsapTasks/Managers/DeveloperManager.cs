/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342
 */
//#define OFFLINE_SYNC_ENABLED

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using AsapTasks.Data;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif

namespace AsapTasks.Managers
{
    public partial class DeveloperManager
    {
        static DeveloperManager defaultInstance = new DeveloperManager();
        MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Developer> devTable;
#else
        IMobileServiceTable<Developer> devTable;
#endif

        const string offlineDbPath = @"localstore.db";

        private DeveloperManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Developer>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.devTable = client.GetSyncTable<Developer>();
#else
            this.devTable = client.GetTable<Developer>();
#endif
        }

        public static DeveloperManager DefaultManager
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

        public MobileServiceClient CurrentClient
        {
            get { return client; }
        }

        public bool IsOfflineEnabled
        {
            get { return devTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Developer>; }
        }

        public async Task<ObservableCollection<Developer>> GetDevelopersAsync(bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                IEnumerable<Developer> items = await devTable
                    .ToEnumerableAsync();

                return new ObservableCollection<Developer>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e.Message });
            }
            return null;
        }

        public async Task<Developer> GetDeveloperAsync(string email, string password, bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                IEnumerable<Developer> items = await devTable
                    .Where(x => x.Email == email)
                    .Where(x => x.Password == password).ToEnumerableAsync();

                return items.FirstOrDefault();
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e.Message });
            }
            return null;
        }

        public async Task<Developer> GetDeveloperFromIdAsync(string developerId)
        {
            try
            {
                IEnumerable<Developer> items = await devTable
                    .Where(x => x.Id == developerId).ToEnumerableAsync();

                return items.FirstOrDefault();
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e.Message });
            }
            return null;
        }

        public async Task<Developer> CheckDeveloperEmailAsync(string email, bool syncItems = false)
        {
            try
            {
                IEnumerable<Developer> items = await devTable
                    .Where(x => x.Email == email).ToEnumerableAsync();

                if(items.Count() > 0)
                {
                    return items.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e.Message });
            }
            return null;
        }

        public async Task<Constants.DataEntryErrors> SaveDeveloperAsync(Developer item)
        {
            try
            {
                if (item.Id == null)
                {
                    IEnumerable<Developer> items = await devTable.Where(x => x.Email == item.Email).ToEnumerableAsync();
                    if (items.Count() == 0)
                    {
                        await devTable.InsertAsync(item);
                        return Constants.DataEntryErrors.SUCCESS;
                    }
                    else
                    {
                        return Constants.DataEntryErrors.INVALID;
                    }
                }
                else
                {
                    await devTable.UpdateAsync(item);
                    return Constants.DataEntryErrors.SUCCESS;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
                return Constants.DataEntryErrors.FAILURE;
            }
        }

        public async Task<Constants.DataCheckErrors> CheckEmailAsync(string email)
        {
            try
            {
                IEnumerable<Developer> items = await devTable
                    .Where(x => x.Email == email).ToEnumerableAsync();

                return (items.Count() > 0) ? Constants.DataCheckErrors.EXISTS : Constants.DataCheckErrors.DN_EXISTS;
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e.Message });
            }
            return Constants.DataCheckErrors.ERROR;
        }


#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.devTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allDevelopers",
                    this.devTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            // Simple error/conflict handling. A real application would handle the various errors like network conditions,
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        //Update failed, reverting to server's copy.
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change.
                        await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
#endif
    }
}
