using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace backend_api.Interceptors
{
    public class RemoveAliasInterceptor : DbCommandInterceptor
    {
        public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
        {
            // Remove " AS [Alias]" do SQL
            command.CommandText = Regex.Replace(command.CommandText, @"\sAS\s sss", " ", RegexOptions.IgnoreCase);
            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
        {
            command.CommandText = Regex.Replace(command.CommandText, @"\sAS\s sss", " ", RegexOptions.IgnoreCase);
            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }
    }
}
