using FluentMigrator;

namespace Ozon.Route256.Five.OrderService.Migrations;

[Migration(2)]
public class CreateIndexTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("index_region_name")
            .WithColumn("id").AsInt64().NotNullable()
            .WithColumn("name").AsString().NotNullable();

        Create.UniqueConstraint("unique_index_region_name")
            .OnTable("index_region_name")
            .Columns("id", "name");

        Create.Table("index_region_client")
            .WithColumn("region_id").AsInt64().NotNullable()
            .WithColumn("client_id").AsInt32().NotNullable();

        Create.UniqueConstraint("unique_index_region_client")
            .OnTable("index_region_client")
            .Columns("region_id", "client_id");

        Create.Table("index_region_order")
            .WithColumn("region_id").AsInt64().NotNullable()
            .WithColumn("order_id").AsInt64().NotNullable();

        Create.UniqueConstraint("unique_index_region_order")
            .OnTable("index_region_order")
            .Columns("region_id", "order_id");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}
