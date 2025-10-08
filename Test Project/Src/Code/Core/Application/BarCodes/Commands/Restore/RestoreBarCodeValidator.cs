// <copyright file="RestoreBarCodeValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Commands.Restore;

internal class RestoreBarCodeValidator : AbstractValidator<RestoreBarCodeCommand>
{
    public RestoreBarCodeValidator()
    {
        this.RuleFor(v => v.Label)
            .NotNull();
    }
}
