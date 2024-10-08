﻿using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Helpers;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Application.Services.Queries;

public class GetBorrowingQueryById : IRequest<BorrowingRecord>
{
    public long Id { get; set; }
}

public class GetBorrowingQueryHandler(IApplicationDbContext context, IRedisCacheService cacheService)
    : IRequestHandler<GetBorrowingQueryById, BorrowingRecord>
{
    private readonly TimeSpan _cacheDuration = CommonVariables.CacheExpirationTime;

    public async Task<BorrowingRecord> Handle(GetBorrowingQueryById request)
    {
        // step 1 => get cached borrowed records if available
        var cachedBorrowedRecord =
            await cacheService.GetCacheDataAsync<BorrowingRecord>($"BorrowingRecord_{request.Id}");
        if (cachedBorrowedRecord != null) return cachedBorrowedRecord;

        // step 2 => else get borrowed record from DB
        var borrowedRecord = await context.BorrowingRecords.FindAsync(request.Id) ??
                             throw new Exception("Borrowing not found");

        // step 3 => set cache in parallel call cacheBorrowingRecordAsync
        await cacheService.SetCacheDataAsync($"BorrowingRecord_{borrowedRecord.Id}", borrowedRecord);
        // step 4 => return borrowed record
        return borrowedRecord;
    }
}