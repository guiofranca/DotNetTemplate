using FluentMigrator;
using Template.Data.Repositories.Shared;
using Template.Core.Models;

namespace Template.MigrationRunner.Migrations;

[Migration(20230101000000)]
public class CreateUsersTable : Migration
{
    public override void Up()
    {
        Create.Table(TableName.Of<User>())
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Email").AsString().Unique()
            .WithColumn("Name").AsString()
            .WithColumn("Password").AsString()
            .WithColumn("Verified").AsBoolean()
            .WithColumn("CreatedAt").AsDateTime()
            .WithColumn("UpdatedAt").AsDateTime();
    }

    public override void Down()
    {
        Delete.Table(TableName.Of<User>());
    }
}
