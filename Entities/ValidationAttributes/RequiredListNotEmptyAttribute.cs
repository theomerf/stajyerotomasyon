using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ValidationAttributes
{
    public class RequiredListNotEmptyAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is IList<string> list)
            {
                return list.Any();
            }
            return false;
        }
    }

}
