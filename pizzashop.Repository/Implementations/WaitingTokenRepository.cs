using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class WaitingTokenRepository : IWaitingTokenRepository
{

    private readonly PizzashopContext _context;

    public WaitingTokenRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task AddNewWaitingToken(Waitingtoken token)
    {
        _context.Waitingtokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Waitingtoken>> GetWaitingList()
    {
        return await _context.Waitingtokens
            .Include(w => w.Customer)
            .Include(w => w.Section)
            .Where(w => w.Isassigned == false)
            .ToListAsync();
    }


    public async Task<List<Waitingtoken>> WaitingListBySectionId(int sectionId)
    {
        return await _context.Waitingtokens
            .Include(w => w.Customer)
            .Include(w => w.Section)
            .Where(w => w.Sectionid == sectionId && w.Isassigned == false)
            .ToListAsync();
    }


    public async Task<Waitingtoken> WTByIdAsync(int? id)
    {
        return await _context.Waitingtokens
        .Include(w => w.Customer)
        .Include(w => w.Section)
        .FirstOrDefaultAsync(wt => wt.Waitingtokenid == id);
    }

    public async Task UpdateWaitingToken(Waitingtoken waitingToken)
    {
        _context.Waitingtokens.Update(waitingToken);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Waitingtoken waitingToken)
    {
        _context.Waitingtokens.Remove(waitingToken);
        await _context.SaveChangesAsync();
    }

    public async Task<List<WaitingListTable>> GetWTTokenListFromSP(int sectionId)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var param = new { p_section_id = sectionId };

        var result = await connection.QueryAsync<WaitingListTable>(
            "SELECT * FROM get_waiting_list_by_section(@p_section_id);", param);

        return result.ToList();
    }

    public async Task<(bool Success, string Message)> AddWaitingTokenPostSP(AddWaitingToken model, int loginId)
    {
        bool isSuccess = false;
        string message = "Unknown error";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand("add_waitingtoken_logic", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        // Input parameters
        command.Parameters.AddWithValue("p_customername", model.Customername ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("p_phoneno", model.Phoneno ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("p_email", model.Email ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("p_noofperson", model.Noofperson);
        command.Parameters.AddWithValue("p_sectionid", model.Sectionid);
        command.Parameters.AddWithValue("p_createdby", loginId);

        // Output parameters
        var successParam = new NpgsqlParameter("success", NpgsqlDbType.Boolean)
        {
            Direction = ParameterDirection.Output
        };
        var messageParam = new NpgsqlParameter("message", NpgsqlDbType.Varchar)
        {
            Direction = ParameterDirection.Output,
            Size = 500
        };

        command.Parameters.Add(successParam);
        command.Parameters.Add(messageParam);

        // Optional: debug PostgreSQL notices
        connection.Notice += (sender, e) => Console.WriteLine("PG NOTICE: " + e.Notice.MessageText);

        await command.ExecuteNonQueryAsync();

        isSuccess = successParam.Value is bool b && b;
        message = messageParam.Value?.ToString() ?? "No message returned";

        return (isSuccess, message);
    }

    public async Task<AddWaitingToken> EditGetWTSP(int id)
    {
        using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var sql = "SELECT * FROM fn_edit_get_waitingtoken(@p_waitingtokenid);";

        var result = await connection.QueryFirstOrDefaultAsync<AddWaitingToken>(sql, new { p_waitingtokenid = id });

        return result;
    }

    public async Task<bool> DeleteWaitingTokenBySP(int id)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand("delete_waitingtoken_logic", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("p_waitingtokenid", id);

        var successParam = new NpgsqlParameter("success", NpgsqlDbType.Boolean)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);

        await command.ExecuteNonQueryAsync();

        return successParam.Value is bool b && b;
    }

    public async Task<bool> EditWaitingTokenPostSP(AddWaitingToken model, int loginId)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("sp_edit_waitingtoken", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("p_waitingtokenid", model.Waitingtokenid);
        command.Parameters.AddWithValue("p_customername", model.Customername ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("p_phoneno", model.Phoneno ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("p_email", model.Email ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("p_noofperson", model.Noofperson);
        command.Parameters.AddWithValue("p_sectionid", model.Sectionid);
        command.Parameters.AddWithValue("p_modifiedby", loginId);

        var successParam = new NpgsqlParameter("success", NpgsqlDbType.Boolean)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(successParam);

        await command.ExecuteNonQueryAsync();
        return successParam.Value is bool b && b;
    }

    public async Task<(int? OrderId, string Message)> AssignTablePostSP(int loginId, int waitingTokenId, List<int> tableIds)
    {
        using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var parameters = new DynamicParameters();
        parameters.Add("p_loginid", loginId);
        parameters.Add("p_waitingtokenid", waitingTokenId);
        parameters.Add("p_tableids", tableIds.ToArray());
        parameters.Add("o_orderid", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("o_message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

        await connection.ExecuteAsync("assign_table_post", parameters, commandType: CommandType.StoredProcedure);

        var orderId = parameters.Get<int?>("o_orderid");
        var message = parameters.Get<string>("o_message");

        return (orderId, message);
    }

    public async Task<List<WaitingListTable>> GetWTBySectionsfromSP(List<int> sectionIds)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var result = await connection.QueryAsync<WaitingListTable>(
            "SELECT * FROM get_waiting_list_by_sections(@section_ids);",
            new { section_ids = sectionIds?.ToArray() }
        );

        return result.ToList();
    }

    public async Task<(int? OrderId, string Message)> Assign_From_Tables_SP(int loginId, AssignTable model, List<int> tableIds){
        using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        var parameters = new DynamicParameters();
        parameters.Add("p_loginid", loginId);
        parameters.Add("p_waitingtokenid", model.Waitingtokenid); // can be null
        parameters.Add("p_email", model.Email);
        parameters.Add("p_customername", model.Customername);
        parameters.Add("p_phoneno", model.Phoneno);
        parameters.Add("p_noofperson", model.Noofperson);
        parameters.Add("p_tableids", tableIds.ToArray());

        parameters.Add("result_orderid", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("result_message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

        await connection.ExecuteAsync("assign_orderapp_table", parameters, commandType: CommandType.StoredProcedure);

        var orderId = parameters.Get<int?>("result_orderid");
        var message = parameters.Get<string>("result_message");

        return (orderId, message);
    }


}
