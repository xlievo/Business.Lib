Thanks for downloading this Business.Lib package.

	=====================lin2db.T4=====================
	
	NamespaceName = "DataModels";
	DataContextName = "Model";
	BaseDataContextClass = "LinqToDB.Entitys";
	PluralizeDataContextPropertyNames = false;
	
	=====================config=====================
	
	"AppSettings": {
		"ConnectionStrings": {
			"PostgreSQL": {
				"ConnectionString": "Server=192.168.1.121;Database=dd;User Id=root;Password=123456;port=5920;",
				"providerName": "PostgreSQL"
				}
		}
	}	
	
	=====================code=====================
	
	LinqToDB.Data.DataConnection.DefaultSettings = new LinqToDB.LinqToDBSection(Configuration.GetSection("AppSettings").GetSection("ConnectionStrings").GetChildren().Select(c => new LinqToDB.ConnectionStringSettings { Name = c.Key, ConnectionString = c.GetValue<string>("ConnectionString"), ProviderName = c.GetValue<string>("ProviderName") }));
	
	public static Business.Data.DB<DataConnection> DB = new Business.Data.DB<DataConnection>(() => new DataConnection(LinqToDB.Data.DataConnection.DefaultSettings.DefaultConfiguration));
	
	public class DataConnection : Business.Data.LinqToDBConnection<DataModels.Model>
    {
        static DataConnection() => LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        public DataConnection(string configuration) : base(configuration) { }
        public override DataModels.Model Entity { get => new DataModels.Model(this.ConfigurationString); }
    }
	