namespace Verto.AssociateRow
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq.Expressions;
    using System.Text;
    using Rhino.Etl.Core;

    /// <summary>
    /// Associate a Row to a table using a class
    /// </summary>
    /// <typeparam name="T">the class to use</typeparam>
    public class AssociateRowTo<T> : IAssociateRowTo
    {
        private readonly string _table;
        private readonly IDictionary<string, string> _columns;
        private readonly IList<string> _useIdentityOnColumns;
        private readonly IDictionary<string, object> _defaults;

        private string _command;

        /// <summary>
        /// start the association
        /// </summary>
        /// <param name="table">the table which to apply the maps to</param>
        public AssociateRowTo(string table)
        {
            _table = table;
            _columns = new Dictionary<string, string>(5);
            _defaults = new Dictionary<string, object>();
            _useIdentityOnColumns = new List<string>(1);
        }

        /// <summary>
        /// map db column to the row column
        /// </summary>
        /// <param name="toColumn">the column in the DB</param>
        /// <param name="rowColumnName">the rows column name</param>
        public AssociateRowTo<T> Map(string toColumn, string rowColumnName)
        {
            _columns.Add(toColumn, rowColumnName);
            return this;
        }

        /// <summary>
        /// maps the db column to the row column
        /// </summary>
        /// <param name="member">the object property (where the property is the same as the target db column)</param>
        /// <param name="rowColumnName">the row's column</param>
        public AssociateRowTo<T> Map(Expression<Func<T, object>> member, string rowColumnName)
        {
            string name = GetMemeberName(member);
            _columns.Add(name, rowColumnName);
            return this;
        }

        /// <summary>
        /// maps the db column to the row column
        /// </summary>
        /// <param name="member">maps the target db column to the row's column, where they are both the same as the objects property name</param>
        public AssociateRowTo<T> Map(Expression<Func<T, object>> member)
        {
            string name = GetMemeberName(member);
            _columns.Add(name, name);
            return this;
        }

        /// <summary>
        /// This will use the last Scope_Identity to populate this field
        /// </summary>
        /// <param name="member">field to populate with Identity, only if the member is the same as the db column</param>
        public AssociateRowTo<T> UseIdentity(Expression<Func<T, object>> member)
        {
            string name = GetMemeberName(member);
            _useIdentityOnColumns.Add(name);
            return this;
        }

        /// <summary>
        /// This will use the last Scope_Identity to populate this field
        /// </summary>
        /// <param name="dbColName">field to populate with Identity</param>
        public AssociateRowTo<T> UseIdentity(string dbColName)
        {
            _useIdentityOnColumns.Add(dbColName);
            return this;
        }


        /// <summary>
        /// This will use the value provided
        /// </summary>
        /// <param name="member">field to set</param>
        /// <param name="value">the value to set it too</param>
        public AssociateRowTo<T> SetValue<TValue>(Expression<Func<T, TValue>> member, TValue value)
        {
            string name = GetMemeberName(member);
            _defaults.Add(name, value);
            return this;
        }

        /// <summary>
        /// This will use the value provided
        /// </summary>
        /// <param name="dbColName">field to set</param>
        /// <param name="value">the value to set it too</param>
        public AssociateRowTo<T> SetValue(string dbColName, object value)
        {
            _defaults.Add(dbColName, value);
            return this;
        }



        private string GetMemeberName<TValue>(Expression<Func<T, TValue>> member)
        {
            var body = member.Body;
            var unaryExpression = body as UnaryExpression;
            MemberExpression memberExpression = unaryExpression != null
                                                    ? (MemberExpression)unaryExpression.Operand
                                                    : (MemberExpression)body;

            return memberExpression.Member.Name;
        }



        /// <summary>
        /// use this to prepare the SQL command
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="cmd">sql command</param>
        public virtual void PrepareCommand(Row row, SqlCommand cmd)
        {
            if (string.IsNullOrEmpty(_command))
            {
                _command = CreateCommand();
            }

            cmd.CommandText = _command;

            AddParameters(row, cmd);
        }

        public virtual void AddParameters(Row row, SqlCommand cmd)
        {
            foreach (KeyValuePair<string, string> column in _columns)
            {
                cmd.Parameters.AddWithValue(string.Format("@{0}", column.Key), row[column.Value].ReturnDbValue());
            }

            foreach (KeyValuePair<string, object> @default in _defaults)
            {
                cmd.Parameters.AddWithValue(string.Format("@{0}", @default.Key), @default.Value);
            }
        }

        public virtual string CreateCommand()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("INSERT INTO [");
            builder.Append(_table);
            builder.Append("] (");

            int pos = 1;
            foreach (string key in _columns.Keys)
            {
                builder.Append(key);
                if (_columns.Count > pos)
                {
                    builder.Append(", ");
                }
                pos++;
            }

            if (_columns.Count > 0 && _defaults.Count > 0)
            {
                builder.Append(", ");
            }

            pos = 1;
            foreach (string key in _defaults.Keys)
            {
                builder.Append(key);
                if (_defaults.Count > pos)
                {
                    builder.Append(", ");
                }
                pos++;
            }

            foreach (string useIdentityOnColumn in _useIdentityOnColumns)
            {
                builder.Append(", ");
                builder.Append(useIdentityOnColumn);
            }

            builder.Append(") VALUES(");
            pos = 1;
            foreach (string key in _columns.Keys)
            {
                builder.Append("@");
                builder.Append(key);
                if (_columns.Count > pos)
                {
                    builder.Append(", ");
                }
                pos++;
            }

            if (_columns.Count > 0 && _defaults.Count > 0)
            {
                builder.Append(", ");
            }

            pos = 1;
            foreach (string key in _defaults.Keys)
            {
                builder.Append("@");
                builder.Append(key);
                if (_defaults.Count > pos)
                {
                    builder.Append(", ");
                }
                pos++;
            }
            for (int i = 0; i < _useIdentityOnColumns.Count; i++)
            {
                builder.Append(", ");
                builder.Append("SCOPE_IDENTITY()");
            }

            builder.Append(");");

            return builder.ToString();
        }
    }
}
