using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using DailyTrackerApp.Models;
using Microsoft.Data.SqlClient;

namespace DailyTrackerApp.Services
{
    public class DbService
    {
        private readonly string _connectionString =
"Server=localhost;Database=MyDailyRoutine;User Id=sa;Password=123456;TrustServerCertificate=True;";

        public async Task<List<TaskItem>> LoadTasksAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryAsync<TaskItem>("SELECT TaskId, TaskName, TimeSlot FROM TaskDefinition");
            return result.AsList();
        }

        public async Task<List<EvaluationItem>> LoadEvaluationsAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryAsync<EvaluationItem>("SELECT EvaluationId, Question, Category FROM EvaluationDefinition");
            return result.AsList();
        }

        public async Task<List<DailyLog>> LoadDailyLogsAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryAsync<DailyLog>("SELECT * FROM DailyLog");
            return result.AsList();
        }

        public async Task<int> SaveOrUpdateDailyLogAsync(DailyLog log)
        {
            using var conn = new SqlConnection(_connectionString);

            var existing = await conn.QueryFirstOrDefaultAsync<DailyLog>(
                "SELECT * FROM DailyLog WHERE LogDate = @LogDate",
                new { log.LogDate });

            if (existing != null)
            {
                log.LogId = existing.LogId;
                await conn.ExecuteAsync(@"
                    UPDATE DailyLog
                    SET TaskScore = @TaskScore,
                        EvaluationAvg = @EvaluationAvg,
                        CombinedScore = @CombinedScore
                    WHERE LogId = @LogId", log);
            }
            else
            {
                log.LogId = await conn.ExecuteScalarAsync<int>(@"
                    INSERT INTO DailyLog (LogDate, TaskScore, EvaluationAvg, CombinedScore)
                    OUTPUT INSERTED.LogId
                    VALUES (@LogDate, @TaskScore, @EvaluationAvg, @CombinedScore)", log);
            }

            return log.LogId;
        }

        public async Task SaveOrUpdateTaskLogAsync(int logId, int taskId, bool isCompleted)
        {
            using var conn = new SqlConnection(_connectionString);

            var existing = await conn.ExecuteScalarAsync<int?>(
                "SELECT TaskLogId FROM TaskLog WHERE LogId = @LogId AND TaskId = @TaskId",
                new { LogId = logId, TaskId = taskId });

            if (existing.HasValue)
            {
                await conn.ExecuteAsync(
                    "UPDATE TaskLog SET IsCompleted = @IsCompleted WHERE TaskLogId = @Id",
                    new { IsCompleted = isCompleted, Id = existing.Value });
            }
            else
            {
                await conn.ExecuteAsync(
                    "INSERT INTO TaskLog (LogId, TaskId, IsCompleted) VALUES (@LogId, @TaskId, @IsCompleted)",
                    new { LogId = logId, TaskId = taskId, IsCompleted = isCompleted });
            }
        }

        public async Task SaveOrUpdateEvaluationLogAsync(int logId, int evaluationId, int score)
        {
            using var conn = new SqlConnection(_connectionString);

            var existing = await conn.ExecuteScalarAsync<int?>(
                "SELECT EvalLogId FROM EvaluationLog WHERE LogId = @LogId AND EvaluationId = @EvaluationId",
                new { LogId = logId, EvaluationId = evaluationId });

            if (existing.HasValue)
            {
                await conn.ExecuteAsync(
                    "UPDATE EvaluationLog SET Score = @Score WHERE EvalLogId = @Id",
                    new { Score = score, Id = existing.Value });
            }
            else
            {
                await conn.ExecuteAsync(
                    "INSERT INTO EvaluationLog (LogId, EvaluationId, Score) VALUES (@LogId, @EvaluationId, @Score)",
                    new { LogId = logId, EvaluationId = evaluationId, Score = score });
            }
        }

        public async Task<List<TaskLog>> LoadTaskLogsAsync(int logId)
        {
            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryAsync<TaskLog>(
                "SELECT TaskId, IsCompleted FROM TaskLog WHERE LogId = @LogId",
                new { LogId = logId });
            return result.AsList();
        }

        public async Task<List<EvaluationLog>> LoadEvaluationLogsAsync(int logId)
        {
            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryAsync<EvaluationLog>(
                "SELECT EvaluationId, Score FROM EvaluationLog WHERE LogId = @LogId",
                new { LogId = logId });
            return result.AsList();
        }
    }
}
