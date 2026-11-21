namespace RestAPICore.Entities
{
    public abstract class BaseEntity
    {
        public const string DEFAULT_DELIM = ":";
        private string _namespace;
        private string entityName;

        /// <summary>
        /// Base Entity must be overrided to create Enitty class.
        /// `_namespace` argument can be passed different in development environment.
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="_namespace"></param>
        public BaseEntity(string entityName, string _namespace = "g")
        {
            this.entityName = entityName;
            this._namespace = _namespace;
        }

        /// <summary>
        /// Returns redis key to be used by inherited entity class.
        /// </summary>
        /// <param name="eVars"></param>
        /// <returns></returns>
        public string getRedisKey(string[] eVars)
        {
            string[] entityParts = new string[2 + eVars.Length];
            entityParts[0] = this._namespace;
            entityParts[1] = this.entityName;
            eVars.CopyTo(entityParts, 2);
            return String.Join(DEFAULT_DELIM, entityParts);
        }

    }
}
