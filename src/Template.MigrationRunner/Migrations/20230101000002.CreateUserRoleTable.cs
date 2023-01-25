using FluentMigrator;
using Template.Data.Repositories.Shared;
using Template.Domain.Models;

namespace Template.MigrationRunner.Migrations;

[Migration(20230101000002)]
public class CreateUserRoleTable : Migration
{
    private const string TABLE_NAME = "user_role";
    public override void Up()
    {
        Create.Table(TABLE_NAME)
            .WithColumn("UserId").AsGuid()
            .WithColumn("RoleId").AsGuid()
            .WithColumn("CreatedAt").AsDateTime();

        Create.ForeignKey()
            .FromTable(TABLE_NAME).ForeignColumn("RoleId")
            .ToTable(TableName.Of<Role>()).PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey()
            .FromTable(TABLE_NAME).ForeignColumn("UserId")
            .ToTable(TableName.Of<User>()).PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index().OnTable(TABLE_NAME)
            .OnColumn("UserId").Ascending()
            .OnColumn("RoleId").Ascending();
    }

    public override void Down()
    {
        Delete.Table(TABLE_NAME);
    }
}
