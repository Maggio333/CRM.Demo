using MediatR;
using AutoMapper;
using System.Linq.Expressions;
using CRM.Demo.Domain.Customers.Entities;
using CRM.Demo.Domain.Customers.ValueObjects;
using CRM.Demo.Application.Customers.DTOs;
using CRM.Demo.Application.Customers.Interfaces;

namespace CRM.Demo.Application.Customers.Queries.SearchCustomers;

/// <summary>
/// Handler dla SearchCustomersQuery.
/// Używa Expression Trees do dynamicznego budowania zapytań.
/// </summary>
public class SearchCustomersQueryHandler : IRequestHandler<SearchCustomersQuery, List<CustomerDto>>
{
    private readonly ICustomerRepository _repository;
    private readonly IMapper _mapper;

    public SearchCustomersQueryHandler(
        ICustomerRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<CustomerDto>> Handle(
        SearchCustomersQuery request,
        CancellationToken cancellationToken)
    {
        // Pobierz wszystkich Customer (w Infrastructure zrobimy Expression Trees)
        var allCustomers = await _repository.GetAllAsync(cancellationToken);

        // Filtruj po kryteriach (tutaj prosty przykład, w Infrastructure będzie Expression Trees)
        var filteredCustomers = allCustomers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.CompanyName))
        {
            filteredCustomers = filteredCustomers.Where(c =>
                c.CompanyName.Contains(request.CompanyName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            filteredCustomers = filteredCustomers.Where(c =>
                c.Email.Value.Contains(request.Email, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.TaxId))
        {
            filteredCustomers = filteredCustomers.Where(c => c.TaxId == request.TaxId);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var status = CustomerStatus.FromString(request.Status);
            filteredCustomers = filteredCustomers.Where(c => c.Status == status);
        }

        var customers = filteredCustomers.ToList();
        return _mapper.Map<List<CustomerDto>>(customers);
    }
}
