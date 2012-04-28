using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Linq;

namespace Rapido
{
    public static class DataAccess
    {
        public static List<PathTime> _pathTimes = new List<PathTime>();

        public static PathTime GetBestTimeForPath(String pathKey, TransportationMode mode = TransportationMode.Bike)
        {
            return _pathTimes
                    .Where(x => x.PathKey == pathKey && x.Mode == mode)
                    .OrderBy(x => x.TotalTime)
                    .FirstOrDefault();
        }

        public static PathTime GetBestTimeForPathByUser(String pathKey, String userName, TransportationMode mode = TransportationMode.Bike)
        {
            return _pathTimes
                        .Where(x => x.PathKey == pathKey && x.User == userName && x.Mode == mode)
                        .OrderBy(x => x.TotalTime)
                        .FirstOrDefault();

        }

        public static PathTime GetNextFastestTime(String pathKey, TimeSpan lastSplitTime, Int32 splitIndex, TransportationMode mode = TransportationMode.Bike)
        {
            try
            {
                return _pathTimes
                    .Where(x => x.PathKey == pathKey && x.SplitTimes[splitIndex] < lastSplitTime && x.Mode == mode)
                    .OrderByDescending(x => x.SplitTimes[splitIndex])
                    .FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public static PathTime GetNextSlowestTime(String pathKey, TimeSpan lastSplitTime, Int32 splitIndex, TransportationMode mode = TransportationMode.Bike)
        {
            try
            {
                return _pathTimes
                    .Where(x => x.PathKey == pathKey && x.SplitTimes[splitIndex] > lastSplitTime && x.Mode == mode)
                    .OrderBy(x => x.SplitTimes[splitIndex])
                    .FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }


        public static void PostPathTime(PathTime pathTime)
        {          
            _pathTimes.Add(pathTime);
            UpdateRanks();
        }

        private static void UpdateRanks()
        {
            var orderedTimes = _pathTimes.OrderBy(x => x.TotalTime).ToList();

            for (Int32 i = 0; i < orderedTimes.Count(); i++)
            {
                orderedTimes[i].Rank = i + 1;
            }
        }

        public static void Save()
        {
            #if WINDOWS_PHONE
            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();
#else
            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForDomain();
#endif

            // open isolated storage, and write the savefile.
            IsolatedStorageFileStream fs = null;
            using (fs = savegameStorage.CreateFile("SaveData.xml"))
            {
                if (fs != null)
                {
                    // just overwrite the existing info for this example.
                    var serializer = new XmlSerializer(typeof(PathTime[]));
                    serializer.Serialize(fs, _pathTimes.ToArray());
                }
            }
        }

        public static void Load()
        {
            // open isolated storage, and load data from the savefile if it exists.
#if WINDOWS_PHONE
            using (IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication())
#else
            using (IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForDomain())
#endif
            {
                if (savegameStorage.FileExists("SaveData.xml"))
                {
                    using (IsolatedStorageFileStream fs = savegameStorage.OpenFile("SaveData.xml", System.IO.FileMode.Open))
                    {
                        if (fs != null)
                        {
                            var serializer = new XmlSerializer(typeof(PathTime[]));
                            var paths = (PathTime[])serializer.Deserialize(fs);

                            _pathTimes.Clear();
                            _pathTimes.AddRange(paths);
                        }
                    }
                }
            }
        }
    }
}
