﻿namespace Product.Infrastructure.Data.EF.Idempotency;

public interface IRequestManager
{
    Task<bool> ExistAsync(Guid id);

    Task CreateRequestForCommandAsync<T>(Guid id);
}
