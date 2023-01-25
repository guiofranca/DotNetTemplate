using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Domain.Exceptions;
using Template.Domain.Models;

namespace Template.Data.Repositories.Shared;

public static class TableName
{
    public static string Of<T>() where T : class
    {
        return typeof(T).Name switch
        {
            nameof(Chirp) => "chirps",
            _ => $"{typeof(T).Name.ToLower()}s",
        };
    }
}
