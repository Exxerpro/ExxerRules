using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IndTrace.Domain.Entities;

namespace IndTrace.TestData.RawData;

/// <summary>
/// Static test data for Product entities with O(1) lookup.
/// Generated with ImmutableDictionary for thread-safety and performance.
/// </summary>
internal static class ProductRawData
{
    private static readonly ImmutableDictionary<int, Product> DictDict =
        new Dictionary<int, Product>
        {
            [1] = new Product
            {
                ProductId = 1,
                PartNumber = "DEFAULT",
                ProductName = "Default Test Product",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "TEST-001",
                AliasPartNumber = "TEST-001",
                Description = "Default product for test data",
                RuleId = 1,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2023, 8, 25, 12, 6, 30),
                CustomerName = "Test Customer",
                CustomerId = 1,
                LineId = 1
            },
            [508] = new Product
            {
                ProductId = 508,
                PartNumber = "L687508",
                ProductName = "L687508",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "Housing CHMSL Q5",
                AliasPartNumber = "Housing CHMSL Q5",
                Description = "Housing CHMSL Q5",
                RuleId = 508,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                ModifiedBy = "0",
                ModifiedOn = new DateTime(2023, 8, 25, 12, 6, 30),
                CustomerName = "Volkswagen",
                CustomerId = 1,
                LineId = 1
            },
            [566] = new Product
            {
                ProductId = 566,
                PartNumber = "L823566",
                ProductName = "L823566",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "Q5 Spoiler 2K Housing Assy",
                AliasPartNumber = "Q5 Spoiler 2K Housing Assy",
                Description = "Q5 Spoiler 2K Housing Assy",
                RuleId = 566,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                ModifiedBy = "0",
                ModifiedOn = new DateTime(2023, 8, 25, 12, 6, 30),
                CustomerName = "Volkswagen",
                CustomerId = 1,
                LineId = 1
            },
            [581] = new Product
            {
                ProductId = 581,
                PartNumber = "L823581",
                ProductName = "L823581",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "Q5 Spoiler PCB LED Assy",
                AliasPartNumber = "Q5 Spoiler PCB LED Assy",
                Description = "Q5 Spoiler PCB LED Assy",
                RuleId = 581,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                ModifiedBy = "0",
                ModifiedOn = new DateTime(2023, 8, 25, 12, 6, 30),
                CustomerName = "Volkswagen",
                CustomerId = 1,
                LineId = 1
            },
            [629] = new Product
            {
                ProductId = 629,
                PartNumber = "L90164629",
                ProductName = "L90164629",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "PCBA CHMSL Q5",
                AliasPartNumber = "PCBA CHMSL Q5",
                Description = "PCBA CHMSL Q5",
                RuleId = 629,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                ModifiedBy = "0",
                ModifiedOn = new DateTime(2023, 8, 25, 12, 6, 30),
                CustomerName = "Volkswagen",
                CustomerId = 1,
                LineId = 1
            },
            [630] = new Product
            {
                ProductId = 630,
                PartNumber = "422290",
                ProductName = "422290",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "422290",
                AliasPartNumber = "422290",
                Description = "Description",
                RuleId = 630,
                CreatedBy = "Fabian@exxerpro.com",
                CreatedOn = new DateTime(2023, 12, 4, 17, 9, 59),
                ModifiedBy = "",
                ModifiedOn = new DateTime(2023, 12, 4, 17, 9, 59),
                CustomerName = "Volkswagen",
                CustomerId = 1,
                LineId = 1
            },
            [631] = new Product
            {
                ProductId = 631,
                PartNumber = "422300",
                ProductName = "422300",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "422300",
                AliasPartNumber = "422300",
                Description = "Description",
                RuleId = 631,
                CreatedBy = "Fabian@exxerpro.com",
                CreatedOn = new DateTime(2023, 12, 4, 17, 10, 8),
                ModifiedBy = "",
                ModifiedOn = new DateTime(2023, 12, 4, 17, 10, 8),
                CustomerName = "Audi",
                CustomerId = 2,
                LineId = 1
            },
            [632] = new Product
            {
                ProductId = 632,
                PartNumber = "R150750",
                ProductName = "Ram Off-Road Bumper",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "R150750",
                AliasPartNumber = "Ram Bumper Heavy Duty",
                Description = "Heavy-duty off-road bumper for Ram trucks",
                RuleId = 1501,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 7, 22, 16, 10, 42),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 10, 42),
                CustomerName = "Audi",
                CustomerId = 2,
                LineId = 1
            },
            [633] = new Product
            {
                ProductId = 633,
                PartNumber = "R150751",
                ProductName = "Ram LED Headlights",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "R150751",
                AliasPartNumber = "Ram Headlights LED",
                Description = "High-performance LED headlights for enhanced visibility",
                RuleId = 1502,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 7, 22, 16, 10, 42),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 10, 42),
                CustomerName = "Audi",
                CustomerId = 2,
                LineId = 1
            },
            [634] = new Product
            {
                ProductId = 634,
                PartNumber = "R150752",
                ProductName = "Ram Performance Exhaust",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "R150752",
                AliasPartNumber = "Ram Exhaust System",
                Description = "Performance exhaust system for improved horsepower",
                RuleId = 1503,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 7, 22, 16, 10, 42),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 10, 42),
                CustomerName = "BMW",
                CustomerId = 3,
                LineId = 1
            },
            [635] = new Product
            {
                ProductId = 635,
                PartNumber = "R150753",
                ProductName = "Ram All-Terrain Tires",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "R150753",
                AliasPartNumber = "Ram Tires AT",
                Description = "Durable all-terrain tires for various driving conditions",
                RuleId = 1504,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 7, 22, 16, 10, 42),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 10, 42),
                CustomerName = "BMW",
                CustomerId = 3,
                LineId = 1
            },
            [636] = new Product
            {
                ProductId = 636,
                PartNumber = "T200500",
                ProductName = "Toyota Hybrid Battery Pack",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "T200500",
                AliasPartNumber = "Toyota Battery Pack Hybrid",
                Description = "High-efficiency hybrid battery pack for Toyota vehicles",
                RuleId = 2001,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 7, 22, 16, 11, 56),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 11, 56),
                CustomerName = "BMW",
                CustomerId = 3,
                LineId = 1
            },
            [637] = new Product
            {
                ProductId = 637,
                PartNumber = "T200501",
                ProductName = "Toyota Advanced Navigation System",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "T200501",
                AliasPartNumber = "Toyota Nav System",
                Description = "State-of-the-art navigation system with real-time updates",
                RuleId = 2002,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 7, 22, 16, 11, 56),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 11, 56),
                CustomerName = "Mercedes-Benz",
                CustomerId = 4,
                LineId = 1
            },
            [638] = new Product
            {
                ProductId = 638,
                PartNumber = "T200502",
                ProductName = "Toyota Eco-Friendly Tires",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "T200502",
                AliasPartNumber = "Toyota Tires Eco",
                Description = "Environmentally friendly tires designed for low rolling resistance",
                RuleId = 2003,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 7, 22, 16, 11, 56),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 11, 56),
                CustomerName = "Toyota",
                CustomerId = 5,
                LineId = 1
            },
            [639] = new Product
            {
                ProductId = 639,
                PartNumber = "T200503",
                ProductName = "Toyota High-Performance Brake Pads",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "T200503",
                AliasPartNumber = "Toyota Brake Pads",
                Description = "Durable and high-performance brake pads for enhanced safety",
                RuleId = 2004,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 7, 22, 16, 11, 56),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 11, 56),
                CustomerName = "Ford",
                CustomerId = 6,
                LineId = 1
            },
            [640] = new Product
            {
                ProductId = 640,
                PartNumber = "T200504",
                ProductName = "Toyota Smart Infotainment System",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "T200504",
                AliasPartNumber = "Toyota Infotainment",
                Description = "Next-gen infotainment system with touch screen and connectivity features",
                RuleId = 2005,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 7, 22, 16, 11, 56),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 11, 56),
                CustomerName = "Ford",
                CustomerId = 6,
                LineId = 1
            },
            [644] = new Product
            {
                ProductId = 644,
                PartNumber = "L90164629",
                ProductName = "L90164629",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "PCBA CHMSL Q5",
                AliasPartNumber = "PCBA CHMSL Q5",
                Description = "PCBA CHMSL Q5",
                RuleId = 629,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2023, 8, 25, 12, 6, 30),
                CustomerName = "Volkswagen",
                CustomerId = 1,
                LineId = 1
            },
            [645] = new Product
            {
                ProductId = 645,
                PartNumber = "L792511",
                ProductName = "L792511",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "L792511",
                AliasPartNumber = "L792511",
                Description = "L792511",
                RuleId = 511,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                CustomerName = "Audi",
                CustomerId = 2,
                LineId = 1
            },
            [646] = new Product
            {
                ProductId = 646,
                PartNumber = "L817618",
                ProductName = "L817618",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "L817618",
                AliasPartNumber = "L817618",
                Description = "L817618",
                RuleId = 618,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                CustomerName = "Audi",
                CustomerId = 2,
                LineId = 1
            },
            [647] = new Product
            {
                ProductId = 647,
                PartNumber = "L817630",
                ProductName = "L817630",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "L817630",
                AliasPartNumber = "L817630",
                Description = "L817630",
                RuleId = 630,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                CustomerName = "Audi",
                CustomerId = 2,
                LineId = 1
            },
            [648] = new Product
            {
                ProductId = 648,
                PartNumber = "L688753",
                ProductName = "L688753",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "L688753",
                AliasPartNumber = "L688753",
                Description = "L688753",
                RuleId = 753,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                CustomerName = "Audi",
                CustomerId = 2,
                LineId = 1
            },
            [649] = new Product
            {
                ProductId = 649,
                PartNumber = "L823566",
                ProductName = "L823566",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "Q5 Spoiler 2K Housing Assy",
                AliasPartNumber = "Q5 Spoiler 2K Housing Assy",
                Description = "Q5 Spoiler 2K Housing Assy",
                RuleId = 566,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2023, 8, 25, 12, 6, 30),
                CustomerName = "BMW",
                CustomerId = 3,
                LineId = 1
            },
            [650] = new Product
            {
                ProductId = 650,
                PartNumber = "431580",
                ProductName = "431580",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "431580",
                AliasPartNumber = "431580",
                Description = "Description",
                RuleId = 632,
                CreatedBy = "Fabian@exxerpro.com",
                CreatedOn = new DateTime(2024, 1, 9, 10, 51, 25),
                ModifiedBy = "Fabian@exxerpro.com",
                ModifiedOn = new DateTime(2024, 1, 9, 10, 51, 25),
                CustomerName = "BMW",
                CustomerId = 3,
                LineId = 1
            },
            [651] = new Product
            {
                ProductId = 651,
                PartNumber = "431581",
                ProductName = "431581",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "431581",
                AliasPartNumber = "431581",
                Description = "Description",
                RuleId = 1632,
                CreatedBy = "Fabian@exxerpro.com",
                CreatedOn = new DateTime(2024, 4, 12, 17, 10, 31),
                ModifiedBy = "Fabian@exxerpro.com",
                ModifiedOn = new DateTime(2024, 4, 12, 17, 10, 31),
                CustomerName = "Mercedes-Benz",
                CustomerId = 4,
                LineId = 1
            },
            [653] = new Product
            {
                ProductId = 653,
                PartNumber = "L90164629",
                ProductName = "L90164629",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "PCBA CHMSL Q5",
                AliasPartNumber = "PCBA CHMSL Q5",
                Description = "PCBA CHMSL Q5",
                RuleId = 11,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 24),
                ModifiedBy = "0",
                ModifiedOn = new DateTime(2023, 8, 25, 12, 6, 30),
                CustomerName = "Volkswagen",
                CustomerId = 1,
                LineId = 1
            },

            // Missing products referenced by RecipeRawData
            [643] = new Product
            {
                ProductId = 643,
                PartNumber = "P643TEST",
                ProductName = "Product 643 Test",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "TEST-643",
                AliasPartNumber = "TEST-643",
                Description = "Test Product 643 for Recipe References",
                RuleId = 2006,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 8, 28, 17, 59, 22),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 8, 28, 17, 59, 22),
                CustomerName = "Test Customer",
                CustomerId = 1,
                LineId = 1
            },
            [652] = new Product
            {
                ProductId = 652,
                PartNumber = "P652TEST",
                ProductName = "Product 652 Test",
                IsActive = 1,
                Version = 1,
                CustomerPartNumber = "TEST-652",
                AliasPartNumber = "TEST-652",
                Description = "Test Product 652 for Recipe References",
                RuleId = 2006,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 8, 28, 17, 59, 22),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 8, 28, 17, 59, 22),
                CustomerName = "Test Customer",
                CustomerId = 1,
                LineId = 1
            },
            // Duplicate entry [508] removed (keeping first occurrence)
            // Duplicate entry [653] removed (keeping first occurrence)

            // Duplicate entry [653] removed (keeping first occurrence)
            // Duplicate entry [508] removed (keeping first occurrence)

        }.ToImmutableDictionary();

    /// <summary>
    /// Static list for backward compatibility
    /// </summary>
    public static readonly List<Product> FixtureProducts = DictDict.Values.ToList();

    /// <summary>
    /// Sample products for static data strategy (serialized as JSON)
    /// </summary>
    public static string SampleProducts => System.Text.Json.JsonSerializer.Serialize(FixtureProducts);

    /// <summary>
    /// Get a specific Product by ID - O(1) lookup
    /// </summary>
    public static Product? GetProduct(int id) =>
        DictDict.TryGetValue(id, out var product) ? product : null;

    /// <summary>
    /// Get all Product entities
    /// </summary>
    public static IReadOnlyList<Product> GetProducts() => DictDict.Values.ToList();

    /// <summary>
    /// Get all Product entities
    /// </summary>
    public static IReadOnlyList<Product> Fixture => DictDict.Values.ToList();

    /// <summary>
    /// Direct dictionary access for advanced scenarios
    /// </summary>
    public static IImmutableDictionary<int, Product> Dict => DictDict;

    /// <summary>
    /// Check if a Product exists by ID
    /// </summary>
    public static bool Contains(int id) => DictDict.ContainsKey(id);

    /// <summary>
    /// Get count of Dict
    /// </summary>
    public static int Count => DictDict.Count;

    /// <summary>
    /// Get Product by PartNumber - O(n) operation
    /// </summary>
    public static Product? GetByPartNumber(string partNumber) =>
        DictDict.Values.FirstOrDefault(p => p.PartNumber == partNumber);
}
