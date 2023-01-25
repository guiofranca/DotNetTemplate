using FluentMigrator;
using Template.Data.Repositories.Shared;
using Template.Domain.Models;

namespace Template.MigrationRunner.Migrations;

[Migration(20230101000004)]
public class CreateBlogCommentTable : Migration
{
    public override void Up()
    {
        Create.Table(TableName.Of<BlogComment>())
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("UserId").AsGuid()
            .WithColumn("BlogPostId").AsGuid()
            .WithColumn("BlogCommentId").AsGuid().Nullable()
            .WithColumn("Content").AsString(500).NotNullable()
            .WithColumn("CreatedAt").AsDateTime()
            .WithColumn("UpdatedAt").AsDateTime();

        Create.ForeignKey()
            .FromTable(TableName.Of<BlogComment>()).ForeignColumn("UserId")
            .ToTable(TableName.Of<User>()).PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey()
            .FromTable(TableName.Of<BlogComment>()).ForeignColumn("BlogPostId")
            .ToTable(TableName.Of<BlogPost>()).PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey()
            .FromTable(TableName.Of<BlogComment>()).ForeignColumn("BlogCommentId")
            .ToTable(TableName.Of<BlogComment>()).PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
        Delete.Table(TableName.Of<BlogComment>());
    }
}
