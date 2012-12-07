/*
 * Project data loader
 */

//#define DEBUG_ON

namespace CIS681.Fall2012.VDS.Data {
    public partial class Project {
        public static Project Current { get; set; }

        protected override void InitializingData() {
            base.InitializingData();
            InitTab();
        }
        protected override void InitializedData() {
            base.InitializedData();
            RefreshTab();

#if DEBUG_ON
            // see when is project updated
            DataUpdated += (s) => {
                System.Console.WriteLine("{0} project updated!", System.DateTime.Now.Millisecond);
            };
#endif
        }

        partial void InitTab();
        partial void RefreshTab();
    }
}
