using NPOI.SS.Formula.Functions;
using System.ComponentModel.DataAnnotations;

namespace YardManagementApplication.Helpers
{
    public interface Ivalidator<T>
    {
        ValidationResult Validate(T instance);
    }
}
