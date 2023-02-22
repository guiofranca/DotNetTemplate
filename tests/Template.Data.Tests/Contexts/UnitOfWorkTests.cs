using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Data.Contexts;
using Template.Data.Repositories;
using Template.Data.Tests.Repositories.Shared;
using Template.Data.Tests.Shared;

namespace Template.Data.Tests.Contexts;

public class UnitOfWorkTests : SQLiteDatabaseBuilder<UnitOfWorkTests>
{
	private readonly UnitOfWork _uow;
	public UnitOfWorkTests()
	{
		_uow = new UnitOfWork(_dbSession);
        _dbSession.Connection.Execute("""CREATE TABLE "UOW" ("Name" TEXT NOT NULL);""");
    }

	[Fact]
	public void Commit_FinishATransaction()
	{
		var expected = "123";
		_uow.BeginTransaction();
		_dbSession.Connection.Execute($"INSERT INTO UOW VALUES ('{expected}')");
		_uow.Commit();

		var actual = _dbSession.Connection.QueryFirst<string>("SELECT * FROM UOW");
		expected.Should().Be(actual);
	}

	[Fact]
	public void Rollback_UndoTheOperations()
	{
        _dbSession.Connection.Execute("INSERT INTO UOW VALUES ('123')");
        _dbSession.Connection.Execute("INSERT INTO UOW VALUES ('123')");
        _dbSession.Connection.Execute("INSERT INTO UOW VALUES ('123')");
        _uow.BeginTransaction();
        _dbSession.Connection.Execute("INSERT INTO UOW VALUES ('123')");
        _uow.Rollback();

        var expected = _dbSession.Connection.QueryFirst<int>("SELECT COUNT(1) FROM UOW");
        expected.Should().Be(3);
    }

	[Fact]
	public void BeginTransaction_BeginsATransaction()
	{
		_uow.BeginTransaction();
        _dbSession.Transaction.Should().NotBeNull();
	}

    [Fact]
    public void BeginTransaction_ShouldKeepSameTransactionWhenCalledMoreThanOnce()
    {
        _uow.BeginTransaction();
		var expected = _dbSession.Transaction.GetHashCode();
        _uow.BeginTransaction();
        var actual = _dbSession.Transaction.GetHashCode();

		expected.Should().Be(actual);
    }

	[Fact]
	public void Commit_ShouldDoNothingWhenThereIsNoTransaction()
	{
		_uow.Commit();
	}

    [Fact]
    public void Rollback_ShouldDoNothingWhenThereIsNoTransaction()
    {
        _uow.Rollback();
    }
}
