﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.PaymentGateway.API.Abstractions
{
    public interface IPaymentProviderPlugin
    {
        string Name { get; }
        void ConfigureServices(IServiceCollection services);
        void Configure(IApplicationBuilder app);
    }
}
