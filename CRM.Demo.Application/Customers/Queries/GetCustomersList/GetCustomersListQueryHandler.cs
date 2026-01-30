using MediatR;
using AutoMapper;
using CRM.Demo.Application.Customers.DTOs;
using CRM.Demo.Application.Customers.Interfaces;

namespace CRM.Demo.Application.Customers.Queries.GetCustomersList;

/// <summary>
/// Handler dla GetCustomersListQuery.
/// Pobiera listę Customer z paginacją i mapuje na DTOs.
/// </summary>
public class GetCustomersListQueryHandler : IRequestHandler<GetCustomersListQuery, List<CustomerDto>>
{
    private readonly ICustomerRepository _repository;
    private readonly IMapper _mapper;

    public GetCustomersListQueryHandler(
        ICustomerRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<CustomerDto>> Handle(
        GetCustomersListQuery request,
        CancellationToken cancellationToken)
    {
        List<Domain.Customers.Entities.Customer> customers;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            customers = await _repository.SearchByCompanyNameAsync(
                request.SearchTerm,
                cancellationToken
            );
        }
        else
        {
            customers = await _repository.GetAllAsync(cancellationToken);
        }

        // Paginacja
        var paginatedCustomers = customers
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return _mapper.Map<List<CustomerDto>>(paginatedCustomers);
    }
}
