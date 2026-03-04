using MediatR;

namespace CoreEngine.Application.Features.StatutoryRegisters.Commands.DeleteStatutoryRegister;

public record DeleteStatutoryRegisterCommand(Guid Id) : IRequest<Unit>;
