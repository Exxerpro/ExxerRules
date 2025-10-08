using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IndTrace.Domain.Entities.BarCodes;
using IndTrace.Domain.Enum;

namespace IndTrace.TestData.RawData;

/// <summary>
/// Static test data for BarCode entities with O(1) lookup.
/// Generated with ImmutableDictionary for thread-safety and performance.
/// IMPORTED: Contains all 189 entities from BarCodes.json
/// Generated on: 2025-09-03 06:01:53
/// </summary>
internal static class BarCodeRawData
{
    private static readonly ImmutableDictionary<int, BarCode> _barCodesDict =
        new Dictionary<int, BarCode>
        {
            [1] = new BarCode
            {
                BarCodeId = 1,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372501",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 00, 27, 24),
                ModifiedOn = new DateTime(2023, 08, 27, 00, 49, 12)
            },
            [2] = new BarCode
            {
                BarCodeId = 2,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372502",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 02, 54, 13),
                ModifiedOn = new DateTime(2023, 08, 27, 02, 54, 19)
            },
            [3] = new BarCode
            {
                BarCodeId = 3,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372503",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 07, 49, 43),
                ModifiedOn = new DateTime(2023, 08, 27, 08, 18, 00)
            },
            [4] = new BarCode
            {
                BarCodeId = 4,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372504",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 03, 18, 15),
                ModifiedOn = new DateTime(2023, 08, 27, 03, 41, 07)
            },
            [5] = new BarCode
            {
                BarCodeId = 5,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372505",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 18, 06),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 27, 28)
            },
            [6] = new BarCode
            {
                BarCodeId = 6,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372506",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 23, 58),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 01, 55)
            },
            [7] = new BarCode
            {
                BarCodeId = 7,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372507",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 11, 46, 55),
                ModifiedOn = new DateTime(2023, 08, 27, 11, 50, 57)
            },
            [8] = new BarCode
            {
                BarCodeId = 8,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372508",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 11, 48, 11),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 03, 04)
            },
            [9] = new BarCode
            {
                BarCodeId = 9,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372509",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 05, 30),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 20, 08)
            },
            [10] = new BarCode
            {
                BarCodeId = 10,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372510",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 12, 19, 30),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 19, 35)
            },
            [11] = new BarCode
            {
                BarCodeId = 11,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372511",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 08, 31, 01),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 22, 03)
            },
            [12] = new BarCode
            {
                BarCodeId = 12,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372512",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 08, 31, 47),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 25, 05)
            },
            [13] = new BarCode
            {
                BarCodeId = 13,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372513",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 11, 50, 42),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 09, 01)
            },
            [14] = new BarCode
            {
                BarCodeId = 14,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372514",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 11, 53, 28),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 12, 01)
            },
            [15] = new BarCode
            {
                BarCodeId = 15,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372515",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 01, 49),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 16, 03)
            },
            [16] = new BarCode
            {
                BarCodeId = 16,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372516",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 06, 30),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 19, 31)
            },
            [17] = new BarCode
            {
                BarCodeId = 17,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372517",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 09, 13),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 20, 56)
            },
            [18] = new BarCode
            {
                BarCodeId = 18,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372518",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 11, 01),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 22, 36)
            },
            [19] = new BarCode
            {
                BarCodeId = 19,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372519",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 12, 15),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 21, 23)
            },
            [20] = new BarCode
            {
                BarCodeId = 20,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372520",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 13, 21),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 22, 16)
            },
            [21] = new BarCode
            {
                BarCodeId = 21,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372521",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 14, 53),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 20, 02)
            },
            [22] = new BarCode
            {
                BarCodeId = 22,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372522",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 16, 43),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 21, 58)
            },
            [23] = new BarCode
            {
                BarCodeId = 23,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372523",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 13, 15, 45),
                ModifiedOn = new DateTime(2023, 08, 27, 13, 15, 45)
            },
            [24] = new BarCode
            {
                BarCodeId = 24,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372524",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 13, 26, 29),
                ModifiedOn = new DateTime(2023, 08, 27, 13, 26, 29)
            },
            [25] = new BarCode
            {
                BarCodeId = 25,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372525",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 13, 31, 55),
                ModifiedOn = new DateTime(2023, 08, 27, 13, 31, 55)
            },
            [26] = new BarCode
            {
                BarCodeId = 26,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372526",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 13, 37, 13),
                ModifiedOn = new DateTime(2023, 08, 27, 13, 37, 13)
            },
            [27] = new BarCode
            {
                BarCodeId = 27,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372527",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 03, 53),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 03, 53)
            },
            [28] = new BarCode
            {
                BarCodeId = 28,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372528",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 07, 23),
                ModifiedOn = new DateTime(2023, 08, 27, 16, 54, 58)
            },
            [29] = new BarCode
            {
                BarCodeId = 29,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372529",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 52, 09),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 52, 09)
            },
            [30] = new BarCode
            {
                BarCodeId = 30,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372530",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 54, 09),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 54, 09)
            },
            [31] = new BarCode
            {
                BarCodeId = 31,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372531",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 00, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 00, 17)
            },
            [32] = new BarCode
            {
                BarCodeId = 32,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372532",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 01, 14),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 01, 14)
            },
            [33] = new BarCode
            {
                BarCodeId = 33,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372533",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 10, 01, 50),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 01, 50)
            },
            [34] = new BarCode
            {
                BarCodeId = 34,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372534",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 03, 23),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 03, 23)
            },
            [35] = new BarCode
            {
                BarCodeId = 35,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372535",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 05, 46),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 05, 46)
            },
            [36] = new BarCode
            {
                BarCodeId = 36,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372536",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 10, 06, 13),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 06, 13)
            },
            [37] = new BarCode
            {
                BarCodeId = 37,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372537",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 06, 27),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 06, 27)
            },
            [38] = new BarCode
            {
                BarCodeId = 38,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372538",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 07, 47),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 07, 47)
            },
            [39] = new BarCode
            {
                BarCodeId = 39,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372539",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 10, 13, 47),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 13, 47)
            },
            [40] = new BarCode
            {
                BarCodeId = 40,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372540",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 13, 59),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 13, 59)
            },
            [41] = new BarCode
            {
                BarCodeId = 41,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372541",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 16, 39),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 16, 39)
            },
            [42] = new BarCode
            {
                BarCodeId = 42,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372542",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 10, 35, 21),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 35, 21)
            },
            [43] = new BarCode
            {
                BarCodeId = 43,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372543",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 35, 49),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 35, 49)
            },
            [44] = new BarCode
            {
                BarCodeId = 44,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372544",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 43, 32),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 43, 32)
            },
            [45] = new BarCode
            {
                BarCodeId = 45,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372545",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 10, 49, 58),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 49, 58)
            },
            [46] = new BarCode
            {
                BarCodeId = 46,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372546",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 10, 50, 56),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 50, 56)
            },
            [47] = new BarCode
            {
                BarCodeId = 47,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372547",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 51, 25),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 51, 25)
            },
            [48] = new BarCode
            {
                BarCodeId = 48,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372548",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 53, 08),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 53, 08)
            },
            [49] = new BarCode
            {
                BarCodeId = 49,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372549",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 10, 53, 32),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 53, 32)
            },
            [50] = new BarCode
            {
                BarCodeId = 50,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372550",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 59, 49),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 59, 49)
            },
            [51] = new BarCode
            {
                BarCodeId = 51,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372551",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 11, 01, 12),
                ModifiedOn = new DateTime(2023, 08, 27, 11, 01, 12)
            },
            [52] = new BarCode
            {
                BarCodeId = 52,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372552",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 11, 01, 23),
                ModifiedOn = new DateTime(2023, 08, 27, 11, 01, 23)
            },
            [53] = new BarCode
            {
                BarCodeId = 53,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372553",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 27, 18),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 27, 18)
            },
            [54] = new BarCode
            {
                BarCodeId = 54,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372554",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 13, 36, 40),
                ModifiedOn = new DateTime(2023, 08, 27, 13, 36, 40)
            },
            [55] = new BarCode
            {
                BarCodeId = 55,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372555",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 14, 01, 00),
                ModifiedOn = new DateTime(2023, 08, 27, 14, 01, 00)
            },
            [56] = new BarCode
            {
                BarCodeId = 56,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372556",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 16, 19, 04),
                ModifiedOn = new DateTime(2023, 08, 27, 16, 25, 12)
            },
            [57] = new BarCode
            {
                BarCodeId = 57,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372557",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 16, 04, 33),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 04, 37)
            },
            [58] = new BarCode
            {
                BarCodeId = 58,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372558",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 17, 08, 48),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 16, 44)
            },
            [59] = new BarCode
            {
                BarCodeId = 59,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372559",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 17, 18, 22),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 19, 30)
            },
            [60] = new BarCode
            {
                BarCodeId = 60,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372560",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            },
            [61] = new BarCode
            {
                BarCodeId = 61,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372561",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 39, 12),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 44, 41)
            },
            [62] = new BarCode
            {
                BarCodeId = 62,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372562",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 40, 31),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 45, 43)
            },
            [63] = new BarCode
            {
                BarCodeId = 63,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372563",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 16, 23, 47),
                ModifiedOn = new DateTime(2023, 08, 27, 16, 25, 05)
            },
            [64] = new BarCode
            {
                BarCodeId = 64,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372564",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 00, 01),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 02, 38)
            },
            [65] = new BarCode
            {
                BarCodeId = 65,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372565",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 18, 14),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 18, 14)
            },
            [66] = new BarCode
            {
                BarCodeId = 66,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372566",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 56, 32),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 58, 00)
            },
            [67] = new BarCode
            {
                BarCodeId = 67,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372567",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 31, 52)
            },
            [68] = new BarCode
            {
                BarCodeId = 68,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372568",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 37, 22),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 46, 00)
            },
            [69] = new BarCode
            {
                BarCodeId = 69,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372569",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 53, 32),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 53, 32)
            },
            [70] = new BarCode
            {
                BarCodeId = 70,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372570",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 59, 49),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 59, 49)
            },
            [71] = new BarCode
            {
                BarCodeId = 71,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372571",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 11, 01, 12),
                ModifiedOn = new DateTime(2023, 08, 27, 11, 01, 12)
            },
            [72] = new BarCode
            {
                BarCodeId = 72,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372572",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 11, 01, 23),
                ModifiedOn = new DateTime(2023, 08, 27, 11, 01, 23)
            },
            [73] = new BarCode
            {
                BarCodeId = 73,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372573",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 27, 18),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 27, 18)
            },
            [74] = new BarCode
            {
                BarCodeId = 74,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372574",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 13, 36, 40),
                ModifiedOn = new DateTime(2023, 08, 27, 13, 36, 40)
            },
            [75] = new BarCode
            {
                BarCodeId = 75,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372575",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 14, 01, 00),
                ModifiedOn = new DateTime(2023, 08, 27, 14, 01, 00)
            },
            [76] = new BarCode
            {
                BarCodeId = 76,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372576",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 16, 19, 04),
                ModifiedOn = new DateTime(2023, 08, 27, 16, 25, 12)
            },
            [77] = new BarCode
            {
                BarCodeId = 77,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372577",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 16, 04, 33),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 04, 37)
            },
            [78] = new BarCode
            {
                BarCodeId = 78,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372578",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 17, 08, 48),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 16, 44)
            },
            [79] = new BarCode
            {
                BarCodeId = 79,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372579",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 17, 18, 22),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 19, 30)
            },
            [80] = new BarCode
            {
                BarCodeId = 80,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372580",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            },
            [81] = new BarCode
            {
                BarCodeId = 81,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372581",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 15, 39, 12),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 44, 41)
            },
            [82] = new BarCode
            {
                BarCodeId = 82,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372582",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 40, 31),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 45, 43)
            },
            [83] = new BarCode
            {
                BarCodeId = 83,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372583",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 16, 23, 47),
                ModifiedOn = new DateTime(2023, 08, 27, 16, 25, 05)
            },
            [84] = new BarCode
            {
                BarCodeId = 84,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372584",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 09, 00, 01),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 02, 38)
            },
            [85] = new BarCode
            {
                BarCodeId = 85,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372585",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 18, 14),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 18, 14)
            },
            [86] = new BarCode
            {
                BarCodeId = 86,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372586",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 56, 32),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 58, 00)
            },
            [87] = new BarCode
            {
                BarCodeId = 87,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372587",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 31, 52)
            },
            [88] = new BarCode
            {
                BarCodeId = 88,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372588",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 37, 22),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 46, 00)
            },
            [89] = new BarCode
            {
                BarCodeId = 89,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372589",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 31, 52)
            },
            [90] = new BarCode
            {
                BarCodeId = 90,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372590",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 15, 37, 22),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 46, 00)
            },
            [91] = new BarCode
            {
                BarCodeId = 91,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372591",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 15, 39, 12),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 44, 41)
            },
            [92] = new BarCode
            {
                BarCodeId = 92,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372592",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 40, 31),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 45, 43)
            },
            [93] = new BarCode
            {
                BarCodeId = 93,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372593",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 16, 23, 47),
                ModifiedOn = new DateTime(2023, 08, 27, 16, 25, 05)
            },
            [94] = new BarCode
            {
                BarCodeId = 94,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372594",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 09, 00, 01),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 02, 38)
            },
            [95] = new BarCode
            {
                BarCodeId = 95,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372595",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 18, 14),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 18, 14)
            },
            [96] = new BarCode
            {
                BarCodeId = 96,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372596",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 56, 32),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 58, 00)
            },
            [97] = new BarCode
            {
                BarCodeId = 97,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372597",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 31, 52)
            },
            [98] = new BarCode
            {
                BarCodeId = 98,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372598",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 37, 22),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 46, 00)
            },
            [99] = new BarCode
            {
                BarCodeId = 99,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372599",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 31, 52)
            },
            [100] = new BarCode
            {
                BarCodeId = 100,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372600",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 15, 37, 22),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 46, 00)
            },
            [101] = new BarCode
            {
                BarCodeId = 101,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372601",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 00, 27, 24),
                ModifiedOn = new DateTime(2023, 08, 27, 00, 49, 12)
            },
            [102] = new BarCode
            {
                BarCodeId = 102,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372602",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 02, 54, 13),
                ModifiedOn = new DateTime(2023, 08, 27, 02, 54, 19)
            },
            [103] = new BarCode
            {
                BarCodeId = 103,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372603",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 07, 49, 43),
                ModifiedOn = new DateTime(2023, 08, 27, 08, 18, 00)
            },
            [104] = new BarCode
            {
                BarCodeId = 104,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372604",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 03, 18, 15),
                ModifiedOn = new DateTime(2023, 08, 27, 03, 41, 07)
            },
            [105] = new BarCode
            {
                BarCodeId = 105,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372605",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 18, 06),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 27, 28)
            },
            [106] = new BarCode
            {
                BarCodeId = 106,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372606",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 23, 58),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 01, 55)
            },
            [107] = new BarCode
            {
                BarCodeId = 107,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372607",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 11, 46, 55),
                ModifiedOn = new DateTime(2023, 08, 27, 11, 50, 57)
            },
            [108] = new BarCode
            {
                BarCodeId = 108,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372608",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 11, 48, 11),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 03, 04)
            },
            [109] = new BarCode
            {
                BarCodeId = 109,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372609",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 05, 30),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 20, 08)
            },
            [110] = new BarCode
            {
                BarCodeId = 110,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372610",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 19, 30),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 19, 35)
            },
            [111] = new BarCode
            {
                BarCodeId = 111,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372611",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 08, 31, 01),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 22, 03)
            },
            [112] = new BarCode
            {
                BarCodeId = 112,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372612",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 08, 31, 47),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 25, 05)
            },
            [113] = new BarCode
            {
                BarCodeId = 113,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372613",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 11, 50, 42),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 09, 01)
            },
            [114] = new BarCode
            {
                BarCodeId = 114,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372614",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 11, 53, 28),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 12, 01)
            },
            [115] = new BarCode
            {
                BarCodeId = 115,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372615",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 01, 49),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 16, 03)
            },
            [116] = new BarCode
            {
                BarCodeId = 116,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372616",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 06, 30),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 19, 31)
            },
            [117] = new BarCode
            {
                BarCodeId = 117,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372617",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 09, 13),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 20, 56)
            },
            [118] = new BarCode
            {
                BarCodeId = 118,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372618",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 11, 01),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 22, 36)
            },
            [119] = new BarCode
            {
                BarCodeId = 119,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372619",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 12, 15),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 21, 23)
            },
            [120] = new BarCode
            {
                BarCodeId = 120,
                ProductId = 508,
                MachineId = 300,
                Label = "L1AL687508232372620",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 13, 21),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 22, 16)
            },
            [121] = new BarCode
            {
                BarCodeId = 121,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372621",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 14, 53),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 20, 02)
            },
            [122] = new BarCode
            {
                BarCodeId = 122,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372622",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 16, 43),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 21, 58)
            },
            [123] = new BarCode
            {
                BarCodeId = 123,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372623",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 13, 15, 45),
                ModifiedOn = new DateTime(2023, 08, 27, 13, 15, 45)
            },
            [124] = new BarCode
            {
                BarCodeId = 124,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372624",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 13, 26, 29),
                ModifiedOn = new DateTime(2023, 08, 27, 13, 26, 29)
            },
            [125] = new BarCode
            {
                BarCodeId = 125,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372625",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 13, 31, 55),
                ModifiedOn = new DateTime(2023, 08, 27, 13, 31, 55)
            },
            [126] = new BarCode
            {
                BarCodeId = 126,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372626",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 13, 37, 13),
                ModifiedOn = new DateTime(2023, 08, 27, 13, 37, 13)
            },
            [127] = new BarCode
            {
                BarCodeId = 127,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372627",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 03, 53),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 03, 53)
            },
            [128] = new BarCode
            {
                BarCodeId = 128,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372628",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 07, 23),
                ModifiedOn = new DateTime(2023, 08, 27, 16, 54, 58)
            },
            [129] = new BarCode
            {
                BarCodeId = 129,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372629",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 09, 52, 09),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 52, 09)
            },
            [130] = new BarCode
            {
                BarCodeId = 130,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372630",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 54, 09),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 54, 09)
            },
            [131] = new BarCode
            {
                BarCodeId = 131,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372631",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 00, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 00, 17)
            },
            [132] = new BarCode
            {
                BarCodeId = 132,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372632",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 10, 01, 14),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 01, 14)
            },
            [133] = new BarCode
            {
                BarCodeId = 133,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372633",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 01, 50),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 01, 50)
            },
            [134] = new BarCode
            {
                BarCodeId = 134,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372634",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 03, 23),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 03, 23)
            },
            [135] = new BarCode
            {
                BarCodeId = 135,
                ProductId = 508,
                MachineId = 500,
                Label = "L1AL687508232372635",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 10, 05, 46),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 05, 46)
            },
            [136] = new BarCode
            {
                BarCodeId = 136,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL687508232372636",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 10, 06, 13),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 06, 13)
            },
            [137] = new BarCode
            {
                BarCodeId = 137,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL687508232372637",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 06, 27),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 06, 27)
            },
            [138] = new BarCode
            {
                BarCodeId = 138,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL687508232372638",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 07, 47),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 07, 47)
            },
            [139] = new BarCode
            {
                BarCodeId = 139,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL687508232372639",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 10, 13, 47),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 13, 47)
            },
            [140] = new BarCode
            {
                BarCodeId = 140,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL687508232372640",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 13, 59),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 13, 59)
            },
            [141] = new BarCode
            {
                BarCodeId = 141,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL687508232372641",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 16, 39),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 16, 39)
            },
            [142] = new BarCode
            {
                BarCodeId = 142,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL687508232372642",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 10, 35, 21),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 35, 21)
            },
            [143] = new BarCode
            {
                BarCodeId = 143,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL687508232372643",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 35, 49),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 35, 49)
            },
            [144] = new BarCode
            {
                BarCodeId = 144,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL687508232372644",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 43, 32),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 43, 32)
            },
            [145] = new BarCode
            {
                BarCodeId = 145,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL687508232372645",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 10, 49, 58),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 49, 58)
            },
            [146] = new BarCode
            {
                BarCodeId = 146,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372646",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 50, 56),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 50, 56)
            },
            [147] = new BarCode
            {
                BarCodeId = 147,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372647",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 51, 25),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 51, 25)
            },
            [148] = new BarCode
            {
                BarCodeId = 148,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372648",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 10, 53, 08),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 53, 08)
            },
            [149] = new BarCode
            {
                BarCodeId = 149,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372649",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 53, 32),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 53, 32)
            },
            [150] = new BarCode
            {
                BarCodeId = 150,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372650",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 59, 49),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 59, 49)
            },
            [151] = new BarCode
            {
                BarCodeId = 151,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372651",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 11, 01, 12),
                ModifiedOn = new DateTime(2023, 08, 27, 11, 01, 12)
            },
            [152] = new BarCode
            {
                BarCodeId = 152,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372652",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 11, 01, 23),
                ModifiedOn = new DateTime(2023, 08, 27, 11, 01, 23)
            },
            [153] = new BarCode
            {
                BarCodeId = 153,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372653",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 27, 18),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 27, 18)
            },
            [154] = new BarCode
            {
                BarCodeId = 154,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372654",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 13, 36, 40),
                ModifiedOn = new DateTime(2023, 08, 27, 13, 36, 40)
            },
            [155] = new BarCode
            {
                BarCodeId = 155,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372655",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 14, 01, 00),
                ModifiedOn = new DateTime(2023, 08, 27, 14, 01, 00)
            },
            [156] = new BarCode
            {
                BarCodeId = 156,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372656",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 16, 19, 04),
                ModifiedOn = new DateTime(2023, 08, 27, 16, 25, 12)
            },
            [157] = new BarCode
            {
                BarCodeId = 157,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372657",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 16, 04, 33),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 04, 37)
            },
            [158] = new BarCode
            {
                BarCodeId = 158,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372658",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 17, 08, 48),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 16, 44)
            },
            [159] = new BarCode
            {
                BarCodeId = 159,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372659",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 17, 18, 22),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 19, 30)
            },
            [160] = new BarCode
            {
                BarCodeId = 160,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372660",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            },
            [161] = new BarCode
            {
                BarCodeId = 161,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372661",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 39, 12),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 44, 41)
            },
            [162] = new BarCode
            {
                BarCodeId = 162,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372662",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 40, 31),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 45, 43)
            },
            [163] = new BarCode
            {
                BarCodeId = 163,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372663",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 16, 23, 47),
                ModifiedOn = new DateTime(2023, 08, 27, 16, 25, 05)
            },
            [164] = new BarCode
            {
                BarCodeId = 164,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372664",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 00, 01),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 02, 38)
            },
            [165] = new BarCode
            {
                BarCodeId = 165,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372665",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 09, 18, 14),
                ModifiedOn = new DateTime(2023, 08, 27, 09, 18, 14)
            },
            [166] = new BarCode
            {
                BarCodeId = 166,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372666",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 56, 32),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 58, 00)
            },
            [167] = new BarCode
            {
                BarCodeId = 167,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372667",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 15, 31, 52),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 31, 52)
            },
            [168] = new BarCode
            {
                BarCodeId = 168,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372668",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 15, 37, 22),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 46, 00)
            },
            [169] = new BarCode
            {
                BarCodeId = 169,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372669",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 53, 32),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 53, 32)
            },
            [170] = new BarCode
            {
                BarCodeId = 170,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372670",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 10, 59, 49),
                ModifiedOn = new DateTime(2023, 08, 27, 10, 59, 49)
            },
            [171] = new BarCode
            {
                BarCodeId = 171,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372671",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 11, 01, 12),
                ModifiedOn = new DateTime(2023, 08, 27, 11, 01, 12)
            },
            [172] = new BarCode
            {
                BarCodeId = 172,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372672",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 11, 01, 23),
                ModifiedOn = new DateTime(2023, 08, 27, 11, 01, 23)
            },
            [173] = new BarCode
            {
                BarCodeId = 173,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372673",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 12, 27, 18),
                ModifiedOn = new DateTime(2023, 08, 27, 12, 27, 18)
            },
            [174] = new BarCode
            {
                BarCodeId = 174,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372674",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 13, 36, 40),
                ModifiedOn = new DateTime(2023, 08, 27, 13, 36, 40)
            },
            [175] = new BarCode
            {
                BarCodeId = 175,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372675",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 14, 01, 00),
                ModifiedOn = new DateTime(2023, 08, 27, 14, 01, 00)
            },
            [176] = new BarCode
            {
                BarCodeId = 176,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372676",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 16, 19, 04),
                ModifiedOn = new DateTime(2023, 08, 27, 16, 25, 12)
            },
            [177] = new BarCode
            {
                BarCodeId = 177,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372677",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 16, 04, 33),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 04, 37)
            },
            [178] = new BarCode
            {
                BarCodeId = 178,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372678",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 17, 08, 48),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 16, 44)
            },
            [179] = new BarCode
            {
                BarCodeId = 179,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372679",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 17, 18, 22),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 19, 30)
            },
            [180] = new BarCode
            {
                BarCodeId = 180,
                ProductId = 629,
                MachineId = 500,
                Label = "L1AL90164629232372680",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            },
            [181] = new BarCode
            {
                BarCodeId = 181,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372681",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 16, 19, 04),
                ModifiedOn = new DateTime(2023, 08, 27, 16, 25, 12)
            },
            [182] = new BarCode
            {
                BarCodeId = 182,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372682",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 16, 04, 33),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 04, 37)
            },
            [183] = new BarCode
            {
                BarCodeId = 183,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL90164629232372683",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 17, 08, 48),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 16, 44)
            },
            [184] = new BarCode
            {
                BarCodeId = 184,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372684",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 17, 18, 22),
                ModifiedOn = new DateTime(2023, 08, 27, 17, 19, 30)
            },
            [185] = new BarCode
            {
                BarCodeId = 185,
                ProductId = 508,
                MachineId = 100,
                Label = "L1AL687508232372685",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            },
            [186] = new BarCode
            {
                BarCodeId = 186,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372554",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            },
            [187] = new BarCode
            {
                BarCodeId = 187,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232372557",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            },
            [188] = new BarCode
            {
                BarCodeId = 188,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372567",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            },
            [189] = new BarCode
            {
                BarCodeId = 189,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232372569",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            },
            [9996] = new BarCode
            {
                BarCodeId = 9996,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232379996",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Created,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            },
            [9997] = new BarCode
            {
                BarCodeId = 9997,
                ProductId = 629,
                MachineId = 100,
                Label = "L1AL90164629232379997",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            },
            [9998] = new BarCode
            {
                BarCodeId = 9998,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232379998",
                PartStatus = PartStatus.Ok,
                FlowStatus = FlowStatus.Finished,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            },
            [9999] = new BarCode
            {
                BarCodeId = 9999,
                ProductId = 629,
                MachineId = 300,
                Label = "L1AL90164629232379999",
                PartStatus = PartStatus.NOk,
                FlowStatus = FlowStatus.InProcess,
                CreatedOn = new DateTime(2023, 08, 27, 14, 22, 17),
                ModifiedOn = new DateTime(2023, 08, 27, 15, 06, 56)
            }
        }.ToImmutableDictionary();

    /// <summary>
    /// Lazy-loaded cached list for maximum performance - best of both worlds
    /// </summary>
    private static readonly Lazy<IReadOnlyList<BarCode>> _fixtureCache =
        new(() => _barCodesDict.Values.ToList());

    /// <summary>
    /// Get all BarCode entities (cached List from dictionary for backward compatibility)
    /// </summary>
    public static IReadOnlyList<BarCode> Fixture => _fixtureCache.Value;

    /// <summary>
    /// Get a specific BarCode by ID - O(1) lookup
    /// </summary>
    public static BarCode? GetById(int id) =>
        _barCodesDict.TryGetValue(id, out var barCode) ? barCode : null;

    /// <summary>
    /// Get a specific BarCode by ID - O(1) lookup (legacy method name)
    /// </summary>
    public static BarCode? GetBarCode(int id) => GetById(id);

    /// <summary>
    /// Direct dictionary access for advanced scenarios
    /// </summary>
    public static IImmutableDictionary<int, BarCode> Dictionary => _barCodesDict;

    /// <summary>
    /// Direct dictionary access (legacy property name)
    /// </summary>
    public static IImmutableDictionary<int, BarCode> BarCodes => _barCodesDict;

    /// <summary>
    /// Check if a BarCode exists by ID - O(1) lookup
    /// </summary>
    public static bool Contains(int id) => _barCodesDict.ContainsKey(id);

    /// <summary>
    /// Get count of BarCodes - O(1) operation
    /// </summary>
    public static int Count => _barCodesDict.Count;

    /// <summary>
    /// Get BarCode by Label - O(n) operation
    /// </summary>
    public static BarCode? GetByLabel(string label) =>
        _barCodesDict.Values.FirstOrDefault(b => b.Label == label);

    /// <summary>
    /// Get BarCodes by MachineId - O(n) operation
    /// </summary>
    public static IEnumerable<BarCode> GetByMachineId(int machineId) =>
        _barCodesDict.Values.Where(b => b.MachineId == machineId);
}
