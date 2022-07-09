using System;
using System.Threading.Tasks;

namespace GGroupp.Infra;

internal static partial class HttpContextExtensions
{
    private static ValueTask<Unit> ToValueTask(this Task<Unit> task) => new(task);
}