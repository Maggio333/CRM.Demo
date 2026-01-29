using MediatR;
using AutoMapper;
using CRM.Demo.Domain.Common;
using CRM.Demo.Application.Customers.DTOs;
using CRM.Demo.Application.Customers.Interfaces;

namespace CRM.Demo.Application.Customers.Queries.GetCustomerById;

/// <summary>
/// Handler dla GetCustomerByIdQuery.
/// Pobiera Customer po ID i mapuje na DTO.
/// </summary>
public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto, string>>
{
    private readonly ICustomerRepository _repository;
    private readonly IMapper _mapper;
    
    public GetCustomerByIdQueryHandler(
        ICustomerRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<Result<CustomerDto, string>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.CustomerId, cancellationToken);
        
        if (customer == null)
        {
            return Result<CustomerDto, string>.Failure(
                $"Customer with ID {request.CustomerId} not found"
            );
        }
        
        var customerDto = _mapper.Map<CustomerDto>(customer);
        return Result<CustomerDto, string>.Success(customerDto);
    }
}
