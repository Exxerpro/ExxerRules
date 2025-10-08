namespace IndTrace.Agregation.Dependices.Generic.Helpers;

public static class GenericTestDataHelper
{
    public static List<GenericTestEntity> CreateTestEntities(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new GenericTestEntity
            {
                Id = i,
                Name = $"Entity_{i:D3}",
                Description = $"Test entity number {i}",
                CreatedAt = DateTime.UtcNow.AddHours(-i)
            })
            .ToList();
    }

    public static List<GenericTestEntity> CreateManufacturingTestEntities(string industry, string equipment, int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new GenericTestEntity
            {
                Id = i,
                Name = $"{industry}_Component_{i:D3}",
                Description = $"{equipment} - Part {i}",
                CreatedAt = DateTime.UtcNow.AddMinutes(-i * 5)
            })
            .ToList();
    }
}
