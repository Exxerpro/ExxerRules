using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IndTrace.Domain.Entities;

namespace IndTrace.TestData.RawData;

/// <summary>
/// Static test data for Recipe entities with O(1) lookup.
/// Generated with ImmutableDictionary for thread-safety and performance.
/// Implements lazy-loaded Dict for best of both worlds: O(1) lookups + List compatibility.
/// Contains all 234 recipes from Recipes.json for complete test coverage.
/// </summary>
internal static class RecipeRawData
{
    /// <summary>
    /// Recipe test data - complete set of 234 recipes
    /// </summary>
    private static readonly ImmutableDictionary<int, Recipe> _recipesDict =
        new Dictionary<int, Recipe>
        {
            [1] = new Recipe
            {
                RecipeId = 1,
                ProductId = 566,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [2] = new Recipe
            {
                RecipeId = 2,
                ProductId = 566,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [3] = new Recipe
            {
                RecipeId = 3,
                ProductId = 566,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [4] = new Recipe
            {
                RecipeId = 4,
                ProductId = 566,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [5] = new Recipe
            {
                RecipeId = 5,
                ProductId = 566,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [6] = new Recipe
            {
                RecipeId = 6,
                ProductId = 566,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [7] = new Recipe
            {
                RecipeId = 7,
                ProductId = 566,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [8] = new Recipe
            {
                RecipeId = 8,
                ProductId = 566,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [9] = new Recipe
            {
                RecipeId = 9,
                ProductId = 566,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [10] = new Recipe
            {
                RecipeId = 10,
                ProductId = 581,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [11] = new Recipe
            {
                RecipeId = 11,
                ProductId = 581,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [12] = new Recipe
            {
                RecipeId = 12,
                ProductId = 581,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [13] = new Recipe
            {
                RecipeId = 13,
                ProductId = 581,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [14] = new Recipe
            {
                RecipeId = 14,
                ProductId = 581,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [15] = new Recipe
            {
                RecipeId = 15,
                ProductId = 581,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [16] = new Recipe
            {
                RecipeId = 16,
                ProductId = 581,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [17] = new Recipe
            {
                RecipeId = 17,
                ProductId = 581,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [18] = new Recipe
            {
                RecipeId = 18,
                ProductId = 581,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [19] = new Recipe
            {
                RecipeId = 19,
                ProductId = 508,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [20] = new Recipe
            {
                RecipeId = 20,
                ProductId = 508,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [21] = new Recipe
            {
                RecipeId = 21,
                ProductId = 508,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [22] = new Recipe
            {
                RecipeId = 22,
                ProductId = 508,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [23] = new Recipe
            {
                RecipeId = 23,
                ProductId = 508,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [24] = new Recipe
            {
                RecipeId = 24,
                ProductId = 508,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [25] = new Recipe
            {
                RecipeId = 25,
                ProductId = 508,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [26] = new Recipe
            {
                RecipeId = 26,
                ProductId = 508,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [27] = new Recipe
            {
                RecipeId = 27,
                ProductId = 508,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [28] = new Recipe
            {
                RecipeId = 28,
                ProductId = 629,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [29] = new Recipe
            {
                RecipeId = 29,
                ProductId = 629,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [30] = new Recipe
            {
                RecipeId = 30,
                ProductId = 629,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [31] = new Recipe
            {
                RecipeId = 31,
                ProductId = 629,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [32] = new Recipe
            {
                RecipeId = 32,
                ProductId = 629,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [33] = new Recipe
            {
                RecipeId = 33,
                ProductId = 629,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [34] = new Recipe
            {
                RecipeId = 34,
                ProductId = 629,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [35] = new Recipe
            {
                RecipeId = 35,
                ProductId = 629,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [36] = new Recipe
            {
                RecipeId = 36,
                ProductId = 629,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [37] = new Recipe
            {
                RecipeId = 37,
                ProductId = 630,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [38] = new Recipe
            {
                RecipeId = 38,
                ProductId = 630,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [39] = new Recipe
            {
                RecipeId = 39,
                ProductId = 630,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [40] = new Recipe
            {
                RecipeId = 40,
                ProductId = 630,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [41] = new Recipe
            {
                RecipeId = 41,
                ProductId = 630,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [42] = new Recipe
            {
                RecipeId = 42,
                ProductId = 630,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [43] = new Recipe
            {
                RecipeId = 43,
                ProductId = 630,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [44] = new Recipe
            {
                RecipeId = 44,
                ProductId = 630,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [45] = new Recipe
            {
                RecipeId = 45,
                ProductId = 630,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [46] = new Recipe
            {
                RecipeId = 46,
                ProductId = 631,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [47] = new Recipe
            {
                RecipeId = 47,
                ProductId = 631,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [48] = new Recipe
            {
                RecipeId = 48,
                ProductId = 631,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [49] = new Recipe
            {
                RecipeId = 49,
                ProductId = 631,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [50] = new Recipe
            {
                RecipeId = 50,
                ProductId = 631,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [51] = new Recipe
            {
                RecipeId = 51,
                ProductId = 631,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [52] = new Recipe
            {
                RecipeId = 52,
                ProductId = 631,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [53] = new Recipe
            {
                RecipeId = 53,
                ProductId = 631,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [54] = new Recipe
            {
                RecipeId = 54,
                ProductId = 631,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [55] = new Recipe
            {
                RecipeId = 55,
                ProductId = 632,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [56] = new Recipe
            {
                RecipeId = 56,
                ProductId = 632,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [57] = new Recipe
            {
                RecipeId = 57,
                ProductId = 632,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [58] = new Recipe
            {
                RecipeId = 58,
                ProductId = 632,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [59] = new Recipe
            {
                RecipeId = 59,
                ProductId = 632,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [60] = new Recipe
            {
                RecipeId = 60,
                ProductId = 632,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [61] = new Recipe
            {
                RecipeId = 61,
                ProductId = 632,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [62] = new Recipe
            {
                RecipeId = 62,
                ProductId = 632,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [63] = new Recipe
            {
                RecipeId = 63,
                ProductId = 632,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [64] = new Recipe
            {
                RecipeId = 64,
                ProductId = 633,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [65] = new Recipe
            {
                RecipeId = 65,
                ProductId = 633,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [66] = new Recipe
            {
                RecipeId = 66,
                ProductId = 633,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [67] = new Recipe
            {
                RecipeId = 67,
                ProductId = 633,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [68] = new Recipe
            {
                RecipeId = 68,
                ProductId = 633,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [69] = new Recipe
            {
                RecipeId = 69,
                ProductId = 633,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [70] = new Recipe
            {
                RecipeId = 70,
                ProductId = 633,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [71] = new Recipe
            {
                RecipeId = 71,
                ProductId = 633,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [72] = new Recipe
            {
                RecipeId = 72,
                ProductId = 633,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [73] = new Recipe
            {
                RecipeId = 73,
                ProductId = 634,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [74] = new Recipe
            {
                RecipeId = 74,
                ProductId = 634,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [75] = new Recipe
            {
                RecipeId = 75,
                ProductId = 634,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [76] = new Recipe
            {
                RecipeId = 76,
                ProductId = 634,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [77] = new Recipe
            {
                RecipeId = 77,
                ProductId = 634,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [78] = new Recipe
            {
                RecipeId = 78,
                ProductId = 634,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [79] = new Recipe
            {
                RecipeId = 79,
                ProductId = 634,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [80] = new Recipe
            {
                RecipeId = 80,
                ProductId = 634,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [81] = new Recipe
            {
                RecipeId = 81,
                ProductId = 634,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [82] = new Recipe
            {
                RecipeId = 82,
                ProductId = 635,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [83] = new Recipe
            {
                RecipeId = 83,
                ProductId = 635,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [84] = new Recipe
            {
                RecipeId = 84,
                ProductId = 635,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [85] = new Recipe
            {
                RecipeId = 85,
                ProductId = 635,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [86] = new Recipe
            {
                RecipeId = 86,
                ProductId = 635,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [87] = new Recipe
            {
                RecipeId = 87,
                ProductId = 635,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [88] = new Recipe
            {
                RecipeId = 88,
                ProductId = 635,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [89] = new Recipe
            {
                RecipeId = 89,
                ProductId = 635,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [90] = new Recipe
            {
                RecipeId = 90,
                ProductId = 635,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [91] = new Recipe
            {
                RecipeId = 91,
                ProductId = 636,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [92] = new Recipe
            {
                RecipeId = 92,
                ProductId = 636,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [93] = new Recipe
            {
                RecipeId = 93,
                ProductId = 636,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [94] = new Recipe
            {
                RecipeId = 94,
                ProductId = 636,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [95] = new Recipe
            {
                RecipeId = 95,
                ProductId = 636,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [96] = new Recipe
            {
                RecipeId = 96,
                ProductId = 636,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [97] = new Recipe
            {
                RecipeId = 97,
                ProductId = 636,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [98] = new Recipe
            {
                RecipeId = 98,
                ProductId = 636,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [99] = new Recipe
            {
                RecipeId = 99,
                ProductId = 636,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [100] = new Recipe
            {
                RecipeId = 100,
                ProductId = 637,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [101] = new Recipe
            {
                RecipeId = 101,
                ProductId = 637,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [102] = new Recipe
            {
                RecipeId = 102,
                ProductId = 637,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [103] = new Recipe
            {
                RecipeId = 103,
                ProductId = 637,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [104] = new Recipe
            {
                RecipeId = 104,
                ProductId = 637,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [105] = new Recipe
            {
                RecipeId = 105,
                ProductId = 637,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [106] = new Recipe
            {
                RecipeId = 106,
                ProductId = 637,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [107] = new Recipe
            {
                RecipeId = 107,
                ProductId = 637,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [108] = new Recipe
            {
                RecipeId = 108,
                ProductId = 637,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [109] = new Recipe
            {
                RecipeId = 109,
                ProductId = 638,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [110] = new Recipe
            {
                RecipeId = 110,
                ProductId = 638,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [111] = new Recipe
            {
                RecipeId = 111,
                ProductId = 638,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [112] = new Recipe
            {
                RecipeId = 112,
                ProductId = 638,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [113] = new Recipe
            {
                RecipeId = 113,
                ProductId = 638,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [114] = new Recipe
            {
                RecipeId = 114,
                ProductId = 638,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [115] = new Recipe
            {
                RecipeId = 115,
                ProductId = 638,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [116] = new Recipe
            {
                RecipeId = 116,
                ProductId = 638,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [117] = new Recipe
            {
                RecipeId = 117,
                ProductId = 638,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [118] = new Recipe
            {
                RecipeId = 118,
                ProductId = 639,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [119] = new Recipe
            {
                RecipeId = 119,
                ProductId = 639,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [120] = new Recipe
            {
                RecipeId = 120,
                ProductId = 639,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [121] = new Recipe
            {
                RecipeId = 121,
                ProductId = 639,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [122] = new Recipe
            {
                RecipeId = 122,
                ProductId = 639,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [123] = new Recipe
            {
                RecipeId = 123,
                ProductId = 639,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [124] = new Recipe
            {
                RecipeId = 124,
                ProductId = 639,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [125] = new Recipe
            {
                RecipeId = 125,
                ProductId = 639,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [126] = new Recipe
            {
                RecipeId = 126,
                ProductId = 639,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [127] = new Recipe
            {
                RecipeId = 127,
                ProductId = 640,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [128] = new Recipe
            {
                RecipeId = 128,
                ProductId = 640,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [129] = new Recipe
            {
                RecipeId = 129,
                ProductId = 640,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [130] = new Recipe
            {
                RecipeId = 130,
                ProductId = 640,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [131] = new Recipe
            {
                RecipeId = 131,
                ProductId = 640,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [132] = new Recipe
            {
                RecipeId = 132,
                ProductId = 640,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [133] = new Recipe
            {
                RecipeId = 133,
                ProductId = 640,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [134] = new Recipe
            {
                RecipeId = 134,
                ProductId = 640,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [135] = new Recipe
            {
                RecipeId = 135,
                ProductId = 640,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [136] = new Recipe
            {
                RecipeId = 136,
                ProductId = 643,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [137] = new Recipe
            {
                RecipeId = 137,
                ProductId = 643,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [138] = new Recipe
            {
                RecipeId = 138,
                ProductId = 643,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [139] = new Recipe
            {
                RecipeId = 139,
                ProductId = 643,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [140] = new Recipe
            {
                RecipeId = 140,
                ProductId = 643,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [141] = new Recipe
            {
                RecipeId = 141,
                ProductId = 643,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [142] = new Recipe
            {
                RecipeId = 142,
                ProductId = 643,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [143] = new Recipe
            {
                RecipeId = 143,
                ProductId = 643,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [144] = new Recipe
            {
                RecipeId = 144,
                ProductId = 643,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [145] = new Recipe
            {
                RecipeId = 145,
                ProductId = 644,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [146] = new Recipe
            {
                RecipeId = 146,
                ProductId = 644,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [147] = new Recipe
            {
                RecipeId = 147,
                ProductId = 644,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [148] = new Recipe
            {
                RecipeId = 148,
                ProductId = 644,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [149] = new Recipe
            {
                RecipeId = 149,
                ProductId = 644,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [150] = new Recipe
            {
                RecipeId = 150,
                ProductId = 644,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [151] = new Recipe
            {
                RecipeId = 151,
                ProductId = 644,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [152] = new Recipe
            {
                RecipeId = 152,
                ProductId = 644,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [153] = new Recipe
            {
                RecipeId = 153,
                ProductId = 644,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [154] = new Recipe
            {
                RecipeId = 154,
                ProductId = 645,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [155] = new Recipe
            {
                RecipeId = 155,
                ProductId = 645,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [156] = new Recipe
            {
                RecipeId = 156,
                ProductId = 645,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [157] = new Recipe
            {
                RecipeId = 157,
                ProductId = 645,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [158] = new Recipe
            {
                RecipeId = 158,
                ProductId = 645,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [159] = new Recipe
            {
                RecipeId = 159,
                ProductId = 645,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [160] = new Recipe
            {
                RecipeId = 160,
                ProductId = 645,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [161] = new Recipe
            {
                RecipeId = 161,
                ProductId = 645,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [162] = new Recipe
            {
                RecipeId = 162,
                ProductId = 645,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [163] = new Recipe
            {
                RecipeId = 163,
                ProductId = 646,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [164] = new Recipe
            {
                RecipeId = 164,
                ProductId = 646,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [165] = new Recipe
            {
                RecipeId = 165,
                ProductId = 646,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [166] = new Recipe
            {
                RecipeId = 166,
                ProductId = 646,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [167] = new Recipe
            {
                RecipeId = 167,
                ProductId = 646,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [168] = new Recipe
            {
                RecipeId = 168,
                ProductId = 646,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [169] = new Recipe
            {
                RecipeId = 169,
                ProductId = 646,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [170] = new Recipe
            {
                RecipeId = 170,
                ProductId = 646,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [171] = new Recipe
            {
                RecipeId = 171,
                ProductId = 646,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [172] = new Recipe
            {
                RecipeId = 172,
                ProductId = 647,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [173] = new Recipe
            {
                RecipeId = 173,
                ProductId = 647,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [174] = new Recipe
            {
                RecipeId = 174,
                ProductId = 647,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [175] = new Recipe
            {
                RecipeId = 175,
                ProductId = 647,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [176] = new Recipe
            {
                RecipeId = 176,
                ProductId = 647,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [177] = new Recipe
            {
                RecipeId = 177,
                ProductId = 647,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [178] = new Recipe
            {
                RecipeId = 178,
                ProductId = 647,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [179] = new Recipe
            {
                RecipeId = 179,
                ProductId = 647,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [180] = new Recipe
            {
                RecipeId = 180,
                ProductId = 647,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [181] = new Recipe
            {
                RecipeId = 181,
                ProductId = 648,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [182] = new Recipe
            {
                RecipeId = 182,
                ProductId = 648,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [183] = new Recipe
            {
                RecipeId = 183,
                ProductId = 648,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [184] = new Recipe
            {
                RecipeId = 184,
                ProductId = 648,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [185] = new Recipe
            {
                RecipeId = 185,
                ProductId = 648,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [186] = new Recipe
            {
                RecipeId = 186,
                ProductId = 648,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [187] = new Recipe
            {
                RecipeId = 187,
                ProductId = 648,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [188] = new Recipe
            {
                RecipeId = 188,
                ProductId = 648,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [189] = new Recipe
            {
                RecipeId = 189,
                ProductId = 648,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [190] = new Recipe
            {
                RecipeId = 190,
                ProductId = 649,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [191] = new Recipe
            {
                RecipeId = 191,
                ProductId = 649,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [192] = new Recipe
            {
                RecipeId = 192,
                ProductId = 649,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [193] = new Recipe
            {
                RecipeId = 193,
                ProductId = 649,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [194] = new Recipe
            {
                RecipeId = 194,
                ProductId = 649,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [195] = new Recipe
            {
                RecipeId = 195,
                ProductId = 649,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [196] = new Recipe
            {
                RecipeId = 196,
                ProductId = 649,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [197] = new Recipe
            {
                RecipeId = 197,
                ProductId = 649,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [198] = new Recipe
            {
                RecipeId = 198,
                ProductId = 649,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [199] = new Recipe
            {
                RecipeId = 199,
                ProductId = 650,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [200] = new Recipe
            {
                RecipeId = 200,
                ProductId = 650,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [201] = new Recipe
            {
                RecipeId = 201,
                ProductId = 650,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [202] = new Recipe
            {
                RecipeId = 202,
                ProductId = 650,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [203] = new Recipe
            {
                RecipeId = 203,
                ProductId = 650,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [204] = new Recipe
            {
                RecipeId = 204,
                ProductId = 650,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [205] = new Recipe
            {
                RecipeId = 205,
                ProductId = 650,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [206] = new Recipe
            {
                RecipeId = 206,
                ProductId = 650,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [207] = new Recipe
            {
                RecipeId = 207,
                ProductId = 650,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [208] = new Recipe
            {
                RecipeId = 208,
                ProductId = 651,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [209] = new Recipe
            {
                RecipeId = 209,
                ProductId = 651,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [210] = new Recipe
            {
                RecipeId = 210,
                ProductId = 651,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [211] = new Recipe
            {
                RecipeId = 211,
                ProductId = 651,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [212] = new Recipe
            {
                RecipeId = 212,
                ProductId = 651,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [213] = new Recipe
            {
                RecipeId = 213,
                ProductId = 651,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [214] = new Recipe
            {
                RecipeId = 214,
                ProductId = 651,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [215] = new Recipe
            {
                RecipeId = 215,
                ProductId = 651,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [216] = new Recipe
            {
                RecipeId = 216,
                ProductId = 651,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [217] = new Recipe
            {
                RecipeId = 217,
                ProductId = 652,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [218] = new Recipe
            {
                RecipeId = 218,
                ProductId = 652,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [219] = new Recipe
            {
                RecipeId = 219,
                ProductId = 652,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [220] = new Recipe
            {
                RecipeId = 220,
                ProductId = 652,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [221] = new Recipe
            {
                RecipeId = 221,
                ProductId = 652,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [222] = new Recipe
            {
                RecipeId = 222,
                ProductId = 652,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [223] = new Recipe
            {
                RecipeId = 223,
                ProductId = 652,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [224] = new Recipe
            {
                RecipeId = 224,
                ProductId = 652,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [225] = new Recipe
            {
                RecipeId = 225,
                ProductId = 652,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [226] = new Recipe
            {
                RecipeId = 226,
                ProductId = 653,
                MachineId = 100,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [227] = new Recipe
            {
                RecipeId = 227,
                ProductId = 653,
                MachineId = 200,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [228] = new Recipe
            {
                RecipeId = 228,
                ProductId = 653,
                MachineId = 300,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [229] = new Recipe
            {
                RecipeId = 229,
                ProductId = 653,
                MachineId = 400,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [230] = new Recipe
            {
                RecipeId = 230,
                ProductId = 653,
                MachineId = 500,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [231] = new Recipe
            {
                RecipeId = 231,
                ProductId = 653,
                MachineId = 600,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [232] = new Recipe
            {
                RecipeId = 232,
                ProductId = 653,
                MachineId = 700,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [233] = new Recipe
            {
                RecipeId = 233,
                ProductId = 653,
                MachineId = 800,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            },
            [234] = new Recipe
            {
                RecipeId = 234,
                ProductId = 653,
                MachineId = 900,
                CycleTimeMinimum = 30,
                CycleTimeMaximum = 1512000,
                MaxCyclesOk = 20,
                MaxCyclesNOk = 20,
                Retry = 1
            }
        }.ToImmutableDictionary();

    /// <summary>
    /// Lazy-loaded cached list for maximum performance - best of both worlds
    /// </summary>
    private static readonly Lazy<IReadOnlyList<Recipe>> _fixtureCache =
        new(() => _recipesDict.Values.ToList());

    /// <summary>
    /// Get all Recipe entities (cached List from dictionary for backward compatibility)
    /// </summary>
    public static IReadOnlyList<Recipe> Fixture => _fixtureCache.Value;

    /// <summary>
    /// Get a specific Recipe by ID - O(1) lookup (standardized pattern)
    /// </summary>
    public static Recipe? GetById(int id) =>
        _recipesDict.TryGetValue(id, out var recipe) ? recipe : null;

    /// <summary>
    /// Direct dictionary access for advanced scenarios (standardized pattern)
    /// </summary>
    public static IImmutableDictionary<int, Recipe> Dictionary => _recipesDict;

    /// <summary>
    /// Check if a Recipe exists by ID - O(1) lookup
    /// </summary>
    public static bool Contains(int id) => _recipesDict.ContainsKey(id);

    /// <summary>
    /// Get count of Recipes - O(1) operation
    /// </summary>
    public static int Count => _recipesDict.Count;
}
