using Microsoft.EntityFrameworkCore.Migrations;

namespace RoomTemp.Data.Migrations
{
    public partial class AddsMissingIndexForTempReadingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsSqlServer())
            {
                migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_TempReading_SensorId_DeviceId_LocationId_TakenAt')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_TempReading_SensorId_DeviceId_LocationId_TakenAt] ON [dbo].[TempReading]
	(
		[SensorId] ASC,
		[DeviceId] ASC,
		[LocationId] ASC,
		[TakenAt] ASC
	)
	INCLUDE([Temperature]) WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
END");
            }

            if (migrationBuilder.IsSqlite())
            {
                migrationBuilder.Sql(
                    @"CREATE INDEX IF NOT EXISTS IX_TempReading_SensorId_DeviceId_LocationId_TakenAt ON TempReading(SensorId, DeviceId,LocationId, TakenAt);");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsSqlServer())
            {
                migrationBuilder.Sql(@"
IF EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_TempReading_SensorId_DeviceId_LocationId_TakenAt')
BEGIN
    DROP INDEX [IX_TempReading_SensorId_DeviceId_LocationId_TakenAt] ON [dbo].[TempReading];
END");
            }

            if (migrationBuilder.IsSqlite())
            {
                migrationBuilder.Sql(@"DROP INDEX IF EXISTS IX_TempReading_SensorId_DeviceId_LocationId_TakenAt;");
            }
        }
    }
}