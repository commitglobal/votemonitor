﻿namespace Vote.Monitor.Api.Feature.NgoAdmin.Specifications;

public sealed class GetNgoAdminByLoginSpecification : Specification<NgoAdminAggregate>
{
    public GetNgoAdminByLoginSpecification(Guid ngoId, string login)
    {
        Query
            .Where(x => x.NgoId == ngoId && x.Login == login);
    }
}
