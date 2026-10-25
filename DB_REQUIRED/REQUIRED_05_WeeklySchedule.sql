-- 2026-10-25 v11 Deck I: Weekly Employee Schedule. Additive and idempotent; safe to run more than once.
-- The legacy schema has no place for PLANNED working hours (TimeSheetEntry / BreakTime hold actual punches),
-- so planned schedules get their own table. Nothing here changes or reads timesheet rows.
-- One row per business + employee + date. ScheduleType: 'Working' or 'DayOff' (times are NULL for DayOff).
IF OBJECT_ID('dbo.AnyminuteWeeklySchedule','U') IS NULL
CREATE TABLE dbo.AnyminuteWeeklySchedule(
    BusinessID int NOT NULL,
    EmployeeID int NOT NULL,
    WorkDate date NOT NULL,
    ScheduleType nvarchar(10) NOT NULL CONSTRAINT DF_AnyminuteWeeklySchedule_Type DEFAULT('Working'),
    StartTime time(0) NULL,
    EndTime time(0) NULL,
    BreakMinutes int NOT NULL CONSTRAINT DF_AnyminuteWeeklySchedule_Break DEFAULT(0),
    Notes nvarchar(200) NULL,
    UpdatedAtUTC datetime2 NOT NULL CONSTRAINT DF_AnyminuteWeeklySchedule_At DEFAULT(SYSUTCDATETIME()),
    UpdatedBy int NULL,
    CONSTRAINT PK_AnyminuteWeeklySchedule PRIMARY KEY(BusinessID,EmployeeID,WorkDate),
    CONSTRAINT CK_AnyminuteWeeklySchedule_Type CHECK (ScheduleType IN ('Working','DayOff')),
    CONSTRAINT CK_AnyminuteWeeklySchedule_Break CHECK (BreakMinutes BETWEEN 0 AND 600));
SELECT name FROM sys.tables WHERE name IN('AnyminuteWeeklySchedule');
