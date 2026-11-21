using System.Data;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace RestAPICore.DB
{
    public class MySqlGDSDB
    {
        private readonly string _connectionString;
        private readonly List<MySqlParameter> _pendingParameters = new();

        public enum InstanceType
        {
            MysqlGdsDb = 0,
            EventLogMysql = 1
        }

        public MySqlGDSDB( IConfiguration configuration,bool isPooling = true, InstanceType instanceType = InstanceType.MysqlGdsDb)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            var settingName = instanceType switch
            {
                InstanceType.MysqlGdsDb => "MysqlGDSConnString",
                InstanceType.EventLogMysql => "MysqlEventLogConnString",
                _ => throw new ArgumentOutOfRangeException(nameof(instanceType), instanceType, "Instance not found/configured.")
            };

            var connectionString = configuration.GetConnectionString(settingName)
                ?? configuration[settingName]
                ?? throw new InvalidOperationException($"MySQL connection string '{settingName}' is not configured.");

            var builder = new MySqlConnectionStringBuilder(connectionString)
            {
                Pooling = isPooling
            };
            _connectionString = builder.ConnectionString;
        }

        public int ExecuteDML(string strSQL, CommandType cmdType, int intTimeout)
        {
            return ExecuteNonQuery(strSQL, cmdType, intTimeout, TakePendingParameters());
        }

        public DataSet ExecuteSelect(
            string strSQL,
            CommandType cmdType = CommandType.StoredProcedure,
            int intTimeout = 300)
        {
            return ExecuteDataSet(strSQL, cmdType, intTimeout, TakePendingParameters());
        }

        public DataSet execute(
            string strSQL,
            Dictionary<string, object> sqlParams,
            CommandType cmdType = CommandType.StoredProcedure,
            int intTimeout = 300)
        {
            ArgumentNullException.ThrowIfNull(sqlParams);

            var parameters = sqlParams
                .Select(param => CreateInputParameter("p_" + param.Key, param.Value))
                .ToList();

            return ExecuteDataSet(strSQL, cmdType, intTimeout, parameters);
        }

        public MySqlParameter AddOutputParameter(string strParamName, MySqlDbType type = MySqlDbType.Int32)
        {
            var parameter = new MySqlParameter("p_" + strParamName, type)
            {
                Direction = ParameterDirection.Output
            };
            AddPendingParameter(parameter);
            return parameter;
        }

        public void AddParameter(string strParamName, string strParamValue, int intParamSize = 0)
        {
            var parameter = new MySqlParameter("p_" + strParamName, MySqlDbType.VarChar)
            {
                Value = strParamValue ?? (object)DBNull.Value,
                Direction = ParameterDirection.Input
            };
            if (intParamSize > 0)
            {
                parameter.Size = intParamSize;
            }

            AddPendingParameter(parameter);
        }

        public void AddParameter(string strParamName, int intParamValue)
        {
            AddInputParameter(strParamName, MySqlDbType.Int32, intParamValue);
        }

        public void AddParameter(string strParamName, bool boolParamValue)
        {
            AddInputParameter(strParamName, MySqlDbType.Bit, boolParamValue);
        }

        public void AddParameter(string strParamName, decimal dclParamValue)
        {
            AddInputParameter(strParamName, MySqlDbType.Decimal, dclParamValue);
        }

        public void AddParameter(string strParamName, double dblParamValue)
        {
            AddInputParameter(strParamName, MySqlDbType.Double, dblParamValue);
        }

        public void AddParameter(string strParamName, DateTime DTParamValue)
        {
            AddInputParameter(strParamName, MySqlDbType.DateTime, DTParamValue);
        }

        public void AddParameter<T>(string strParamName, List<T> arrayParamValue)
        {
            AddInputParameter(strParamName, MySqlDbType.JSON, JsonSerializer.Serialize(arrayParamValue));
        }

        private int ExecuteNonQuery(
            string sql,
            CommandType commandType,
            int timeout,
            IEnumerable<MySqlParameter> parameters)
        {
            using var connection = new MySqlConnection(_connectionString);
            using var command = CreateCommand(connection, sql, commandType, timeout, parameters);
            connection.Open();
            return command.ExecuteNonQuery();
        }

        private DataSet ExecuteDataSet(
            string sql,
            CommandType commandType,
            int timeout,
            IEnumerable<MySqlParameter> parameters)
        {
            using var connection = new MySqlConnection(_connectionString);
            using var command = CreateCommand(connection, sql, commandType, timeout, parameters);
            using var adapter = new MySqlDataAdapter(command);
            var dataSet = new DataSet();
            connection.Open();
            adapter.Fill(dataSet);
            return dataSet;
        }

        private static MySqlCommand CreateCommand(
            MySqlConnection connection,
            string sql,
            CommandType commandType,
            int timeout,
            IEnumerable<MySqlParameter> parameters)
        {
            var command = new MySqlCommand(sql, connection)
            {
                CommandType = commandType,
                CommandTimeout = timeout
            };
            command.Parameters.AddRange(parameters.ToArray());
            return command;
        }

        private void AddInputParameter(string name, MySqlDbType type, object value)
        {
            AddPendingParameter(CreateInputParameter("p_" + name, value, type));
        }

        private static MySqlParameter CreateInputParameter(string name, object? value, MySqlDbType? type = null)
        {
            var parameter = type.HasValue
                ? new MySqlParameter(name, type.Value)
                : new MySqlParameter(name, MySqlDbType.VarChar);
            parameter.Value = value ?? DBNull.Value;
            parameter.Direction = ParameterDirection.Input;
            return parameter;
        }

        private void AddPendingParameter(MySqlParameter parameter)
        {
            lock (_pendingParameters)
            {
                _pendingParameters.Add(parameter);
            }
        }

        private List<MySqlParameter> TakePendingParameters()
        {
            lock (_pendingParameters)
            {
                var parameters = _pendingParameters.ToList();
                _pendingParameters.Clear();
                return parameters;
            }
        }
    }
}
