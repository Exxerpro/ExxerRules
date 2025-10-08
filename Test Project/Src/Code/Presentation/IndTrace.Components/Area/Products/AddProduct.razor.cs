using IndTrace.Application.ConfigStations.Queries.GetConfigStationList;
using IndTrace.Application.Machines.Queries.GetMachinesList;
using IndTrace.Application.Products.Queries.GetProductDetail;
using IndTrace.Application.WorkFlows.Dto;
using IndTrace.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace IndTrace.Components.Area.Products
{
    /// <summary>
    /// Provides functionality for adding new products to the system with workflow configuration.
    /// </summary>
    public partial class AddProduct
    {
        /// <summary>
        /// Gets or sets the result of the last operation performed.
        /// </summary>
        [Parameter]
        public Result? Result { get; set; }

        /// <summary>
        /// Gets or sets the callback function invoked when a product is successfully added.
        /// </summary>
        [Parameter]
        [Required]
        public Func<ProductDto, Task>? OnProductAdded { get; set; }

        /// <summary>
        /// Gets or sets the event callback for canceling the add operation.
        /// </summary>
        [Parameter]
        public EventCallback OnCancel { get; set; }

        /// <summary>
        /// Gets or sets the message to display to the user.
        /// </summary>
        [Parameter]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the application configuration containing system data.
        /// </summary>
        [Parameter]
        public ApplicationConfiguration? ApplicationConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the dictionary mapping machine IDs to their display names.
        /// </summary>
        [Parameter]
        public Dictionary<int, string> MachineNames { get; set; } = new();

        /// <summary>
        /// Gets or sets the product data transfer object.
        /// </summary>
        [Parameter]
        public ProductDto ProductDto { get; set; } = default!;

        /// <summary>
        /// Gets or sets the list of workflow data transfer objects.
        /// </summary>
        [Parameter]
        public List<WorkFlowDto> WorkFlowsDto { get; set; } = new();

        /// <summary>
        /// Gets or sets the callback function executed when workflow configuration is complete.
        /// </summary>
        [Parameter]
        public Func<List<MachineDto>, Task>? OnConfigured { get; set; }

        /// <summary>
        /// Gets or sets the currently selected customer name.
        /// </summary>
        [Parameter]
        public string SelectedCustomer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the save operation is enabled.
        /// </summary>
        public bool SaveEnabled { get; set; }

        /// <summary>
        /// Gets or sets the new product being created.
        /// </summary>
        [Parameter]
        public ProductDto? NewProduct { get; set; }

        /// <summary>
        /// Cancels the add product operation and clears the new product data.
        /// </summary>
        public void CancelAddProduct()
        {
            this.NewProduct = null;
            this.OnCancel.InvokeAsync();
        }

        /// <summary>
        /// Saves the new product model to the system.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveNewModel()
        {
            if (this.NewProduct is null || this.OnProductAdded is null)
            {
                return;
            }

            this.logger.LogInformation("Adding new product {product} {Customer} ", this.NewProduct.PartNumber, this.NewProduct.CustomerName);

            this.NewProduct.Customer.Name = this.NewProduct.CustomerName;

            this.NewProduct.Customer.CustomerId = this.ApplicationConfiguration!.Customers
                .Where(e => e.Name == this.NewProduct.Customer.Name).
                Select(e => e.CustomerId).SingleOrDefault();

            var customer = this.ApplicationConfiguration!.Customers
                .FirstOrDefault(e => e.CustomerId == this.NewProduct.Customer.CustomerId);

            if (customer is null)
            {
                throw new InvalidOperationException($"Customer with ID {this.NewProduct.Customer.CustomerId} not found in configuration.");
            }

            this.NewProduct.Customer.Name = customer.Name;

            if (this.OnProductAdded is not null && this.NewProduct is not null)
            {
                await this.OnProductAdded.Invoke(this.NewProduct);
            }
            if (this.Result is not null)
            {
                if (this.Result.IsSuccess)
                {
                    this.NewProduct = null;
                    this.Message = "Product added successfully";
                }
                else
                {
                    this.Message = $"Product not added Error {this.Result.Errors.FirstOrDefault()}";
                }
            }
        }

        /// <summary>
        /// Triggers the workflow configuration dialog for the new product.
        /// </summary>
        private async Task TriggerConfigureWorkflow()
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                NoHeader = false,
                FullWidth = true,
                MaxWidth = MaxWidth.ExtraLarge,
                CloseButton = true,
                Position = DialogPosition.Center,
            };

            var editorParams = new DialogParameters
            {
                { "ApplicationConfiguration", this.ApplicationConfiguration },
                { "MachineNames", this.MachineNames },
                { "ListProductsDto", this.NewProduct is not null ? new List<ProductDto> { this.NewProduct } : new List<ProductDto>() },
                { "ProductDto", this.NewProduct },
                { "ListWorkFlowsDto", this.WorkFlowsDto },
                { "SelectedCustomer", this.NewProduct?.CustomerName ?? string.Empty },
                { "OnConfigured"  , new EventCallback(this, OnConfiguredCallBack) },
            };

            if (this.NewProduct is null)
            {
                this.logger.LogWarning("No product available to configure workflow.");
                return;
            }

            var result = await this.dialogService.ShowAsync<ProductWorkflowEdit>($"Configure: {this.NewProduct.ProductName}", editorParams, options);

            if (this.Result is not null)
            {
                if (this.Result.IsSuccess)
                {
                    this.NewProduct = null;
                    this.Message = "Product added successfully";
                }
                else
                {
                    this.Message = $"Product not added Error {this.Result.Errors.FirstOrDefault()}";
                }
            }
        }

        /// <summary>
        /// Handles the callback when workflow configuration is completed.
        /// </summary>
        /// <param name="machines">The list of configured machines.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task OnConfiguredCallBack(List<MachineDto> machines)
        {
            foreach (var machine in machines)
            {
                this.logger.LogInformation("Machine {Name} selected", machine.Name);
            }

            var machinesWorkFlow = machines.Select(m => m.MachineId).ToList();
            if (this.NewProduct is null)
            {
                this.logger.LogWarning("No product available to configure workflow.");
                return Task.CompletedTask;
            }
            this.NewProduct.Machines = machinesWorkFlow;
            this.SaveEnabled = true;
            this.InvokeAsync(this.StateHasChanged);

            return Task.CompletedTask;
        }
    }
}
