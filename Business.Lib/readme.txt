Thanks for downloading this Business.Lib package.

	a:=====================lin2db.T4.tt=====================
	
	NamespaceName = "DataModel";
	DataContextName = "Connection";
	BaseDataContextClass = "LinqToDB.LinqToDBConnection";
	PluralizeDataContextPropertyNames = false;
	NormalizeNames = false;
	
	b:=====================appsettings.json=====================
	
	"AppSettings": {
		"ConnectionStrings": {
			"PostgreSQL": {
				"ConnectionString": "Server=MyServer;Database=MyDatabase;User Id=postgres;Password=TestPassword;port=5432;",
				"providerName": "PostgreSQL"
				}
		}
	}	
	
	c:=====================Startup(IConfiguration configuration)=====================
	
	LinqToDB.Data.DataConnection.DefaultSettings = new LinqToDB.LinqToDBSection(Configuration.GetSection("AppSettings").GetSection("ConnectionStrings").GetChildren().Select(c => new LinqToDB.ConnectionStringSettings { Name = c.Key, ConnectionString = c.GetValue<string>("ConnectionString"), ProviderName = c.GetValue<string>("ProviderName") }));
	
	d:=====================Definition=====================

	public class DataBase : Business.Data.DataBase<DataModel.Connection>
    {
		public static DataBase DB = new DataBase();

        static DataBase()
        {
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
            LinqToDB.Data.DataConnection.TurnTraceSwitchOn();
            LinqToDB.Data.DataConnection.OnTrace = c => { };
        }

        public override DataModel.Connection GetConnection([CallerMemberName] string callMethod = null) => new DataModel.Connection(LinqToDB.Data.DataConnection.DefaultSettings.DefaultConfiguration) { TraceMethod = callMethod };
    }
	