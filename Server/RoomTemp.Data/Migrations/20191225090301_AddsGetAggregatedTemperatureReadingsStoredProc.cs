using Microsoft.EntityFrameworkCore.Migrations;

namespace RoomTemp.Data.Migrations
{
    public partial class AddsGetAggregatedTemperatureReadingsStoredProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE [dbo].[sp_GetAggregatedTemperatureReadings]
(
	@LocationId int, 
	@DeviceId int, 
	@SensorId int, 
	@SearchStartDateTime Datetime2(7),
	@SearchEndDateTime Datetime2(7)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
    SET NOCOUNT ON
	SET ARITHABORT ON

	SELECT	DATEADD(hour, DATEPART(hour,[TakenAt]), CAST(CAST([TakenAt] as date) as datetime2)) as TakenAt, 
			ROUND(AVG([Temperature]), 2) as Temperature
	FROM [TempReading] AS [t]
	WHERE	[t].[DeviceId] = @DeviceId AND 
			[t].[LocationId] = @LocationId AND 
			[t].[SensorId] = @SensorId AND 
			[t].[TakenAt] >= @SearchStartDateTime AND 
			[t].[TakenAt] < @SearchEndDateTime
	GROUP BY CAST([TakenAt] as date), DATEPART(hour, [TakenAt])
	ORDER BY TakenAt
END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsSqlServer())
            {
                migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS [dbo].[sp_GetAggregatedTemperatureReadings];");
            }
        }
    }
}