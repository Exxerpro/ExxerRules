// <copyright file="CreateConfigStationValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigStations.Commands.Create;

/// <summary>
/// Represents the CreateConfigStationValidator.
/// </summary>
public class CreateConfigStationValidator : AbstractValidator<CreateConfigStationCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateConfigStationValidator"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public CreateConfigStationValidator()
    {
        this.RuleFor(v => v.ConfigId).NotEmpty().Length(10);
        this.RuleFor(v => v.MachineId).GreaterThan(0);
        this.RuleFor(v => v.Pc).GreaterThan(0);
        this.RuleFor(v => v.Client).NotEmpty().MaximumLength(100);
        this.RuleFor(v => v.Factorie).NotEmpty().MaximumLength(100);
        this.RuleFor(v => v.Line).NotEmpty().MaximumLength(100);
        this.RuleFor(v => v.Project).NotEmpty().MaximumLength(100);
        this.RuleFor(v => v.Version).NotEmpty().MaximumLength(50);
    }
}
