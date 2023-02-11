using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Domain.Interfaces;

public interface IFileStorage
{
    void Save(string name, Stream ms);
    void Delete(string name);
}
