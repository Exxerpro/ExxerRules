using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Enum;

namespace IndTrace.TestData.RawData;

/// <summary>
/// Static test data for Cycle entities with O(1) lookup.
/// Generated with ImmutableDictionary for thread-safety and performance.
/// IMPORTED: Contains all 370 entities from Cycles.json
/// Generated on: 2025-09-03 06:01:53
/// </summary>
internal static class CyclesRawData
{
    private static readonly ImmutableDictionary<int, Cycle> _cyclesDict =
        new Dictionary<int, Cycle>
        {
            [1] = new Cycle
            {
                CycleId = 1,
                BarCodeId = 1,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 82,
                TaktTime = 139,
                StartedOn = new DateTime(2023, 08, 27, 00, 49, 12),
                FinishedOn = new DateTime(2023, 08, 27, 00, 50, 34)
            },
            [2] = new Cycle
            {
                CycleId = 2,
                BarCodeId = 2,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 90,
                TaktTime = 171,
                StartedOn = new DateTime(2023, 08, 27, 02, 54, 19),
                FinishedOn = new DateTime(2023, 08, 27, 02, 55, 49)
            },
            [3] = new Cycle
            {
                CycleId = 3,
                BarCodeId = 3,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 77,
                TaktTime = 176,
                StartedOn = new DateTime(2023, 08, 27, 08, 18, 00),
                FinishedOn = new DateTime(2023, 08, 27, 08, 19, 17)
            },
            [4] = new Cycle
            {
                CycleId = 4,
                BarCodeId = 4,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 94,
                TaktTime = 166,
                StartedOn = new DateTime(2023, 08, 27, 03, 41, 07),
                FinishedOn = new DateTime(2023, 08, 27, 03, 42, 41)
            },
            [5] = new Cycle
            {
                CycleId = 5,
                BarCodeId = 5,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 154,
                StartedOn = new DateTime(2023, 08, 27, 09, 27, 28),
                FinishedOn = new DateTime(2023, 08, 27, 09, 29, 07)
            },
            [6] = new Cycle
            {
                CycleId = 6,
                BarCodeId = 6,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 56,
                TaktTime = 145,
                StartedOn = new DateTime(2023, 08, 27, 10, 01, 55),
                FinishedOn = new DateTime(2023, 08, 27, 10, 02, 51)
            },
            [7] = new Cycle
            {
                CycleId = 7,
                BarCodeId = 7,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 91,
                TaktTime = 152,
                StartedOn = new DateTime(2023, 08, 27, 11, 50, 57),
                FinishedOn = new DateTime(2023, 08, 27, 11, 52, 28)
            },
            [8] = new Cycle
            {
                CycleId = 8,
                BarCodeId = 8,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 85,
                TaktTime = 156,
                StartedOn = new DateTime(2023, 08, 27, 12, 03, 04),
                FinishedOn = new DateTime(2023, 08, 27, 12, 04, 29)
            },
            [9] = new Cycle
            {
                CycleId = 9,
                BarCodeId = 9,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 78,
                TaktTime = 162,
                StartedOn = new DateTime(2023, 08, 27, 12, 20, 08),
                FinishedOn = new DateTime(2023, 08, 27, 12, 21, 26)
            },
            [10] = new Cycle
            {
                CycleId = 10,
                BarCodeId = 10,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 55,
                TaktTime = 134,
                StartedOn = new DateTime(2023, 08, 27, 12, 19, 35),
                FinishedOn = new DateTime(2023, 08, 27, 12, 20, 30)
            },
            [11] = new Cycle
            {
                CycleId = 11,
                BarCodeId = 11,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 64,
                TaktTime = 153,
                StartedOn = new DateTime(2023, 08, 27, 09, 22, 03),
                FinishedOn = new DateTime(2023, 08, 27, 09, 23, 07)
            },
            [12] = new Cycle
            {
                CycleId = 12,
                BarCodeId = 12,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 178,
                StartedOn = new DateTime(2023, 08, 27, 09, 25, 05),
                FinishedOn = new DateTime(2023, 08, 27, 09, 26, 29)
            },
            [13] = new Cycle
            {
                CycleId = 13,
                BarCodeId = 13,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 87,
                TaktTime = 139,
                StartedOn = new DateTime(2023, 08, 27, 12, 09, 01),
                FinishedOn = new DateTime(2023, 08, 27, 12, 10, 28)
            },
            [14] = new Cycle
            {
                CycleId = 14,
                BarCodeId = 14,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 76,
                TaktTime = 155,
                StartedOn = new DateTime(2023, 08, 27, 12, 12, 01),
                FinishedOn = new DateTime(2023, 08, 27, 12, 13, 17)
            },
            [15] = new Cycle
            {
                CycleId = 15,
                BarCodeId = 15,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 95,
                TaktTime = 171,
                StartedOn = new DateTime(2023, 08, 27, 12, 16, 03),
                FinishedOn = new DateTime(2023, 08, 27, 12, 17, 38)
            },
            [16] = new Cycle
            {
                CycleId = 16,
                BarCodeId = 16,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 73,
                TaktTime = 149,
                StartedOn = new DateTime(2023, 08, 27, 12, 19, 31),
                FinishedOn = new DateTime(2023, 08, 27, 12, 20, 44)
            },
            [17] = new Cycle
            {
                CycleId = 17,
                BarCodeId = 16,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 73,
                TaktTime = 132,
                StartedOn = new DateTime(2023, 08, 27, 12, 20, 44),
                FinishedOn = new DateTime(2023, 08, 27, 12, 21, 57)
            },
            [18] = new Cycle
            {
                CycleId = 18,
                BarCodeId = 17,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 86,
                TaktTime = 175,
                StartedOn = new DateTime(2023, 08, 27, 12, 20, 56),
                FinishedOn = new DateTime(2023, 08, 27, 12, 22, 22)
            },
            [19] = new Cycle
            {
                CycleId = 19,
                BarCodeId = 17,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 97,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 12, 22, 22),
                FinishedOn = new DateTime(2023, 08, 27, 12, 23, 59)
            },
            [20] = new Cycle
            {
                CycleId = 20,
                BarCodeId = 18,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 169,
                StartedOn = new DateTime(2023, 08, 27, 12, 22, 36),
                FinishedOn = new DateTime(2023, 08, 27, 12, 24, 15)
            },
            [21] = new Cycle
            {
                CycleId = 21,
                BarCodeId = 18,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 90,
                TaktTime = 149,
                StartedOn = new DateTime(2023, 08, 27, 12, 24, 15),
                FinishedOn = new DateTime(2023, 08, 27, 12, 25, 45)
            },
            [22] = new Cycle
            {
                CycleId = 22,
                BarCodeId = 19,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 68,
                TaktTime = 168,
                StartedOn = new DateTime(2023, 08, 27, 12, 21, 23),
                FinishedOn = new DateTime(2023, 08, 27, 12, 22, 31)
            },
            [23] = new Cycle
            {
                CycleId = 23,
                BarCodeId = 19,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 90,
                TaktTime = 165,
                StartedOn = new DateTime(2023, 08, 27, 12, 22, 31),
                FinishedOn = new DateTime(2023, 08, 27, 12, 24, 01)
            },
            [24] = new Cycle
            {
                CycleId = 24,
                BarCodeId = 20,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 63,
                TaktTime = 145,
                StartedOn = new DateTime(2023, 08, 27, 12, 22, 16),
                FinishedOn = new DateTime(2023, 08, 27, 12, 23, 19)
            },
            [25] = new Cycle
            {
                CycleId = 25,
                BarCodeId = 20,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 88,
                TaktTime = 174,
                StartedOn = new DateTime(2023, 08, 27, 12, 23, 19),
                FinishedOn = new DateTime(2023, 08, 27, 12, 24, 47)
            },
            [26] = new Cycle
            {
                CycleId = 26,
                BarCodeId = 21,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 61,
                TaktTime = 134,
                StartedOn = new DateTime(2023, 08, 27, 12, 20, 02),
                FinishedOn = new DateTime(2023, 08, 27, 12, 21, 03)
            },
            [27] = new Cycle
            {
                CycleId = 27,
                BarCodeId = 21,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 77,
                TaktTime = 154,
                StartedOn = new DateTime(2023, 08, 27, 12, 21, 03),
                FinishedOn = new DateTime(2023, 08, 27, 12, 22, 20)
            },
            [28] = new Cycle
            {
                CycleId = 28,
                BarCodeId = 22,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 96,
                TaktTime = 148,
                StartedOn = new DateTime(2023, 08, 27, 12, 21, 58),
                FinishedOn = new DateTime(2023, 08, 27, 12, 23, 34)
            },
            [29] = new Cycle
            {
                CycleId = 29,
                BarCodeId = 22,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 85,
                TaktTime = 152,
                StartedOn = new DateTime(2023, 08, 27, 12, 23, 34),
                FinishedOn = new DateTime(2023, 08, 27, 12, 24, 59)
            },
            [30] = new Cycle
            {
                CycleId = 30,
                BarCodeId = 23,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 66,
                TaktTime = 166,
                StartedOn = new DateTime(2023, 08, 27, 13, 15, 45),
                FinishedOn = new DateTime(2023, 08, 27, 13, 16, 51)
            },
            [31] = new Cycle
            {
                CycleId = 31,
                BarCodeId = 23,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 76,
                TaktTime = 167,
                StartedOn = new DateTime(2023, 08, 27, 13, 16, 51),
                FinishedOn = new DateTime(2023, 08, 27, 13, 18, 07)
            },
            [32] = new Cycle
            {
                CycleId = 32,
                BarCodeId = 24,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 75,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 13, 26, 29),
                FinishedOn = new DateTime(2023, 08, 27, 13, 27, 44)
            },
            [33] = new Cycle
            {
                CycleId = 33,
                BarCodeId = 24,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 62,
                TaktTime = 123,
                StartedOn = new DateTime(2023, 08, 27, 13, 27, 44),
                FinishedOn = new DateTime(2023, 08, 27, 13, 28, 46)
            },
            [34] = new Cycle
            {
                CycleId = 34,
                BarCodeId = 25,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 60,
                TaktTime = 159,
                StartedOn = new DateTime(2023, 08, 27, 13, 31, 55),
                FinishedOn = new DateTime(2023, 08, 27, 13, 32, 55)
            },
            [35] = new Cycle
            {
                CycleId = 35,
                BarCodeId = 25,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 87,
                TaktTime = 168,
                StartedOn = new DateTime(2023, 08, 27, 13, 32, 55),
                FinishedOn = new DateTime(2023, 08, 27, 13, 34, 22)
            },
            [36] = new Cycle
            {
                CycleId = 36,
                BarCodeId = 26,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 95,
                TaktTime = 187,
                StartedOn = new DateTime(2023, 08, 27, 13, 37, 13),
                FinishedOn = new DateTime(2023, 08, 27, 13, 38, 48)
            },
            [37] = new Cycle
            {
                CycleId = 37,
                BarCodeId = 26,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 54,
                TaktTime = 139,
                StartedOn = new DateTime(2023, 08, 27, 13, 38, 48),
                FinishedOn = new DateTime(2023, 08, 27, 13, 39, 42)
            },
            [38] = new Cycle
            {
                CycleId = 38,
                BarCodeId = 27,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 56,
                TaktTime = 147,
                StartedOn = new DateTime(2023, 08, 27, 15, 03, 53),
                FinishedOn = new DateTime(2023, 08, 27, 15, 04, 49)
            },
            [39] = new Cycle
            {
                CycleId = 39,
                BarCodeId = 27,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 97,
                TaktTime = 174,
                StartedOn = new DateTime(2023, 08, 27, 15, 04, 49),
                FinishedOn = new DateTime(2023, 08, 27, 15, 06, 26)
            },
            [40] = new Cycle
            {
                CycleId = 40,
                BarCodeId = 28,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 67,
                TaktTime = 129,
                StartedOn = new DateTime(2023, 08, 27, 16, 54, 58),
                FinishedOn = new DateTime(2023, 08, 27, 16, 56, 05)
            },
            [41] = new Cycle
            {
                CycleId = 41,
                BarCodeId = 28,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 60,
                TaktTime = 157,
                StartedOn = new DateTime(2023, 08, 27, 16, 56, 05),
                FinishedOn = new DateTime(2023, 08, 27, 16, 57, 05)
            },
            [42] = new Cycle
            {
                CycleId = 42,
                BarCodeId = 29,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 79,
                TaktTime = 139,
                StartedOn = new DateTime(2023, 08, 27, 09, 52, 09),
                FinishedOn = new DateTime(2023, 08, 27, 09, 53, 28)
            },
            [43] = new Cycle
            {
                CycleId = 43,
                BarCodeId = 29,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 83,
                TaktTime = 138,
                StartedOn = new DateTime(2023, 08, 27, 09, 53, 28),
                FinishedOn = new DateTime(2023, 08, 27, 09, 54, 51)
            },
            [44] = new Cycle
            {
                CycleId = 44,
                BarCodeId = 30,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 54,
                TaktTime = 106,
                StartedOn = new DateTime(2023, 08, 27, 09, 54, 09),
                FinishedOn = new DateTime(2023, 08, 27, 09, 55, 03)
            },
            [45] = new Cycle
            {
                CycleId = 45,
                BarCodeId = 30,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 73,
                TaktTime = 158,
                StartedOn = new DateTime(2023, 08, 27, 09, 55, 03),
                FinishedOn = new DateTime(2023, 08, 27, 09, 56, 16)
            },
            [46] = new Cycle
            {
                CycleId = 46,
                BarCodeId = 31,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 66,
                TaktTime = 158,
                StartedOn = new DateTime(2023, 08, 27, 10, 00, 17),
                FinishedOn = new DateTime(2023, 08, 27, 10, 01, 23)
            },
            [47] = new Cycle
            {
                CycleId = 47,
                BarCodeId = 31,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 87,
                TaktTime = 153,
                StartedOn = new DateTime(2023, 08, 27, 10, 01, 23),
                FinishedOn = new DateTime(2023, 08, 27, 10, 02, 50)
            },
            [48] = new Cycle
            {
                CycleId = 48,
                BarCodeId = 31,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 80,
                TaktTime = 155,
                StartedOn = new DateTime(2023, 08, 27, 10, 02, 50),
                FinishedOn = new DateTime(2023, 08, 27, 10, 04, 10)
            },
            [49] = new Cycle
            {
                CycleId = 49,
                BarCodeId = 32,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 85,
                TaktTime = 149,
                StartedOn = new DateTime(2023, 08, 27, 10, 01, 14),
                FinishedOn = new DateTime(2023, 08, 27, 10, 02, 39)
            },
            [50] = new Cycle
            {
                CycleId = 50,
                BarCodeId = 32,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 88,
                TaktTime = 185,
                StartedOn = new DateTime(2023, 08, 27, 10, 02, 39),
                FinishedOn = new DateTime(2023, 08, 27, 10, 04, 07)
            },
            [51] = new Cycle
            {
                CycleId = 51,
                BarCodeId = 32,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 62,
                TaktTime = 128,
                StartedOn = new DateTime(2023, 08, 27, 10, 01, 14),
                FinishedOn = new DateTime(2023, 08, 27, 10, 02, 16)
            },
            [52] = new Cycle
            {
                CycleId = 52,
                BarCodeId = 33,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 95,
                TaktTime = 177,
                StartedOn = new DateTime(2023, 08, 27, 10, 01, 50),
                FinishedOn = new DateTime(2023, 08, 27, 10, 03, 25)
            },
            [53] = new Cycle
            {
                CycleId = 53,
                BarCodeId = 33,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 100,
                TaktTime = 160,
                StartedOn = new DateTime(2023, 08, 27, 10, 03, 25),
                FinishedOn = new DateTime(2023, 08, 27, 10, 05, 05)
            },
            [54] = new Cycle
            {
                CycleId = 54,
                BarCodeId = 33,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 74,
                TaktTime = 174,
                StartedOn = new DateTime(2023, 08, 27, 10, 05, 05),
                FinishedOn = new DateTime(2023, 08, 27, 10, 06, 19)
            },
            [55] = new Cycle
            {
                CycleId = 55,
                BarCodeId = 34,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 79,
                TaktTime = 139,
                StartedOn = new DateTime(2023, 08, 27, 10, 03, 23),
                FinishedOn = new DateTime(2023, 08, 27, 10, 04, 42)
            },
            [56] = new Cycle
            {
                CycleId = 56,
                BarCodeId = 34,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 65,
                TaktTime = 133,
                StartedOn = new DateTime(2023, 08, 27, 10, 04, 42),
                FinishedOn = new DateTime(2023, 08, 27, 10, 05, 47)
            },
            [57] = new Cycle
            {
                CycleId = 57,
                BarCodeId = 34,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 75,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 10, 05, 47),
                FinishedOn = new DateTime(2023, 08, 27, 10, 07, 02)
            },
            [58] = new Cycle
            {
                CycleId = 58,
                BarCodeId = 35,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 89,
                TaktTime = 182,
                StartedOn = new DateTime(2023, 08, 27, 10, 05, 46),
                FinishedOn = new DateTime(2023, 08, 27, 10, 07, 15)
            },
            [59] = new Cycle
            {
                CycleId = 59,
                BarCodeId = 35,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 52,
                TaktTime = 145,
                StartedOn = new DateTime(2023, 08, 27, 10, 07, 15),
                FinishedOn = new DateTime(2023, 08, 27, 10, 08, 07)
            },
            [60] = new Cycle
            {
                CycleId = 60,
                BarCodeId = 35,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 52,
                TaktTime = 105,
                StartedOn = new DateTime(2023, 08, 27, 10, 05, 46),
                FinishedOn = new DateTime(2023, 08, 27, 10, 06, 38)
            },
            [61] = new Cycle
            {
                CycleId = 61,
                BarCodeId = 36,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 98,
                TaktTime = 165,
                StartedOn = new DateTime(2023, 08, 27, 10, 06, 13),
                FinishedOn = new DateTime(2023, 08, 27, 10, 07, 51)
            },
            [62] = new Cycle
            {
                CycleId = 62,
                BarCodeId = 36,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 79,
                TaktTime = 163,
                StartedOn = new DateTime(2023, 08, 27, 10, 07, 51),
                FinishedOn = new DateTime(2023, 08, 27, 10, 09, 10)
            },
            [63] = new Cycle
            {
                CycleId = 63,
                BarCodeId = 36,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 100,
                TaktTime = 174,
                StartedOn = new DateTime(2023, 08, 27, 10, 09, 10),
                FinishedOn = new DateTime(2023, 08, 27, 10, 10, 50)
            },
            [64] = new Cycle
            {
                CycleId = 64,
                BarCodeId = 37,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 78,
                TaktTime = 132,
                StartedOn = new DateTime(2023, 08, 27, 10, 06, 27),
                FinishedOn = new DateTime(2023, 08, 27, 10, 07, 45)
            },
            [65] = new Cycle
            {
                CycleId = 65,
                BarCodeId = 37,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 81,
                TaktTime = 144,
                StartedOn = new DateTime(2023, 08, 27, 10, 07, 45),
                FinishedOn = new DateTime(2023, 08, 27, 10, 09, 06)
            },
            [66] = new Cycle
            {
                CycleId = 66,
                BarCodeId = 37,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 173,
                StartedOn = new DateTime(2023, 08, 27, 10, 09, 06),
                FinishedOn = new DateTime(2023, 08, 27, 10, 10, 30)
            },
            [67] = new Cycle
            {
                CycleId = 67,
                BarCodeId = 38,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 55,
                TaktTime = 112,
                StartedOn = new DateTime(2023, 08, 27, 10, 07, 47),
                FinishedOn = new DateTime(2023, 08, 27, 10, 08, 42)
            },
            [68] = new Cycle
            {
                CycleId = 68,
                BarCodeId = 38,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 100,
                TaktTime = 158,
                StartedOn = new DateTime(2023, 08, 27, 10, 08, 42),
                FinishedOn = new DateTime(2023, 08, 27, 10, 10, 22)
            },
            [69] = new Cycle
            {
                CycleId = 69,
                BarCodeId = 38,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 154,
                StartedOn = new DateTime(2023, 08, 27, 10, 07, 47),
                FinishedOn = new DateTime(2023, 08, 27, 10, 09, 11)
            },
            [70] = new Cycle
            {
                CycleId = 70,
                BarCodeId = 39,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 90,
                TaktTime = 149,
                StartedOn = new DateTime(2023, 08, 27, 10, 13, 47),
                FinishedOn = new DateTime(2023, 08, 27, 10, 15, 17)
            },
            [71] = new Cycle
            {
                CycleId = 71,
                BarCodeId = 39,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 68,
                TaktTime = 139,
                StartedOn = new DateTime(2023, 08, 27, 10, 15, 17),
                FinishedOn = new DateTime(2023, 08, 27, 10, 16, 25)
            },
            [72] = new Cycle
            {
                CycleId = 72,
                BarCodeId = 39,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 86,
                TaktTime = 154,
                StartedOn = new DateTime(2023, 08, 27, 10, 16, 25),
                FinishedOn = new DateTime(2023, 08, 27, 10, 17, 51)
            },
            [73] = new Cycle
            {
                CycleId = 73,
                BarCodeId = 40,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 63,
                TaktTime = 135,
                StartedOn = new DateTime(2023, 08, 27, 10, 13, 59),
                FinishedOn = new DateTime(2023, 08, 27, 10, 15, 02)
            },
            [74] = new Cycle
            {
                CycleId = 74,
                BarCodeId = 40,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 100,
                TaktTime = 159,
                StartedOn = new DateTime(2023, 08, 27, 10, 15, 02),
                FinishedOn = new DateTime(2023, 08, 27, 10, 16, 42)
            },
            [75] = new Cycle
            {
                CycleId = 75,
                BarCodeId = 40,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 100,
                TaktTime = 183,
                StartedOn = new DateTime(2023, 08, 27, 10, 16, 42),
                FinishedOn = new DateTime(2023, 08, 27, 10, 18, 22)
            },
            [76] = new Cycle
            {
                CycleId = 76,
                BarCodeId = 41,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 61,
                TaktTime = 133,
                StartedOn = new DateTime(2023, 08, 27, 10, 16, 39),
                FinishedOn = new DateTime(2023, 08, 27, 10, 17, 40)
            },
            [77] = new Cycle
            {
                CycleId = 77,
                BarCodeId = 41,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 92,
                TaktTime = 163,
                StartedOn = new DateTime(2023, 08, 27, 10, 17, 40),
                FinishedOn = new DateTime(2023, 08, 27, 10, 19, 12)
            },
            [78] = new Cycle
            {
                CycleId = 78,
                BarCodeId = 41,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 93,
                TaktTime = 157,
                StartedOn = new DateTime(2023, 08, 27, 10, 16, 39),
                FinishedOn = new DateTime(2023, 08, 27, 10, 18, 12)
            },
            [79] = new Cycle
            {
                CycleId = 79,
                BarCodeId = 42,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 92,
                TaktTime = 190,
                StartedOn = new DateTime(2023, 08, 27, 10, 35, 21),
                FinishedOn = new DateTime(2023, 08, 27, 10, 36, 53)
            },
            [80] = new Cycle
            {
                CycleId = 80,
                BarCodeId = 42,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 83,
                TaktTime = 165,
                StartedOn = new DateTime(2023, 08, 27, 10, 36, 53),
                FinishedOn = new DateTime(2023, 08, 27, 10, 38, 16)
            },
            [81] = new Cycle
            {
                CycleId = 81,
                BarCodeId = 42,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 77,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 10, 38, 16),
                FinishedOn = new DateTime(2023, 08, 27, 10, 39, 33)
            },
            [82] = new Cycle
            {
                CycleId = 82,
                BarCodeId = 43,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 82,
                TaktTime = 134,
                StartedOn = new DateTime(2023, 08, 27, 10, 35, 49),
                FinishedOn = new DateTime(2023, 08, 27, 10, 37, 11)
            },
            [83] = new Cycle
            {
                CycleId = 83,
                BarCodeId = 43,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 92,
                TaktTime = 185,
                StartedOn = new DateTime(2023, 08, 27, 10, 37, 11),
                FinishedOn = new DateTime(2023, 08, 27, 10, 38, 43)
            },
            [84] = new Cycle
            {
                CycleId = 84,
                BarCodeId = 43,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 80,
                TaktTime = 147,
                StartedOn = new DateTime(2023, 08, 27, 10, 38, 43),
                FinishedOn = new DateTime(2023, 08, 27, 10, 40, 03)
            },
            [85] = new Cycle
            {
                CycleId = 85,
                BarCodeId = 44,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 173,
                StartedOn = new DateTime(2023, 08, 27, 10, 43, 32),
                FinishedOn = new DateTime(2023, 08, 27, 10, 45, 11)
            },
            [86] = new Cycle
            {
                CycleId = 86,
                BarCodeId = 44,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 69,
                TaktTime = 120,
                StartedOn = new DateTime(2023, 08, 27, 10, 45, 11),
                FinishedOn = new DateTime(2023, 08, 27, 10, 46, 20)
            },
            [87] = new Cycle
            {
                CycleId = 87,
                BarCodeId = 44,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 174,
                StartedOn = new DateTime(2023, 08, 27, 10, 43, 32),
                FinishedOn = new DateTime(2023, 08, 27, 10, 45, 11)
            },
            [88] = new Cycle
            {
                CycleId = 88,
                BarCodeId = 45,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 97,
                TaktTime = 193,
                StartedOn = new DateTime(2023, 08, 27, 10, 49, 58),
                FinishedOn = new DateTime(2023, 08, 27, 10, 51, 35)
            },
            [89] = new Cycle
            {
                CycleId = 89,
                BarCodeId = 45,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 61,
                TaktTime = 119,
                StartedOn = new DateTime(2023, 08, 27, 10, 51, 35),
                FinishedOn = new DateTime(2023, 08, 27, 10, 52, 36)
            },
            [90] = new Cycle
            {
                CycleId = 90,
                BarCodeId = 45,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 66,
                TaktTime = 143,
                StartedOn = new DateTime(2023, 08, 27, 10, 52, 36),
                FinishedOn = new DateTime(2023, 08, 27, 10, 53, 42)
            },
            [91] = new Cycle
            {
                CycleId = 91,
                BarCodeId = 46,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 93,
                TaktTime = 153,
                StartedOn = new DateTime(2023, 08, 27, 10, 50, 56),
                FinishedOn = new DateTime(2023, 08, 27, 10, 52, 29)
            },
            [92] = new Cycle
            {
                CycleId = 92,
                BarCodeId = 47,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 83,
                TaktTime = 162,
                StartedOn = new DateTime(2023, 08, 27, 10, 51, 25),
                FinishedOn = new DateTime(2023, 08, 27, 10, 52, 48)
            },
            [93] = new Cycle
            {
                CycleId = 93,
                BarCodeId = 48,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 100,
                TaktTime = 200,
                StartedOn = new DateTime(2023, 08, 27, 10, 53, 08),
                FinishedOn = new DateTime(2023, 08, 27, 10, 54, 48)
            },
            [94] = new Cycle
            {
                CycleId = 94,
                BarCodeId = 49,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 65,
                TaktTime = 148,
                StartedOn = new DateTime(2023, 08, 27, 10, 53, 32),
                FinishedOn = new DateTime(2023, 08, 27, 10, 54, 37)
            },
            [95] = new Cycle
            {
                CycleId = 95,
                BarCodeId = 50,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 60,
                TaktTime = 145,
                StartedOn = new DateTime(2023, 08, 27, 10, 59, 49),
                FinishedOn = new DateTime(2023, 08, 27, 11, 00, 49)
            },
            [96] = new Cycle
            {
                CycleId = 96,
                BarCodeId = 51,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 76,
                TaktTime = 163,
                StartedOn = new DateTime(2023, 08, 27, 11, 01, 12),
                FinishedOn = new DateTime(2023, 08, 27, 11, 02, 28)
            },
            [97] = new Cycle
            {
                CycleId = 97,
                BarCodeId = 52,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 57,
                TaktTime = 113,
                StartedOn = new DateTime(2023, 08, 27, 11, 01, 23),
                FinishedOn = new DateTime(2023, 08, 27, 11, 02, 20)
            },
            [98] = new Cycle
            {
                CycleId = 98,
                BarCodeId = 53,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 72,
                TaktTime = 122,
                StartedOn = new DateTime(2023, 08, 27, 12, 27, 18),
                FinishedOn = new DateTime(2023, 08, 27, 12, 28, 30)
            },
            [99] = new Cycle
            {
                CycleId = 99,
                BarCodeId = 54,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 72,
                TaktTime = 139,
                StartedOn = new DateTime(2023, 08, 27, 13, 36, 40),
                FinishedOn = new DateTime(2023, 08, 27, 13, 37, 52)
            },
            [100] = new Cycle
            {
                CycleId = 100,
                BarCodeId = 55,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 90,
                TaktTime = 181,
                StartedOn = new DateTime(2023, 08, 27, 14, 01, 00),
                FinishedOn = new DateTime(2023, 08, 27, 14, 02, 30)
            },
            [101] = new Cycle
            {
                CycleId = 101,
                BarCodeId = 56,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 82,
                TaktTime = 171,
                StartedOn = new DateTime(2023, 08, 27, 16, 25, 12),
                FinishedOn = new DateTime(2023, 08, 27, 16, 26, 34)
            },
            [102] = new Cycle
            {
                CycleId = 102,
                BarCodeId = 57,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 67,
                TaktTime = 153,
                StartedOn = new DateTime(2023, 08, 27, 17, 04, 37),
                FinishedOn = new DateTime(2023, 08, 27, 17, 05, 44)
            },
            [103] = new Cycle
            {
                CycleId = 103,
                BarCodeId = 58,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 95,
                TaktTime = 162,
                StartedOn = new DateTime(2023, 08, 27, 17, 16, 44),
                FinishedOn = new DateTime(2023, 08, 27, 17, 18, 19)
            },
            [104] = new Cycle
            {
                CycleId = 104,
                BarCodeId = 59,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 94,
                TaktTime = 165,
                StartedOn = new DateTime(2023, 08, 27, 17, 19, 30),
                FinishedOn = new DateTime(2023, 08, 27, 17, 21, 04)
            },
            [105] = new Cycle
            {
                CycleId = 105,
                BarCodeId = 60,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 85,
                TaktTime = 143,
                StartedOn = new DateTime(2023, 08, 27, 15, 06, 56),
                FinishedOn = new DateTime(2023, 08, 27, 15, 08, 21)
            },
            [106] = new Cycle
            {
                CycleId = 106,
                BarCodeId = 61,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 60,
                TaktTime = 160,
                StartedOn = new DateTime(2023, 08, 27, 15, 44, 41),
                FinishedOn = new DateTime(2023, 08, 27, 15, 45, 41)
            },
            [107] = new Cycle
            {
                CycleId = 107,
                BarCodeId = 61,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 82,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 15, 45, 41),
                FinishedOn = new DateTime(2023, 08, 27, 15, 47, 03)
            },
            [108] = new Cycle
            {
                CycleId = 108,
                BarCodeId = 62,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 72,
                TaktTime = 169,
                StartedOn = new DateTime(2023, 08, 27, 15, 45, 43),
                FinishedOn = new DateTime(2023, 08, 27, 15, 46, 55)
            },
            [109] = new Cycle
            {
                CycleId = 109,
                BarCodeId = 62,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 63,
                TaktTime = 154,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 55),
                FinishedOn = new DateTime(2023, 08, 27, 15, 47, 58)
            },
            [110] = new Cycle
            {
                CycleId = 110,
                BarCodeId = 63,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 96,
                TaktTime = 191,
                StartedOn = new DateTime(2023, 08, 27, 16, 25, 05),
                FinishedOn = new DateTime(2023, 08, 27, 16, 26, 41)
            },
            [111] = new Cycle
            {
                CycleId = 111,
                BarCodeId = 63,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 96,
                TaktTime = 179,
                StartedOn = new DateTime(2023, 08, 27, 16, 26, 41),
                FinishedOn = new DateTime(2023, 08, 27, 16, 28, 17)
            },
            [112] = new Cycle
            {
                CycleId = 112,
                BarCodeId = 64,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 93,
                TaktTime = 155,
                StartedOn = new DateTime(2023, 08, 27, 09, 02, 38),
                FinishedOn = new DateTime(2023, 08, 27, 09, 04, 11)
            },
            [113] = new Cycle
            {
                CycleId = 113,
                BarCodeId = 64,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 88,
                TaktTime = 179,
                StartedOn = new DateTime(2023, 08, 27, 09, 04, 11),
                FinishedOn = new DateTime(2023, 08, 27, 09, 05, 39)
            },
            [114] = new Cycle
            {
                CycleId = 114,
                BarCodeId = 65,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 93,
                TaktTime = 175,
                StartedOn = new DateTime(2023, 08, 27, 09, 18, 14),
                FinishedOn = new DateTime(2023, 08, 27, 09, 19, 47)
            },
            [115] = new Cycle
            {
                CycleId = 115,
                BarCodeId = 65,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 57,
                TaktTime = 108,
                StartedOn = new DateTime(2023, 08, 27, 09, 19, 47),
                FinishedOn = new DateTime(2023, 08, 27, 09, 20, 44)
            },
            [116] = new Cycle
            {
                CycleId = 116,
                BarCodeId = 66,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 90,
                TaktTime = 153,
                StartedOn = new DateTime(2023, 08, 27, 10, 58, 00),
                FinishedOn = new DateTime(2023, 08, 27, 10, 59, 30)
            },
            [117] = new Cycle
            {
                CycleId = 117,
                BarCodeId = 66,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 75,
                TaktTime = 140,
                StartedOn = new DateTime(2023, 08, 27, 10, 59, 30),
                FinishedOn = new DateTime(2023, 08, 27, 11, 00, 45)
            },
            [118] = new Cycle
            {
                CycleId = 118,
                BarCodeId = 67,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 89,
                TaktTime = 144,
                StartedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                FinishedOn = new DateTime(2023, 08, 27, 15, 33, 21)
            },
            [119] = new Cycle
            {
                CycleId = 119,
                BarCodeId = 67,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 71,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 33, 21),
                FinishedOn = new DateTime(2023, 08, 27, 15, 34, 32)
            },
            [120] = new Cycle
            {
                CycleId = 120,
                BarCodeId = 68,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 100,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 00),
                FinishedOn = new DateTime(2023, 08, 27, 15, 47, 40)
            },
            [121] = new Cycle
            {
                CycleId = 121,
                BarCodeId = 68,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 61,
                TaktTime = 149,
                StartedOn = new DateTime(2023, 08, 27, 15, 47, 40),
                FinishedOn = new DateTime(2023, 08, 27, 15, 48, 41)
            },
            [122] = new Cycle
            {
                CycleId = 122,
                BarCodeId = 69,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 69,
                TaktTime = 131,
                StartedOn = new DateTime(2023, 08, 27, 10, 53, 32),
                FinishedOn = new DateTime(2023, 08, 27, 10, 54, 41)
            },
            [123] = new Cycle
            {
                CycleId = 123,
                BarCodeId = 69,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 87,
                TaktTime = 181,
                StartedOn = new DateTime(2023, 08, 27, 10, 54, 41),
                FinishedOn = new DateTime(2023, 08, 27, 10, 56, 08)
            },
            [124] = new Cycle
            {
                CycleId = 124,
                BarCodeId = 70,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 92,
                TaktTime = 188,
                StartedOn = new DateTime(2023, 08, 27, 10, 59, 49),
                FinishedOn = new DateTime(2023, 08, 27, 11, 01, 21)
            },
            [125] = new Cycle
            {
                CycleId = 125,
                BarCodeId = 70,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 56,
                TaktTime = 118,
                StartedOn = new DateTime(2023, 08, 27, 11, 01, 21),
                FinishedOn = new DateTime(2023, 08, 27, 11, 02, 17)
            },
            [126] = new Cycle
            {
                CycleId = 126,
                BarCodeId = 71,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 64,
                TaktTime = 133,
                StartedOn = new DateTime(2023, 08, 27, 11, 01, 12),
                FinishedOn = new DateTime(2023, 08, 27, 11, 02, 16)
            },
            [127] = new Cycle
            {
                CycleId = 127,
                BarCodeId = 71,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 68,
                TaktTime = 159,
                StartedOn = new DateTime(2023, 08, 27, 11, 02, 16),
                FinishedOn = new DateTime(2023, 08, 27, 11, 03, 24)
            },
            [128] = new Cycle
            {
                CycleId = 128,
                BarCodeId = 72,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 52,
                TaktTime = 138,
                StartedOn = new DateTime(2023, 08, 27, 11, 01, 23),
                FinishedOn = new DateTime(2023, 08, 27, 11, 02, 15)
            },
            [129] = new Cycle
            {
                CycleId = 129,
                BarCodeId = 72,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 65,
                TaktTime = 165,
                StartedOn = new DateTime(2023, 08, 27, 11, 02, 15),
                FinishedOn = new DateTime(2023, 08, 27, 11, 03, 20)
            },
            [130] = new Cycle
            {
                CycleId = 130,
                BarCodeId = 73,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 71,
                TaktTime = 126,
                StartedOn = new DateTime(2023, 08, 27, 12, 27, 18),
                FinishedOn = new DateTime(2023, 08, 27, 12, 28, 29)
            },
            [131] = new Cycle
            {
                CycleId = 131,
                BarCodeId = 73,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 72,
                TaktTime = 149,
                StartedOn = new DateTime(2023, 08, 27, 12, 28, 29),
                FinishedOn = new DateTime(2023, 08, 27, 12, 29, 41)
            },
            [132] = new Cycle
            {
                CycleId = 132,
                BarCodeId = 74,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 91,
                TaktTime = 156,
                StartedOn = new DateTime(2023, 08, 27, 13, 36, 40),
                FinishedOn = new DateTime(2023, 08, 27, 13, 38, 11)
            },
            [133] = new Cycle
            {
                CycleId = 133,
                BarCodeId = 74,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 72,
                TaktTime = 143,
                StartedOn = new DateTime(2023, 08, 27, 13, 38, 11),
                FinishedOn = new DateTime(2023, 08, 27, 13, 39, 23)
            },
            [134] = new Cycle
            {
                CycleId = 134,
                BarCodeId = 75,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 71,
                TaktTime = 135,
                StartedOn = new DateTime(2023, 08, 27, 14, 01, 00),
                FinishedOn = new DateTime(2023, 08, 27, 14, 02, 11)
            },
            [135] = new Cycle
            {
                CycleId = 135,
                BarCodeId = 75,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 157,
                StartedOn = new DateTime(2023, 08, 27, 14, 02, 11),
                FinishedOn = new DateTime(2023, 08, 27, 14, 03, 35)
            },
            [136] = new Cycle
            {
                CycleId = 136,
                BarCodeId = 76,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 56,
                TaktTime = 106,
                StartedOn = new DateTime(2023, 08, 27, 16, 25, 12),
                FinishedOn = new DateTime(2023, 08, 27, 16, 26, 08)
            },
            [137] = new Cycle
            {
                CycleId = 137,
                BarCodeId = 76,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 66,
                TaktTime = 117,
                StartedOn = new DateTime(2023, 08, 27, 16, 26, 08),
                FinishedOn = new DateTime(2023, 08, 27, 16, 27, 14)
            },
            [138] = new Cycle
            {
                CycleId = 138,
                BarCodeId = 76,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 76,
                TaktTime = 143,
                StartedOn = new DateTime(2023, 08, 27, 16, 27, 14),
                FinishedOn = new DateTime(2023, 08, 27, 16, 28, 30)
            },
            [139] = new Cycle
            {
                CycleId = 139,
                BarCodeId = 77,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 87,
                TaktTime = 147,
                StartedOn = new DateTime(2023, 08, 27, 17, 04, 37),
                FinishedOn = new DateTime(2023, 08, 27, 17, 06, 04)
            },
            [140] = new Cycle
            {
                CycleId = 140,
                BarCodeId = 77,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 56,
                TaktTime = 112,
                StartedOn = new DateTime(2023, 08, 27, 17, 06, 04),
                FinishedOn = new DateTime(2023, 08, 27, 17, 07, 00)
            },
            [141] = new Cycle
            {
                CycleId = 141,
                BarCodeId = 77,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 96,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 17, 04, 37),
                FinishedOn = new DateTime(2023, 08, 27, 17, 06, 13)
            },
            [142] = new Cycle
            {
                CycleId = 142,
                BarCodeId = 78,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 163,
                StartedOn = new DateTime(2023, 08, 27, 17, 16, 44),
                FinishedOn = new DateTime(2023, 08, 27, 17, 18, 23)
            },
            [143] = new Cycle
            {
                CycleId = 143,
                BarCodeId = 78,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 88,
                TaktTime = 160,
                StartedOn = new DateTime(2023, 08, 27, 17, 18, 23),
                FinishedOn = new DateTime(2023, 08, 27, 17, 19, 51)
            },
            [144] = new Cycle
            {
                CycleId = 144,
                BarCodeId = 78,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 74,
                TaktTime = 162,
                StartedOn = new DateTime(2023, 08, 27, 17, 19, 51),
                FinishedOn = new DateTime(2023, 08, 27, 17, 21, 05)
            },
            [145] = new Cycle
            {
                CycleId = 145,
                BarCodeId = 79,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 66,
                TaktTime = 166,
                StartedOn = new DateTime(2023, 08, 27, 17, 19, 30),
                FinishedOn = new DateTime(2023, 08, 27, 17, 20, 36)
            },
            [146] = new Cycle
            {
                CycleId = 146,
                BarCodeId = 79,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 55,
                TaktTime = 114,
                StartedOn = new DateTime(2023, 08, 27, 17, 20, 36),
                FinishedOn = new DateTime(2023, 08, 27, 17, 21, 31)
            },
            [147] = new Cycle
            {
                CycleId = 147,
                BarCodeId = 79,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 50,
                TaktTime = 103,
                StartedOn = new DateTime(2023, 08, 27, 17, 21, 31),
                FinishedOn = new DateTime(2023, 08, 27, 17, 22, 21)
            },
            [148] = new Cycle
            {
                CycleId = 148,
                BarCodeId = 80,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 70,
                TaktTime = 163,
                StartedOn = new DateTime(2023, 08, 27, 15, 06, 56),
                FinishedOn = new DateTime(2023, 08, 27, 15, 08, 06)
            },
            [149] = new Cycle
            {
                CycleId = 149,
                BarCodeId = 80,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 80,
                TaktTime = 139,
                StartedOn = new DateTime(2023, 08, 27, 15, 08, 06),
                FinishedOn = new DateTime(2023, 08, 27, 15, 09, 26)
            },
            [150] = new Cycle
            {
                CycleId = 150,
                BarCodeId = 80,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 67,
                TaktTime = 145,
                StartedOn = new DateTime(2023, 08, 27, 15, 06, 56),
                FinishedOn = new DateTime(2023, 08, 27, 15, 08, 03)
            },
            [151] = new Cycle
            {
                CycleId = 151,
                BarCodeId = 81,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 184,
                StartedOn = new DateTime(2023, 08, 27, 15, 44, 41),
                FinishedOn = new DateTime(2023, 08, 27, 15, 46, 05)
            },
            [152] = new Cycle
            {
                CycleId = 152,
                BarCodeId = 81,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 71,
                TaktTime = 132,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 05),
                FinishedOn = new DateTime(2023, 08, 27, 15, 47, 16)
            },
            [153] = new Cycle
            {
                CycleId = 153,
                BarCodeId = 81,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 76,
                TaktTime = 142,
                StartedOn = new DateTime(2023, 08, 27, 15, 47, 16),
                FinishedOn = new DateTime(2023, 08, 27, 15, 48, 32)
            },
            [154] = new Cycle
            {
                CycleId = 154,
                BarCodeId = 82,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 144,
                StartedOn = new DateTime(2023, 08, 27, 15, 45, 43),
                FinishedOn = new DateTime(2023, 08, 27, 15, 47, 07)
            },
            [155] = new Cycle
            {
                CycleId = 155,
                BarCodeId = 82,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 96,
                TaktTime = 177,
                StartedOn = new DateTime(2023, 08, 27, 15, 47, 07),
                FinishedOn = new DateTime(2023, 08, 27, 15, 48, 43)
            },
            [156] = new Cycle
            {
                CycleId = 156,
                BarCodeId = 82,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 97,
                TaktTime = 194,
                StartedOn = new DateTime(2023, 08, 27, 15, 48, 43),
                FinishedOn = new DateTime(2023, 08, 27, 15, 50, 20)
            },
            [157] = new Cycle
            {
                CycleId = 157,
                BarCodeId = 83,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 95,
                TaktTime = 150,
                StartedOn = new DateTime(2023, 08, 27, 16, 25, 05),
                FinishedOn = new DateTime(2023, 08, 27, 16, 26, 40)
            },
            [158] = new Cycle
            {
                CycleId = 158,
                BarCodeId = 83,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 93,
                TaktTime = 185,
                StartedOn = new DateTime(2023, 08, 27, 16, 26, 40),
                FinishedOn = new DateTime(2023, 08, 27, 16, 28, 13)
            },
            [159] = new Cycle
            {
                CycleId = 159,
                BarCodeId = 83,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 74,
                TaktTime = 141,
                StartedOn = new DateTime(2023, 08, 27, 16, 25, 05),
                FinishedOn = new DateTime(2023, 08, 27, 16, 26, 19)
            },
            [160] = new Cycle
            {
                CycleId = 160,
                BarCodeId = 84,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 69,
                TaktTime = 143,
                StartedOn = new DateTime(2023, 08, 27, 09, 02, 38),
                FinishedOn = new DateTime(2023, 08, 27, 09, 03, 47)
            },
            [161] = new Cycle
            {
                CycleId = 161,
                BarCodeId = 84,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 81,
                TaktTime = 156,
                StartedOn = new DateTime(2023, 08, 27, 09, 03, 47),
                FinishedOn = new DateTime(2023, 08, 27, 09, 05, 08)
            },
            [162] = new Cycle
            {
                CycleId = 162,
                BarCodeId = 84,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 74,
                TaktTime = 171,
                StartedOn = new DateTime(2023, 08, 27, 09, 05, 08),
                FinishedOn = new DateTime(2023, 08, 27, 09, 06, 22)
            },
            [163] = new Cycle
            {
                CycleId = 163,
                BarCodeId = 85,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 88,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 09, 18, 14),
                FinishedOn = new DateTime(2023, 08, 27, 09, 19, 42)
            },
            [164] = new Cycle
            {
                CycleId = 164,
                BarCodeId = 85,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 52,
                TaktTime = 110,
                StartedOn = new DateTime(2023, 08, 27, 09, 19, 42),
                FinishedOn = new DateTime(2023, 08, 27, 09, 20, 34)
            },
            [165] = new Cycle
            {
                CycleId = 165,
                BarCodeId = 85,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 54,
                TaktTime = 115,
                StartedOn = new DateTime(2023, 08, 27, 09, 20, 34),
                FinishedOn = new DateTime(2023, 08, 27, 09, 21, 28)
            },
            [166] = new Cycle
            {
                CycleId = 166,
                BarCodeId = 86,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 96,
                TaktTime = 154,
                StartedOn = new DateTime(2023, 08, 27, 10, 58, 00),
                FinishedOn = new DateTime(2023, 08, 27, 10, 59, 36)
            },
            [167] = new Cycle
            {
                CycleId = 167,
                BarCodeId = 86,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 67,
                TaktTime = 127,
                StartedOn = new DateTime(2023, 08, 27, 10, 59, 36),
                FinishedOn = new DateTime(2023, 08, 27, 11, 00, 43)
            },
            [168] = new Cycle
            {
                CycleId = 168,
                BarCodeId = 86,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 91,
                TaktTime = 185,
                StartedOn = new DateTime(2023, 08, 27, 10, 58, 00),
                FinishedOn = new DateTime(2023, 08, 27, 10, 59, 31)
            },
            [169] = new Cycle
            {
                CycleId = 169,
                BarCodeId = 87,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 88,
                TaktTime = 153,
                StartedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                FinishedOn = new DateTime(2023, 08, 27, 15, 33, 20)
            },
            [170] = new Cycle
            {
                CycleId = 170,
                BarCodeId = 87,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 63,
                TaktTime = 128,
                StartedOn = new DateTime(2023, 08, 27, 15, 33, 20),
                FinishedOn = new DateTime(2023, 08, 27, 15, 34, 23)
            },
            [171] = new Cycle
            {
                CycleId = 171,
                BarCodeId = 87,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 68,
                TaktTime = 148,
                StartedOn = new DateTime(2023, 08, 27, 15, 34, 23),
                FinishedOn = new DateTime(2023, 08, 27, 15, 35, 31)
            },
            [172] = new Cycle
            {
                CycleId = 172,
                BarCodeId = 88,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 56,
                TaktTime = 111,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 00),
                FinishedOn = new DateTime(2023, 08, 27, 15, 46, 56)
            },
            [173] = new Cycle
            {
                CycleId = 173,
                BarCodeId = 88,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 100,
                TaktTime = 179,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 56),
                FinishedOn = new DateTime(2023, 08, 27, 15, 48, 36)
            },
            [174] = new Cycle
            {
                CycleId = 174,
                BarCodeId = 88,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 66,
                TaktTime = 137,
                StartedOn = new DateTime(2023, 08, 27, 15, 48, 36),
                FinishedOn = new DateTime(2023, 08, 27, 15, 49, 42)
            },
            [175] = new Cycle
            {
                CycleId = 175,
                BarCodeId = 89,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 63,
                TaktTime = 155,
                StartedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                FinishedOn = new DateTime(2023, 08, 27, 15, 32, 55)
            },
            [176] = new Cycle
            {
                CycleId = 176,
                BarCodeId = 89,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 59,
                TaktTime = 155,
                StartedOn = new DateTime(2023, 08, 27, 15, 32, 55),
                FinishedOn = new DateTime(2023, 08, 27, 15, 33, 54)
            },
            [177] = new Cycle
            {
                CycleId = 177,
                BarCodeId = 89,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 96,
                TaktTime = 169,
                StartedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                FinishedOn = new DateTime(2023, 08, 27, 15, 33, 28)
            },
            [178] = new Cycle
            {
                CycleId = 178,
                BarCodeId = 90,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 63,
                TaktTime = 158,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 00),
                FinishedOn = new DateTime(2023, 08, 27, 15, 47, 03)
            },
            [179] = new Cycle
            {
                CycleId = 179,
                BarCodeId = 90,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 89,
                TaktTime = 168,
                StartedOn = new DateTime(2023, 08, 27, 15, 47, 03),
                FinishedOn = new DateTime(2023, 08, 27, 15, 48, 32)
            },
            [180] = new Cycle
            {
                CycleId = 180,
                BarCodeId = 90,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 82,
                TaktTime = 178,
                StartedOn = new DateTime(2023, 08, 27, 15, 48, 32),
                FinishedOn = new DateTime(2023, 08, 27, 15, 49, 54)
            },
            [181] = new Cycle
            {
                CycleId = 181,
                BarCodeId = 91,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 193,
                StartedOn = new DateTime(2023, 08, 27, 15, 44, 41),
                FinishedOn = new DateTime(2023, 08, 27, 15, 46, 20)
            },
            [182] = new Cycle
            {
                CycleId = 182,
                BarCodeId = 91,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 64,
                TaktTime = 137,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 20),
                FinishedOn = new DateTime(2023, 08, 27, 15, 47, 24)
            },
            [183] = new Cycle
            {
                CycleId = 183,
                BarCodeId = 91,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 65,
                TaktTime = 155,
                StartedOn = new DateTime(2023, 08, 27, 15, 47, 24),
                FinishedOn = new DateTime(2023, 08, 27, 15, 48, 29)
            },
            [184] = new Cycle
            {
                CycleId = 184,
                BarCodeId = 92,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 69,
                TaktTime = 139,
                StartedOn = new DateTime(2023, 08, 27, 15, 45, 43),
                FinishedOn = new DateTime(2023, 08, 27, 15, 46, 52)
            },
            [185] = new Cycle
            {
                CycleId = 185,
                BarCodeId = 92,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 70,
                TaktTime = 157,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 52),
                FinishedOn = new DateTime(2023, 08, 27, 15, 48, 02)
            },
            [186] = new Cycle
            {
                CycleId = 186,
                BarCodeId = 92,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 72,
                TaktTime = 130,
                StartedOn = new DateTime(2023, 08, 27, 15, 45, 43),
                FinishedOn = new DateTime(2023, 08, 27, 15, 46, 55)
            },
            [187] = new Cycle
            {
                CycleId = 187,
                BarCodeId = 93,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 100,
                TaktTime = 196,
                StartedOn = new DateTime(2023, 08, 27, 16, 25, 05),
                FinishedOn = new DateTime(2023, 08, 27, 16, 26, 45)
            },
            [188] = new Cycle
            {
                CycleId = 188,
                BarCodeId = 93,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 100,
                TaktTime = 199,
                StartedOn = new DateTime(2023, 08, 27, 16, 26, 45),
                FinishedOn = new DateTime(2023, 08, 27, 16, 28, 25)
            },
            [189] = new Cycle
            {
                CycleId = 189,
                BarCodeId = 93,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 98,
                TaktTime = 195,
                StartedOn = new DateTime(2023, 08, 27, 16, 28, 25),
                FinishedOn = new DateTime(2023, 08, 27, 16, 30, 03)
            },
            [190] = new Cycle
            {
                CycleId = 190,
                BarCodeId = 94,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 97,
                TaktTime = 182,
                StartedOn = new DateTime(2023, 08, 27, 09, 02, 38),
                FinishedOn = new DateTime(2023, 08, 27, 09, 04, 15)
            },
            [191] = new Cycle
            {
                CycleId = 191,
                BarCodeId = 94,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 86,
                TaktTime = 165,
                StartedOn = new DateTime(2023, 08, 27, 09, 04, 15),
                FinishedOn = new DateTime(2023, 08, 27, 09, 05, 41)
            },
            [192] = new Cycle
            {
                CycleId = 192,
                BarCodeId = 94,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 62,
                TaktTime = 137,
                StartedOn = new DateTime(2023, 08, 27, 09, 05, 41),
                FinishedOn = new DateTime(2023, 08, 27, 09, 06, 43)
            },
            [193] = new Cycle
            {
                CycleId = 193,
                BarCodeId = 95,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 71,
                TaktTime = 140,
                StartedOn = new DateTime(2023, 08, 27, 09, 18, 14),
                FinishedOn = new DateTime(2023, 08, 27, 09, 19, 25)
            },
            [194] = new Cycle
            {
                CycleId = 194,
                BarCodeId = 95,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 160,
                StartedOn = new DateTime(2023, 08, 27, 09, 19, 25),
                FinishedOn = new DateTime(2023, 08, 27, 09, 20, 49)
            },
            [195] = new Cycle
            {
                CycleId = 195,
                BarCodeId = 95,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 94,
                TaktTime = 183,
                StartedOn = new DateTime(2023, 08, 27, 09, 18, 14),
                FinishedOn = new DateTime(2023, 08, 27, 09, 19, 48)
            },
            [196] = new Cycle
            {
                CycleId = 196,
                BarCodeId = 96,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 92,
                TaktTime = 148,
                StartedOn = new DateTime(2023, 08, 27, 10, 58, 00),
                FinishedOn = new DateTime(2023, 08, 27, 10, 59, 32)
            },
            [197] = new Cycle
            {
                CycleId = 197,
                BarCodeId = 96,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 77,
                TaktTime = 156,
                StartedOn = new DateTime(2023, 08, 27, 10, 59, 32),
                FinishedOn = new DateTime(2023, 08, 27, 11, 00, 49)
            },
            [198] = new Cycle
            {
                CycleId = 198,
                BarCodeId = 96,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 58,
                TaktTime = 116,
                StartedOn = new DateTime(2023, 08, 27, 11, 00, 49),
                FinishedOn = new DateTime(2023, 08, 27, 11, 01, 47)
            },
            [199] = new Cycle
            {
                CycleId = 199,
                BarCodeId = 97,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 70,
                TaktTime = 163,
                StartedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                FinishedOn = new DateTime(2023, 08, 27, 15, 33, 02)
            },
            [200] = new Cycle
            {
                CycleId = 200,
                BarCodeId = 97,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 64,
                TaktTime = 123,
                StartedOn = new DateTime(2023, 08, 27, 15, 33, 02),
                FinishedOn = new DateTime(2023, 08, 27, 15, 34, 06)
            },
            [201] = new Cycle
            {
                CycleId = 201,
                BarCodeId = 97,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 98,
                TaktTime = 157,
                StartedOn = new DateTime(2023, 08, 27, 15, 34, 06),
                FinishedOn = new DateTime(2023, 08, 27, 15, 35, 44)
            },
            [202] = new Cycle
            {
                CycleId = 202,
                BarCodeId = 98,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 85,
                TaktTime = 147,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 00),
                FinishedOn = new DateTime(2023, 08, 27, 15, 47, 25)
            },
            [203] = new Cycle
            {
                CycleId = 203,
                BarCodeId = 98,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 61,
                TaktTime = 122,
                StartedOn = new DateTime(2023, 08, 27, 15, 47, 25),
                FinishedOn = new DateTime(2023, 08, 27, 15, 48, 26)
            },
            [204] = new Cycle
            {
                CycleId = 204,
                BarCodeId = 98,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 60,
                TaktTime = 132,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 00),
                FinishedOn = new DateTime(2023, 08, 27, 15, 47, 00)
            },
            [205] = new Cycle
            {
                CycleId = 205,
                BarCodeId = 99,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 88,
                TaktTime = 170,
                StartedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                FinishedOn = new DateTime(2023, 08, 27, 15, 33, 20)
            },
            [206] = new Cycle
            {
                CycleId = 206,
                BarCodeId = 99,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 63,
                TaktTime = 137,
                StartedOn = new DateTime(2023, 08, 27, 15, 33, 20),
                FinishedOn = new DateTime(2023, 08, 27, 15, 34, 23)
            },
            [207] = new Cycle
            {
                CycleId = 207,
                BarCodeId = 99,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 81,
                TaktTime = 134,
                StartedOn = new DateTime(2023, 08, 27, 15, 34, 23),
                FinishedOn = new DateTime(2023, 08, 27, 15, 35, 44)
            },
            [208] = new Cycle
            {
                CycleId = 208,
                BarCodeId = 100,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 81,
                TaktTime = 139,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 00),
                FinishedOn = new DateTime(2023, 08, 27, 15, 47, 21)
            },
            [209] = new Cycle
            {
                CycleId = 209,
                BarCodeId = 100,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 55,
                TaktTime = 140,
                StartedOn = new DateTime(2023, 08, 27, 15, 47, 21),
                FinishedOn = new DateTime(2023, 08, 27, 15, 48, 16)
            },
            [210] = new Cycle
            {
                CycleId = 210,
                BarCodeId = 100,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 60,
                TaktTime = 139,
                StartedOn = new DateTime(2023, 08, 27, 15, 48, 16),
                FinishedOn = new DateTime(2023, 08, 27, 15, 49, 16)
            },
            [211] = new Cycle
            {
                CycleId = 211,
                BarCodeId = 101,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 65,
                TaktTime = 135,
                StartedOn = new DateTime(2023, 08, 27, 00, 49, 12),
                FinishedOn = new DateTime(2023, 08, 27, 00, 50, 17)
            },
            [212] = new Cycle
            {
                CycleId = 212,
                BarCodeId = 102,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 97,
                TaktTime = 154,
                StartedOn = new DateTime(2023, 08, 27, 02, 54, 19),
                FinishedOn = new DateTime(2023, 08, 27, 02, 55, 56)
            },
            [213] = new Cycle
            {
                CycleId = 213,
                BarCodeId = 103,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 83,
                TaktTime = 143,
                StartedOn = new DateTime(2023, 08, 27, 08, 18, 00),
                FinishedOn = new DateTime(2023, 08, 27, 08, 19, 23)
            },
            [214] = new Cycle
            {
                CycleId = 214,
                BarCodeId = 104,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 92,
                TaktTime = 187,
                StartedOn = new DateTime(2023, 08, 27, 03, 41, 07),
                FinishedOn = new DateTime(2023, 08, 27, 03, 42, 39)
            },
            [215] = new Cycle
            {
                CycleId = 215,
                BarCodeId = 105,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 88,
                TaktTime = 186,
                StartedOn = new DateTime(2023, 08, 27, 09, 27, 28),
                FinishedOn = new DateTime(2023, 08, 27, 09, 28, 56)
            },
            [216] = new Cycle
            {
                CycleId = 216,
                BarCodeId = 106,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 90,
                TaktTime = 143,
                StartedOn = new DateTime(2023, 08, 27, 10, 01, 55),
                FinishedOn = new DateTime(2023, 08, 27, 10, 03, 25)
            },
            [217] = new Cycle
            {
                CycleId = 217,
                BarCodeId = 107,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 91,
                TaktTime = 149,
                StartedOn = new DateTime(2023, 08, 27, 11, 50, 57),
                FinishedOn = new DateTime(2023, 08, 27, 11, 52, 28)
            },
            [218] = new Cycle
            {
                CycleId = 218,
                BarCodeId = 108,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 95,
                TaktTime = 153,
                StartedOn = new DateTime(2023, 08, 27, 12, 03, 04),
                FinishedOn = new DateTime(2023, 08, 27, 12, 04, 39)
            },
            [219] = new Cycle
            {
                CycleId = 219,
                BarCodeId = 109,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 90,
                TaktTime = 185,
                StartedOn = new DateTime(2023, 08, 27, 12, 20, 08),
                FinishedOn = new DateTime(2023, 08, 27, 12, 21, 38)
            },
            [220] = new Cycle
            {
                CycleId = 220,
                BarCodeId = 110,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 94,
                TaktTime = 153,
                StartedOn = new DateTime(2023, 08, 27, 12, 19, 35),
                FinishedOn = new DateTime(2023, 08, 27, 12, 21, 09)
            },
            [221] = new Cycle
            {
                CycleId = 221,
                BarCodeId = 111,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 77,
                TaktTime = 160,
                StartedOn = new DateTime(2023, 08, 27, 09, 22, 03),
                FinishedOn = new DateTime(2023, 08, 27, 09, 23, 20)
            },
            [222] = new Cycle
            {
                CycleId = 222,
                BarCodeId = 112,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 59,
                TaktTime = 120,
                StartedOn = new DateTime(2023, 08, 27, 09, 25, 05),
                FinishedOn = new DateTime(2023, 08, 27, 09, 26, 04)
            },
            [223] = new Cycle
            {
                CycleId = 223,
                BarCodeId = 113,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 51,
                TaktTime = 118,
                StartedOn = new DateTime(2023, 08, 27, 12, 09, 01),
                FinishedOn = new DateTime(2023, 08, 27, 12, 09, 52)
            },
            [224] = new Cycle
            {
                CycleId = 224,
                BarCodeId = 114,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 93,
                TaktTime = 166,
                StartedOn = new DateTime(2023, 08, 27, 12, 12, 01),
                FinishedOn = new DateTime(2023, 08, 27, 12, 13, 34)
            },
            [225] = new Cycle
            {
                CycleId = 225,
                BarCodeId = 115,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 96,
                TaktTime = 148,
                StartedOn = new DateTime(2023, 08, 27, 12, 16, 03),
                FinishedOn = new DateTime(2023, 08, 27, 12, 17, 39)
            },
            [226] = new Cycle
            {
                CycleId = 226,
                BarCodeId = 116,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 98,
                TaktTime = 148,
                StartedOn = new DateTime(2023, 08, 27, 12, 19, 31),
                FinishedOn = new DateTime(2023, 08, 27, 12, 21, 09)
            },
            [227] = new Cycle
            {
                CycleId = 227,
                BarCodeId = 116,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 166,
                StartedOn = new DateTime(2023, 08, 27, 12, 21, 09),
                FinishedOn = new DateTime(2023, 08, 27, 12, 22, 48)
            },
            [228] = new Cycle
            {
                CycleId = 228,
                BarCodeId = 117,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 75,
                TaktTime = 145,
                StartedOn = new DateTime(2023, 08, 27, 12, 20, 56),
                FinishedOn = new DateTime(2023, 08, 27, 12, 22, 11)
            },
            [229] = new Cycle
            {
                CycleId = 229,
                BarCodeId = 117,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 59,
                TaktTime = 109,
                StartedOn = new DateTime(2023, 08, 27, 12, 22, 11),
                FinishedOn = new DateTime(2023, 08, 27, 12, 23, 10)
            },
            [230] = new Cycle
            {
                CycleId = 230,
                BarCodeId = 118,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 58,
                TaktTime = 145,
                StartedOn = new DateTime(2023, 08, 27, 12, 22, 36),
                FinishedOn = new DateTime(2023, 08, 27, 12, 23, 34)
            },
            [231] = new Cycle
            {
                CycleId = 231,
                BarCodeId = 118,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 186,
                StartedOn = new DateTime(2023, 08, 27, 12, 23, 34),
                FinishedOn = new DateTime(2023, 08, 27, 12, 25, 13)
            },
            [232] = new Cycle
            {
                CycleId = 232,
                BarCodeId = 119,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 12, 21, 23),
                FinishedOn = new DateTime(2023, 08, 27, 12, 22, 47)
            },
            [233] = new Cycle
            {
                CycleId = 233,
                BarCodeId = 119,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 76,
                TaktTime = 147,
                StartedOn = new DateTime(2023, 08, 27, 12, 22, 47),
                FinishedOn = new DateTime(2023, 08, 27, 12, 24, 03)
            },
            [234] = new Cycle
            {
                CycleId = 234,
                BarCodeId = 120,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 50,
                TaktTime = 131,
                StartedOn = new DateTime(2023, 08, 27, 12, 22, 16),
                FinishedOn = new DateTime(2023, 08, 27, 12, 23, 06)
            },
            [235] = new Cycle
            {
                CycleId = 235,
                BarCodeId = 120,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 83,
                TaktTime = 181,
                StartedOn = new DateTime(2023, 08, 27, 12, 23, 06),
                FinishedOn = new DateTime(2023, 08, 27, 12, 24, 29)
            },
            [236] = new Cycle
            {
                CycleId = 236,
                BarCodeId = 121,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 12, 20, 02),
                FinishedOn = new DateTime(2023, 08, 27, 12, 21, 26)
            },
            [237] = new Cycle
            {
                CycleId = 237,
                BarCodeId = 121,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 89,
                TaktTime = 149,
                StartedOn = new DateTime(2023, 08, 27, 12, 21, 26),
                FinishedOn = new DateTime(2023, 08, 27, 12, 22, 55)
            },
            [238] = new Cycle
            {
                CycleId = 238,
                BarCodeId = 122,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 85,
                TaktTime = 154,
                StartedOn = new DateTime(2023, 08, 27, 12, 21, 58),
                FinishedOn = new DateTime(2023, 08, 27, 12, 23, 23)
            },
            [239] = new Cycle
            {
                CycleId = 239,
                BarCodeId = 122,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 94,
                TaktTime = 180,
                StartedOn = new DateTime(2023, 08, 27, 12, 23, 23),
                FinishedOn = new DateTime(2023, 08, 27, 12, 24, 57)
            },
            [240] = new Cycle
            {
                CycleId = 240,
                BarCodeId = 123,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 88,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 13, 15, 45),
                FinishedOn = new DateTime(2023, 08, 27, 13, 17, 13)
            },
            [241] = new Cycle
            {
                CycleId = 241,
                BarCodeId = 123,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 82,
                TaktTime = 169,
                StartedOn = new DateTime(2023, 08, 27, 13, 17, 13),
                FinishedOn = new DateTime(2023, 08, 27, 13, 18, 35)
            },
            [242] = new Cycle
            {
                CycleId = 242,
                BarCodeId = 124,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 87,
                TaktTime = 156,
                StartedOn = new DateTime(2023, 08, 27, 13, 26, 29),
                FinishedOn = new DateTime(2023, 08, 27, 13, 27, 56)
            },
            [243] = new Cycle
            {
                CycleId = 243,
                BarCodeId = 124,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 75,
                TaktTime = 125,
                StartedOn = new DateTime(2023, 08, 27, 13, 27, 56),
                FinishedOn = new DateTime(2023, 08, 27, 13, 29, 11)
            },
            [244] = new Cycle
            {
                CycleId = 244,
                BarCodeId = 125,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 81,
                TaktTime = 157,
                StartedOn = new DateTime(2023, 08, 27, 13, 31, 55),
                FinishedOn = new DateTime(2023, 08, 27, 13, 33, 16)
            },
            [245] = new Cycle
            {
                CycleId = 245,
                BarCodeId = 125,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 94,
                TaktTime = 147,
                StartedOn = new DateTime(2023, 08, 27, 13, 33, 16),
                FinishedOn = new DateTime(2023, 08, 27, 13, 34, 50)
            },
            [246] = new Cycle
            {
                CycleId = 246,
                BarCodeId = 126,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 180,
                StartedOn = new DateTime(2023, 08, 27, 13, 37, 13),
                FinishedOn = new DateTime(2023, 08, 27, 13, 38, 37)
            },
            [247] = new Cycle
            {
                CycleId = 247,
                BarCodeId = 126,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 65,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 13, 38, 37),
                FinishedOn = new DateTime(2023, 08, 27, 13, 39, 42)
            },
            [248] = new Cycle
            {
                CycleId = 248,
                BarCodeId = 127,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 62,
                TaktTime = 134,
                StartedOn = new DateTime(2023, 08, 27, 15, 03, 53),
                FinishedOn = new DateTime(2023, 08, 27, 15, 04, 55)
            },
            [249] = new Cycle
            {
                CycleId = 249,
                BarCodeId = 127,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 59,
                TaktTime = 150,
                StartedOn = new DateTime(2023, 08, 27, 15, 04, 55),
                FinishedOn = new DateTime(2023, 08, 27, 15, 05, 54)
            },
            [250] = new Cycle
            {
                CycleId = 250,
                BarCodeId = 128,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 69,
                TaktTime = 133,
                StartedOn = new DateTime(2023, 08, 27, 16, 54, 58),
                FinishedOn = new DateTime(2023, 08, 27, 16, 56, 07)
            },
            [251] = new Cycle
            {
                CycleId = 251,
                BarCodeId = 128,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 75,
                TaktTime = 141,
                StartedOn = new DateTime(2023, 08, 27, 16, 56, 07),
                FinishedOn = new DateTime(2023, 08, 27, 16, 57, 22)
            },
            [252] = new Cycle
            {
                CycleId = 252,
                BarCodeId = 129,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 85,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 09, 52, 09),
                FinishedOn = new DateTime(2023, 08, 27, 09, 53, 34)
            },
            [253] = new Cycle
            {
                CycleId = 253,
                BarCodeId = 129,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 87,
                TaktTime = 185,
                StartedOn = new DateTime(2023, 08, 27, 09, 53, 34),
                FinishedOn = new DateTime(2023, 08, 27, 09, 55, 01)
            },
            [254] = new Cycle
            {
                CycleId = 254,
                BarCodeId = 130,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 58,
                TaktTime = 149,
                StartedOn = new DateTime(2023, 08, 27, 09, 54, 09),
                FinishedOn = new DateTime(2023, 08, 27, 09, 55, 07)
            },
            [255] = new Cycle
            {
                CycleId = 255,
                BarCodeId = 130,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 83,
                TaktTime = 159,
                StartedOn = new DateTime(2023, 08, 27, 09, 55, 07),
                FinishedOn = new DateTime(2023, 08, 27, 09, 56, 30)
            },
            [256] = new Cycle
            {
                CycleId = 256,
                BarCodeId = 131,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 65,
                TaktTime = 139,
                StartedOn = new DateTime(2023, 08, 27, 10, 00, 17),
                FinishedOn = new DateTime(2023, 08, 27, 10, 01, 22)
            },
            [257] = new Cycle
            {
                CycleId = 257,
                BarCodeId = 131,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 58,
                TaktTime = 124,
                StartedOn = new DateTime(2023, 08, 27, 10, 01, 22),
                FinishedOn = new DateTime(2023, 08, 27, 10, 02, 20)
            },
            [258] = new Cycle
            {
                CycleId = 258,
                BarCodeId = 131,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 57,
                TaktTime = 122,
                StartedOn = new DateTime(2023, 08, 27, 10, 00, 17),
                FinishedOn = new DateTime(2023, 08, 27, 10, 01, 14)
            },
            [259] = new Cycle
            {
                CycleId = 259,
                BarCodeId = 132,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 66,
                TaktTime = 124,
                StartedOn = new DateTime(2023, 08, 27, 10, 01, 14),
                FinishedOn = new DateTime(2023, 08, 27, 10, 02, 20)
            },
            [260] = new Cycle
            {
                CycleId = 260,
                BarCodeId = 132,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 80,
                TaktTime = 156,
                StartedOn = new DateTime(2023, 08, 27, 10, 02, 20),
                FinishedOn = new DateTime(2023, 08, 27, 10, 03, 40)
            },
            [261] = new Cycle
            {
                CycleId = 261,
                BarCodeId = 132,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 168,
                StartedOn = new DateTime(2023, 08, 27, 10, 03, 40),
                FinishedOn = new DateTime(2023, 08, 27, 10, 05, 04)
            },
            [262] = new Cycle
            {
                CycleId = 262,
                BarCodeId = 133,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 78,
                TaktTime = 159,
                StartedOn = new DateTime(2023, 08, 27, 10, 01, 50),
                FinishedOn = new DateTime(2023, 08, 27, 10, 03, 08)
            },
            [263] = new Cycle
            {
                CycleId = 263,
                BarCodeId = 133,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 70,
                TaktTime = 153,
                StartedOn = new DateTime(2023, 08, 27, 10, 03, 08),
                FinishedOn = new DateTime(2023, 08, 27, 10, 04, 18)
            },
            [264] = new Cycle
            {
                CycleId = 264,
                BarCodeId = 133,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 73,
                TaktTime = 141,
                StartedOn = new DateTime(2023, 08, 27, 10, 04, 18),
                FinishedOn = new DateTime(2023, 08, 27, 10, 05, 31)
            },
            [265] = new Cycle
            {
                CycleId = 265,
                BarCodeId = 134,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 64,
                TaktTime = 142,
                StartedOn = new DateTime(2023, 08, 27, 10, 03, 23),
                FinishedOn = new DateTime(2023, 08, 27, 10, 04, 27)
            },
            [266] = new Cycle
            {
                CycleId = 266,
                BarCodeId = 134,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 98,
                TaktTime = 166,
                StartedOn = new DateTime(2023, 08, 27, 10, 04, 27),
                FinishedOn = new DateTime(2023, 08, 27, 10, 06, 05)
            },
            [267] = new Cycle
            {
                CycleId = 267,
                BarCodeId = 134,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 59,
                TaktTime = 118,
                StartedOn = new DateTime(2023, 08, 27, 10, 03, 23),
                FinishedOn = new DateTime(2023, 08, 27, 10, 04, 22)
            },
            [268] = new Cycle
            {
                CycleId = 268,
                BarCodeId = 135,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 59,
                TaktTime = 159,
                StartedOn = new DateTime(2023, 08, 27, 10, 05, 46),
                FinishedOn = new DateTime(2023, 08, 27, 10, 06, 45)
            },
            [269] = new Cycle
            {
                CycleId = 269,
                BarCodeId = 135,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 71,
                TaktTime = 163,
                StartedOn = new DateTime(2023, 08, 27, 10, 06, 45),
                FinishedOn = new DateTime(2023, 08, 27, 10, 07, 56)
            },
            [270] = new Cycle
            {
                CycleId = 270,
                BarCodeId = 135,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 61,
                TaktTime = 120,
                StartedOn = new DateTime(2023, 08, 27, 10, 07, 56),
                FinishedOn = new DateTime(2023, 08, 27, 10, 08, 57)
            },
            [271] = new Cycle
            {
                CycleId = 271,
                BarCodeId = 136,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 90,
                TaktTime = 167,
                StartedOn = new DateTime(2023, 08, 27, 10, 06, 13),
                FinishedOn = new DateTime(2023, 08, 27, 10, 07, 43)
            },
            [272] = new Cycle
            {
                CycleId = 272,
                BarCodeId = 136,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 78,
                TaktTime = 151,
                StartedOn = new DateTime(2023, 08, 27, 10, 07, 43),
                FinishedOn = new DateTime(2023, 08, 27, 10, 09, 01)
            },
            [273] = new Cycle
            {
                CycleId = 273,
                BarCodeId = 136,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 95,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 10, 09, 01),
                FinishedOn = new DateTime(2023, 08, 27, 10, 10, 36)
            },
            [274] = new Cycle
            {
                CycleId = 274,
                BarCodeId = 137,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 86,
                TaktTime = 154,
                StartedOn = new DateTime(2023, 08, 27, 10, 06, 27),
                FinishedOn = new DateTime(2023, 08, 27, 10, 07, 53)
            },
            [275] = new Cycle
            {
                CycleId = 275,
                BarCodeId = 137,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 83,
                TaktTime = 133,
                StartedOn = new DateTime(2023, 08, 27, 10, 07, 53),
                FinishedOn = new DateTime(2023, 08, 27, 10, 09, 16)
            },
            [276] = new Cycle
            {
                CycleId = 276,
                BarCodeId = 137,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 78,
                TaktTime = 136,
                StartedOn = new DateTime(2023, 08, 27, 10, 06, 27),
                FinishedOn = new DateTime(2023, 08, 27, 10, 07, 45)
            },
            [277] = new Cycle
            {
                CycleId = 277,
                BarCodeId = 138,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 62,
                TaktTime = 122,
                StartedOn = new DateTime(2023, 08, 27, 10, 07, 47),
                FinishedOn = new DateTime(2023, 08, 27, 10, 08, 49)
            },
            [278] = new Cycle
            {
                CycleId = 278,
                BarCodeId = 138,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 82,
                TaktTime = 141,
                StartedOn = new DateTime(2023, 08, 27, 10, 08, 49),
                FinishedOn = new DateTime(2023, 08, 27, 10, 10, 11)
            },
            [279] = new Cycle
            {
                CycleId = 279,
                BarCodeId = 138,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 64,
                TaktTime = 129,
                StartedOn = new DateTime(2023, 08, 27, 10, 10, 11),
                FinishedOn = new DateTime(2023, 08, 27, 10, 11, 15)
            },
            [280] = new Cycle
            {
                CycleId = 280,
                BarCodeId = 139,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 76,
                TaktTime = 160,
                StartedOn = new DateTime(2023, 08, 27, 10, 13, 47),
                FinishedOn = new DateTime(2023, 08, 27, 10, 15, 03)
            },
            [281] = new Cycle
            {
                CycleId = 281,
                BarCodeId = 139,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 96,
                TaktTime = 188,
                StartedOn = new DateTime(2023, 08, 27, 10, 15, 03),
                FinishedOn = new DateTime(2023, 08, 27, 10, 16, 39)
            },
            [282] = new Cycle
            {
                CycleId = 282,
                BarCodeId = 139,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 10, 16, 39),
                FinishedOn = new DateTime(2023, 08, 27, 10, 18, 03)
            },
            [283] = new Cycle
            {
                CycleId = 283,
                BarCodeId = 140,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 183,
                StartedOn = new DateTime(2023, 08, 27, 10, 13, 59),
                FinishedOn = new DateTime(2023, 08, 27, 10, 15, 23)
            },
            [284] = new Cycle
            {
                CycleId = 284,
                BarCodeId = 140,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 87,
                TaktTime = 154,
                StartedOn = new DateTime(2023, 08, 27, 10, 15, 23),
                FinishedOn = new DateTime(2023, 08, 27, 10, 16, 50)
            },
            [285] = new Cycle
            {
                CycleId = 285,
                BarCodeId = 140,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 72,
                TaktTime = 171,
                StartedOn = new DateTime(2023, 08, 27, 10, 13, 59),
                FinishedOn = new DateTime(2023, 08, 27, 10, 15, 11)
            },
            [286] = new Cycle
            {
                CycleId = 286,
                BarCodeId = 141,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 88,
                TaktTime = 157,
                StartedOn = new DateTime(2023, 08, 27, 10, 16, 39),
                FinishedOn = new DateTime(2023, 08, 27, 10, 18, 07)
            },
            [287] = new Cycle
            {
                CycleId = 287,
                BarCodeId = 141,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 74,
                TaktTime = 164,
                StartedOn = new DateTime(2023, 08, 27, 10, 18, 07),
                FinishedOn = new DateTime(2023, 08, 27, 10, 19, 21)
            },
            [288] = new Cycle
            {
                CycleId = 288,
                BarCodeId = 141,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 88,
                TaktTime = 165,
                StartedOn = new DateTime(2023, 08, 27, 10, 19, 21),
                FinishedOn = new DateTime(2023, 08, 27, 10, 20, 49)
            },
            [289] = new Cycle
            {
                CycleId = 289,
                BarCodeId = 142,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 90,
                TaktTime = 140,
                StartedOn = new DateTime(2023, 08, 27, 10, 35, 21),
                FinishedOn = new DateTime(2023, 08, 27, 10, 36, 51)
            },
            [290] = new Cycle
            {
                CycleId = 290,
                BarCodeId = 142,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 64,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 10, 36, 51),
                FinishedOn = new DateTime(2023, 08, 27, 10, 37, 55)
            },
            [291] = new Cycle
            {
                CycleId = 291,
                BarCodeId = 142,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 50,
                TaktTime = 138,
                StartedOn = new DateTime(2023, 08, 27, 10, 37, 55),
                FinishedOn = new DateTime(2023, 08, 27, 10, 38, 45)
            },
            [292] = new Cycle
            {
                CycleId = 292,
                BarCodeId = 143,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 55,
                TaktTime = 123,
                StartedOn = new DateTime(2023, 08, 27, 10, 35, 49),
                FinishedOn = new DateTime(2023, 08, 27, 10, 36, 44)
            },
            [293] = new Cycle
            {
                CycleId = 293,
                BarCodeId = 143,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 58,
                TaktTime = 108,
                StartedOn = new DateTime(2023, 08, 27, 10, 36, 44),
                FinishedOn = new DateTime(2023, 08, 27, 10, 37, 42)
            },
            [294] = new Cycle
            {
                CycleId = 294,
                BarCodeId = 143,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 64,
                TaktTime = 164,
                StartedOn = new DateTime(2023, 08, 27, 10, 35, 49),
                FinishedOn = new DateTime(2023, 08, 27, 10, 36, 53)
            },
            [295] = new Cycle
            {
                CycleId = 295,
                BarCodeId = 144,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 66,
                TaktTime = 145,
                StartedOn = new DateTime(2023, 08, 27, 10, 43, 32),
                FinishedOn = new DateTime(2023, 08, 27, 10, 44, 38)
            },
            [296] = new Cycle
            {
                CycleId = 296,
                BarCodeId = 144,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 53,
                TaktTime = 119,
                StartedOn = new DateTime(2023, 08, 27, 10, 44, 38),
                FinishedOn = new DateTime(2023, 08, 27, 10, 45, 31)
            },
            [297] = new Cycle
            {
                CycleId = 297,
                BarCodeId = 144,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 76,
                TaktTime = 172,
                StartedOn = new DateTime(2023, 08, 27, 10, 45, 31),
                FinishedOn = new DateTime(2023, 08, 27, 10, 46, 47)
            },
            [298] = new Cycle
            {
                CycleId = 298,
                BarCodeId = 145,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 93,
                TaktTime = 179,
                StartedOn = new DateTime(2023, 08, 27, 10, 49, 58),
                FinishedOn = new DateTime(2023, 08, 27, 10, 51, 31)
            },
            [299] = new Cycle
            {
                CycleId = 299,
                BarCodeId = 145,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 82,
                TaktTime = 173,
                StartedOn = new DateTime(2023, 08, 27, 10, 51, 31),
                FinishedOn = new DateTime(2023, 08, 27, 10, 52, 53)
            },
            [300] = new Cycle
            {
                CycleId = 300,
                BarCodeId = 145,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 77,
                TaktTime = 164,
                StartedOn = new DateTime(2023, 08, 27, 10, 52, 53),
                FinishedOn = new DateTime(2023, 08, 27, 10, 54, 10)
            },
            [301] = new Cycle
            {
                CycleId = 301,
                BarCodeId = 146,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 97,
                TaktTime = 192,
                StartedOn = new DateTime(2023, 08, 27, 10, 50, 56),
                FinishedOn = new DateTime(2023, 08, 27, 10, 52, 33)
            },
            [302] = new Cycle
            {
                CycleId = 302,
                BarCodeId = 147,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 91,
                TaktTime = 150,
                StartedOn = new DateTime(2023, 08, 27, 10, 51, 25),
                FinishedOn = new DateTime(2023, 08, 27, 10, 52, 56)
            },
            [303] = new Cycle
            {
                CycleId = 303,
                BarCodeId = 148,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 96,
                TaktTime = 147,
                StartedOn = new DateTime(2023, 08, 27, 10, 53, 08),
                FinishedOn = new DateTime(2023, 08, 27, 10, 54, 44)
            },
            [304] = new Cycle
            {
                CycleId = 304,
                BarCodeId = 149,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 57,
                TaktTime = 118,
                StartedOn = new DateTime(2023, 08, 27, 10, 53, 32),
                FinishedOn = new DateTime(2023, 08, 27, 10, 54, 29)
            },
            [305] = new Cycle
            {
                CycleId = 305,
                BarCodeId = 150,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 53,
                TaktTime = 125,
                StartedOn = new DateTime(2023, 08, 27, 10, 59, 49),
                FinishedOn = new DateTime(2023, 08, 27, 11, 00, 42)
            },
            [306] = new Cycle
            {
                CycleId = 306,
                BarCodeId = 151,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 86,
                TaktTime = 167,
                StartedOn = new DateTime(2023, 08, 27, 11, 01, 12),
                FinishedOn = new DateTime(2023, 08, 27, 11, 02, 38)
            },
            [307] = new Cycle
            {
                CycleId = 307,
                BarCodeId = 152,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 52,
                TaktTime = 128,
                StartedOn = new DateTime(2023, 08, 27, 11, 01, 23),
                FinishedOn = new DateTime(2023, 08, 27, 11, 02, 15)
            },
            [308] = new Cycle
            {
                CycleId = 308,
                BarCodeId = 153,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 74,
                TaktTime = 138,
                StartedOn = new DateTime(2023, 08, 27, 12, 27, 18),
                FinishedOn = new DateTime(2023, 08, 27, 12, 28, 32)
            },
            [309] = new Cycle
            {
                CycleId = 309,
                BarCodeId = 154,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 81,
                TaktTime = 168,
                StartedOn = new DateTime(2023, 08, 27, 13, 36, 40),
                FinishedOn = new DateTime(2023, 08, 27, 13, 38, 01)
            },
            [310] = new Cycle
            {
                CycleId = 310,
                BarCodeId = 155,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 50,
                TaktTime = 130,
                StartedOn = new DateTime(2023, 08, 27, 14, 01, 00),
                FinishedOn = new DateTime(2023, 08, 27, 14, 01, 50)
            },
            [311] = new Cycle
            {
                CycleId = 311,
                BarCodeId = 156,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 92,
                TaktTime = 163,
                StartedOn = new DateTime(2023, 08, 27, 16, 25, 12),
                FinishedOn = new DateTime(2023, 08, 27, 16, 26, 44)
            },
            [312] = new Cycle
            {
                CycleId = 312,
                BarCodeId = 157,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 94,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 17, 04, 37),
                FinishedOn = new DateTime(2023, 08, 27, 17, 06, 11)
            },
            [313] = new Cycle
            {
                CycleId = 313,
                BarCodeId = 158,
                MachineId = 100,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 92,
                TaktTime = 188,
                StartedOn = new DateTime(2023, 08, 27, 17, 16, 44),
                FinishedOn = new DateTime(2023, 08, 27, 17, 18, 16)
            },
            [314] = new Cycle
            {
                CycleId = 314,
                BarCodeId = 159,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 62,
                TaktTime = 122,
                StartedOn = new DateTime(2023, 08, 27, 17, 19, 30),
                FinishedOn = new DateTime(2023, 08, 27, 17, 20, 32)
            },
            [315] = new Cycle
            {
                CycleId = 315,
                BarCodeId = 160,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 81,
                TaktTime = 156,
                StartedOn = new DateTime(2023, 08, 27, 15, 06, 56),
                FinishedOn = new DateTime(2023, 08, 27, 15, 08, 17)
            },
            [316] = new Cycle
            {
                CycleId = 316,
                BarCodeId = 161,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 60,
                TaktTime = 158,
                StartedOn = new DateTime(2023, 08, 27, 15, 44, 41),
                FinishedOn = new DateTime(2023, 08, 27, 15, 45, 41)
            },
            [317] = new Cycle
            {
                CycleId = 317,
                BarCodeId = 161,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 75,
                TaktTime = 141,
                StartedOn = new DateTime(2023, 08, 27, 15, 45, 41),
                FinishedOn = new DateTime(2023, 08, 27, 15, 46, 56)
            },
            [318] = new Cycle
            {
                CycleId = 318,
                BarCodeId = 162,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 53,
                TaktTime = 105,
                StartedOn = new DateTime(2023, 08, 27, 15, 45, 43),
                FinishedOn = new DateTime(2023, 08, 27, 15, 46, 36)
            },
            [319] = new Cycle
            {
                CycleId = 319,
                BarCodeId = 162,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 85,
                TaktTime = 140,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 36),
                FinishedOn = new DateTime(2023, 08, 27, 15, 48, 01)
            },
            [320] = new Cycle
            {
                CycleId = 320,
                BarCodeId = 163,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 79,
                TaktTime = 147,
                StartedOn = new DateTime(2023, 08, 27, 16, 25, 05),
                FinishedOn = new DateTime(2023, 08, 27, 16, 26, 24)
            },
            [321] = new Cycle
            {
                CycleId = 321,
                BarCodeId = 163,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 92,
                TaktTime = 150,
                StartedOn = new DateTime(2023, 08, 27, 16, 26, 24),
                FinishedOn = new DateTime(2023, 08, 27, 16, 27, 56)
            },
            [322] = new Cycle
            {
                CycleId = 322,
                BarCodeId = 164,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 54,
                TaktTime = 150,
                StartedOn = new DateTime(2023, 08, 27, 09, 02, 38),
                FinishedOn = new DateTime(2023, 08, 27, 09, 03, 32)
            },
            [323] = new Cycle
            {
                CycleId = 323,
                BarCodeId = 164,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 93,
                TaktTime = 184,
                StartedOn = new DateTime(2023, 08, 27, 09, 03, 32),
                FinishedOn = new DateTime(2023, 08, 27, 09, 05, 05)
            },
            [324] = new Cycle
            {
                CycleId = 324,
                BarCodeId = 165,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 75,
                TaktTime = 175,
                StartedOn = new DateTime(2023, 08, 27, 09, 18, 14),
                FinishedOn = new DateTime(2023, 08, 27, 09, 19, 29)
            },
            [325] = new Cycle
            {
                CycleId = 325,
                BarCodeId = 165,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 67,
                TaktTime = 143,
                StartedOn = new DateTime(2023, 08, 27, 09, 19, 29),
                FinishedOn = new DateTime(2023, 08, 27, 09, 20, 36)
            },
            [326] = new Cycle
            {
                CycleId = 326,
                BarCodeId = 166,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 154,
                StartedOn = new DateTime(2023, 08, 27, 10, 58, 00),
                FinishedOn = new DateTime(2023, 08, 27, 10, 59, 39)
            },
            [327] = new Cycle
            {
                CycleId = 327,
                BarCodeId = 166,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 78,
                TaktTime = 172,
                StartedOn = new DateTime(2023, 08, 27, 10, 59, 39),
                FinishedOn = new DateTime(2023, 08, 27, 11, 00, 57)
            },
            [328] = new Cycle
            {
                CycleId = 328,
                BarCodeId = 167,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 97,
                TaktTime = 177,
                StartedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                FinishedOn = new DateTime(2023, 08, 27, 15, 33, 29)
            },
            [329] = new Cycle
            {
                CycleId = 329,
                BarCodeId = 167,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 77,
                TaktTime = 151,
                StartedOn = new DateTime(2023, 08, 27, 15, 33, 29),
                FinishedOn = new DateTime(2023, 08, 27, 15, 34, 46)
            },
            [330] = new Cycle
            {
                CycleId = 330,
                BarCodeId = 168,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 92,
                TaktTime = 145,
                StartedOn = new DateTime(2023, 08, 27, 15, 46, 00),
                FinishedOn = new DateTime(2023, 08, 27, 15, 47, 32)
            },
            [331] = new Cycle
            {
                CycleId = 331,
                BarCodeId = 168,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 80,
                TaktTime = 150,
                StartedOn = new DateTime(2023, 08, 27, 15, 47, 32),
                FinishedOn = new DateTime(2023, 08, 27, 15, 48, 52)
            },
            [332] = new Cycle
            {
                CycleId = 332,
                BarCodeId = 169,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 60,
                TaktTime = 136,
                StartedOn = new DateTime(2023, 08, 27, 10, 53, 32),
                FinishedOn = new DateTime(2023, 08, 27, 10, 54, 32)
            },
            [333] = new Cycle
            {
                CycleId = 333,
                BarCodeId = 169,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 75,
                TaktTime = 131,
                StartedOn = new DateTime(2023, 08, 27, 10, 54, 32),
                FinishedOn = new DateTime(2023, 08, 27, 10, 55, 47)
            },
            [334] = new Cycle
            {
                CycleId = 334,
                BarCodeId = 170,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 93,
                TaktTime = 158,
                StartedOn = new DateTime(2023, 08, 27, 10, 59, 49),
                FinishedOn = new DateTime(2023, 08, 27, 11, 01, 22)
            },
            [335] = new Cycle
            {
                CycleId = 335,
                BarCodeId = 170,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 59,
                TaktTime = 122,
                StartedOn = new DateTime(2023, 08, 27, 11, 01, 22),
                FinishedOn = new DateTime(2023, 08, 27, 11, 02, 21)
            },
            [336] = new Cycle
            {
                CycleId = 336,
                BarCodeId = 171,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 180,
                StartedOn = new DateTime(2023, 08, 27, 11, 01, 12),
                FinishedOn = new DateTime(2023, 08, 27, 11, 02, 36)
            },
            [337] = new Cycle
            {
                CycleId = 337,
                BarCodeId = 171,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 72,
                TaktTime = 127,
                StartedOn = new DateTime(2023, 08, 27, 11, 02, 36),
                FinishedOn = new DateTime(2023, 08, 27, 11, 03, 48)
            },
            [338] = new Cycle
            {
                CycleId = 338,
                BarCodeId = 172,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 66,
                TaktTime = 166,
                StartedOn = new DateTime(2023, 08, 27, 11, 01, 23),
                FinishedOn = new DateTime(2023, 08, 27, 11, 02, 29)
            },
            [339] = new Cycle
            {
                CycleId = 339,
                BarCodeId = 172,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 59,
                TaktTime = 127,
                StartedOn = new DateTime(2023, 08, 27, 11, 02, 29),
                FinishedOn = new DateTime(2023, 08, 27, 11, 03, 28)
            },
            [340] = new Cycle
            {
                CycleId = 340,
                BarCodeId = 173,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 57,
                TaktTime = 125,
                StartedOn = new DateTime(2023, 08, 27, 12, 27, 18),
                FinishedOn = new DateTime(2023, 08, 27, 12, 28, 15)
            },
            [341] = new Cycle
            {
                CycleId = 341,
                BarCodeId = 173,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 91,
                TaktTime = 151,
                StartedOn = new DateTime(2023, 08, 27, 12, 28, 15),
                FinishedOn = new DateTime(2023, 08, 27, 12, 29, 46)
            },
            [342] = new Cycle
            {
                CycleId = 342,
                BarCodeId = 174,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 72,
                TaktTime = 152,
                StartedOn = new DateTime(2023, 08, 27, 13, 36, 40),
                FinishedOn = new DateTime(2023, 08, 27, 13, 37, 52)
            },
            [343] = new Cycle
            {
                CycleId = 343,
                BarCodeId = 174,
                MachineId = 300,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 92,
                TaktTime = 148,
                StartedOn = new DateTime(2023, 08, 27, 13, 37, 52),
                FinishedOn = new DateTime(2023, 08, 27, 13, 39, 24)
            },
            [344] = new Cycle
            {
                CycleId = 344,
                BarCodeId = 175,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 63,
                TaktTime = 146,
                StartedOn = new DateTime(2023, 08, 27, 14, 01, 00),
                FinishedOn = new DateTime(2023, 08, 27, 14, 02, 03)
            },
            [345] = new Cycle
            {
                CycleId = 345,
                BarCodeId = 175,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 184,
                StartedOn = new DateTime(2023, 08, 27, 14, 02, 03),
                FinishedOn = new DateTime(2023, 08, 27, 14, 03, 27)
            },
            [346] = new Cycle
            {
                CycleId = 346,
                BarCodeId = 176,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 94,
                TaktTime = 154,
                StartedOn = new DateTime(2023, 08, 27, 16, 25, 12),
                FinishedOn = new DateTime(2023, 08, 27, 16, 26, 46)
            },
            [347] = new Cycle
            {
                CycleId = 347,
                BarCodeId = 176,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 84,
                TaktTime = 174,
                StartedOn = new DateTime(2023, 08, 27, 16, 26, 46),
                FinishedOn = new DateTime(2023, 08, 27, 16, 28, 10)
            },
            [348] = new Cycle
            {
                CycleId = 348,
                BarCodeId = 176,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 74,
                TaktTime = 166,
                StartedOn = new DateTime(2023, 08, 27, 16, 25, 12),
                FinishedOn = new DateTime(2023, 08, 27, 16, 26, 26)
            },
            [349] = new Cycle
            {
                CycleId = 349,
                BarCodeId = 177,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 68,
                TaktTime = 135,
                StartedOn = new DateTime(2023, 08, 27, 17, 04, 37),
                FinishedOn = new DateTime(2023, 08, 27, 17, 05, 45)
            },
            [350] = new Cycle
            {
                CycleId = 350,
                BarCodeId = 177,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 190,
                StartedOn = new DateTime(2023, 08, 27, 17, 05, 45),
                FinishedOn = new DateTime(2023, 08, 27, 17, 07, 24)
            },
            [351] = new Cycle
            {
                CycleId = 351,
                BarCodeId = 177,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 81,
                TaktTime = 145,
                StartedOn = new DateTime(2023, 08, 27, 17, 07, 24),
                FinishedOn = new DateTime(2023, 08, 27, 17, 08, 45)
            },
            [352] = new Cycle
            {
                CycleId = 352,
                BarCodeId = 178,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 56,
                TaktTime = 121,
                StartedOn = new DateTime(2023, 08, 27, 17, 16, 44),
                FinishedOn = new DateTime(2023, 08, 27, 17, 17, 40)
            },
            [353] = new Cycle
            {
                CycleId = 353,
                BarCodeId = 178,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 83,
                TaktTime = 152,
                StartedOn = new DateTime(2023, 08, 27, 17, 17, 40),
                FinishedOn = new DateTime(2023, 08, 27, 17, 19, 03)
            },
            [354] = new Cycle
            {
                CycleId = 354,
                BarCodeId = 178,
                MachineId = 500,
                CycleStatus = CycleStatus.Started,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 79,
                TaktTime = 143,
                StartedOn = new DateTime(2023, 08, 27, 17, 19, 03),
                FinishedOn = new DateTime(2023, 08, 27, 17, 20, 22)
            },
            [355] = new Cycle
            {
                CycleId = 355,
                BarCodeId = 179,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 64,
                TaktTime = 157,
                StartedOn = new DateTime(2023, 08, 27, 17, 19, 30),
                FinishedOn = new DateTime(2023, 08, 27, 17, 20, 34)
            },
            [356] = new Cycle
            {
                CycleId = 356,
                BarCodeId = 179,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 91,
                TaktTime = 168,
                StartedOn = new DateTime(2023, 08, 27, 17, 20, 34),
                FinishedOn = new DateTime(2023, 08, 27, 17, 22, 05)
            },
            [357] = new Cycle
            {
                CycleId = 357,
                BarCodeId = 179,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedNok,
                PartStatus = PartStatus.NOk,
                CyclesOk = 1,
                CycleTime = 86,
                TaktTime = 147,
                StartedOn = new DateTime(2023, 08, 27, 17, 19, 30),
                FinishedOn = new DateTime(2023, 08, 27, 17, 20, 56)
            },
            [358] = new Cycle
            {
                CycleId = 358,
                BarCodeId = 180,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 55,
                TaktTime = 135,
                StartedOn = new DateTime(2023, 08, 27, 15, 06, 56),
                FinishedOn = new DateTime(2023, 08, 27, 15, 07, 51)
            },
            [359] = new Cycle
            {
                CycleId = 359,
                BarCodeId = 180,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 58,
                TaktTime = 110,
                StartedOn = new DateTime(2023, 08, 27, 15, 07, 51),
                FinishedOn = new DateTime(2023, 08, 27, 15, 08, 49)
            },
            [360] = new Cycle
            {
                CycleId = 360,
                BarCodeId = 180,
                MachineId = 500,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 08, 49),
                FinishedOn = new DateTime(2023, 08, 27, 15, 10, 28)
            },
            [361] = new Cycle
            {
                CycleId = 361,
                BarCodeId = 181,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 08, 49),
                FinishedOn = new DateTime(2023, 08, 27, 15, 10, 28)
            },
            [362] = new Cycle
            {
                CycleId = 362,
                BarCodeId = 181,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 08, 49),
                FinishedOn = new DateTime(2023, 08, 27, 15, 10, 28)
            },
            [363] = new Cycle
            {
                CycleId = 363,
                BarCodeId = 182,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 08, 49),
                FinishedOn = new DateTime(2023, 08, 27, 15, 10, 28)
            },
            [364] = new Cycle
            {
                CycleId = 364,
                BarCodeId = 182,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 08, 49),
                FinishedOn = new DateTime(2023, 08, 27, 15, 10, 28)
            },
            [365] = new Cycle
            {
                CycleId = 365,
                BarCodeId = 183,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 08, 49),
                FinishedOn = new DateTime(2023, 08, 27, 15, 10, 28)
            },
            [366] = new Cycle
            {
                CycleId = 366,
                BarCodeId = 183,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 08, 49),
                FinishedOn = new DateTime(2023, 08, 27, 15, 10, 28)
            },
            [367] = new Cycle
            {
                CycleId = 367,
                BarCodeId = 184,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 08, 49),
                FinishedOn = new DateTime(2023, 08, 27, 15, 10, 28)
            },
            [368] = new Cycle
            {
                CycleId = 368,
                BarCodeId = 184,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 08, 49),
                FinishedOn = new DateTime(2023, 08, 27, 15, 10, 28)
            },
            [369] = new Cycle
            {
                CycleId = 369,
                BarCodeId = 185,
                MachineId = 100,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 08, 49),
                FinishedOn = new DateTime(2023, 08, 27, 15, 10, 28)
            },
            [370] = new Cycle
            {
                CycleId = 370,
                BarCodeId = 185,
                MachineId = 300,
                CycleStatus = CycleStatus.FinishedOk,
                PartStatus = PartStatus.Ok,
                CyclesOk = 1,
                CycleTime = 99,
                TaktTime = 161,
                StartedOn = new DateTime(2023, 08, 27, 15, 08, 49),
                FinishedOn = new DateTime(2023, 08, 27, 15, 10, 28)
            }
        }.ToImmutableDictionary();

    /// <summary>
    /// Lazy-loaded cached list for maximum performance - best of both worlds
    /// </summary>
    private static readonly Lazy<IReadOnlyList<Cycle>> _fixtureCache =
        new(() => _cyclesDict.Values.ToList());

    /// <summary>
    /// Get all Cycle entities (cached List from dictionary for backward compatibility)
    /// </summary>
    public static IReadOnlyList<Cycle> Fixture => _fixtureCache.Value;

    /// <summary>
    /// Get a specific Cycle by ID - O(1) lookup
    /// </summary>
    public static Cycle? GetById(int id) =>
        _cyclesDict.TryGetValue(id, out var cycle) ? cycle : null;

    /// <summary>
    /// Get a specific Cycle by ID - O(1) lookup (legacy method name)
    /// </summary>
    public static Cycle? GetCycle(int id) => GetById(id);

    /// <summary>
    /// Direct dictionary access for advanced scenarios
    /// </summary>
    public static IImmutableDictionary<int, Cycle> Dictionary => _cyclesDict;

    /// <summary>
    /// Direct dictionary access (legacy property name)
    /// </summary>
    public static IImmutableDictionary<int, Cycle> Cycles => _cyclesDict;

    /// <summary>
    /// Check if a Cycle exists by ID
    /// </summary>
    public static bool Contains(int id) => _cyclesDict.ContainsKey(id);

    /// <summary>
    /// Get count of Cycles
    /// </summary>
    public static int Count => _cyclesDict.Count;

    /// <summary>
    /// Get Cycle by MachineId - O(n) operation
    /// </summary>
    public static IEnumerable<Cycle> GetByMachineId(int machineId) =>
        _cyclesDict.Values.Where(c => c.MachineId == machineId);

    /// <summary>
    /// Get Cycle by BarCodeId - O(n) operation
    /// </summary>
    public static Cycle? GetByBarCodeId(int barCodeId) =>
        _cyclesDict.Values.FirstOrDefault(c => c.BarCodeId == barCodeId);

    /// <summary>
    /// Get Cycles by CycleStatus - O(n) operation
    /// </summary>
    public static IEnumerable<Cycle> GetByStatus(CycleStatus status) =>
        _cyclesDict.Values.Where(c => c.CycleStatus == status);
}
