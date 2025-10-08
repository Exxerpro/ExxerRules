using System.Text.Json.Serialization;

namespace IndTrace.DataStore.ModelsComs
{
    /// <summary>
    /// Represents a PLC database block, including its data, identifier, size, and name.
    /// </summary>
    public class Db
    {
        /// <summary>
        /// The raw data of the database block.
        /// </summary>
        public byte[] Data;
        /// <summary>
        /// Gets the identifier of the database block.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Gets the size of the database block.
        /// </summary>
        public int Size { get; }
        /// <summary>
        /// Gets or sets the name of the database block.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Db"/> class.
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="size">The size.</param>

        public Db(int id, int size)
        {
            this.Id = id;
            this.Size = size;
            this.Name = "DB" + id.ToString("D3");
            this.Data = new byte[this.Size];
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Db"/> class.
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The id.</param>

        public Db(int id)
        {
            this.Id = id;
            this.Name = "DB" + id.ToString("D3");
            this.Size = 2048;
            this.Data = new byte[this.Size];
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Db"/> class.
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="size">The size.</param>
        /// <param name="data">The data.</param>

        [JsonConstructor]
        public Db(int id, string name, int size, byte[] data)
        {
            this.Id = id;
            this.Size = size;
            this.Name = name;
            this.Data = new byte[this.Size];
            Array.Copy(data, this.Data, data.Length);
        }

        //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate DB logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    }
}
