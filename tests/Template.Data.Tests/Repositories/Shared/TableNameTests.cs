using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Data.Repositories.Shared;

namespace Template.Data.Tests.Repositories.Shared;

public class TableNameTests
{
    [Fact]
    public void Of_ReturnsTheClassNameInLowercaseSuffixedWithS()
    {
        var expected = "TableNameTestss".ToLower();
        var actual = TableName.Of<TableNameTests>();
        actual.Should().Be(expected);
    }
}
