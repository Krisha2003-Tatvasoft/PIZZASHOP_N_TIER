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
        using var connection = _context.Database.GetDbConnection();

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



}
